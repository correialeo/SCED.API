using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Infrastructure.Mappings
{
    public class DeviceMapping : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            builder.Property(d => d.Type).IsRequired()
                .HasConversion(v => v.ToString(), v => Enum.Parse<DeviceType>(v));

            builder.Property(d => d.Status).IsRequired()
                .HasConversion(v => v.ToString(), v => Enum.Parse<DeviceStatus>(v));

            builder.Property(d => d.Latitude).IsRequired();
            builder.Property(d => d.Longitude).IsRequired();

            builder.HasMany(d => d.DeviceData)
                .WithOne(dd => dd.Device)
                .HasForeignKey(dd => dd.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}