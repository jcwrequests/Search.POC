using POC.Search.Domain.Infrastructure;
using POC.Search.Domain.ValueObjects;
using System;

[Serializable]
public class AddNewCategory : ICommand
{
    public AddNewCategory(CategoryId id, string name,CategoryId parent, params CategoryAlias[] aliases)
    {
        Ensure.NotNull(id, "id");
        Ensure.NotNull(name, "name");
        this.Id = id;
        this.Name = name;
        this.Aliases = aliases;
        this.Parent = parent;
    }
    public CategoryId Id { get; private set; }
    public string Name { get; private set; }
    public CategoryId Parent { get; private set; }
    public CategoryAlias[] Aliases { get; private set; }
}

[Serializable]
public class NewCategoryAdded : IEvent
{
    public NewCategoryAdded(CategoryId id, string name,CategoryId parent, params CategoryAlias[] aliases)
    {
        Ensure.NotNull(id, "id");
        Ensure.NotNull(name, "name");
        this.Id = id;
        this.Name = name;
        this.Parent = parent;
        this.Aliases = aliases;
    }
    public CategoryId Id { get; private set; }
    public string Name { get; private set; }
    public CategoryId Parent { get; private set; }
    public CategoryAlias[] Aliases { get; private set; }
}

[Serializable]
public class AddAliasToCategory :ICommand
{
    public AddAliasToCategory(CategoryId id, CategoryAlias alias)
    {
        Ensure.NotNull(id, "id");
        Ensure.NotNull(alias, "alias");
        this.Id = id;
        this.Alias = alias;
    }

    public CategoryId Id { get; private set; }
    public CategoryAlias Alias { get; private set; }
}
[Serializable]
public class AliasAddedToCategory : IEvent
{
    public AliasAddedToCategory(CategoryId id, CategoryAlias alias)
    {
        Ensure.NotNull(id, "id");
        Ensure.NotNull(alias, "alias");
        this.Id = id;
        this.Alias = alias;
    }

    public CategoryId Id { get; private set; }
    public CategoryAlias Alias { get; private set; }
}