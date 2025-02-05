using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.AppointmentStaffs;

namespace MDBackoffice.Infrastructure.AppointmentStaffs
{
    internal class AppointmentTypeConfiguration : IEntityTypeConfiguration<AppointmentStaff>
    {
        public void Configure(EntityTypeBuilder<AppointmentStaff> builder)
        {

            //primary key
            builder.HasKey(b => b.Id);

              builder.HasOne(b => b.Appointment)
            .WithMany(b => b.AppointmentStaffs)
            .HasForeignKey("AppointmentId")
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Staff)
            .WithMany(b => b.AppointmentStaffs)
            .HasForeignKey("StaffId")
            .OnDelete(DeleteBehavior.Cascade); 

            // Configure the table name for AppointmentStaff
            builder.ToTable("AppointmentStaff");
        }
    }

}
