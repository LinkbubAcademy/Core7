using Common.Lib.Core.Expressions;
using System.Reflection;

namespace Common.Lib.Core.Metadata
{
    public static class MetadataHandler
    {
        public static Dictionary<string, Dictionary<int, ValueTypes>> EntitiesMetadataTypes { get; private set; } = new Dictionary<string, Dictionary<int, ValueTypes>>();

        public static Dictionary<string, Func<Entity>> ModelsConstructors { get; set; } = new Dictionary<string, Func<Entity>>();
        public static Dictionary<string, Func<string[], IQueryExpression>> QueryExpressionConstructors { get; set; } = new Dictionary<string, Func<string[], IQueryExpression>>();
        public static Dictionary<string, Func<IPropertySelector>> PropertySelectorsConstructors { get; set; } = new Dictionary<string, Func<IPropertySelector>>();


        
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

        public static void InitMetadata()
        {

            if (EntitiesMetadataTypes.Count == 0)
            {
                var baseTypeEntity = typeof(Entity);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => baseTypeEntity.IsAssignableFrom(p));

                foreach (var t in types)
                {

                    var map = (Dictionary<int, ValueTypes>)t.GetTypeInfo()
                                .GetProperty("MetadataMaps", BindingFlags.Static | BindingFlags.Public)
                                .GetValue(null, null);

                    EntitiesMetadataTypes.Add(t.FullName, map);
                }
            }

            RegisterExpressions();
            RegisterPropertySelectors();
        }

        public static void RegisterModels()
        {
            ModelsConstructors.Add(typeof(Authentication.User).FullName, () => new Authentication.User());
        }

        public static void RegisterExpressions()
        {            
            var baseType = typeof(IQueryExpression);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract && !p.IsGenericType).ToList();

            foreach (var t in types)
            {
                // the rationale is to use delegate instead of reflection so we create a query expression of each type
                // and we keep then in memory to use them as factories of their types
                var qeFactory = (IQueryExpression)Activator.CreateInstance(t);
                QueryExpressionConstructors.Add(t.FullName, (parameters) => qeFactory.Create(parameters));
            }
        }

        public static void RegisterPropertySelectors()
        {
            var baseType = typeof(IPropertySelector);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

            foreach (var t in types)
            {
                // the rationale is to use delegate instead of reflection so we create a property selector of each type
                // and we keep then in memory to use them as factories of their types
                var psFactory = (IPropertySelector)Activator.CreateInstance(t);
                PropertySelectorsConstructors.Add(t.FullName, () => psFactory.CreateSelector());
            }
        }


#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
    }
}
