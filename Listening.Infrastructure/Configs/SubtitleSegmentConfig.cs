namespace Listening.Infrastructure.Configs;
using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class SubtitleSegmentConfig:IEntityTypeConfiguration<SubtitleSegment>
{
    public void Configure(EntityTypeBuilder<SubtitleSegment> builder)
    {
        builder.ToTable("SubtitleSegments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExerciseId).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();

        builder.Property<string>("_text")
            .HasColumnName("Text")
            .IsRequired();

        builder.Property<TimeSpan>("_startTime")
            .HasColumnName("StartTime")
            .IsRequired();

        builder.Property<TimeSpan>("_endTime")
            .HasColumnName("EndTime")
            .IsRequired();
    }
    
}