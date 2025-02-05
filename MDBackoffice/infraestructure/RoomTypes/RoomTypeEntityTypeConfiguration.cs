using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.Rooms;
using System;
using MDBackoffice.Domain.RoomTypes;

namespace MDBackoffice.Infrastructure.RoomTypes
{
    internal class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            // Primary key
            builder.HasKey(r => r.Id);
            builder.OwnsOne(s => s.Designation, n =>
                  {
                      n.Property(designation => designation.Designation)
                      .IsRequired()
                      .HasColumnName("Designation");
                  });
            builder.OwnsOne(s => s.Description, n =>
            {
                n.Property(description => description.Description)
                .IsRequired()
                .HasColumnName("Description");
            });

            // Configure the table name for Room
            builder.ToTable("RoomTypes");
        }
    }
}
