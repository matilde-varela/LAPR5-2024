using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.StaffProfiles;
using System;

namespace MDBackoffice.Infrastructure.OperationRequests
{
    internal class OperationRequestEntityTypeConfiguration : IEntityTypeConfiguration<OperationRequest>
    {
        public void Configure(EntityTypeBuilder<OperationRequest> builder)
        {
            //primary key
            builder.HasKey(b => b.Id);

            // DeadLineDate as value object
            builder.OwnsOne(b => b.DeadLineDate, dld =>
            {
                dld.Property(deadLineDate => deadLineDate.Start)
                .IsRequired()
                .HasColumnName("DeadLineDate");
            });

            // Priority as value object
            builder.OwnsOne(b => b.Priority, p =>
            {
                p.Property(priority => priority.Name)
                .IsRequired()
                .HasColumnName("Priority");
            });

            // DateOfRequest as value object
            builder.OwnsOne(b => b.DateOfRequest, dr =>
            {
                dr.Property(dateOfRequest => dateOfRequest.Start)
                .IsRequired()
                .HasColumnName("DateOfRequest");
            });


            // Status as value object
            builder.OwnsOne(b => b.Status, s =>
            {
                s.Property(status => status.Description)
                .IsRequired()
                .HasColumnName("Status");
            });

            // StaffId as value object
            builder.HasOne<Patient>()
               .WithMany()
               .HasForeignKey(b => b.PatientId)
               .IsRequired();

            // Description as a value object
            builder.OwnsOne(b => b.Description, d =>
            {
                d.Property(description => description.DescriptionText)
                .IsRequired()
                .HasColumnName("Description");
            });


            // PatientId as value object
            builder.HasOne<Staff>()
               .WithMany()
               .HasForeignKey(b => b.StaffId)
               .IsRequired();


            // OperationTypeId as value object
            builder.HasOne<OperationType>()
               .WithMany()
               .HasForeignKey(b => b.OperationTypeId)
               .IsRequired();

        }
    }
}