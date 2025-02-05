using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.OperationTypesRecords;


namespace MDBackoffice.Infrastructure.OperationTypeRecords
{
    internal class RequiredStaffRecordEntityTypeConfiguration : IEntityTypeConfiguration<RequiredStaffRecord>
    {
        public void Configure(EntityTypeBuilder<RequiredStaffRecord> builder)
        {
            // primary key
            builder.HasKey(b => b.RequiredStaffRecordId);
        
            builder.OwnsOne(b => b.Function, f =>
            {
                f.Property(func => func.Description)
                    .IsRequired()
                    .HasColumnName("Function");
            });

            builder.OwnsOne(b => b.StaffQuantity, f =>
            {
                f.Property(func => func.NumberRequired)
                    .IsRequired()
                    .HasColumnName("NumberRequired");
            });
            
            builder.OwnsOne<StaffSpecialization>(b => b.SpecializationId, f =>
            {
                f.Property(spec => spec.SpeciId)
                    .IsRequired()
                    .HasColumnName("Specialization");
            });

            builder.ToTable("RequiredStaffRecords");
        
        }
    }
}
