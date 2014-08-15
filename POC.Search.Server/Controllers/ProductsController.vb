Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Net
Imports System.Web.Http
Imports System.Net.Http
Imports System.Net.Http.Formatting
Imports POC.Search.Domain.Services
Imports POC.Search.Domain.Contracts
Imports POC.Search.Domain.ValueObjects

Public Class ProductsController
    Inherits ApiController

    Private applicationService As IApplicationService
    Private logger As ILogger
    Public Class facet
        Public Property facetId As String
        Public Property name As String
    End Class
    Public Class product
        Public Property Id As String
        Public Property name As String
        Public Property description As String
        Public Property brandId As String
        Public Property facets As List(Of facet)
    End Class

    Public Sub New(foodApplicationService As IApplicationService,
                   logger As ILogger)

        Ensure.NotNull(foodApplicationService, "foodApplicationService")
        Ensure.NotNull(logger, "logger")

        Me.applicationService = foodApplicationService
        Me.logger = logger
    End Sub

    <HttpPut>
    Public Function AddNewProduction(product As product) As HttpResponseMessage
        Try
            Dim getFacets =
                Function()
                    If product.facets Is Nothing Then Return Nothing
                    Return product.
                            facets.
                            Select(Function(f As facet) New Domain.
                                                            ValueObjects.
                                                            Facet(New FacetId(f.facetId),
                                                                  f.name)).
                            ToArray()
                End Function

            applicationService.
                Execute(New AddNewProduct(New ProductId(product.Id),
                                          product.name,
                                          product.description,
                                          New BrandId(product.brandId),
                                          getFacets()))

            Return New HttpResponseMessage(HttpStatusCode.OK)
        Catch ex As Exception
            logger.Error(ex.ToString())
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End Try
    End Function

    <HttpGet>
    Public Function GetAllProducts() As HttpResponseMessage

    End Function

    <HttpGet>
    Public Function GetAllProductsByBrand(brand As String) As HttpResponseMessage

    End Function

    <HttpGet>
    Public Function GetAllProductsByName(name As String) As HttpResponseMessage

    End Function

    <HttpGet>
    Public Function GetAllProductsById(id As String) As HttpResponseMessage

    End Function


End Class
