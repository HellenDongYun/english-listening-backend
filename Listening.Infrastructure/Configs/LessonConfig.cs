using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Listening.Infrastructure.Configs;


public class LessonConfig:IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("T_Lessons");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Title).HasMaxLength(200).IsRequired();
        builder.Property(l => l.Description).HasMaxLength(500);

        // 💡 关键：配置私有字段集合映射
        builder.HasMany(l => l.Exercises)
            .WithOne() // 因为 Exercise 端只有 LessonId 属性，没有 Lesson 导航属性
            .HasForeignKey(e => e.LessonId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // 告诉 EF 读写 Exercises 集合时直接操作私有字段 _exercises
        var navigation = builder.Metadata.FindNavigation(nameof(Lesson.Exercises));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}