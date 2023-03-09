namespace Common.Lib.Core.Tracking
{
    public interface IEntityInfo
    {
        Guid EntityId { get; set; }

        bool IsNew { get; set; }

        string EntityModelType { get; set; }

        ChangeUnit GetChangeUnit(int metadataId);

        List<ChangeUnit> GetChangeUnits();

    }
}
