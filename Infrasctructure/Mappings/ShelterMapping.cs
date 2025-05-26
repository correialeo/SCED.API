using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCED.API.Domain.Entity;

namespace SCED.API.Infrastructure.Mappings
{
    public class ShelterMapping : IEntityTypeConfiguration<Shelter>
    {
        public void Configure(EntityTypeBuilder<Shelter> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Address).IsRequired().HasMaxLength(500);
            builder.Property(s => s.Capacity).IsRequired();
            builder.Property(s => s.CurrentOccupancy).IsRequired();
            builder.Property(s => s.Latitude).IsRequired();
            builder.Property(s => s.Longitude).IsRequired();
        }
    }
}