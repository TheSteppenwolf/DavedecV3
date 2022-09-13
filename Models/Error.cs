namespace DavedecV3.Models;

public class Error
{
    #region Atributos

    private string _id;
    private string _tipo;
    private string _mensaje;
    private string _sugenrecia;
    private string _campo;
    private string _contenido;

    #endregion

    #region Propiedades

    public string Id { get { return _id; } set { _id = value; } }
    public string Tipo { get { return _tipo; } set { _tipo = value; } }
    public string Mensaje { get { return _mensaje; } set { _mensaje = value; } }
    public string Sugerencia { get { return _sugenrecia; } set { _sugenrecia = value; } }
    public string Campo { get { return _campo; } set { _campo = value; } }
    public string Contenido { get { return _contenido; } set { _contenido = value; } }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor por defecto
    /// </summary>
    public Error()
    {

    }

    /// <summary>
    /// Constructor por arreglo de strings
    /// </summary>
    /// <param name="id_error"></param>
    /// <param name="ubicacion"></param>
    public Error(string id_error, string[] ubicacion)
    {
        Id = id_error;
        Campo = String.IsNullOrEmpty(ubicacion[0]) || ubicacion[0] == " " ? " " : ubicacion[0];
        Contenido = String.IsNullOrEmpty(ubicacion[1]) || ubicacion[1] == " " ? " " : ubicacion[1];        
        GenerarError();
        Sugerencia = GenerarRecomendacion();
        Mensaje = String.IsNullOrEmpty(Mensaje) || Mensaje == " " ? "No aplica" : Mensaje;
        Sugerencia = String.IsNullOrEmpty(Sugerencia) ? "No aplica" : Sugerencia;
    }

    /// <summary>
    /// Constructor por strings separadas
    /// </summary>
    /// <param name="id_error"></param>
    /// <param name="campo"></param>
    /// <param name="contenido"></param>
    public Error(string id_error, string campo, string contenido)
    {
        Id = id_error;
        Campo = String.IsNullOrEmpty(campo) || Campo == " " ? " ":contenido;
        Contenido = String.IsNullOrEmpty(contenido) || Contenido == " " ? " ":contenido;        
        GenerarError();
        if (id_error != "DDv7") Sugerencia = GenerarRecomendacion();
        Mensaje = String.IsNullOrEmpty(Mensaje) || Mensaje == " " ? "No aplica" : Mensaje;
        Sugerencia = String.IsNullOrEmpty(Sugerencia) ? "No aplica" : Sugerencia;
    }

    #endregion

    #region Métodos

