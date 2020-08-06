Imports System
Imports System.IO
Imports System.Linq
Imports System.Collections.Generic
Imports Microsoft.Extensions.Configuration
Imports DocuWare.Platform.ServerClient
Imports DocuWare.Services.Http.Client

Module Program
    Sub Main(args As String())

        Dim config As IConfiguration = New ConfigurationBuilder() _
            .SetBasePath(Directory.GetCurrentDirectory()) _
            .AddJsonFile("appsettings.json", False, True) _
            .Build()

        Dim connection As ServiceConnection =
            ServiceConnection.Create(New Uri(config("DocuwareApi")), config("user"), config("password"))

        Dim cabinet As FileCabinet = connection.GetFileCabinet(config("Cabinet"))

        Console.WriteLine("Busqueda:")
        Busqueda(cabinet)

        Dim testDoc = CargarDocumento(cabinet)
        Console.WriteLine("Documento cargado Id: {0}", testDoc.Id)

        Console.WriteLine("Nueva Busqueda:")
        Busqueda(cabinet)

        connection.Disconnect()

        Console.Write("Presione [Enter] para finalizar...")
        Console.ReadLine()
    End Sub

    Sub Busqueda(cabinet As FileCabinet)
        Dim dialogos As DialogInfos = cabinet.GetDialogInfosFromSearchesRelation()
        Dim dialogoBusqueda As Dialog = dialogos.Dialog(0).GetDialogFromSelfRelation

        Dim query As New DialogExpression With {
            .Operation = DialogExpressionOperation.And,
            .Count = 100,
            .Condition = New List(Of DialogExpressionCondition) From {
                DialogExpressionCondition.Create("ENTE", "NOT EMPTY()")
            }
        }

        Dim queryResult As DocumentsQueryResult = dialogoBusqueda.GetDocumentsResult(query)

        For Each document As Document In queryResult.Items
            Console.WriteLine("Id: {0} (Tipo:""{1}"" > Subtipo:""{2}"")",
document.Id.ToString,
            document.Fields.FirstOrDefault(Function(f As DocumentIndexField) f.FieldName = "TIPO").Item,
            document.Fields.FirstOrDefault(Function(f As DocumentIndexField) f.FieldName = "SUBTIPO").Item
            )
        Next
    End Sub

    Function CargarDocumento(cabinet As FileCabinet) As Document
        Dim files = {
            New FileInfo("TEST.pdf")
        }
        Dim indexData As New Document With {
                .Fields = New List(Of DocumentIndexField) From {
                    DocumentIndexField.Create("ENTE", "001"),
                    DocumentIndexField.Create("TIPO", "Prueba"),
                    DocumentIndexField.Create("SUBTIPO", "Anexo")
                }
            }
        Return cabinet.EasyUploadDocument(files, indexData)
    End Function
End Module
