using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POC.Search.Domain.Infrastructure;
using POC.Search.Domain.ValueObjects;

namespace POC.Search.Domain.Contracts
{
    [Serializable]
    public class AddNewProduct : ICommand
    {
        public AddNewProduct(ProductId id, string name, string description,BrandId brand, params Facet[] facets)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(name, "name");
            Ensure.NotNull(description, "description");
            Ensure.NotNull(brand, "brand");

            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Brand = brand;
            this.Facets = facets;

        }

        public ProductId Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public BrandId Brand { get; private set; }
        public Facet[] Facets { get; private set; }
    }
    [Serializable]
    public class NewProductAdded : IEvent
    {
        public NewProductAdded(ProductId id, string name, string description,BrandId brand, params Facet[] facets)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(name, "name");
            Ensure.NotNull(description, "description");
            Ensure.NotNull(brand, "brand");

            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Brand = brand;
            this.Facets = facets;

        }

        public ProductId Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public BrandId Brand { get; private set; }
        public Facet[] Facets { get; private set; }
    }
    [Serializable]
    public class AddFacetToProduct : ICommand
    {
        public AddFacetToProduct(ProductId id, Facet facet)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(facet, "facet");

            this.Id = id;
            this.Facet = facet;

        }

        public ProductId Id { get; private set; }
        public Facet Facet { get; private set; }
    }

    [Serializable]
    public class FacetAddedToProduct : IEvent
    {
        public FacetAddedToProduct(ProductId id, Facet facet)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(facet, "facet");

            this.Id = id;
            this.Facet = facet;

        }

        public ProductId Id { get; private set; }
        public Facet Facet { get; private set; }}
    
}
