using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.OperationTypes;


namespace MDBackoffice.Infrastructure.OperationTypes
{
    internal class OperationTypeEntityTypeConfiguration : IEntityTypeConfiguration<OperationType>
    {
        public void Configure(EntityTypeBuilder<OperationType> builder)
        {
            // primary key
            builder.HasKey(b => b.Id);
        
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

            //RequiredStaff as value object
            builder.HasMany(s => s.RequiredStaff)
            .WithOne()
            .HasForeignKey(s => s.OperationTypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);


            // Phases is a collection of value objects
            builder.OwnsMany(o => o.Phases, phaseBuilder =>
            {
                phaseBuilder.WithOwner().HasForeignKey(p => p.OperationTypeId);

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
            builder.ToTable("OperationType");
        
        }
    }
}
