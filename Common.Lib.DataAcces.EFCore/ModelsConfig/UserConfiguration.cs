using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Lib.Authentication;

namespace Common.Lib.DataAccess.EFCore
{
    public class UserConfiguration : UserConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
        }
    }

    public class UserConfiguration<T> : EntityConfiguration<T> where T : User
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure<T>(builder);
            builder.ToTable("users");
        }
    }
}