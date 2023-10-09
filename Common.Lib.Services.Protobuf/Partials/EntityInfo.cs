using Common.Lib.Core;
using Common.Lib.Core.Metadata;
using Common.Lib.Core.Tracking;
using Google.Protobuf.WellKnownTypes;
using System;

namespace Common.Lib.Services.Protobuf
{
    public partial class EntityInfo : IEntityInfo
    {

        #region types

        static System.Type StringType = typeof(string);
        static System.Type IntType = typeof(int);
        static System.Type BoolType = typeof(bool);
        static System.Type ByteArrayType = typeof(byte[]);
        static System.Type DoubleType = typeof(double);
        static System.Type DateTimeType = typeof(DateTime);
        static System.Type GuidType = typeof(Guid);

        #endregion

        public Guid EntityId
        {
            get
            {
                return Guid.Parse(SEntityId);
            }
            set
            {
                SEntityId = value.ToString();
            }
        }

        Dictionary<int, ValueTypes> EntitiesMetadataTypes
        {
            get
            {
                if (_entitiesMetadataTypes.Count == 0)
                    _entitiesMetadataTypes = MetadataHandler.EntitiesMetadataTypes[this.EntityModelType];
                
                return _entitiesMetadataTypes;
            }
        }
        Dictionary<int, ValueTypes> _entitiesMetadataTypes = new();

        public EntityInfo(IEntityInfo entityInfo) : base()
        {
            EntityId = entityInfo.EntityId;
            IsNew = entityInfo.IsNew;
            EntityModelType = entityInfo.EntityModelType;

            foreach (var change in entityInfo.GetChangeUnits())
                SetValue(change.MetadataId, change.Value);
        }

        public ChangeUnit GetChangeUnit(int metadataId)
        {
            var output = new ChangeUnit()
            {
                MetadataId = metadataId,
                Value = GetValue(metadataId)
            };

            return output;
        }

        public object? GetValue(int metadataId)
        {            
            var input = SerializedChanges[metadataId];
            var valueType = EntitiesMetadataTypes[metadataId];

            return DeserializeValue(input, valueType);  
        }

        public static object? DeserializeValue(string input, ValueTypes valueType)
        {
            switch (valueType)
            {
                case ValueTypes.ReferencedImage:
                case ValueTypes.String:
                    return input;
                case ValueTypes.Int:
                    return int.Parse(input);
                case ValueTypes.Bool:
                    return input == "1" || input == "True" ? true : false;
                case ValueTypes.ByteArray:
                    throw new NotImplementedException();
                case ValueTypes.Double:
                    return double.Parse(input);
                case ValueTypes.DateTime:
                    return new DateTime().AddTicks(long.Parse(input));
                case ValueTypes.Guid:
                    return Guid.Parse(input);
                default:
                    throw new NotImplementedException();
            }
        }

        public static string SerializeValue(object value, ValueTypes valueType)
        {
            switch (valueType)
            {
                case ValueTypes.String:
                case ValueTypes.ReferencedImage:                   
                    return (string)value;

                case ValueTypes.Bool:
                    return ((bool)value) ? "1"  : "0";

                case ValueTypes.ByteArray:
                    throw new NotImplementedException();

                case ValueTypes.Int:
                case ValueTypes.Double:
                case ValueTypes.Guid:
                default:
                    if (value is System.Enum)
                        return ((int)value).ToString();
                    else
                        return value.ToString();

                case ValueTypes.DateTime:
                    return ((DateTime)value).Ticks.ToString();
            }                    
        }

        public static Func<string, TOut> GetParseFunc<TOut>()
        {
            var vtype = GetValueType(typeof(TOut));
            
            return (value) =>
            {
                var output = EntityInfo.DeserializeValue(value, vtype);                
                return output != default ? (TOut)output : default;
            };
        }

        public static Func<object, string> GetSerializeFunc(System.Type type)
        {
            var vtype = GetValueType(type);
            return (o) => SerializeValue(o, vtype);
        }

        public static ValueTypes GetValueType(System.Type type)
        {
            var vtype = ValueTypes.String;

            if (type == IntType)
                vtype = ValueTypes.Int;
            else if (type == DoubleType)
                vtype = ValueTypes.Double;
            else if (type == BoolType)
                vtype = ValueTypes.Bool;
            else if (type == GuidType)
                vtype = ValueTypes.Guid;
            else if (type == DateTimeType)
                vtype = ValueTypes.DateTime;
            else if (type == ByteArrayType)
                vtype = ValueTypes.ByteArray;

            return vtype;
        }

        public void SetValue(int metadataId, object? value)
        {
            var valueType = EntitiesMetadataTypes[metadataId];

            if (value == null)
            {
                SerializedChanges.Add(metadataId, null);
                return;
            }


            SerializedChanges.Add(metadataId, SerializeValue(value, valueType));
        }

        public List<ChangeUnit> GetChangeUnits()
        {
            var output = SerializedChanges.Select(x => GetChangeUnit(x.Key)).ToList();
            return output;
        }
    }
}
