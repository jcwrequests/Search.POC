Imports System.Runtime.CompilerServices
Imports POC.Search.Domain.ValueObjects
Imports POC.Search.Server.BrandsController
Imports POC.Search.Server.CategoriesController

Module Extensions
    <Extension> Public Function ToBrand(source As BrandLookUpValue) As brand
        Dim getTermValues =
           Function(terms() As BrandTerm)
               If terms Is Nothing Then Return Nothing
               Return terms.Select(Function(t) t.Term).ToList()
           End Function

        Return New brand() With {.brandId = source.Id.Value,
                                .name = source.Name,
                                .brandTerms = getTermValues(source.Terms)}
    End Function

    <Extension> Public Function ToCategory(source As CategoryLookUpValue) As category
        Dim getParentId =
            Function(parent As CategoryId)
                If IsNothing(parent) Then Return Nothing
                Return parent.Value
            End Function

        Dim getAliases =
            Function(aliases As CategoryAlias())
                If aliases Is Nothing Then Return Nothing
                Return aliases.Select(Function(a) a.Alias).ToList()
            End Function

        Return New category With {.categoryId = source.Id.Value,
                                  .name = source.Name,
                                  .parentCategory = getParentId(source.Parent),
                                  .categoryAliases = getAliases(source.CategoryAliases)}
    End Function
End Module
