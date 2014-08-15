using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POC.Search.Domain.ValueObjects;
using POC.Search.Domain.Infrastructure;

namespace POC.Search.Domain.Contracts
{
    [Serializable]
    public class AddBrand : ICommand
    {
        public AddBrand(BrandId id, string name, params BrandTerm[] brandTerms)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(name, "name");
            this.Id = id;
            this.Name = name;
            this.BrandTerms = brandTerms;

        }
        public BrandId Id { get; private set; }
        public string Name { get; private set; }
        public BrandTerm[] BrandTerms { get; private set; }
    }

    [Serializable]
    public class BrandAdded : IEvent
    {
        public BrandAdded(BrandId id, string name, params BrandTerm[] brandTerms)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(name, "name");
            this.Id = id;
            this.Name = name;
            this.BrandTerms = brandTerms;

        }
        public BrandId Id { get; private set; }
        public string Name { get; private set; }
        public BrandTerm[] BrandTerms { get; private set; }
    }

    [Serializable]
    public class AddTermToBrand : ICommand
    {
        public AddTermToBrand(BrandId id, BrandTerm brandTerm)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(brandTerm, "brandTerm");
            this.Id = id;
            this.BrandTerm = brandTerm;
        }
        public BrandId Id { get; private set; }
        public BrandTerm BrandTerm { get; private set; }
    }
    [Serializable]
    public class TermAddedToBrand : IEvent
    {
        public TermAddedToBrand(BrandId id, BrandTerm brandTerm)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(brandTerm, "brandTerm");
            this.Id = id;
            this.BrandTerm = brandTerm;
        }
        public BrandId Id { get; private set; }
        public BrandTerm BrandTerm { get; private set; }
    }
}
