namespace Common.Lib.Core.Tracking
{
    public class ChangeUnit : ComplexType
    {
        /// <summary>
        /// Each compilation may have different value for each Metadata
        /// We prefer int to minime size of services messages
        /// In case of storying, use Metadata Permanent Id (Guid)
        /// </summary>
        public int MetadataId { get; set; }


        /// <summary>
        /// Used for storing the change with a permanent id
        /// </summary>
        public Guid MetadataPermanentId { get; set; }


        /// <summary>
        /// The value of the change
        /// </summary>
        public object? Value { get; set; }

        #region Get Values

        public T GetValueAsEnum<T>() where T : System.Enum, new()
        {
            return Value != null ? new T() : default;
        }

        public Guid GetValueAsGuid()
        {
            return Value != null ? (Guid)Value : default;
        }

        public DateTime GetValueAsDateTime()
        {
            return Value != null ? (DateTime)Value : default;
        }

        public string GetValueAsString()
        {
            return Value != null ? (string)Value : string.Empty;
        }

        public int GetValueAsInt()
        {
            return Value != null ? (int)Value : 0;
        }
        public double GetValueAsDouble()
        {
            return Value != null ? (double)Value : 0.0;
        }

        public bool GetValueAsBool()
        {
            return Value != null ? (bool)Value : false;
        }


        #endregion
    }
}
