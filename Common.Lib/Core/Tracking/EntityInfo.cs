namespace Common.Lib.Core.Tracking
{
    public class EntityInfo : IEntityInfo
    {
        public Guid EntityId { get; set; }

        public bool IsNew { get; set; }

        public string EntityModelType { get; set; } = string.Empty;

        public Dictionary<int, ChangeUnit> Changes { get; set; } = new Dictionary<int, ChangeUnit>();

        public ChangeUnit GetChangeUnit(int metadataId)
        {
            return Changes[metadataId];
        }

        public List<ChangeUnit> GetChangeUnits()
        {
            return Changes.Values.ToList();
        }

        #region AddChange

        public void AddChange(int propertyId, object? value)
        {
            Changes.Add(propertyId, new ChangeUnit()
            {
                MetdataId = propertyId,
                Value = value
            });
        }


        #endregion
    }
}
