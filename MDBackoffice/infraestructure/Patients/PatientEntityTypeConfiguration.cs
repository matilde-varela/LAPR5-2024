using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Users;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Infrastructure.Products
{
    internal class PatientEntityTypeConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            //builder.ToTable("Products", SchemaNames.MDBackoffice);
            builder.HasKey(b => b.Id);
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

            builder.HasMany(p => p.AppointmentList)
                .WithOne() 
                .HasForeignKey(ah => ah.PatientId)
                .IsRequired(); 

            builder.Property(b => b.Status)
                .IsRequired()
                .HasColumnName("Status");

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
            builder.HasOne<User>()
                 .WithOne() // Assuming User does not have a direct reference to Staff
                 .HasForeignKey<Patient>(b => b.UserReference) // Foreign key on the Staff entity
                 .IsRequired(false) // Make the relationship optional
                 .OnDelete(DeleteBehavior.SetNull); // Optional: Set UserId to null on delete

            builder.OwnsOne(c => c.PhoneNumber, pn =>
             {
                 pn.Property(p => p.PhoneNumber)
                     .IsRequired()
                     .HasColumnName("PhoneNumber");

                 pn.Property(p => p.CountryCode)
                     .IsRequired()
                     .HasColumnName("CountryCode");
             });
            builder.OwnsOne(c => c.EmergencyContact, pn =>
            {
                pn.Property(p => p.PhoneNumber)
                    .IsRequired()
                    .HasColumnName("EmergencyContact");

                pn.Property(p => p.CountryCode)
                    .IsRequired()
                    .HasColumnName("EmergencyContactCountryCode");
            });
            builder.OwnsOne(c => c.Email, ea =>
           {
               ea.Property(e => e.EmailAddress)
                   .IsRequired()
                   .HasColumnName("EmailAddress");
           });

            builder.OwnsOne(b => b.Gender, f =>
            {
                f.Property(func => func.Denomination)
                    .IsRequired()
                    .HasColumnName("Gender");
            });

            builder.OwnsOne(b => b.MedicalCondition, f =>
            {
                f.Property(func => func.Description)
                    .HasColumnName("MedicalCondition");
            });

            builder.Property(e => e.DateBirth)
                .IsRequired()
                .HasColumnType("DATE");
        }
    }
}