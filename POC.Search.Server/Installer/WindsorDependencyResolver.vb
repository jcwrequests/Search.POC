Imports System.Web.Http.Dependencies
Imports Castle.Windsor
Imports Castle.MicroKernel.Lifestyle.LifestyleExtensions
Public Class WindsorDependencyResolver
    Implements IDependencyResolver
    Private ReadOnly _container As IWindsorContainer

    Public Sub New(container As IWindsorContainer)
        If container Is Nothing Then Throw New ArgumentNullException("container")
        Me._container = container
    End Sub
    Public Function BeginScope() As IDependencyScope Implements IDependencyResolver.BeginScope
        Return New WindsorDependencyScope(_container)
    End Function

    Public Function GetService(serviceType As Type) As Object Implements IDependencyScope.GetService
        If _container.Kernel.HasComponent(serviceType) Then Return _container.Resolve(serviceType)
        Return Nothing
    End Function

    Public Function GetServices(serviceType As Type) As IEnumerable(Of Object) Implements IDependencyScope.GetServices
        Return _container.ResolveAll(serviceType).Cast(Of Object)()
    End Function



    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then

            End If
        End If
        Me.disposedValue = True
    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub


End Class