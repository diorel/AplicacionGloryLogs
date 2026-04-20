using ClosedXML.Excel;
using AppGlory.Models;

namespace AppGlory.Services
{
    public static class ExcelExporter
    {
        private static readonly string[] Headers =
        {
            "Archivo", "Serie", "Versión", "Fecha Primera", "Veces",
            "Fecha Última", "Error Cambio Modo", "Op. con Error",
            "Error Almacenaje", "Depósitos", "Contando Almacenado",
            "Usuarios Distintos", "Cantidad Usuarios", "Recolecciones"
        };

        public static void Export(IEnumerable<LogRecord> records, string path)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Análisis Logs");

            // Header row
            for (int i = 0; i < Headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = Headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E79");
                cell.Style.Font.FontColor = XLColor.White;
            }

            // Data rows
            int row = 2;
            foreach (var r in records)
            {
                ws.Cell(row, 1).Value  = r.Archivo;
                ws.Cell(row, 2).Value  = r.NumeroSerie;
                ws.Cell(row, 3).Value  = r.Version;
                ws.Cell(row, 4).Value  = r.FechaPrimeraStr;
                ws.Cell(row, 5).Value  = r.VecesAparecio;
                ws.Cell(row, 6).Value  = r.FechaUltimaStr;
                ws.Cell(row, 7).Value  = r.ErrorCambioDeModo;
                ws.Cell(row, 8).Value  = r.OperacionesConError;
                ws.Cell(row, 9).Value  = r.ErrorAlmacenaje;
                ws.Cell(row, 10).Value = r.Depositos;
                ws.Cell(row, 11).Value = r.ContandoAlmacenado;
                ws.Cell(row, 12).Value = r.UsuariosStr;
                ws.Cell(row, 13).Value = r.UsuariosCount;
                ws.Cell(row, 14).Value = r.Recolecciones;

                // Alternate row color
                if (row % 2 == 0)
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#D6E4F0");

                row++;
            }

            ws.RangeUsed()?.SetAutoFilter();
            ws.Columns().AdjustToContents();
            wb.SaveAs(path);
        }
    }
}
