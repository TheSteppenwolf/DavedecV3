namespace DavedecV3.Models;

public class Documento
{
    #region Atributos
    
    private string _nombre;
    private string _filePath;
    private string _formato;
    private string _tipo;
    private bool _estado;
    private List<string> _datos;
    private InformeGeneral _informeGeneral;
    private List<Error> _errores;
    private string _columnas;

    #endregion

    #region Propiedades    
    public string Nombre { get => _nombre; set { _nombre = value; } }
    public string FilePath { get => _filePath; set { _filePath = value; } }
    public string Formato { get => _formato; set { _formato = value; } }
    public string Tipo { get => _tipo; set { _tipo = value; } }
    public bool Estado { get => _estado; set { _estado = value; } }    
    public List<string> Datos { get => _datos; set { _datos = value; } }
    public InformeGeneral InformeGeneral { get => _informeGeneral; set { _informeGeneral = value; } }
    public List<Error> Errores { get => _errores; set { _errores = value; } }
    public string Columnas { get => _columnas; set { _columnas = value; } }

    #endregion

    public Documento()
    {
        Datos = new List<string>();
        Errores = new List<Error>();
        Nombre = "No asignado";
        Estado = false;
        InformeGeneral = new InformeGeneral();
        Columnas = "";
    }

    #region Métodos: Setters

    /// <summary>
    /// Establece los datos preliminares del documento
    /// </summary>
    /// <param name="p_nombre"></param>
    /// <param name="p_formato"></param>
    /// <param name="p_tipo"></param>
    public void EstablecerDatosPreliminares(string p_nombre, string p_formato, string p_tipo)
    {
        Nombre = p_nombre;
        Formato = p_formato;
        Tipo = p_tipo;      
    }

    /// <summary>
    /// Establece el filepath del documento
    /// </summary>
    /// <param name="p_filepath"></param>
    public void EstablecerFilePath(string p_filepath)
    {
        FilePath = p_filepath;
    }

    public string GetErrores()
    {
        string resultado = "";
        foreach(var error in Errores)
        {
            resultado += error.GetError() + "\n";            
        }
        return resultado;
    }

    #endregion
}
