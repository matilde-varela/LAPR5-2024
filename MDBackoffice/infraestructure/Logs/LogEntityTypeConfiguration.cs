using MDBackoffice.Domain.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MDBackoffice.Infrastructure.EntityConfigurations
{
    public class LogEntityTypeConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {

            builder.ToTable("Logs");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .HasConversion(
                    id => id.AsString(), 
                    value => new LogId(value) 
                )
                .HasColumnName("LogId")
                .IsRequired();

            builder.Property(l => l.ObjectReference)
                .HasColumnName("ObjectReference")
                .IsRequired();

            builder.Property(l => l.ObjectClass)
                .HasColumnName("ObjectClass")
                .IsRequired();

            builder.OwnsOne(l => l.Change, change =>
            {
                change.Property(c => c.Type)
                    .HasColumnName("ChangeType")
                    .IsRequired();

                change.Property(c => c.ChangeDescription)
                    .HasColumnName("ChangeDescription")
                    .IsRequired();
            });

            builder.OwnsOne(b => b.DateOfChange, dr => 
            {
                dr.Property(dateOfChange => dateOfChange.Start)
                .IsRequired()
                .HasColumnName("DateOfChange");
            });
                
        }
    }
}
