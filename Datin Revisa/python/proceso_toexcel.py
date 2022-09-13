'''
Módulo de Python para facilitar procesos de Datin Revisa

Desarrollado por: Sebastián Felipe Tamayo Proaño
Github: https://github.com/TheSteppenwolf
Fecha: 11/09/2022

Descripción del desarrollo: 
    Tiene un enfoque bastante simple y se enfoca en resolver un problema en especifico, 
    debido a esto se realizo un programa con un paradigma estructurado.

Descripción del módulo:
    Toma la dirección de archivos temporales, los ubica y transforma en documentos tipo
    excel en un ubicación especifica. Adicionalmente, realiza la evaluación del conjunto
    de datos y lo devuelve como resultado del proceso al aplicativo Datin Revisa.    

Parametros de ingreso:
    1. Ubicación de la carpeta result_data
    2. Ubicación de la carpeta error_data
    3. Ubicación del conjunto de datos ingresado
    4. Ubicación del diccionario de datos ingresado
    5. Ubicación del metadato ingresado

Salida:
    1. Evaluación del conjunto de datos.
'''


'''
Librerias requeridas para el procesamiento de los datos
'''
import pandas as pd
import numpy as np
import sys
import shutil

'''
Sección de generación de dataframes
'''
def get_conjuntodatos_df(filepath):
    '''
    Devuelve un dataframe de un archivo de tipo Conjunto de datos
    '''    
    # Si el formato es tipo excel
    if filepath[-4:] == "xlsx":
        # Traemos el archivo de conjunto de datos
        df = pd.read_excel(io=filepath, sheet_name=0)
    
    # Si el formato es tipo open document
    elif filepath[-3:] == "ods":
        # Traemos el archivo de conjunto de datos
        df = pd.read_excel(io=filepath, sheet_name=0, engine="odf")

    # Si el formato es tipo csv
    elif filepath[-3:] == "csv":
        # Intentamos traer el documento separado por comas
        try: 
            # Traemos la información
            df = pd.read_csv(filepath, sep=",", engine="python")
        except:
            # Intentamos traer el documento separado por punto y comas
            try:
                # Traemos la información
                df = pd.read_csv(filepath, sep=";", engine="python")
            except:
                return -1         
    # Se retorna el dataframe   
    return df 

def get_diccionariodatos_df(filepath):
    '''
    Devuelve un dataframe de un archivo de tipo Diccionario de datos
    '''
    # Si el formato es tipo excel
    if filepath[-4:] == "xlsx":
        # Se devuelve -1 para únicamente mover el documento
        df = -1     
    
    # Si el formato es tipo open document
    elif filepath[-3:] == "ods":
        # Traemos el archivo de Diccionario de datos
        df = pd.read_excel(io=filepath, sheet_name=0, header=None, engine="odf")

    # Si el formato es tipo csv
    elif filepath[-3:] == "csv":
        # Intentamos traer el documento separado por comas
        try: 
            # Traemos la información
            df = pd.read_csv(filepath, sep=",", engine="python")
        except:
            # Intentamos traer el documento separado por punto y comas
            try:
                # Traemos la información
                df = pd.read_csv(filepath, sep=";", engine="python")
            except:
                print("aqui")
                return -1   
    # Se retorna el dataframe    
    return df

def get_metadatos_df(filepath):
    '''
    Devuelve un dataframe del archivo de tipo metadatos
    '''
    # Evaluamos el formato del archivo
    formato = filepath.split(".")[-1]
    # Si el formato es tipo excel
    if filepath[-4:] == "xlsx":
        # Se devuelve -1 para únicamente mover el documento
        df = -1;     
    
    # Si el formato es tipo open document
    elif filepath[-3:] == "ods":
        # Traemos el archivo de Diccionario de datos
        df = pd.read_excel(io=filepath, sheet_name=0, header=None, engine="odf")

    # Si el formato es tipo csv
    elif filepath[-3:] == "csv":
        # Intentamos traer el documento separado por comas
        try: 
            # Traemos la información
            df = pd.read_csv(filepath, sep=",", engine="python")
        except:
            # Intentamos traer el documento separado por punto y comas
            try:
                # Traemos la información
                df = pd.read_csv(filepath, sep=";", engine="python")
            except:
                return -1   
    # Se retorna el dataframe    
    return df

'''
Sección de generación de documentos de texto
'''
def generar_conjuntodatos():
    '''
    Genera excel del conjunto de datos
    '''    
    name = conjuntodatos_path.split("\\")[-1].split(".")[0]
    # Path de ubicación donde se guardaran los archivos procesados
    path = result_path + "\\" +  name + ".xlsx"
    conjuntodatos_df.to_excel(path, index=False, encoding="utf8")

def generar_diccionariodatos():
    '''
    Genera excel del diccionario de datos
    '''
    name = diccionariodatos_path.split("\\")[-1].split(".")[0]
    # Path de ubicación donde se guardaran los archivos procesados
    path = result_path + "\\" +  name + ".xlsx"
    diccionariodatos_df.to_excel(path, index=False, header=None, encoding="utf8")

def generar_metadatos():
    '''
    Genera excel del metadato
    '''    
    name = metadatos_path.split("\\")[-1].split(".")[0]
    # Path de ubicación donde se guardaran los archivos procesados
    path = result_path + "\\" +  name + ".xlsx"
    metadatos_df.to_excel(path, index=False, header=None, encoding="utf8")

'''
Sección de copiado de documentos excel
'''

