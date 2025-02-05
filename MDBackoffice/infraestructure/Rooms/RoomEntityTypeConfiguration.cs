using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.Rooms;
using System;
using MDBackoffice.Domain.RoomTypes;

namespace MDBackoffice.Infrastructure.Rooms
{
    internal class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            // Primary key
            builder.HasKey(r => r.Id);
            
            builder.HasOne<RoomType>()
                .WithMany()
                .HasForeignKey(b => b.Type)
                .IsRequired();

            // Capacity as value object
            builder.OwnsOne(r => r.Capacity, capacityBuilder =>
            {
                capacityBuilder.Property(c => c.CapcityNumber)
                    .IsRequired()
                    .HasColumnName("Capacity");
            });

            // CurrentStatus as value object
            builder.OwnsOne(r => r.CurrentStatus, statusBuilder =>
            {
                statusBuilder.Property(s => s.Description)
                    .IsRequired()
                    .HasColumnName("CurrentStatus");
            });

            // AvailableEquipment as a collection of value objects
            builder.OwnsMany(r => r.AvailableEquipment, equipmentBuilder =>
            {
                equipmentBuilder.WithOwner()
                    .HasForeignKey("RoomId");

                equipmentBuilder.Property(e => e.EquipmentName)
                    .IsRequired()
                    .HasColumnName("EquipmentName");

                equipmentBuilder.ToTable("RoomAvailableEquipment");
            });

            // MaintenanceSlots as a collection of value objects
            builder.OwnsMany(r => r.MaintenanceSlots, slotBuilder =>
            {
                slotBuilder.WithOwner()
                    .HasForeignKey("RoomId");

                slotBuilder.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();
                slotBuilder.HasKey("Id");

                slotBuilder.OwnsOne(s => s.TimeInterval, intervalBuilder =>
                {
                    intervalBuilder.Property(i => i.Start)
                        .IsRequired()
                        .HasColumnName("StartTime");

                    intervalBuilder.Property(i => i.End)
                        .IsRequired()
                        .HasColumnName("EndTime");
                });

                slotBuilder.OwnsOne(s => s.Date, dateBuilder =>
                {
                    dateBuilder.Property(d => d.Start)
                        .IsRequired()
                        .HasColumnName("StartDate");

                    dateBuilder.Property(d => d.End)
                        .IsRequired()
                        .HasColumnName("EndDate");
                });
                slotBuilder.Property(s => s.Description)
                    .HasColumnName("Description");
                slotBuilder.ToTable("RoomMaintenanceSlots");
            });

            // Configure the table name for Room
            builder.ToTable("Rooms");
        }
    }
}
