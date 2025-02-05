using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.OperationTypesRecords;


namespace MDBackoffice.Infrastructure.OperationTypeRecords
{
    internal class OperationTypeRecordEntityTypeConfiguration : IEntityTypeConfiguration<OperationTypeRecord>
    {
        public void Configure(EntityTypeBuilder<OperationTypeRecord> builder)
        {
            // primary key
            builder.HasKey(b => b.Id);

            // Version as value object
            builder.OwnsOne(b => b.Version, n =>
            {
                n.Property(version => version.Version)
                    .IsRequired()
                    .HasColumnName("Version");
            });

            // EffectiveDate as value object
             builder.OwnsOne(b => b.EffectiveDate, ef =>
            {
                ef.Property(effectiveDate => effectiveDate.Start)
                .IsRequired()
                .HasColumnName("EffectiveDate");
            });

            builder.OwnsOne(b => b.OperationTypeId, n =>
            {
                n.Property(operation  => operation.OpID)
                    .IsRequired()
                    .HasColumnName("OperationTypeId");
            });

            // OperationTypeName as value object
            builder.OwnsOne(b => b.Name, n =>
            {
                n.Property(name => name.OperationName)
                    .IsRequired()
                    .HasColumnName("OperationTypeName");
            });


            // EstimatedDuration as value object
            builder.OwnsOne(b => b.EstimatedDuration, ed =>
            {
                ed.Property(duration => duration.TotalDurationMinutes)
                    .IsRequired()
                    .HasColumnName("EstimatedDuration");
            });

            // OperationTypeStatus as value object
            builder.OwnsOne(b => b.Status, st =>
            {
                st.Property(status => status.Active)
                    .IsRequired()
                    .HasColumnName("OperationTypeStatus");
            });

            //RequiredStaffRecords as value object
            builder.HasMany(s => s.RequiredStaffRecords)
            .WithOne()
            .HasForeignKey(s => s.OperationTypeRecordId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

            // Phases is a collection of value objects
            builder.OwnsMany(o => o.Phases, phaseBuilder =>
            {

                phaseBuilder.WithOwner().HasForeignKey(p => p.OperationTypeRecordId);

                phaseBuilder.HasKey(p => p.PhaseId);


                phaseBuilder.OwnsOne(p => p.Description, phase =>
                {
                    phase.Property(p => p.Description)
                        .IsRequired()
                        .HasColumnName("PhaseDescription");
                });
                phaseBuilder.OwnsOne(p => p.Duration, phase =>
                {
                    phase.Property(p => p.DurationMinutes)
                        .IsRequired()
                        .HasColumnName("PhaseDuration");
                });
            });

            // Configure the table name for Staff
            builder.ToTable("OperationTypeRecords");
        
        }
    }
}
