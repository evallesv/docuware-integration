Imports System
Imports System.IO
Imports System.Linq
Imports System.Collections.Generic
Imports Microsoft.Extensions.Configuration
Imports DocuWare.Platform.ServerClient
Imports DocuWare.Services.Http.Client

Module Program
    Sub Main(args As String())
        ''Api de configuracion para cargar y leer el archivo appsettings.json con las configuraciones reales
        Dim config As IConfiguration = New ConfigurationBuilder() _
            .SetBasePath(Directory.GetCurrentDirectory()) _
            .AddJsonFile("appsettings.json", False, True) _
            .Build()

        ''Creamos la conexión de mediante la Clase Factory "ServiceConnection"
        ''TODO: Recuerde actualizar el archivo appsettings.json para poder connectarse correctamente a docuware 
        Dim connection As ServiceConnection =
            ServiceConnection.Create(New Uri(config("DocuwareApi")), config("user"), config("password"))
        
        ''Obtenemos el Archivador desde la configuración utilizando el Guid de Archivador
        Dim cabinet As FileCabinet = connection.GetFileCabinet(config("Cabinet"))

        Console.WriteLine("Busqueda:")
        Busqueda(cabinet)

        ''Obtenemos el documento recien creado y leemos su información
        Dim testDoc = CargarDocumento(cabinet)
        Console.WriteLine("Documento cargado Id: {0}", testDoc.Id)

        ''Buscamos nuevamente para verificar el documento recien cargado
        Console.WriteLine("Nueva Busqueda:")
        Busqueda(cabinet)

        ''Cerramos la sesion y liberamos la licencia
        connection.Disconnect()

        Console.Write("Presione [Enter] para finalizar...")
        Console.ReadLine()
    End Sub

    ''Ejemplo de Busqueda
    Sub Busqueda(cabinet As FileCabinet)
        ''Obtenemos la lista de dialogos de busqueda
        Dim dialogos As DialogInfos = cabinet.GetDialogInfosFromSearchesRelation()
        ''Obtenemos el primer dialogo disponible al usuario y creamos la instancia
        Dim dialogoBusqueda As Dialog = dialogos.Dialog(0).GetDialogFromSelfRelation

        ''Creamos nuestra busqueda para obtener todos los documentos
        ''si los campos ENTE y TIPO no estan vacios
        ''Pueden utilizarse tambien operadores LIKE "Ejem*" para busquedas parciales
        ''http://help.docuware.com/#/home/57929/2/2
        Dim query As New DialogExpression() With {
            .Operation = DialogExpressionOperation.And, ''Se puede utilizar una condicion And u Or
            .Count = 100, ''Obtenemos un maximo de 100 documentos
            .Start = 0, ''Obtenemos desde el primer documento, siguiente pagina ".Start = .Start + .Count"
            .Condition = New List(Of DialogExpressionCondition) From {
                DialogExpressionCondition.Create("ENTE", "NOT EMPTY()"), ''Creamos  la condicion
                DialogExpressionCondition.Create("TIPO", "NOT EMPTY()")
            }
        }
        ''Ejecutamos la consulta
        Dim queryResult As DocumentsQueryResult = dialogoBusqueda.GetDocumentsResult(query)
        ''consultamos los resultados
        For Each document As Document In queryResult.Items
            '' Para acceder a los indices utilizamos la propiedad Fields
            '' y buscamos por el nombre "FieldName" luego obtenemos su valor "Item"
            Console.WriteLine("Id: {0} (Tipo:""{1}"" > Subtipo:""{2}"")",
                document.Id.ToString,
                document.Fields.FirstOrDefault(Function(f As DocumentIndexField) f.FieldName = "TIPO").Item,
                document.Fields.FirstOrDefault(Function(f As DocumentIndexField) f.FieldName = "SUBTIPO").Item
            )
        Next
    End Sub

    ''Ejemplo de carga de documentos
    Function CargarDocumento(cabinet As FileCabinet) As Document
        ''Generamos el listado de documentos
        Dim files = {
            New FileInfo("TEST.pdf")
        }
        '' Definimos el valor de los indices
        Dim indexData As New Document With {
                .Fields = New List(Of DocumentIndexField) From {
                    DocumentIndexField.Create("ENTE", "001"),
                    DocumentIndexField.Create("TIPO", "Prueba"),
                    DocumentIndexField.Create("SUBTIPO", "Anexo")
                }
            }
        ''Cargamos el documento y devolvemos el resultado
        Return cabinet.EasyUploadDocument(files, indexData)
    End Function
End Module
