using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Lib.Core;

namespace Common.Lib.DataAccess.EFCore
{
    public class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Ignore(x => x.IsNew);
            builder.Ignore(x => x.SaveAction);
            builder.Ignore(x => x.DeleteAction);
            builder.Ignore(x => x.ContextFactory);

            builder.ToTable("entities");
        }

        public virtual void Configure<T1>(EntityTypeBuilder<T1> builder) where T1 : Entity
        {
            builder.Ignore(x => x.IsNew);
            builder.Ignore(x => x.SaveAction);
            builder.Ignore(x => x.DeleteAction);
            builder.Ignore(x => x.ContextFactory);
            builder.ToTable("entities");
        }
    }

    public class EntityConfiguration : EntityConfiguration<Entity>
    {
    }
}
