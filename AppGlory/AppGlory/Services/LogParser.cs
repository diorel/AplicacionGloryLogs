using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using AppGlory.Models;

namespace AppGlory.Services
{
    public static class LogParser
    {
        private static readonly Regex RxTimestamp = new(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})", RegexOptions.Compiled);
        private static readonly Regex RxSerial    = new(@"Cofre:\s*(N\w+)|NUMERO DE SERIE\s*-+\s*>\s*(N\w+)|safe id '(N\w+)'", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RxVersion   = new(@"[Vv]ersi[oó]n\s+([^:]+?)\s*:", RegexOptions.Compiled);
        private static readonly Regex RxUsuario   = new(@"Usuario:\s*(\w+)", RegexOptions.Compiled);

        public static List<LogRecord> Parse(string filePath)
        {
            var filename  = Path.GetFileNameWithoutExtension(filePath);
            var groups    = new Dictionary<(string serial, string version), LogRecord>();
            var serialVer = new Dictionary<string, string>();

            string currentSerial  = "DESCONOCIDO";
            string currentVersion = "N/A";

            foreach (var line in File.ReadLines(filePath))
            {
                // 1. Extract serial from this line (keeps last seen)
                var serial = ExtractSerial(line);
                if (serial != null) currentSerial = serial;

                // 2. Extract version from this line
                var version = ExtractVersion(line);
                if (version != null)
                {
                    currentVersion = version;
                    serialVer[currentSerial] = version;
                }
                else if (serialVer.TryGetValue(currentSerial, out var sv))
                {
                    currentVersion = sv;
                }

                // 3. Get or create group
                var key = (currentSerial, currentVersion);
                if (!groups.TryGetValue(key, out var rec))
                {
                    rec = new LogRecord { Archivo = filename, NumeroSerie = currentSerial, Version = currentVersion };
                    groups[key] = rec;
                }

                var ts = ParseTimestamp(line);

                // 4. Version appearance = startup event
                if (version != null && ts.HasValue)
                {
                    rec.VecesAparecio++;
                    if (rec.FechaPrimera == null) rec.FechaPrimera = ts;
                    rec.FechaUltima = ts;
                }

                // 5. Count events
                if (line.Contains("NO FUE POSIBLE PASAR A MODO DEPOSITO"))
                    rec.ErrorCambioDeModo++;

                if (line.Contains("TERMINO DE DEPOSITO CON ERROR"))
                    rec.OperacionesConError++;

                if (line.Contains("Error durante el almacenaje") || line.Contains("se procesará el error", StringComparison.OrdinalIgnoreCase))
                    rec.ErrorAlmacenaje++;

                if (line.Contains("Deposito exitoso"))
                    rec.Depositos++;

                if (line.Contains("-- CONTANDO --") || line.Contains("-- ALMACENAJE --"))
                    rec.ContandoAlmacenado++;

                if (line.Contains("Deposito enviado a CIRREON con exito", StringComparison.OrdinalIgnoreCase))
                {
                    var user = RxUsuario.Match(line);
                    if (user.Success) rec.Usuarios.Add(user.Groups[1].Value.Trim());
                }

                if (line.Contains("TERMINO DE RECOLECCION"))
                    rec.Recolecciones++;
            }

            return groups.Values
                         .OrderBy(r => r.NumeroSerie)
                         .ThenBy(r => r.FechaPrimera)
                         .ToList();
        }

        private static DateTime? ParseTimestamp(string line)
        {
            var m = RxTimestamp.Match(line);
            if (m.Success && DateTime.TryParseExact(m.Groups[1].Value, "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            return null;
        }

        private static string? ExtractSerial(string line)
        {
            var m = RxSerial.Match(line);
            if (!m.Success) return null;
            return m.Groups.Cast<Group>().Skip(1).FirstOrDefault(g => g.Success)?.Value;
        }

        private static string? ExtractVersion(string line)
        {
            var m = RxVersion.Match(line);
            return m.Success ? m.Groups[1].Value.Trim() : null;
        }
    }
}