    /// <summary>
    /// Genera un error en base al id del error.
    /// </summary>
    private void GenerarError()
    {
        if (Id.StartsWith("DD"))
        {
            // Campo obligatorio inexistente
            if (Id == "DDv1")
            {
                Tipo = "Error de campo";
                Mensaje = $"No existe o esta mal escrito el campo obligatorio '{Campo}' establecido en la plantilla para diccionario de datos. Revisa el documento, revisa la plantilla para diccionario de datos y añade o edita el campo.";                
            }
            // Descrión vacía
            else if (Id == "DDv2")
            {
                Tipo = "Error de descripción";
                Mensaje = $"El campo '{Campo}' no tiene ningún contenido, ingresa al documento y añade una descripción al campo.";
            }
            // Descripción de campo menor a 10 caracteres
            else if (Id == "DDv3")
            {
                Tipo = "Error de descripción";
                Mensaje = $"La descripción del campo '{Campo}' no sobrepasa los 10 caracteres de largo, ingresa al documento y redacta una mejor descripción.";
            }
            // Descrión igual al nombre de campo
            else if (Id == "DDv4")
            {
                Tipo = "Error de descripción";
                Mensaje = $"El campo '{Campo}' es idéntico a su descripción, ingresa al documento y redacta una mejor descripción.";
            }
            // Nombre de campo con tílde
            else if (Id == "DDv5")
            {
                Tipo = "Error de nombre de campo";
                Mensaje = $"El nombre campo '{Campo}' contiene tíldes, ¡Remuevelas!";
            }
            // Estructura del documento
            else if (Id == "DDv6")
            {
                Tipo = "Error de estructura";
                Mensaje = $"El documento no posee la estructura correcta para un diccionario de datos según su plantilla. El campo '{Campo}' y/o el campo '{Contenido}' no existen en el diccionario de datos evaluado.";
            }
            // Documento de referencia inválido
            else if (Id == "DDv7")
            {
                Tipo = "Error de referencia";
                Mensaje = $"El documento de referencia especificado en el diccionario de datos no concuerda con el conjunto de datos ingresado.";
                Sugerencia = $"Revisa que el documento de referencia especificado en el diccionario de datos concuerde con el conjunto de datos ingresado. El documento de referencia especificado es: {Campo}, el conjunto de datos ingresado es: {Contenido}";
            }
        }
        else if (Id.StartsWith("PM"))
        {
            if (Id == "PMv1")
            {
                Tipo = "Error de campo";
                Mensaje = $"No existe o esta mal escrito el campo obligatorio '{Campo}' establecido en la plantilla para diccionario de datos. Revisa el documento, revisa la plantilla para diccionario de datos y añade o edita el campo.";
            }
            else if (Id == "PMv2")
            {
                Tipo = "Error de campo";
                Mensaje = $"El campo '{Campo}' no tiene ningún contenido, ingresa al documento y añade una descripción al campo.";
            }
            else if (Id == "PMv3")
            {
                Tipo = "Error de campo";
                Mensaje = $"La descripción del campo '{Campo}' no sobrepasa los 10 caracteres de largo, ingresa al documento y redacta una mejor descripción.";
            }
            else if (Id == "PMv4")
            {
                Tipo = "Error de contenido";
                Mensaje = $"El contenido del campo '{Campo}' no es una dirección web válida, ingresa al documento y añade una dirección web válida.";
            }
            else if (Id == "PMv5")
            {
                Tipo = "Error de formato";
                Mensaje = $"El contenido del campo '{Campo}' no posee una fecha válida, ingresa al documento y modifica la fecha en el formato correcto.";
            }
            else if (Id == "PMv6")
            {
                Tipo = "Error de tipo";
                Mensaje = $"El contenido del campo '{Campo}' no posee un tipo de frecuencia válida, ingresa al documento y coloca una frecuencia correcta.";
            }
            else if (Id == "PMv7")
            {
                Tipo = "Error de tipo";
                Mensaje = $"El contenido del campo '{Campo}' no posee un tipo de anonimización válido, ingresa al documento y coloca un tipo de anonimización correcto.";
            }
            else if (Id == "PMv8")
            {
                Tipo = "Error de contenido";
                Mensaje = $"El contenido del campo '{Campo}' no es una dirección de correo electrónico, ingresa al documento y añade una dirección de correo electrónico válido.";
            }
            else if (Id == "PMv9")
            {
                Tipo = "Error de contenido";
                Mensaje = $"El contenido del campo '{Campo}' no es una dirección web válida, ingresa al documento y añade una dirección web válida.";
            }
        }
        else
        {
            if (Campo == "Cantidad de registros")
            {
                Tipo = "Estadística general";
                Mensaje = $"Representa al total de registros que el documento posee.";
            }
            else if (Campo == "Cantidad de errores")
            {
                Tipo = "Estadística general";
                Mensaje = $"Representa al total de errores que se reportaron en el documento.";
            }
            else if (Campo == "Proporción de errores")
            {
                Tipo = "Estadística general";
                Mensaje = $"Indica cuanto representa la cantidad de errores del documento con respecto a su total de registros.";
            }
            else if (Campo == "Cantidad de registros vacíos")
            {
                Tipo = "Estadística general";
                Mensaje = $"Representa al total de registros vacíos del documento.";
            }
            else if (Campo == "Cantidad de columnas")
            {
                Tipo = "Estadística general";
                Mensaje = $"Representa a la cantidad de columnas que el documento posee.";
            }
            else if (Campo.StartsWith("Cantidad de errores de la columna"))
            {
                Tipo = "Estadística de columna";
                Mensaje = $"Representa la cantidad de errores encontrados en la columna.";
            }
            else if (Campo.StartsWith("Cantidad de valores vacíos de la columna"))
            {
                Tipo = "Estadística de columna";
                Mensaje = $"Representa la cantidad de registros vacíos encontrados en la columna.";
            }
        }
    }

