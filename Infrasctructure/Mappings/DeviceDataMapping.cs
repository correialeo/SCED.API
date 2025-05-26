using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCED.API.Domain.Entity;

namespace SCED.API.Infrastructure.Mappings
{
    public class DeviceDataMapping : IEntityTypeConfiguration<DeviceData>
    {
        public void Configure(EntityTypeBuilder<DeviceData> builder)
        {
            builder.HasKey(dd => dd.Id);
            builder.Property(dd => dd.Id).ValueGeneratedOnAdd();

            builder.Property(dd => dd.DeviceId).IsRequired();
            builder.Property(dd => dd.Value).IsRequired();
            builder.Property(dd => dd.Timestamp).IsRequired();
        }
    }
}