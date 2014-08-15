Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Net
Imports System.Web.Http
Imports System.Net.Http
Imports System.Net.Http.Formatting
Imports POC.Search.Domain.Contracts
Imports POC.Search.Domain.ValueObjects
Imports POC.Search.Domain.Services
Imports POC.Search.Domain.Aggregates
Imports POC.Search.Domain.Comparers
Imports System.Web.Http.Controllers

Public Class BrandsController
    Inherits ApiController
    Private applicationService As IApplicationService
    Private logger As ILogger
    Private brandsView As IDocumentReader(Of brandLookUpunit, BrandLookUp)
    Private termsLookUpView As IDocumentReader(Of unit, BrandTermLookUp)

    Public Class brand
        Public Property brandId As String
        Public Property name As String
        Public Property brandTerms As List(Of String)
    End Class
    Public Sub New(brandApplicationService As IApplicationService,
                   logger As ILogger,
                   brandsView As IDocumentReader(Of brandLookUpunit, BrandLookUp),
                   termsLookUpView As IDocumentReader(Of unit, BrandTermLookUp))

        Ensure.NotNull(brandsView, "view")
        Ensure.NotNull(brandApplicationService, "brandApplicationService")
        Ensure.NotNull(logger, "logger")
        Ensure.NotNull(termsLookUpView, "termsLookUpView")

        Me.applicationService = brandApplicationService
        Me.logger = logger
        Me.brandsView = brandsView
        Me.termsLookUpView = termsLookUpView

    End Sub
    <HttpPut>
    Public Function AddNewBrand(brand As brand) As HttpResponseMessage
        Try
            applicationService.
                        Execute(New AddBrand(New BrandId(brand.brandId),
                                                brand.name,
                                                brand.
                                                brandTerms.
                                                Select(Function(t) New BrandTerm(brandId:=New BrandId(brand.brandId),
                                                                                    term:=t)).ToArray()))
            Return New HttpResponseMessage(HttpStatusCode.OK)
        Catch ex As Exception
            logger.Error(ex.ToString)
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End Try



    End Function

    <HttpGet>
    Public Function ListOfBrands() As HttpResponseMessage
        Dim context = Me.ControllerContext
        Dim formatter As MediaTypeFormatter = GetFormatter(context)

        Dim resp = New HttpResponseMessage(HttpStatusCode.OK)
        Dim key As New brandLookUpunit
        Dim lookUp As BrandLookUp = Nothing

        Dim getTermValues =
            Function(terms() As BrandTerm)
                If terms Is Nothing Then Return Nothing
                Return terms.Select(Function(t) t.Term).ToList()
            End Function

        If brandsView.TryGet(key, lookUp) Then
            Dim results =
                lookUp.
                Brands.
                Select(Function(b) b.Value.ToBrand).
                ToList()

            resp.Content = New ObjectContent(Of List(Of brand))(results, formatter)
            Return resp
        Else
            logger.Error("Could not look up Brands")
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If



    End Function

    <HttpGet>
    Public Function ListOfBrandsByTerm(term As String) As HttpResponseMessage
        Dim context = Me.ControllerContext
        Dim formatter As MediaTypeFormatter = GetFormatter(context)

        Dim resp = New HttpResponseMessage(HttpStatusCode.OK)
        Dim brandKey As New brandLookUpunit
        Dim lookUpKey As New unit
        Dim brandsLookUp As BrandLookUp = Nothing
        Dim termsLookUp As BrandTermLookUp = Nothing

        If Not brandsView.TryGet(brandKey, brandsLookUp) Then
            logger.Error("Could not look up Brands")
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If
        If Not termsLookUpView.TryGet(lookUpKey, termsLookUp) Then
            logger.Error("Could not look up terms")
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If

        Dim termsIgnoreCase =
            termsLookUp.
                Brands.
                ToDictionary(keySelector:=Function(k) k.Key,
                             elementSelector:=Function(e) e.Value,
                             comparer:=StringComparer.InvariantCultureIgnoreCase)

        Dim brands =
            brandsLookUp.
                Brands.ToDictionary(keySelector:=Function(k) k.Key,
                                    elementSelector:=Function(e) e.Value,
                                    comparer:=New BrandIdComparer())

        If termsIgnoreCase.ContainsKey(term) Then
            Dim results =
                termsIgnoreCase(term).
                Select(Function(t) brands(t.BrandId).ToBrand()).
            ToList()

            resp.Content = New ObjectContent(Of List(Of brand))(results, formatter)
        End If

        Return resp

    End Function

    Private Shared Function GetFormatter(context As HttpControllerContext) As MediaTypeFormatter

        Dim acceptHeaders = context.Request.Headers.Accept
        Dim formatter As MediaTypeFormatter =
            context.Configuration.Formatters.First()

        If Not acceptHeaders Is Nothing Then
            For Each header In acceptHeaders
                Dim mappedFormater =
                    context.
                    Configuration.
                    Formatters.
                    Where(Function(f) f.SupportedMediaTypes.Contains(header)).
                    FirstOrDefault()
                If Not mappedFormater Is Nothing Then
                    formatter = mappedFormater
                    Exit For
                End If

            Next

        End If


        Return formatter

    End Function

End Class
