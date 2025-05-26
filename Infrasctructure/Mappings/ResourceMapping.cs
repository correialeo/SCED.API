using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Infrastructure.Mappings
{
    public class ResourceMapping : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            builder.Property(r => r.Type).IsRequired()
                .HasConversion(v => v.ToString(), v => Enum.Parse<ResourceType>(v));

            builder.Property(r => r.Quantity).IsRequired();
            builder.Property(r => r.Latitude).IsRequired();
            builder.Property(r => r.Longitude).IsRequired();

            builder.Property(r => r.Status).IsRequired()
                .HasConversion(v => v.ToString(), v => Enum.Parse<ResourceStatus>(v));
        }
    }
}