using DavedecV3.Models;
using Microsoft.AspNetCore.Components.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics;
using System.Collections.ObjectModel;
using DocumentFormat.OpenXml.Wordprocessing;
using DavedecV3.Pages;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace DavedecV3.Data;

public class DocumentoService
{
    #region Atributos

    // Core
    private Documento _conjuntoDatos;
    private Documento _diccionarioDatos;
    private Documento _metadatos;

    // Generales para evaluaciones
    string alfabeto = "abcdefghijklmnñopqstuvwxyz";
    string numeros = "1234567890";
    string simbolos_permitidos = "+-*/% ,.;:";
    string meses = "Enero,Febrero,Marzo,Abril,Mayo,Junio,Julio,Agosto,Septiembre,Octubre,Noviembre,Diciembre";

    #endregion

    #region Propiedades

    public Documento ConjuntoDatos { get => _conjuntoDatos; set { _conjuntoDatos = value; } }
    public Documento DiccionarioDatos { get => _diccionarioDatos; set { _diccionarioDatos = value; } }
    public Documento Metadatos { get => _metadatos; set { _metadatos = value; } }

    #endregion

    #region Constructor

    public DocumentoService()
    {
        ConjuntoDatos = new Documento();
        DiccionarioDatos = new Documento();
        Metadatos = new Documento();
    }

    #endregion

    #region Métodos: Setters 
    
    /// <summary>
    /// Establece todos los documentos
    /// </summary>
    /// <param name="p_conjuntodatos"></param>
    /// <param name="p_diccionariodatos"></param>
    /// <param name="p_metadatos"></param>
    public void EstablecerDocumentos(string p_conjuntodatos, string p_diccionariodatos, string p_metadatos)
    {
        EstablecerFilepaths(p_conjuntodatos, p_diccionariodatos, p_metadatos);
        EstablecerNombres(p_conjuntodatos, p_diccionariodatos, p_metadatos);
        EstablecerDatos();
        RealizarEvaluaciones();
        GenerarDocumentosError();
    }

    public void EstablecerNombres(string p_conjuntodatos, string p_diccionariodatos, string p_metadatos)
    {
        ConjuntoDatos.Nombre = p_conjuntodatos;
        DiccionarioDatos.Nombre = p_diccionariodatos;
        Metadatos.Nombre = p_metadatos;
    }


    public void EstablecerFilepaths(string p_conjuntodatos, string p_diccionariodatos, string p_metadatos)
    {
        // Establecimiento de filepaths
        ConjuntoDatos.EstablecerFilePath(p_conjuntodatos);
        ConjuntoDatos.Formato = "xlsx";
        DiccionarioDatos.EstablecerFilePath(p_diccionariodatos);
        DiccionarioDatos.Formato = "xlsx";
        Metadatos.EstablecerFilePath(p_metadatos);
        Metadatos.Formato = "xlsx";
    }

    /// <summary>
    /// Establece los datos para cada documento
    /// </summary>
    public void EstablecerDatos()
    {
        // Establecer datos para el conjunto de datos
        EstablecerDatosCD();
        // Establecer datos para el diccionario de datos
        EstablecerDatosDD();
        // Establecer datos el metadato
        EstablecerDatosPM();
    }

