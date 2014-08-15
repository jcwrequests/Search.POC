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

Public Class FoodsController
    Inherits ApiController

    Private applicationService As IApplicationService
    Private logger As ILogger
    Private foodView As IDocumentReader(Of unit, FoodLookUp)
    Private categoryView As IDocumentReader(Of unit, CategoryLookUp)

    Public Class Food
        Public Property id As String
        Public Property categoryId As String
        Public Property name As String
        Public Property foodTerms As List(Of String)
    End Class
    Public Sub New(foodApplicationService As IApplicationService,
                   logger As ILogger,
                   foodView As IDocumentReader(Of unit, FoodLookUp),
                   categoryView As IDocumentReader(Of unit, CategoryLookUp))

        Ensure.NotNull(foodApplicationService, "foodApplicationService")
        Ensure.NotNull(logger, "logger")
        Ensure.NotNull(foodView, "view")
        Ensure.NotNull(categoryView,"categoryView")

        Me.applicationService = foodApplicationService
        Me.logger = logger
        Me.foodView = foodView
        Me.categoryView = categoryView
    End Sub

    <HttpPut>
    Public Function AddNewFood(food As Food) As HttpResponseMessage

        Dim getFoodTerms =
            Function()
                If food.foodTerms Is Nothing Then Return Nothing
                Return food.
                        foodTerms.
                        Select(Function(term) New FoodTerm(New FoodId(food.id), term)).
                        ToArray()
            End Function
        Try
            Me.
                applicationService.
                Execute(New AddNewFood(New CategoryId(food.categoryId),
                                       New FoodId(food.id),
                                       food.name,
                                       getFoodTerms()))
            Return New HttpResponseMessage(HttpStatusCode.OK)
        Catch ex As Exception
            logger.Error(ex.ToString)
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End Try
    End Function

    <HttpGet>
    Public Function ListOfFoods() As HttpResponseMessage
        Dim context = Me.ControllerContext
        Dim formatter As MediaTypeFormatter = GetFormatter(context)

        Dim resp = New HttpResponseMessage(HttpStatusCode.OK)
        Dim key As New unit()
        Dim lookUp As FoodLookUp = Nothing

        If foodView.TryGet(key, lookUp) Then

            Dim results =
                lookUp.
                Foods.
                Values.
                Select(Function(f) New Food With {.id = f.Id.Value,
                                                  .name = f.Name,
                                                  .categoryId = f.Category.Value,
                                                  .foodTerms = f.FoodTerms.
                                                                 Select(Function(t) t.Term).
                                                                 ToList()}).
                ToList()

            resp.Content = New ObjectContent(Of List(Of Food))(results, formatter)
            Return resp
        Else
            logger.Error("Could not look up Categories")
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If
    End Function

    <HttpGet>
    Public Function ListOfFoodsByCategory(cat As String) As HttpResponseMessage
        Dim context = Me.ControllerContext
        Dim formatter As MediaTypeFormatter = GetFormatter(context)

        Dim resp = New HttpResponseMessage(HttpStatusCode.OK)
        Dim key As New unit()
        Dim foodLookUp As FoodLookUp = Nothing
        Dim categoryLookUp As CategoryLookUp = Nothing

        If Not foodView.TryGet(New unit, foodLookUp) Then
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If

        If Not categoryView.TryGet(New unit, categoryLookUp) Then
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If

        Dim category =
            categoryLookUp.
            Categories.
            Values.
            Where(Function(c) c.Name.Equals(cat,
                                            StringComparison.InvariantCultureIgnoreCase)).
            FirstOrDefault()

        If category Is Nothing Then
            Return New HttpResponseMessage(HttpStatusCode.OK)
        End If

        Dim results =
            foodLookUp.
            Foods.
            Values.
            Where(Function(f) category.Id.Value.Equals(f.Category.Value, StringComparison.InvariantCultureIgnoreCase)).
            Select(Function(f) New Food() With {.id = f.Id.Value,
                                                .name = f.Name,
                                                .categoryId = f.Category.Value,
                                                .foodTerms = f.FoodTerms.
                                                                Select(Function(t) t.Term).
                                                                ToList()}).
            ToList()

        resp.Content = New ObjectContent(Of List(Of Food))(results, formatter)
        Return resp

    End Function

    <HttpGet>
    Public Function ListOfFoodsByTerm(term As String) As HttpResponseMessage
        Dim context = Me.ControllerContext
        Dim formatter As MediaTypeFormatter = GetFormatter(context)

        Dim resp = New HttpResponseMessage(HttpStatusCode.OK)
        Dim key As New unit()
        Dim foodLookUp As FoodLookUp = Nothing

        If Not foodView.TryGet(New unit, foodLookUp) Then
            Return New HttpResponseMessage(HttpStatusCode.InternalServerError)
        End If

        Dim results =
            foodLookUp.
            Foods.
            Values.
            Where(Function(f) f.FoodTerms.
                                Any(Function(t) t.Term.Equals(term, StringComparison.InvariantCultureIgnoreCase))).
            Select(Function(f) New Food() With {.id = f.Id.Value,
                                                .name = f.Name,
                                                .categoryId = f.Category.Value,
                                                .foodTerms = f.FoodTerms.Select(Function(t) t.Term).ToList()}).
            ToList()

        resp.Content = New ObjectContent(Of List(Of Food))(results, formatter)
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
