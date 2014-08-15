Module Ensure
    Public Sub NotNull(parameter As Object, parameterName As String)
        If IsNothing(parameter) Then Throw New ArgumentNullException(parameterName)
    End Sub
    Public Sub PathExists(path As String, parameterName As String)
        If Not IO.Directory.Exists(path) Then Throw New IO.DirectoryNotFoundException(String.Format("Path does not exists {0}", parameterName))
    End Sub
End Module
