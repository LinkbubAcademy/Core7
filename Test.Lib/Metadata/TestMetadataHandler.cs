using Common.Lib.Core.Context;
using Common.Lib.Core.Metadata;
using Test.Lib.Models;

namespace Test.Lib.Metadata
{
    public static class TestMetadataHandler
    {
        public static void InitMetadata()
        {
            MetadataHandler.InitMetadata();
            RegisterModels();
        }

        public static void RegisterModels()
        {
            MetadataHandler.ModelsConstructors.Add(typeof(Person).FullName, () => new Person());
            MetadataHandler.ModelsConstructors.Add(typeof(Post).FullName, () => new Post());

            var tname = typeof(IRepository<Person>).FullName;
        }
    }
}
