using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listening.Infrastructure.Configs;

internal class ExerciseConfig:IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("T_Exercises");
        builder.HasKey(e => e.Id);
        
        // 💡 显式设置 Id 不由数据库生成，因为你在构造函数里用了 Guid.NewGuid()
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();

        builder.OwnsOne(e => e.Audio, ar =>
        {
            ar.Property(a => a.FileName).HasColumnName("AudioFileName").HasMaxLength(200).IsRequired();
            ar.Property(a => a.ContentType).HasColumnName("AudioContentType").HasMaxLength(200);
            ar.Property(a => a.Size).HasColumnName("AudioSize");
        });

        // 💡 字段映射确保一致
        builder.Property(e => e.Transcript)
            .HasField("_transcript") 
            .HasColumnName("Transcript")
            .IsRequired();
    }
}