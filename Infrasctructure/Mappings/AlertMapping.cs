using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Infrastructure.Mappings
{
    public class AlertMapping : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.Type).IsRequired()
                .HasConversion(v => v.ToString(), v => Enum.Parse<AlertType>(v));

            builder.Property(a => a.Severity).IsRequired();
            builder.Property(a => a.Latitude).IsRequired();
            builder.Property(a => a.Longitude).IsRequired();
            builder.Property(a => a.Timestamp).IsRequired();
            builder.Property(a => a.Description).IsRequired().HasMaxLength(1000);
        }
    }
}