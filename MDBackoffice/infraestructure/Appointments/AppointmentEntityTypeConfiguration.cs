using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.Rooms;

namespace MDBackoffice.Infrastructure.Appointments
{
    internal class AppointmentTypeConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            //primary key
            builder.HasKey(b => b.Id);

            // Status as a value object
            builder.OwnsOne(a => a.Status, s =>
            {
                s.Property(status => status.Description)
                 .IsRequired()
                 .HasColumnName("Status");
            });

            // OperationRequestId as a reference
            builder.HasOne<OperationRequest>()
                   .WithMany()
                   .HasForeignKey(a => a.OperationRequestId)
                   .IsRequired();

            // RoomNumber as a value object
            builder.HasOne<Room>()
                   .WithMany()
                   .HasForeignKey(a => a.RoomNumber)
                   .IsRequired();

            // Slot as a value object
            builder.OwnsOne(b => b.Slot, slotBuilder =>
            {
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
            });

            // Staff List as a value object
            // builder.HasMany(a => a.StaffsList)
            //    .WithOne()
            //    .HasForeignKey("AppointmentId")
            //    .IsRequired();

            //    builder.HasMany(a => a.StaffsList)
            //     .WithMany(s => s.Appointments)
            //     .UsingEntity<AppointmentStaff>(
            //          j =>
            // {
            //     j.HasKey(e => new { e.AppointmentId, e.StaffId }); // Define a chave primária composta
            //     j.ToTable("AppointmentStaff"); // Nome da tabela intermediária
            // });
                 // Nome da tabela intermediária

        // Relacionamento Many-to-Many com Staff
        // builder.HasMany(a => a.StaffsList)
        //     .WithMany(s => s.Appointments)
        //     .UsingEntity(
        //     "StaffAppointment",
        //     l => l.HasOne(typeof(Staff)).WithMany().HasForeignKey("StaffId").HasPrincipalKey(nameof(Staff.Id)),
        //     r => r.HasOne(typeof(Appointment)).WithMany().HasForeignKey("AppointmentId").HasPrincipalKey(nameof(Appointment.Id)),
        //     j => j.HasKey("AppointmentId", "StaffId"));

            // .UsingEntity(j => j.ToTable("StaffAppointment"));

            // builder.HasMany(a => a.StaffsList)
            //     .WithMany(s => s.Appointments)
            //     .UsingEntity<Dictionary<string, object>>(
            //         j => j.HasOne<Staff>().WithMany().HasForeignKey("StaffId"),  // Chave estrangeira para Staff
            //         j => j.HasOne<Appointment>().WithMany().HasForeignKey("AppointmentId")  // Chave estrangeira para Appointment
            // )
            // .ToTable("AppointmentStaff");  // Nome da tabela intermediária

            // Relacionamento Many-to-Many com Staff usando uma tabela intermediária explícita
            // builder.HasMany(a => a.StaffsList)
            //    .WithMany(s => s.Appointments)
            //    .UsingEntity<AppointmentStaff>(
            //         j => j.HasOne<Staff>().WithMany().HasForeignKey(a => a.StaffId),
            //         j => j.HasOne<Appointment>().WithMany().HasForeignKey(a => a.AppointmentId)
            //    )
            //    .ToTable("AppointmentStaff");


            // AppointmentStaff as a value object
             builder.HasMany(b => b.AppointmentStaffs)
                .WithOne(b => b.Appointment)
                .HasForeignKey("AppointmentId")
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the table name for Appointment
            builder.ToTable("Appointment");
        }
    }

}
