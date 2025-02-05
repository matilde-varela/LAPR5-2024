using System;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Org.BouncyCastle.Asn1.Eac;

namespace MDBackoffice.Infrastructure.StaffProfiles
{
    internal class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            // Configure the primary key
            builder.HasKey(b => b.Id);

            // Configure LicenseNumber as a value object
            builder.OwnsOne(b => b.LicenseNumber, ln =>
            {
                ln.Property(l => l.Number)
                    .IsRequired()
                    .HasColumnName("LicenseNumber");
            });

            builder.Property(b => b.Status)
                .IsRequired()
                .HasColumnName("Status");

            builder.HasOne<User>()
                .WithOne() // Assuming User does not have a direct reference to Staff
                .HasForeignKey<Staff>(b => b.UserReference) // Foreign key on the Staff entity
                .IsRequired(false) // Make the relationship optional
                .OnDelete(DeleteBehavior.SetNull); // Optional: Set UserId to null on delete

            // Configure Name as a value object
            builder.OwnsOne(b => b.Name, n =>
            {
                n.Property(name => name.FirstName)
                    .IsRequired()
                    .HasColumnName("FirstName");
                n.Property(name => name.LastName)
                    .IsRequired()
                    .HasColumnName("LastName");
                n.Property(name => name.FullName)
                    .IsRequired()
                    .HasColumnName("FullName");
            });
            builder.OwnsOne(b => b.Address, n =>
            {
                n.Property(ad => ad.Country)
                    .IsRequired()
                    .HasColumnName("Country");
                n.Property(ad => ad.PostalCode)
                    .IsRequired()
                    .HasColumnName("PostalCode");
                n.Property(ad => ad.Residence)
                    .IsRequired()
                    .HasColumnName("Residence");
            });
            builder.OwnsOne(c => c.Phone, pn =>
            {
                pn.Property(p => p.PhoneNumber)
                    .IsRequired() // Ensure PhoneNumber is required
                    .HasColumnName("PhoneNumber");

                pn.Property(p => p.CountryCode)
                    .IsRequired() // Ensure CountryCode is required
                    .HasColumnName("CountryCode");
            });

            builder.OwnsOne(c => c.Email, ea =>
            {
                ea.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasColumnName("EmailAddress");
            });


            builder.OwnsOne(b => b.Function, f =>
            {
                f.Property(func => func.Description)
                    .IsRequired()
                    .HasColumnName("Function");
            });

            builder.HasOne<Specialization>()
                .WithMany()
                .HasForeignKey(b => b.SpecializationId)
                .IsRequired();

            builder.OwnsMany(b => b.Slots, slotBuilder =>
            {
                slotBuilder.WithOwner();

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

                slotBuilder.OwnsOne(s => s.Date, intervalBuilder =>
                {
                    intervalBuilder.Property(i => i.Start)
                        .IsRequired()
                        .HasColumnName("StartDate");

                    intervalBuilder.Property(i => i.End)
                        .IsRequired()
                        .HasColumnName("EndDate");
                });
                slotBuilder.Property(s => s.Description)
                        .HasColumnName("Description");
                slotBuilder.ToTable("StaffSlots");
            });

            builder.HasMany(b => b.AppointmentStaffs)
                .WithOne(b => b.Staff)
                .HasForeignKey("StaffId")
                .OnDelete(DeleteBehavior.Cascade);


            // Configure the table name for Staff
            builder.ToTable("Staff");
        }
    }
}
