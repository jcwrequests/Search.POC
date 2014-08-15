Imports System.Web.Http.Dependencies
Imports Castle.MicroKernel
Imports Castle.MicroKernel.Lifestyle
Imports Castle.Windsor

Public Class WindsorDependencyScope
    Implements IDependencyScope
    Private _container As IWindsorContainer
    Private _scope As IDisposable

    Public Sub New(container As IWindsorContainer)
        If container Is Nothing Then Throw New ArgumentNullException("container")
        _container = container
        _scope = _container.BeginScope()
    End Sub
    Public Function GetService(serviceType As Type) As Object Implements IDependencyScope.GetService
        Return IIf(_container.Kernel.HasComponent(serviceType), _container.Resolve(serviceType), Nothing)
    End Function

    Public Function GetServices(serviceType As Type) As IEnumerable(Of Object) Implements IDependencyScope.GetServices
        Return _container.ResolveAll(serviceType).Cast(Of Object)()
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _scope.Dispose()
            End If

        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region



End Class
