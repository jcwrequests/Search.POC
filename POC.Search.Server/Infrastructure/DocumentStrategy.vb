Imports POC.Search.Domain.Storage
Imports System.Runtime.Serialization.Formatters.Binary
Imports POC.Search.Domain.Services

Public Class DocumentStrategy
    Implements IDocumentStrategy

    Private projectionPath As String
    ReadOnly _formatter As New BinaryFormatter()

    Public Sub New(projectionPath As String)
        Ensure.NotNull(projectionPath, "projectionPath")
        Ensure.PathExists(projectionPath, "projectionPath")
        Me.projectionPath = projectionPath
    End Sub
    Public Function Deserialize(Of TEntity)(stream As IO.Stream) As TEntity Implements IDocumentStrategy.Deserialize
        Dim data(stream.Length - 1) As Byte
        stream.Read(data, 0, stream.Length)

        Using mem = New IO.MemoryStream(data)
            Return CType(_formatter.Deserialize(mem), TEntity)
        End Using
    End Function

    Public Function GetEntityBucket(Of TEntity)() As String Implements IDocumentStrategy.GetEntityBucket
        Return projectionPath
    End Function

    Public Function GetEntityLocation(Of TEntity)(key As Object) As String Implements IDocumentStrategy.GetEntityLocation
        Return String.Format("{0}.pb", GetType(TEntity).Name.ToLower)
    End Function

    Public Sub Serialize(Of TEntity)(entity As TEntity, stream As IO.Stream) Implements IDocumentStrategy.Serialize
        _formatter.Serialize(stream, entity)
    End Sub
End Class