    /// <summary>
    /// Establece los datos del conjunto de datos
    /// </summary>    
    public void EstablecerDatosCD() 
    {
        // Leemos los datos de evaluación del documento txt
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Datin Revisa",
            "error_data",
            "CDerrors.txt");
        // Se establece el conjunto de datos
        string[] lines = System.IO.File.ReadAllLines(path);
        string[] resultados = new string[5];
        int count = 0;
        foreach (var line in lines)
        {
            string[] temp = line.Split("|");
            resultados[count++] = temp[1].ToString();
            if (count == 5) break;
        }
        CDEstablecerInformeGeneral(resultados);
    }

    /// <summary>
    /// Establece los datos del diccionario de datos
    /// </summary>
    public void EstablecerDatosDD() 
    {           
        if (DiccionarioDatos.Formato == "xlsx")
        {
            try
            {
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(DiccionarioDatos.FilePath, false))
                {
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    SharedStringTable sst = sstpart.SharedStringTable;

                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    Worksheet sheet = worksheetPart.Worksheet;

                    var cells = sheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                    var rows = sheet.Descendants<Row>();

                    DiccionarioDatos.InformeGeneral.CantidadRegistros = cells.Count();

                    // One way: go through each cell in the sheet
                    string contenido = "";
                    string temp = "";
                    int contador = 1;
                    foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in cells)
                    {
                        temp = cell.CellReference;
                        if (temp[0] == 'A' && contador == 1)
                        {
                            if (((cell.DataType != null) && (cell.DataType == CellValues.SharedString)))
                            {
                                int ssid = int.Parse(cell.CellValue.Text);
                                contenido = sst.ChildElements[ssid].InnerText;
                                contenido += "|";
                            }
                            else if (cell.CellValue != null)
                            {
                                contenido = "|";
                            }
                        }
                        else if (temp[0] == 'B')
                        {
                            if (((cell.DataType != null) && (cell.DataType == CellValues.SharedString)))
                            {
                                int ssid = int.Parse(cell.CellValue.Text);
                                contenido += sst.ChildElements[ssid].InnerText;
                                DiccionarioDatos.Datos.Add(contenido);
                            }
                            else if (cell.CellValue != null)
                            {
                                contenido += " ";
                                DiccionarioDatos.Datos.Add(contenido);
                            }
                            contador = 0;
                        }
                        else
                        {
                            contenido += " ";
                            DiccionarioDatos.Datos.Add(contenido);
                            if (((cell.DataType != null) && (cell.DataType == CellValues.SharedString)))
                            {
                                int ssid = int.Parse(cell.CellValue.Text);
                                contenido = sst.ChildElements[ssid].InnerText;
                                contenido += "|";
                            }
                            else if (cell.CellValue != null)
                            {
                                contenido = "|";
                            }
                            contador = 0;
                        }
                        contador++;
                    }
                    doc.Close();
                }
            }
            catch
            {
                // Error abriendo el documento
            }
        }
        else if(DiccionarioDatos.Formato == "ods")
        {

        }
        else if(DiccionarioDatos.Formato == "csv")
        {

        }
    }

    /// <summary>
    /// Establece los datos del metadato
    /// </summary>
    public void EstablecerDatosPM() 
    {
        if (Metadatos.Formato == "xlsx")
        {
            try
            {
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(Metadatos.FilePath, false))
                {
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    SharedStringTable sst = sstpart.SharedStringTable;

                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    Worksheet sheet = worksheetPart.Worksheet;

                    var cells = sheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>();
                    var rows = sheet.Descendants<Row>();

                    Metadatos.InformeGeneral.CantidadRegistros = cells.Count();

                    // One way: go through each cell in the sheet
                    string contenido = "";
                    string temp = "";
                    int contador = 1;
                    foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in cells)
                    {
                        temp = cell.CellReference;
                        if (temp[0] == 'A' && contador == 1)
                        {
                            if (((cell.DataType != null) && (cell.DataType == CellValues.SharedString)))
                            {
                                int ssid = int.Parse(cell.CellValue.Text);
                                contenido = sst.ChildElements[ssid].InnerText;
                                contenido += "|";
                            }
                            else if (cell.CellValue != null)
                            {
                                contenido = "|";
                            }
                        }
                        else if (temp[0] == 'B')
                        {
                            if (((cell.DataType != null) && (cell.DataType == CellValues.SharedString)))
                            {
                                int ssid = int.Parse(cell.CellValue.Text);
                                contenido += sst.ChildElements[ssid].InnerText;
                                Metadatos.Datos.Add(contenido);
                            }
                            else if(cell.CellValue != null)
                            {
                                contenido += " ";
                                Metadatos.Datos.Add(contenido);
                            }
                            contador = 0;
                        }
                        else
                        {
                            contenido += " ";
                            Metadatos.Datos.Add(contenido);
                            if (((cell.DataType != null) && (cell.DataType == CellValues.SharedString)))
                            {
                                int ssid = int.Parse(cell.CellValue.Text);
                                contenido = sst.ChildElements[ssid].InnerText;
                                contenido += "|";
                            }
                            else if (cell.CellValue != null)
                            {
                                contenido = "|";
                            }
                            contador = 0;
                        }
                        contador++;                        
                    }
                    doc.Close();
                }
            }
            catch
            {
                // Error abriendo el documento
            }
        }
        else if (Metadatos.Formato == "ods")
        {

        }
        else if (Metadatos.Formato == "csv")
        {

        }
    }

    #endregion

    #region Métodos: Getters

    public List<Documento> GetDocumentos()
    {
        List<Documento> salida = new List<Documento>();
        salida.Add(ConjuntoDatos);
        salida.Add(DiccionarioDatos);
        salida.Add(Metadatos);
        return salida;
    }

    #endregion

    #region Métodos: Verificadores

    /// <summary>
    /// Verifica la nomenclatura del documento ingresado, devuelve true si es válido.
    /// id -> 1: Conjunto de datos, 2: Diccionario de datos, 3: Metadatos
    /// </summary>
    /// <param name="id"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public bool VerificarNomenclatura(int id, string filename)
    {
        // Tomamos unicamente el nombre del archivo
        filename = filename.Split(".")[0];
        // Tomamos datos del nombre
        string[] datos = filename.Split("_");
        // Variable temporal para procesamientos
        string temp = "";
        string temporal = "";
        // Variable para poder completar algoritmos de verificación
        bool verificador = false;

        // Validaciones generales de nomenclatura
        // Si el nombre tiene caracteres no permitidos
        foreach (var iter in filename.ToLower())
        {
            foreach (var letter in alfabeto + numeros + simbolos_permitidos)
                if (iter == letter) { verificador = true; break; }
            if (verificador == false) return verificador;
        }

        // Si el nombre no cumple con el formato SIGLAS_Nombre_AñoMes, especializado en el uso de "_"
        if (datos.Length < 3 || datos.Length > 4) return false;

        // Si las siglas de la entidad no estan en mayúsculas
        foreach (var iter in datos[0]) if (!Char.IsUpper(iter)) return false;

        // Si la primera letra del nombre del conjunto de datos no es mayúscula
        if (!Char.IsUpper(datos[1][0])) return false;


        // Validar nomenclatura conjunto de datos
        if (id == 1)
        {
            // Verificamos que cumpla el formato AñoMes para el conjunto de datos
            temp = "";
            verificador = false;
            for (int i = 0; i < datos[2].Length; i++)
                if (i < 4 && !char.IsNumber(datos[2][i])) return false; else if (i > 3) temp += datos[2][i];
            temporal = new string(temp);
            foreach (var mes in meses.Split(",")) if (temporal == mes) { verificador = true; break; }
            if (verificador == false) return verificador;
        }
        else
        {
            // Verificamos si se ingreso algo diferente a un diccionario de datos o metadato
            if (datos.Length < 4) return false;
            // Verificamos que cumpla el formato AñoMes para el diccionario de datos y el metadato
            temp = "";
            verificador = false;
            for (int i = 0; i < datos[3].Length; i++)
                if (i < 4 && !char.IsNumber(datos[3][i])) return false; else if (i > 3) temp += datos[3][i];
            temporal = new string(temp);
            foreach (var mes in meses.Split(",")) if (temporal == mes) { verificador = true; break; }
            if (verificador == false) return verificador;
        }
        // Validar nomenclatura diccionario de datos
        if (id == 2)
        {
            // Verificamos que tenga su caracteristico DD
            if (!(datos[2] == "DD")) return false;
        }
        // Validar nomenclatura metadato
        else if (id == 3)
        {
            // Verificamos que tenga su caracteristico PM
            if (!(datos[2] == "PM")) return false;
        }
        return true;
    }

    /// <summary>
    /// Verifica que el formato del archivo sea uno permitido
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public bool VerificarFormato(string filename)
    {
        if (filename.EndsWith(".xlsx") || filename.EndsWith(".csv") || filename.EndsWith(".ods")) return true;
        return false;
    }

    /// <summary>
    /// Verifica si un contenido es una url web
    /// </summary>
    /// <param name="p_contenido"></param>
    /// <returns></returns>
    private bool VerificarUrl(string p_contenido)
    {
        bool verificador = false;
        if (p_contenido.StartsWith("http") || p_contenido.StartsWith("www") || p_contenido.EndsWith(".com") || p_contenido.EndsWith(".ec") || p_contenido.EndsWith(".org") || p_contenido.EndsWith(".edu"))
            verificador = true;
        return verificador;
    }

    /// <summary>
    /// Verifica que el contenido sea un correo electronico válido
    /// </summary>
    /// <param name="p_contenido"></param>
    /// <returns></returns>
    private bool VerificarCorreo(string p_contenido)
    {
        bool verificador = false;
        try
        {
            p_contenido.IndexOf("@");
            verificador = true;
        }
        catch { }
        return verificador;
    }

    /// <summary>
    /// Verifica que el contenido tenga una frecuancia válida.
    /// </summary>
    /// <param name="p_contenido"></param>
    /// <param name="frecuencias"></param>
    /// <returns></returns>
    private bool VerificarFrecuencia(string p_contenido, string[] frecuencias)
    {
        bool verificador = false;
        foreach (var iter in frecuencias) if (p_contenido == iter) { verificador = true; break; }
        return verificador;
    }

    /// <summary>
    /// Verifica si el contenido posee una fecha válida
    /// </summary>
    /// <param name="p_contenido"></param>
    /// <returns></returns>
    private bool VerificarFecha(string p_contenido)
    {
        bool verificador = false;
        string formato = "yyyy/MM/d";
        try
        {
            DateTime date = DateTime.ParseExact(p_contenido, formato, System.Globalization.CultureInfo.InvariantCulture);
            verificador = true;
        }
        catch { }
        return verificador;
    }

    #endregion

    #region Evaluadores

    /// <summary>
    /// Ejecuta un script de python que genera documentos .txt con los resultados de la evaluación
    /// </summary>
    public Task GenerarEvaluacion(int id = 0)
    {
        // 1) Create process info
        var psi = new ProcessStartInfo();
        psi.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Datin Revisa",
            "python",
            "python.exe");

        // 2) Provide script and arguments        
        var script = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Datin Revisa",
                "python",
                "proceso_toexcel.py");
        var resultpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Datin Revisa",
                "result_data");
        var errorpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Datin Revisa",
                "error_data");

        string conjuntodatos = "";
        string diccionariodatos = "";
        string metadatos = "";

        if(id == 0)
        {
            conjuntodatos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Datin Revisa",
                    "temp_data",
                    "temp_cd",
                    ConjuntoDatos.Nombre);
            diccionariodatos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Datin Revisa",
                    "temp_data",
                    "temp_dd",
                    DiccionarioDatos.Nombre);
            metadatos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Datin Revisa",
                    "temp_data",
                    "temp_pm",
                    Metadatos.Nombre);            
        }
        else if (id == 1)
        {
            conjuntodatos = ConjuntoDatos.Nombre;
            diccionariodatos = DiccionarioDatos.Nombre;
            metadatos = Metadatos.Nombre;
        }

        // Se mandan los argumentos
        psi.Arguments = $"\"{script}\" \"{resultpath}\" \"{errorpath}\" \"{conjuntodatos}\" \"{diccionariodatos}\" \"{metadatos}\"";

        // 3) Process configuration
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        // 4) Execute process and get output
        var errors = "";
        var results = "";

        using (var process = Process.Start(psi))
        {
            errors = process.StandardError.ReadToEnd();
            results = process.StandardOutput.ReadToEnd();
        }
        if (errors.Count() > 0) throw new Exception();
        return Task.CompletedTask;
    }    

    public void RealizarEvaluaciones()
    {
        CDRealizarEvaluacion();
        DDRealizarEvaluacion();
        PMRealizarEvaluacion();
    }

    /// <summary>
    /// Realiza la evaluación del diccionario de datos
    /// </summary>
    public void CDRealizarEvaluacion()
    {
        // No realiza nada ya que no es necesario
    }

    /// <summary>
    /// Realiza la evaluación del diccionario de datos
    /// </summary>
    public void DDRealizarEvaluacion()
    {
        // Preparamos información
        string[] obligatorios = { "Institución", "Identificador", "Documento Referencia", "Nombre", "Descripción" };
        bool verificador_struct = false;
        bool verificador = false;
        List<string> temp_list = new List<string>();
        string[] temp = new string[2];

        // Verifica que la estructura del documento sea la correcta y documento de referencia
        foreach (var iter in DiccionarioDatos.Datos)
        {
            string[] temporal = iter.Split("|");
            if (temporal[0] == "Nombre del campo")
            {
                verificador_struct = true;
                break;
            }                
            if (temporal[0] == "Documento Referencia")
            {
                int temp_length = ConjuntoDatos.Nombre.Split("\\").Length;
                string ref_doc = ConjuntoDatos.Nombre.Split("\\")[temp_length - 1].Split(".")[0];
                if (temporal[1].ToLower() != ref_doc.ToLower())
                {
                    DDEstablecerError("DDv7", temporal[1], ref_doc);
                }                           
            }
        }
            
        if(verificador_struct == true)
        {
            foreach (var iter in obligatorios) temp_list.Add(iter);            
            foreach(var iter in DiccionarioDatos.Datos)
            {
                var campo_valor = iter.Split("|");
                if (campo_valor[0] == "Nombre del campo") break;
                foreach (var obligatorio in obligatorios)
                {
                    if (campo_valor[0] == obligatorio) 
                    {
                        verificador = true;
                        temp_list.Remove(obligatorio);
                        break; 
                    }
                }
            }
            foreach (var iter in temp_list)
            {
                temp[0] = iter;
                temp[1] = "";
                DDEstablecerError("DDv1", temp);
            }

            // Comienza la verificación        
            foreach (var iter in DiccionarioDatos.Datos)
            {
                //string tildes = "áéíóú";
                var campo_valor = iter.Split("|");
                verificador = false;

                // Verificamos que el nombre del campo relacionado con columnas del conjunto de datos no tenga tíldes
                //foreach (var letter in campo_valor[0].ToLower())
                //    foreach (var item in tildes) if (item == letter)
                //        {
                //            if (campo_valor[0] != "Institución" && campo_valor[0] != "Descripción")
                //                DDEstablecerError("DDv5", campo_valor);
                //        }

                // Verificamos que el contenido de los campos no este vacío
                if (campo_valor[1] == "" || campo_valor[1] == " ") DDEstablecerError("DDv2", campo_valor);
                // Verificamos que el contenido sea mayor a 10 caracteres
                if (campo_valor[1].Length < 10) DDEstablecerError("DDv3", campo_valor);
                // Verificamos que el nombre de campo se diferente de su descripción
                if (campo_valor[0].ToLower() == campo_valor[1].ToLower()) DDEstablecerError("DDv4", campo_valor);
            }
        }
        else
        {
            temp[0] = "Nombre del campo";
            temp[1] = "Descripción del campo";
            DDEstablecerError("DDv6", temp);
        }
        // Genera un informe general de errores
        DDEstablecerInformeGeneral();
    }

    /// <summary>
    /// Realiza la evaluación del metadato
    /// </summary>
    public void PMRealizarEvaluacion()
    {
        // Preparamos la información
        string[] obligatorios =
        {
            "Institución",
            "Identificador",
            "Documento Referencia",
            "Nombre",
            "Descripción",
            "Código institución",
            "Licencia",
            "Fecha de creación",
            "Frecuencia de actualización",
            "Anonimización",
            "Responsable",
            "Lenguaje",
            "Etiquetas",
            "URI"
        };
        string[] frecuencias =
        {
            "Tiempo real",
            "Diaria",
            "Semanal",
            "Mensual",
            "Anual"
        };
        // Almacenra los campos obligatorios inexistentes
        List<string> inexistentes = new List<string>();
        inexistentes = obligatorios.ToList();
        string[] temp = new string[2];

        // Comienza la evaluación
        foreach (var iter in Metadatos.Datos)
        {
            var campo_valor = iter.Split("|");
            temp = new string[2];

            // Verificamos que los campos obligatorios existan
            for (int i = 0; i < obligatorios.Length; i++)
            {
                if (campo_valor[0] == obligatorios[i])
                {
                    // Eliminamos cualquier campo obligatorio encontrado
                    inexistentes.Remove(obligatorios[i]);
                    // Verificamos que el contenido del campo obligatorio tenga contenido
                    if (campo_valor[1] == " ") PMEstablecerError("PMv2", campo_valor);
                    // Verificamos que el contenido de la descripción se mayor a 10 caracteres
                    else if (campo_valor[0] == "Descripción" && campo_valor[1].Length < 10) PMEstablecerError("PMv3", campo_valor);
                    // Verificamos que el código institución posea una dirección web
                    else if (campo_valor[0] == "Código institución" && !VerificarUrl(campo_valor[1])) PMEstablecerError("PMv4", campo_valor);
                    // Verificamos que la fecha de creación posea una fecha válida en el formato Año-Mes-Día
                    else if (campo_valor[0] == "Fecha de creación" && !VerificarFecha(campo_valor[1])) PMEstablecerError("PMv5", campo_valor);
                    // Verificamos que la frecuencia sea válida
                    else if (campo_valor[0] == "Frecuencia de actualización" && !VerificarFrecuencia(campo_valor[1], frecuencias)) PMEstablecerError("PMv6", campo_valor);
                    // Verificamos que la anonimización sea Sí o No
                    else if (campo_valor[0] == "Anonimización" && (campo_valor[1] != "Sí" || campo_valor[1] != "No")) PMEstablecerError("PMv7", campo_valor);
                    // Verificamos que el responsable tenga un correo válido
                    else if (campo_valor[0] == "Responsable" && !VerificarCorreo(campo_valor[1])) PMEstablecerError("PMv8", campo_valor);
                    // Verificamos que el URI sea un sitio web válido
                    else if (campo_valor[0] == "URI" && !VerificarUrl(campo_valor[1])) PMEstablecerError("PMv9", campo_valor);
                    break;
                }
            }
        }
        foreach (var iter in inexistentes)
        {
            temp[0] = iter; temp[1] = " ";
            PMEstablecerError("PMv1", temp);
        }
        // Genera el informe general de errores
        PMEstablecerInformeGeneral();
    }

    #endregion

    #region Métodos: Helpers

    /// <summary>
    /// Establece y crea el informe general de errores del conjunto de datos
    /// </summary>
    public void CDEstablecerInformeGeneral(string[] resultados)
    {
        InformeGeneral informe = new InformeGeneral(
            Int32.Parse(resultados[0]),
            Int32.Parse(resultados[1]),
            resultados[2],
            Int32.Parse(resultados[3]),
            Int32.Parse(resultados[4])
        );
        ConjuntoDatos.InformeGeneral = informe;
    }

    /// <summary>
    /// Establece y añade un error del diccionario de datos
    /// </summary>
    public void DDEstablecerError(string id_error, string[] ubicacion)
    {
        Error error = new Error(id_error, ubicacion);
        try
        {
            DiccionarioDatos.Errores.Add(error);
        }
        catch
        {
            DiccionarioDatos.Errores = new List<Error>();
            DiccionarioDatos.Errores.Add(error);
        }
    }

    /// <summary>
    /// Estable y añade un error en base a dos strings
    /// </summary>
    /// <param name="id_error"></param>
    /// <param name="campo"></param>
    /// <param name="contenido"></param>
    public void DDEstablecerError(string id_error, string campo, string contenido)
    {
        Error error = new Error(id_error, campo, contenido);
        try
        {
            DiccionarioDatos.Errores.Add(error);
        }
        catch
        {
            DiccionarioDatos.Errores = new List<Error>();
            DiccionarioDatos.Errores.Add(error);
        }
    }

    /// <summary>
    /// Establece y crea el informe general de errores del diccionario de datos
    /// </summary>
    public void DDEstablecerInformeGeneral()
    {
        InformeGeneral informe = new InformeGeneral(DiccionarioDatos.InformeGeneral.CantidadRegistros, DiccionarioDatos.Errores.Count);
        DiccionarioDatos.InformeGeneral = informe;
    }

    /// <summary>
    /// Establece y añade un error del metadato
    /// </summary>
    public void PMEstablecerError(string id_error, string[] ubicacion)
    {
        Error error = new Error(id_error, ubicacion);
        try
        {
            Metadatos.Errores.Add(error);
        }
        catch
        {
            Metadatos.Errores = new List<Error>();
            Metadatos.Errores.Add(error);
        }
    }

    /// <summary>
    /// Establece y crea el informe general de errores del metadato
    /// </summary>
    public void PMEstablecerInformeGeneral()
    {
        InformeGeneral informe = new InformeGeneral(Metadatos.InformeGeneral.CantidadRegistros, Metadatos.Errores.Count);
        Metadatos.InformeGeneral = informe;
    }    

    /// <summary>
    /// Genera documentos txt de errores por tipo de documento
    /// </summary>
    public void GenerarDocumentosError()
    {
        string path;
        // Conjunto de datos
        // No es necesario debido a que el módulo de Python ya realiza la evaluación del conjunto de datos
        //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        //    "Datin Revisa",
        //    "error_data",
        //    "CDerrors.txt");
        //using StreamWriter swCD = new StreamWriter(path);
        //swCD.Write(ConjuntoDatos.GetErrores());
        //swCD.Close();

        // Diccionario de datos
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Datin Revisa",
            "error_data",
            "DDerrors.txt");
        using StreamWriter swDD = new StreamWriter(path);
        swDD.Write(DiccionarioDatos.GetErrores());
        swDD.Close();

        // Metadatos
        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Datin Revisa",
            "error_data",
            "PMerrors.txt");
        using StreamWriter swPM = new StreamWriter(path);
        swPM.Write(Metadatos.GetErrores());
        swPM.Close();
    }

    #endregion
}
