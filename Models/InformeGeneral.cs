namespace DavedecV3.Models;

public class InformeGeneral
{
    #region Atributos
    
    private int _cantidad;          // Cantidad de errores    
    private string _proporcion;     // Proporción de errores    
    private int _vacios;            // Cantidad registros vacíos    
    private double _simbologia;     // Tasa de campos con simbología inapropiada
    private int _columnas;          // Cantidad de columnas
    private int _cantidadRegistros; // Cantidad de registros

    #endregion

    #region Propiedades

    public int Cantidad { get => _cantidad; set { _cantidad = value; } }
    public string Proporcion { get => _proporcion; set { _proporcion = value; } }
    public int Vacios { get => _vacios; set { _vacios = value; } }
    public double Simbologia { get => _simbologia; set { _simbologia = value; } }
    public int Columnas { get => _columnas; set { _columnas = value; } }
    public int CantidadRegistros { get => _cantidadRegistros; set { _cantidadRegistros = value; } }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor por defecto
    /// </summary>
    public InformeGeneral()
    {
        Cantidad = 0;
        Proporcion = "0%";
        Vacios = 0;
        Simbologia = 0;
        Columnas = 0;
        CantidadRegistros = 0;
    }

    /// <summary>
    /// Constructor para Diccionario de datos y metadatos.
    /// </summary>
    /// <param name="total_registros"></param>
    /// <param name="cantidad_errores"></param>
    public InformeGeneral(int total_registros, int cantidad_errores)
    {
        Cantidad = cantidad_errores;
        Proporcion = $"{Math.Round(Convert.ToDouble(cantidad_errores) / Convert.ToDouble(total_registros) * 100),4}%";
    }

    /// <summary>
    /// Constructor para un conjunto de datos
    /// </summary>
    /// <param name="total_registros"></param>
    /// <param name="cantidad_errores"></param>
    /// <param name="vacios"></param>
    /// <param name="simbologia"></param>
    public InformeGeneral(int total_registros, int cantidad_errores, string proporcion, int vacios, int columnas)
    {        
        CantidadRegistros = total_registros;
        Cantidad = cantidad_errores;
        Proporcion = proporcion;
        Vacios = vacios;
        Columnas = columnas;
    }

    #endregion
}