using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listening.Infrastructure.Configs;

internal class ExerciseConfig : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("T_Exercises");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.LessonId)
            .IsRequired();

        builder.Property(e => e.Difficulty)
            .IsRequired();

        builder.Property(e => e.Transcript)
            .HasField("_transcript")
            .HasColumnName("Transcript")
            .IsRequired();

        builder.Property(e => e.Duration)
            .HasField("_duration")
            .HasColumnName("Duration")
            .IsRequired();

        builder.OwnsOne(e => e.Audio, ar =>
        {
            ar.Ignore(a => a.Id);

            ar.Property(a => a.FileName)
                .HasColumnName("AudioFileName")
                .HasMaxLength(200)
                .IsRequired();

            ar.Property(a => a.ContentType)
                .HasColumnName("AudioContentType")
                .HasMaxLength(200)
                .IsRequired();

            ar.Property(a => a.Size)
                .HasColumnName("AudioSize")
                .IsRequired();
        });

        builder.Navigation(e => e.Audio)
            .IsRequired();

        builder.HasMany(e => e.SubtitleSegments)
            .WithOne()
            .HasForeignKey(s => s.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.SubtitleSegments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}