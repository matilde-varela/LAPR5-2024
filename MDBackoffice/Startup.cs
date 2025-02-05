using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MDBackoffice.Infrastructure;
using MDBackoffice.Infrastructure.Tokens;
using MDBackoffice.Infrastructure.Shared;
using MDBackoffice.Infrastructure.StaffProfiles;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Patients;
using Microsoft.AspNetCore.Identity;
using MDBackoffice.Domain.Users;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Infrastructure.OperationTypes;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Infrastructure.Specializations;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Infrastructure.OperationRequests;
using MDBackoffice.Infrastructure.Patients;
using MDBackoffice.Infrastructure.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Infrastructure.Logs;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Infrastructure.OperationTypeRecords;
using System;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using Microsoft.AspNetCore.Http;
using MDBackoffice.Infrastructure.Users;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Infrastructure.Rooms;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Infrastructure.Appointments;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Infrastructure.RoomTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Infrastructure.AppointmentStaffs;

namespace MDBackoffice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontEnd",
                    builder => builder
                        .WithOrigins("http://localhost:4200") // Allow only this origin
                        .AllowAnyHeader()                     // Allow any headers
                        .AllowAnyMethod());                   // Allow any HTTP methods
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = Configuration["GoogleKeys:ClientId"];
                options.ClientSecret = Configuration["GoogleKeys:ClientSecret"];
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Doctor", policy => policy.RequireRole("Doctor"));
                options.AddPolicy("Technician", policy => policy.RequireRole("Technician"));
                options.AddPolicy("Nurse", policy => policy.RequireRole("Nurse"));
                options.AddPolicy("Patient", policy => policy.RequireRole("Patient"));
                options.AddPolicy("AuthenticatedUsers", policy => policy.RequireRole("Admin", "Nurse", "Doctor", "Technician", "Patient"));
                options.AddPolicy("Staff", policy => policy.RequireRole("Admin", "Nurse", "Doctor", "Technician"));
            });

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 10;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<MDBackofficeDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

           services.AddDbContext<MDBackofficeDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(5, 7, 37)), // Adjust version as needed
                    options => options.EnableRetryOnFailure(
                        maxRetryCount: 10,      // Number of retries
                        maxRetryDelay: TimeSpan.FromSeconds(20), // Delay between retries
                        errorNumbersToAdd: null)     
                )
            .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>()
            );
            ConfigureMyServices(services);

            services.AddSwaggerGen();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("AllowFrontEnd");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SeedData(app);
        }

        public void SeedData(IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            RoleManager<Role> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            SpecializationService specializationService = scope.ServiceProvider.GetRequiredService<SpecializationService>();
            UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string[] roleNames = { "Admin", "Technician", "Doctor", "Nurse", "Patient" };
            foreach (string roleName in roleNames)
            {
                if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new Role(roleName)).GetAwaiter().GetResult();
                }
            }

        }


        public void ConfigureMyServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<EmailService>();

            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<TokenService>();
            services.AddTransient<IEmailAdapter, SendEmailGoogleAdapter>();
            services.AddTransient<ILoginAdapter, GoogleLoginAdapter>();
            services.AddTransient<IStaffRepository, StaffRepository>();
            services.AddTransient<StaffService>();

            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<PatientService>();

            services.AddTransient<IOperationTypeRepository, OperationTypeRepository>();
            services.AddTransient<OperationTypeService>();

            services.AddTransient<ISpecializationRepository, SpecializationRepository>();
            services.AddTransient<SpecializationService>();

            services.AddTransient<IOperationRequestRepository, OperationRequestRepository>();
            services.AddTransient<OperationRequestService>();

            services.AddTransient<UserService>();

            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<LogService>();

            services.AddTransient<IOperationTypeRecordRepository, OperationTypeRecordRepository>();
            services.AddTransient<OperationTypeRecordService>();

            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<RoomService>();

            services.AddTransient<IRoomTypeRepository, RoomTypeRepository>();
            services.AddTransient<RoomTypeService>();

            services.AddTransient<IAppointmentRepository, AppointmentRepository>();
            services.AddTransient<AppointmentService>();

            services.AddTransient<IOperationSchedulerAdapter, PlanningSchedulerAdapter>();
            services.AddTransient<PlanningSchedulerAdapter>();

            services.AddTransient<IAppointmentStaffRepository, AppointmentStaffRepository>();

            services.AddTransient<IRequiredStaffRepository, RequiredStaffRepository>();

            services.AddTransient<IPatientMedicalRecordAdapter, PatientMedicalRecordAdapter>();
            services.AddTransient<PatientMedicalRecordAdapter>();
        }


    }
}