def copiar_conjuntodatos():
    '''
    Copia el documento de excel ingresado a una carpeta especifica
    '''
    name = conjuntodatos_path.split("\\")[-1].split(".")[0]
    path = result_path + "\\" + name + ".xlsx"
    try:
        shutil.copyfile(conjuntodatos_path, path)
    except shutil.SameFileError:
        pass    


def copiar_diccionariodatos():
    '''
    Copia el documento de excel ingresado a una carpeta especifica
    '''
    name = diccionariodatos_path.split("\\")[-1].split(".")[0]
    path = result_path + "\\" + name + ".xlsx"
    try:
        shutil.copyfile(diccionariodatos_path, path)
    except shutil.SameFileError:
        pass    

def copiar_metadatos():
    '''
    Copia el documento de excel ingresado a una carpeta especifica
    '''
    name = metadatos_path.split("\\")[-1].split(".")[0]
    path = result_path + "\\" + name + ".xlsx"
    try:
        shutil.copyfile(metadatos_path, path)
    except shutil.SameFileError:
        pass    

'''
Sección de evaluación del Conjunto de datos
'''
def evaluar_conjuntodatos():
    '''
    Genera un documento txt con un informe de errores del conjunto de datos
    '''
    # Se prepara donde se va a guardar la información
    cantidad_registros = conjuntodatos_df.size
    columnas = []
    registros_columnas = []
    total_errores = 0
    errores_columna = []
    vacios_columna = []
    cantidad_vacios = 0
    proporcion = 0            
    proporcion_columnas = []

    # Se prepara los datos
    conjuntodatos_df.replace("", np.NaN, inplace=True)
    conjuntodatos_df.replace(" ", np.NaN, inplace=True)
    conjuntodatos_df.replace("N/A", np.NaN, inplace=True)
    conjuntodatos_df.replace("nan", np.NaN, inplace=True)
    conjuntodatos_df.replace("SIN VALOR", np.NaN, inplace=True)
    conjuntodatos_df.replace("null", np.NaN, inplace=True)
    conjuntodatos_df.replace("Null", np.NaN, inplace=True)

    # Columnas
    columnas = conjuntodatos_df.columns.tolist()

    # Cantidad de valores vacios
    cantidad_vacios = conjuntodatos_df.isna().sum().sum()
    for columna in columnas:
        vacios_columna.append(conjuntodatos_df[columna].isna().sum())

    # Cantidad de errores
    total_errores = cantidad_vacios
    errores_columna = [temp_vacios for temp_vacios in vacios_columna]

    # Proporción de errores
    try:
        proporcion = round((total_errores / cantidad_registros) * 100, 4)
    except:
        proporcion = 0

    # Preparación de datos para la salida
    columnas_text = ", ".join(columnas)

    # Generación del documento txt
    path = error_path + "\\CDerrors.txt"
    with open(path, "w", encoding="utf-8") as f:
        f.write(f"Cantidad de registros|{cantidad_registros}\n")
        f.write(f"Cantidad de errores|{str(total_errores)}\n")
        f.write(f"Proporción de errores|{str(proporcion)}%\n")
        f.write(f"Cantidad de registros vacíos|{str(cantidad_vacios)}\n")
        f.write(f"Cantidad de columnas|{len(columnas)}\n")
        f.write(f"Columnas|{str(columnas_text)}\n")
        for i in range(len(columnas)):
            f.write(f"Cantidad de errores de la columna '{columnas[i]}'|{str(errores_columna[i])}\n")
            f.write(f"Cantidad de valores vacíos de la columna '{columnas[i]}'|{str(vacios_columna[i])}\n")   
        #f.write(str(conjuntodatos_df.dtypes.tolist()))    
    # Retorna la evaluación del conjunto de datos
    # print(f"Cantidad de registros|{cantidad_registros}")
    # print(f"Cantidad de errores|{str(total_errores)}")
    # print(f"Proporción de errores|{str(proporcion)}%")
    # print(f"Cantidad de registros vacios|{str(cantidad_vacios)}")
    # print(f"Cantidad de columnas|{len(str(columnas))}")
    # print(f"Columnas|{str(columnas_text)}")
    # for i in range(len(columnas)):
    #     print(f"Cantidad de errores de la columna '{columnas[i]}'|{str(errores_columna[i])}")
    #     print(f"Cantidad de valores vacios de la columna '{columnas[i]}'|{str(vacios_columna[i])}")  

    return conjuntodatos_df

'''
Sección de funcionamiento del programa
'''
# Argumentos
result_path = sys.argv[1]
error_path = sys.argv[2]
conjuntodatos_path = sys.argv[3]
diccionariodatos_path = sys.argv[4]
metadatos_path = sys.argv[5]
# Dataframes
conjuntodatos_df = get_conjuntodatos_df(conjuntodatos_path)
diccionariodatos_df = get_diccionariodatos_df(diccionariodatos_path)
metadatos_df = get_metadatos_df(metadatos_path)
# Genera o mueve los documentos de texto para Datin Revisa
conjuntodatos_df = evaluar_conjuntodatos()
if conjuntodatos_path.split(".")[-1] == "xlsx":
    copiar_conjuntodatos()
else:
    generar_conjuntodatos()
if type(diccionariodatos_df) == int:
    copiar_diccionariodatos()
else:
    generar_diccionariodatos()
if type(metadatos_df) == int:
    copiar_metadatos()
else:    
    generar_metadatos()
