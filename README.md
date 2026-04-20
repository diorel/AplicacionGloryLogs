# AppGlory

AppGlory es una aplicación de escritorio desarrollada en C# con WPF que analiza archivos de log del equipo **DE100P** de Glory Global Solutions México. Extrae y contabiliza eventos clave agrupados por número de serie y versión de software, y exporta los resultados a Excel con un solo clic.

---

## Descripción del Proyecto

### Finalidad
Los equipos DE100P generan archivos de log continuamente durante su operación. Revisar esos archivos de forma manual para detectar errores, contar depósitos o identificar reinicios es una tarea lenta y propensa a errores. AppGlory automatiza ese proceso: el usuario selecciona uno o varios archivos de log, la aplicación los analiza en segundos y presenta un resumen estructurado listo para revisar o exportar.

### Funcionalidades
- Selección de **uno o múltiples archivos** de log (`.txt` / `.log`) de forma simultánea
- Extracción automática de:
  - Número de serie del equipo (soporta 3 formatos distintos en el log)
  - Versión del software instalado
- Conteo de eventos por combinación única de **Archivo + Serie + Versión**:
  - Reinicios / encendidos del equipo
  - Depósitos exitosos
  - Operaciones con error
  - Errores de cambio de modo
  - Errores de almacenaje
  - Fases de conteo y almacenaje
  - Recolecciones
  - Usuarios distintos que realizaron operaciones (vía CIRREON)
- Visualización de resultados en tabla interactiva con fechas de primera y última aparición
- Exportación a **Excel (`.xlsx`)** con formato profesional: encabezados estilizados, filas alternas, filtros automáticos y columnas ajustadas al contenido
- Análisis asíncrono — la interfaz **no se congela** mientras se procesan archivos grandes

### Tecnologías Utilizadas
| Tecnología | Versión | Uso |
|---|---|---|
| C# | 12 | Lenguaje principal |
| .NET | 8.0 | Runtime de la aplicación |
| WPF (Windows Presentation Foundation) | — | Interfaz gráfica de escritorio |
| ClosedXML | 0.102.3 | Generación de archivos Excel sin necesidad de Office |
| Regex (System.Text.RegularExpressions) | — | Extracción de patrones en los logs |
| MVVM | — | Patrón de arquitectura (Model - View - ViewModel) |

---

## Requisitos del Sistema

### Para ejecutar el instalador / ejecutable publicado
- **Sistema Operativo:** Windows 10 / Windows 11 (64 bits)
- **Runtime:** [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/es-es/download/dotnet/8.0) *(x64)*
  > Si el equipo no tiene .NET 8 instalado, Windows lo solicitará automáticamente al abrir la aplicación, o puede descargarlo desde el enlace anterior.
- **Microsoft Office:** NO requerido — la exportación a Excel funciona sin tener Office instalado

### Para compilar desde el código fuente
- **Visual Studio 2022** (v17.8 o superior) con la carga de trabajo **"Desarrollo de escritorio de .NET"**
  — O bien —
- **.NET 8.0 SDK** instalado y cualquier editor (VS Code, Rider, etc.)
- Conexión a internet (primera vez, para restaurar el paquete NuGet `ClosedXML`)

---

## Instalación y Ejecución

### Opción A — Ejecutar el binario publicado

1. Descarga la carpeta `publish/` del repositorio (o el archivo `.zip` de la release)
2. Extrae el contenido en cualquier carpeta de tu equipo
3. Ejecuta `AppGlory.exe`
4. Si Windows solicita instalar .NET 8.0 Runtime, acepta e instálalo, luego vuelve a abrir el ejecutable

> No requiere instalación adicional. La aplicación es portable.

---

### Opción B — Compilar y ejecutar con Visual Studio

1. Clona o descarga el repositorio:
   ```
   git clone https://github.com/tu-usuario/AppGlory.git
   ```
2. Abre el archivo `AppGlory.sln` con **Visual Studio 2022**
3. Visual Studio restaurará los paquetes NuGet automáticamente
4. Presiona **F5** para compilar y ejecutar en modo Debug
   — O presiona **Ctrl + F5** para ejecutar sin depurador

---

### Opción C — Compilar y ejecutar con .NET CLI

1. Clona o descarga el repositorio
2. Abre una terminal en la carpeta `AppGlory/AppGlory/`
3. Restaura dependencias y ejecuta:
   ```bash
   dotnet restore
   dotnet run
   ```
4. Para generar un ejecutable publicado:
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained false -o ./publish
   ```
   El ejecutable quedará en la carpeta `./publish/`

---

## Uso Rápido

1. Abre la aplicación
2. Haz clic en **"Seleccionar Archivos"** y elige uno o varios archivos de log del DE100P
3. Haz clic en **"Analizar"** — los resultados aparecerán en la tabla
4. (Opcional) Haz clic en **"Exportar a Excel"** para guardar el reporte en `.xlsx`

---

## Estructura del Proyecto

```
AppGlory/
├── AppGlory.sln
└── AppGlory/
    ├── AppGlory.csproj
    ├── App.xaml / App.xaml.cs
    ├── MainWindow.xaml / MainWindow.xaml.cs
    ├── Models/
    │   └── LogRecord.cs
    ├── Services/
    │   ├── LogParser.cs
    │   └── ExcelExporter.cs
    ├── Commands/
    │   └── RelayCommand.cs
    └── ViewModels/
        └── MainViewModel.cs
```
