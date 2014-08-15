using POC.Search.Domain.Infrastructure;
using POC.Search.Domain.ValueObjects;
using System;

[Serializable]
public class AddNewFood : ICommand
{
    public AddNewFood(CategoryId category, FoodId id, string name, params FoodTerm[] foodTerms)
    {
        Ensure.NotNull(category, "category");
        Ensure.NotNull(id, "id");
        Ensure.NotNull(name, "name");
        this.Category = category;
        this.Id = id;
        this.Name = name;
        this.FoodTerms = foodTerms;
    }
    public CategoryId Category { get; private set; }
    public FoodId Id { get; private set; }
    public string Name { get; private set; }
    public FoodTerm[] FoodTerms {get;private set;}
}

[Serializable]
public class NewFoodAdded : IEvent
{
    public NewFoodAdded(CategoryId category, FoodId id, string name, params FoodTerm[] foodTerms)
    {
        Ensure.NotNull(category, "category");
        Ensure.NotNull(id, "id");
        Ensure.NotNull(name, "name");
        this.Category = category;
        this.Id = id;
        this.Name = name;
        this.FoodTerms = foodTerms;
    }
    public CategoryId Category { get; private set; }
    public FoodId Id { get; private set; }
    public string Name { get; private set; }
    public FoodTerm[] FoodTerms {get;private set;}
}

[Serializable]
public class AddNewFoodTermToFood : ICommand
{
    public AddNewFoodTermToFood(FoodId id, FoodTerm foodTerm)
    {
        Ensure.NotNull(id, "id");
        Ensure.NotNull(foodTerm, "foodTerm");
        this.Id = id;
        this.FoodTerm = foodTerm;
    }
    public FoodId Id { get; private set; }
    public FoodTerm FoodTerm{ get; private set;}
}

[Serializable]
public class NewFoodTermAddedToFood : IEvent
{
    public NewFoodTermAddedToFood(FoodId id, FoodTerm foodTerm)
    {
        Ensure.NotNull(id, "id");
        Ensure.NotNull(foodTerm, "foodTerm");
        this.Id = id;
        this.FoodTerm = foodTerm;
    }
    public FoodId Id { get; private set; }
    public FoodTerm FoodTerm { get; private set; }
}
