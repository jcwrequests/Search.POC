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

Public Class CategoriesController
    Inherits ApiController
    Private applicationService As IApplicationService
    Private logger As ILogger
    Private categoriesView As IDocumentReader(Of unit, CategoryLookUp)

    Public Class category
        Public Property categoryId As String
        Public Property name As String
        Public Property parentCategory As String
        Public Property categoryAliases As List(Of String)
    End Class

    Public Sub New(categoryApplicationService As IApplicationService,
                   logger As ILogger,
                   categoriesView As IDocumentReader(Of unit, CategoryLookUp))

        Ensure.NotNull(categoryApplicationService, "categoryApplicationService")
        Ensure.NotNull(logger, "logger")
        Ensure.NotNull(categoriesView, "categoriesView")

        Me.applicationService = categoryApplicationService
        Me.logger = logger
        Me.categoriesView = categoriesView
    End Sub
    <HttpPut>
    Public Function AddNewCategory(category As category) As HttpResponseMessage
        Try
            Dim getParent = Function()
                                If category.parentCategory Is Nothing Then Return Nothing
                                Return New CategoryId(category.parentCategory)
                            End Function

            Dim getAliases =
                Function()
                    If category.categoryAliases Is Nothing Then Return Nothing
                    Return category.
                           categoryAliases.
                           Select(Function(c) New CategoryAlias(New CategoryId(category.categoryId), c)).
                           ToArray()
                End Function


            applicationService.
                        Execute(New AddNewCategory(New CategoryId(category.categoryId),
                                                   category.name,
                                                   getParent(),
                                                   getAliases()))

            Return New HttpResponseMessage(HttpStatusCode.OK)
        Catch ex As Exception
            logger.Error(ex.ToString)
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End Try
    End Function

    <HttpGet>
    Public Function ListOfCategories() As HttpResponseMessage
        Dim context = Me.ControllerContext
        Dim formatter As MediaTypeFormatter = GetFormatter(context)

        Dim resp = New HttpResponseMessage(HttpStatusCode.OK)
        Dim key As New unit()
        Dim lookUp As CategoryLookUp = Nothing

        If categoriesView.TryGet(key, lookUp) Then
            Dim results =
                lookUp.
                Categories.
                Select(Function(c) c.Value.ToCategory()).
                ToList()

            resp.Content = New ObjectContent(Of List(Of category))(results, formatter)
            Return resp
        Else
            logger.Error("Could not look up Categories")
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If

    End Function

    <HttpGet>
    Public Function ListOfCategoriesByParentCategory(pcat As String) As HttpResponseMessage
        Dim context = Me.ControllerContext
        Dim formatter As MediaTypeFormatter = GetFormatter(context)

        Dim resp = New HttpResponseMessage(HttpStatusCode.OK)
        Dim key As New unit()
        Dim lookUp As CategoryLookUp = Nothing

        



        If categoriesView.TryGet(key, lookUp) Then
            Dim parentId As String = String.Empty
            Dim parentName As String = String.Empty

            Dim idMatch =
                lookUp.
                Categories.
                Where(Function(c) Not c.Value.Parent Is Nothing).
                Where(Function(c) c.Value.Parent.Value.Equals(pcat, StringComparison.CurrentCultureIgnoreCase)).
                Select(Function(r) r.Value.Id.Value).
                FirstOrDefault()

            Dim nameMatch =
                lookUp.
                Categories.
                Where(Function(c) c.Value.Name.Equals(pcat, StringComparison.InvariantCultureIgnoreCase)).
                Select(Function(r)
                           If lookUp.Categories.ContainsKey(r.Key) Then Return r.Value.Id.Value
                           Return Nothing
                       End Function).
                   FirstOrDefault()

            Dim lookUpID As String = IIf(IsNothing(idMatch), nameMatch, pcat)
            If lookUpID Is Nothing Then Return resp

            Dim groupyByResults =
                lookUp.
                Categories.
                Where(Function(c) Not c.Value.Parent Is Nothing).
                Where(Function(c) c.Value.Parent.Value.Equals(lookUpID, StringComparison.InvariantCultureIgnoreCase)).
                GroupBy(keySelector:=Function(k) k.Value.Parent.Value,
                        elementSelector:=Function(e) e.Value,
                        comparer:=StringComparer.InvariantCultureIgnoreCase)

            If groupyByResults.Count.Equals(0) Then Return resp

            Dim results =
                groupyByResults.
                First.
                Select(Function(r) r.ToCategory).
                ToList()


            resp.Content = New ObjectContent(Of List(Of category))(results, formatter)
            Return resp
        Else
            logger.Error("Could not look up Categories")
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If
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
