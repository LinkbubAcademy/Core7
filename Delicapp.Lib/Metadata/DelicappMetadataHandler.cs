using Common.Lib.Core.Context;
using Common.Lib.Core.Metadata;
using Delicapp.Lib.Models;

namespace Delicapp.Lib.Metadata
{
    public static class DelicappMetadataHandler
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
