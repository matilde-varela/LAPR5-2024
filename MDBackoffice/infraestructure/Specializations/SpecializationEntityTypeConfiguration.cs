using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.Specializations;

namespace MDBackoffice.Infrastructure.Specializations
{
    internal class SpecializationEntityTypeConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.HasKey(s => s.Id);
            builder.OwnsOne(s => s.Denomination, n =>
            {
                n.Property(denomination => denomination.Denomination)
                .IsRequired()
                .HasColumnName("Denomination");
            });

            builder.OwnsOne(s => s.Description, n =>
           {
               n.Property(description => description.Description)
               .IsRequired()
               .HasColumnName("Description");
           });
            builder.ToTable("Specialization");
        }
    }
}