using Microsoft.EntityFrameworkCore;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Infrastructure.Tokens;
using MDBackoffice.Infrastructure.Products;
using MDBackoffice.Infrastructure.Specializations;
using MDBackoffice.Infrastructure.StaffProfiles;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MDBackoffice.Infrastructure.OperationTypes;
using MDBackoffice.Infrastructure.OperationRequests;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Infrastructure.EntityConfigurations;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Infrastructure.OperationTypeRecords;
using Microsoft.EntityFrameworkCore.Internal;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Infrastructure.Appointments;
using MDBackoffice.Infrastructure.Rooms;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Infrastructure.RoomTypes;
using MDBackoffice.Domain.AppointmentStaffs;


namespace MDBackoffice.Infrastructure
{
    public class MDBackofficeDbContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Staff> StaffProfiles { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<RequiredStaff> RequiredStaff { get; set; }
        public DbSet<OperationRequest> OperationRequests { get; set; }
        public DbSet<OperationTypeRecord> OperationTypeRecords { get; set; }
        public DbSet<RequiredStaffRecord> RequiredStaffRecords { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<AppointmentHistory> appointmentHistories { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentStaff> AppointmentStaffs { get; set;}
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public MDBackofficeDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new TokenEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SpecializationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperationTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StaffEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredStaffEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperationRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LogEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperationTypeRecordEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredStaffRecordEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new RoomTypeConfiguration());
        }
    }
}
