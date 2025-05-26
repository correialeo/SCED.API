using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Infrastructure.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255); 

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion(v => v.ToString(), v => Enum.Parse<UserRole>(v));

            builder.Property(u => u.Necessities)
                .HasColumnType("TEXT")
                .IsRequired(false);

            builder.Property(u => u.Latitude).IsRequired();
            builder.Property(u => u.Longitude).IsRequired();
        }
    }
}