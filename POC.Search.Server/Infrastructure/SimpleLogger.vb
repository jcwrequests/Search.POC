Imports POC.Search.Domain.Services

Public Class SimpleLogger
    Implements ILogger

    Public Sub Debug(message As String) Implements ILogger.Debug
        Console.WriteLine(String.Format("Debug: {0}", message))
    End Sub

    Public Sub [Error](message As String) Implements ILogger.Error
        Console.WriteLine(String.Format("Error: {0}", message))
    End Sub

    Public Sub Info(message As String) Implements ILogger.Info
        Console.WriteLine(String.Format("Info: {0}", message))
    End Sub

    Public Sub System([event] As IEvent, projectionType As Type, message As String, systemEventType As SystemEventTypes) Implements ILogger.System

    End Sub

    Public Sub Warn(message As String) Implements ILogger.Warn
        Console.WriteLine(String.Format("Warn: {0}", message))
    End Sub
End Class
