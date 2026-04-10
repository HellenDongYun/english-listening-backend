using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listening.Infrastructure.Configs;

internal class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("T_Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
        //映射私有字段
        builder.Property<string>("_passwordHash")
            .HasField("_passwordHash")
            .UsePropertyAccessMode(PropertyAccessMode.Field).HasMaxLength(100)
            .IsUnicode(false);
        // AccessFail 导航字段映射（私有字段）
        builder.HasOne(u=>u.AccessFail)
            .WithOne(u=>u.User).HasForeignKey<UserAccessFail>(u=>u.UserId).IsRequired();
    }
}