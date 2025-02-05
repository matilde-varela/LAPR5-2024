using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.Patients;

public class AppointmentHistoryConfiguration : IEntityTypeConfiguration<AppointmentHistory>
{
    public void Configure(EntityTypeBuilder<AppointmentHistory> builder)
    {
        builder.HasKey(ah => ah.Id);

        builder.Property(ah => ah.ObjectId)
            .IsRequired(); 

        builder.Property(ah => ah.Status)
            .IsRequired(); 

        builder.Property(ah => ah.Type)
            .IsRequired(); 

        builder.OwnsOne(b => b.CreatedAt, dr => 
            {
                dr.Property(createdAt => createdAt.Start)
                .IsRequired()
                .HasColumnName("CreatedAt");
            });

        builder.Property(ah => ah.PatientId) 
            .IsRequired(); 

        builder.HasOne<Patient>()
            .WithMany(p => p.AppointmentList)
            .HasForeignKey(ah => ah.PatientId); 
         builder.ToTable("AppointmentHistory");
    }
}
