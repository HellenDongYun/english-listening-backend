using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Listening.Infrastructure.Configs;

internal class UserAccessFailConfig:IEntityTypeConfiguration<UserAccessFail>
{
    public void Configure(EntityTypeBuilder<UserAccessFail> builder)
    {
        builder.ToTable("T_UserAccessFails");
        builder.HasKey(e => e.Id);
        // 1. 私有字段 _lockOut 的映射
        builder.Property<bool>("_lockOut").HasField("_lockOut").UsePropertyAccessMode(PropertyAccessMode.Field);
    }

}