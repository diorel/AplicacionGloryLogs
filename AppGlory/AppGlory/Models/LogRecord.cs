namespace AppGlory.Models
{
    public class LogRecord
    {
        public string Archivo { get; set; } = "";
        public string NumeroSerie { get; set; } = "";
        public string Version { get; set; } = "N/A";
        public DateTime? FechaPrimera { get; set; }
        public int VecesAparecio { get; set; }
        public DateTime? FechaUltima { get; set; }
        public int ErrorCambioDeModo { get; set; }
        public int OperacionesConError { get; set; }
        public int ErrorAlmacenaje { get; set; }
        public int Depositos { get; set; }
        public int ContandoAlmacenado { get; set; }
        public HashSet<string> Usuarios { get; set; } = new();
        public int Recolecciones { get; set; }

        public string FechaPrimeraStr => FechaPrimera?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        public string FechaUltimaStr  => FechaUltima?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        public string UsuariosStr     => string.Join(", ", Usuarios);
        public int    UsuariosCount   => Usuarios.Count;
    }
}