    /// <summary>
    /// Genera una recomendación en base al nombre del campo
    /// </summary>
    /// <returns></returns>
    private string GenerarRecomendacion()
    {
        string recomendacion = $"El campo '{Campo}' se refiere a ";
        if (Id == "ConjuntoDatos") recomendacion = "Abre el conjunto de datos y revisa el contenido.";
        else if (Id == "Columnas") recomendacion = "Revisa el conjunto de datos y analiza la columna presentada.";
        else if (Campo == "Institución") recomendacion += "la institución a quien pertenece el documento.";
        else if (Campo == "Identificador" && Tipo.StartsWith("DD")) recomendacion += "el código del diccionario de datos.";
        else if (Campo == "Documento Referencia" && Tipo.StartsWith("DD")) recomendacion += "el nombre del archivo de datos.";
        else if (Campo == "Nombre" && Tipo.StartsWith("DD")) recomendacion += "el nombre o título del conjunto de datos.";
        else if (Campo == "Descripción" && Tipo.StartsWith("DD")) recomendacion += "la descripción del conjunto de datos.";
        else if (Campo == "Institución") recomendacion += "la institución a quien pertenece el documento.";
        else if (Campo == "Identificador" && Tipo.StartsWith("PM")) recomendacion += "el código del metadato.";
        else if (Campo == "Documento Referencia" && Tipo.StartsWith("PM")) recomendacion += "el nombre del archivo de datos.";
        else if (Campo == "Nombre" && Tipo.StartsWith("PM")) recomendacion += "el nombre o tíitulo del conjunto de datos.";
        else if (Campo == "Descripción" && Tipo.StartsWith("PM")) recomendacion += "la descripción del conjunto de datos.";
        else if (Campo == "Código institución") recomendacion += "la dirección web de la institución. (www.instituciónejemplo.gob.ec)";
        else if (Campo == "Licencia") recomendacion += "la licencia de archivo de datos.";
        else if (Campo == "Fecha de creación") recomendacion += "la fecha de creación del archivo de datos. Debe estar en el siguiente formato: (AAAA-MM-DD)";
        else if (Campo == "Frecuencia de actualización") recomendacion += "la frecuencia de actualización del archivo de datos. Las frecuencias permitidas son: - Tiempo real \t- Diaria \t- Semanal \t- Mensual \t- Anual";
        else if (Campo == "Anonimización") recomendacion += "el estado de anonimización del archivo de datos.";
        else if (Campo == "Responsable") recomendacion += "el correo del funcionario dentro de la institución encargado.";
        else if (Campo == "Lenguaje") recomendacion += "el lenguaje del archivo de datos.";
        else if (Campo == "Etiquetas") recomendacion += "las etiquetas referentes al archivo de datos.";
        else if (Campo == "URI") recomendacion += "la dirección web donde se puede ubicar el archivo de datos.";
        else if (Tipo == "DDv6") recomendacion = $"Añade el campo '{Campo}' seguido del campo '{Contenido}' al diccionario de datos.";        
        else recomendacion = "";
        return recomendacion;
    }

    /// <summary>
    /// Devuelve un error como una string con todos los datos separados por un |
    /// </summary>
    /// <returns></returns>
    public string GetError()
    {
        return $"{Id}|{Tipo}|{Campo}|{Contenido}|{Mensaje}|{Sugerencia}";
    }

    #endregion
}
