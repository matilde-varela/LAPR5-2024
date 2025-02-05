using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using Moq;
using Xunit;

namespace MDBackofficeTests.controllertests
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _repoAppointMock = new Mock<IAppointmentRepository>();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IRoomRepository> _repoRoomMock = new Mock<IRoomRepository>();
        private readonly Mock<IOperationTypeRepository> _repoOpTypeMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<IStaffRepository> _repoStaMock = new Mock<IStaffRepository>();
        private readonly Mock<IRequiredStaffRepository> _repoReqStaMock = new Mock<IRequiredStaffRepository>();
        private readonly Mock<IOperationRequestRepository> _repoOpReqMock = new Mock<IOperationRequestRepository>();
        private readonly Mock<IAppointmentStaffRepository> _appointmentStaffRepoMock = new Mock<IAppointmentStaffRepository>();

        [Fact]
        public async Task Create_ReturnsOkResult_WithValidData()
        {
            // Arrange
            var _service = new AppointmentService(_unitOfWorkMock.Object, _repoAppointMock.Object, _repoOpReqMock.Object,
                _repoRoomMock.Object, _repoOpTypeMock.Object, _repoStaMock.Object, _repoReqStaMock.Object, _appointmentStaffRepoMock.Object);

            var createDto = new CreatingAppointmentDto(
                "test id",                 
                "R102",                    
                "14:00:00",                
                "16:00:00",               
                "2024-04-04",              
                "2024-04-04",              
                new List<string> { "D202500001" } 
            );

            var id = "test id";
            var dto = new AppointmentDto(
                Guid.NewGuid(),
                "Scheduled",
                id,
                "R102",
                 "14:00:00",
                 "16:00:00",
                 "4/4/2024 12:00:00 AM",
                 "4/4/2024 12:00:00 AM",
                 new List<string> { "D202500001" }
            );


            _repoOpReqMock
                .Setup(x => x.GetByIdAsync(It.IsAny<OperationRequestId>()))
                .ReturnsAsync(new OperationRequest(
                    "test id",
                    "2024-07-01",
                    "Elective",
                    "2024-06-01",
                    "O202400001",
                    "Test operation",
                    "202412000001",
                    "operation-type-id-1"
                ));

            _repoOpTypeMock
                .Setup(x => x.GetByIdAsync(It.IsAny<OperationTypeId>()))
                .ReturnsAsync(new OperationType(
                    "Type1",
                    120,
                    true,
                    new List<RequiredStaffDto>{
                        new RequiredStaffDto
                        {
                            StaffQuantity = 1,
                            Function = "Doctor",
                            Specialization = "10101010"
                        }
                    },
                    new List<PhaseDto>
                    {
                        new PhaseDto
                        {
                            Description = "Phase 1",
                            Duration = 60
                        },
                         new PhaseDto
                        {
                            Description = "Phase 2",
                            Duration = 60
                        },
                          new PhaseDto
                        {
                            Description = "Phase 3",
                            Duration = 60
                        }
                    }
                ));

            _repoOpTypeMock
                .Setup(x => x.GetRequiredStaffByOperationTypeIdAsync(It.IsAny<OperationTypeId>()))
                .ReturnsAsync(new List<RequiredStaff>
                {
                     new RequiredStaff(1, "Doctor", "10101010")
                });


            var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "10101010");

            _repoStaMock
                .Setup(x => x.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staffMock.Object);

            _appointmentStaffRepoMock
                .Setup(x => x.IsStaffAvailableAsync(It.IsAny<StaffId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            var roomMock = new Mock<Room>(
                      new RoomNumber("R102"),
                      new RoomTypeCode("BLCOP-T1"),
                      new Mock<Capacity>(5).Object,
                       new List<Equipment>
                       {
                    new Mock<Equipment>("Surgical Light").Object,
                    new Mock<Equipment>("Surgical Table").Object
                       },
                       CurrentStatus.Available,
                       new List<Slot>
                       {
                    new Mock<Slot>("Routine surgery", "14:00", "16:00", "2024-07-08", "2024-07-08").Object
                       });

            _repoRoomMock.Setup(x => x.GetByIdAsync(It.IsAny<RoomNumber>())).ReturnsAsync(roomMock.Object);
            _repoRoomMock.Setup(x => x.IsRoomAvailableAsync(It.IsAny<RoomNumber>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(true);


            // Act
            var result = await _service.AddAsync(createDto);

            // Assert
            Assert.Equal(dto.Status, result.Status);
            Assert.Equal(dto.OperationRequestId, result.OperationRequestId);
            Assert.Equal(dto.RoomNumber, result.RoomNumber);
            Assert.Equal(dto.StartTime, result.StartTime);
            Assert.Equal(dto.EndTime, result.EndTime);
            Assert.Equal(
                DateTime.Parse(dto.StartDate).ToString("MM/dd/yyyy HH:mm"),
                DateTime.Parse(result.StartDate).ToString("MM/dd/yyyy HH:mm")
            );
            Assert.Equal(
                DateTime.Parse(dto.EndDate).ToString("MM/dd/yyyy HH:mm"),
                DateTime.Parse(result.EndDate).ToString("MM/dd/yyyy HH:mm")
            );

            Assert.NotNull(result.Staffs);
            Assert.Equal(dto.Staffs.Count, result.Staffs.Count);
            for (int i = 0; i < dto.Staffs.Count; i++)
            {
                Assert.Equal(dto.Staffs[i], result.Staffs[i]);
            }
        }
       
        [Fact]
        public async Task UpdateAppointment_ReturnsOkResult()
        {
            // Arrange
            var _service = new AppointmentService(
                _unitOfWorkMock.Object,
                _repoAppointMock.Object,
                _repoOpReqMock.Object,
                _repoRoomMock.Object,
                _repoOpTypeMock.Object,
                _repoStaMock.Object,
                _repoReqStaMock.Object,
                _appointmentStaffRepoMock.Object
            );

            string operationRequestIdValue = "test1";
            var operationRequestId = new OperationRequestId(operationRequestIdValue);

            // Mock existing appointment
            var existingAppointment = new Appointment(
                operationRequestId,
                "R102",
                "14:00:00",
                "16:00:00",
                "2024-04-04",
                "2024-04-04"
            );

            var appointmentId = Guid.Parse(existingAppointment.Id.AsString());

            // Input DTO for update
            var updateDto = new UpdateAppointmentDto(
                appointmentId.ToString(),
                "R201",
                "14:00:00",
                "16:00:00",
                "2024-04-04",
                "2024-04-04",
                new List<string> { "D202500001" }
            );

            // Expected updated appointment DTO
            var expectedAppointment = new AppointmentDto(
                appointmentId,
                "Scheduled",
                operationRequestIdValue,
                "R201",
                "14:00:00",
                "16:00:00",
                "4/4/2024 12:00:00 AM",
                "4/4/2024 12:00:00 AM",
                new List<string> { "D202500001" }
            );

            // Mock repository behaviors
            _repoAppointMock.Setup(repo => repo.GetAppointmentByIdWithStaff(appointmentId.ToString()))
                .ReturnsAsync(existingAppointment);

            _repoOpReqMock.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationRequestId>()))
                .ReturnsAsync(new OperationRequest(
                    operationRequestIdValue,
                    "2024-07-01",
                    "Elective",
                    "2024-06-01",
                    "O202400001",
                    "Test operation",
                    "202501000001",
                    "operation-type-id-1"
                ));

            _repoOpTypeMock.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationTypeId>()))
                .ReturnsAsync(new OperationType(
                    "Type1",
                    120,
                    true,
                    new List<RequiredStaffDto>
                    {
                        new RequiredStaffDto { StaffQuantity = 1, Function = "Doctor", Specialization = "10101010" }
                    },
                    new List<PhaseDto>
                    {
                        new PhaseDto { Description = "Phase 1", Duration = 60 },
                        new PhaseDto { Description = "Phase 2", Duration = 60 },
                        new PhaseDto { Description = "Phase 3", Duration = 60 }
                    }
                ));

            _repoOpTypeMock.Setup(repo => repo.GetRequiredStaffByOperationTypeIdAsync(It.IsAny<OperationTypeId>()))
                .ReturnsAsync(new List<RequiredStaff>
                {
                    new RequiredStaff(1, "Doctor", "10101010")
                });

            _repoStaMock.Setup(repo => repo.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(new Staff(
                    "00001",
                    "country, 12345, street test",
                    "12345",
                    "first",
                    "last",
                    "first last",
                    "email@email.com",
                    "+123",
                    "12345678",
                    "doctor",
                    "10101010"
                ));

            _appointmentStaffRepoMock.Setup(repo => repo.IsStaffAvailableAsync(It.IsAny<StaffId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            _repoRoomMock.Setup(repo => repo.GetByIdAsync(It.IsAny<RoomNumber>()))
                .ReturnsAsync(new Room(
                    new RoomNumber("R102"),
                    new RoomTypeCode("BLCOP-T1"),
                    new Capacity(5),
                    new List<Equipment> { new Equipment("Surgical Light"), new Equipment("Surgical Table") },
                    CurrentStatus.Available,
                    new List<Slot> { new Slot("Routine surgery", "14:00", "16:00", "2024-04-04", "2024-04-04") }
                ));

            _repoRoomMock.Setup(repo => repo.IsRoomAvailableAsync(It.IsAny<RoomNumber>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.Equal(expectedAppointment.Id, result.Id);
            Assert.Equal(expectedAppointment.Status, result.Status);
            Assert.Equal(expectedAppointment.OperationRequestId, result.OperationRequestId);
            Assert.Equal(expectedAppointment.RoomNumber, result.RoomNumber);
            Assert.Equal(expectedAppointment.StartTime, result.StartTime);
            Assert.Equal(expectedAppointment.EndTime, result.EndTime);
            Assert.Equal(
                DateTime.Parse(expectedAppointment.StartDate).ToString("MM/dd/yyyy HH:mm"),
                DateTime.Parse(result.StartDate).ToString("MM/dd/yyyy HH:mm")
            );
            Assert.Equal(
                DateTime.Parse(expectedAppointment.EndDate).ToString("MM/dd/yyyy HH:mm"),
                DateTime.Parse(result.EndDate).ToString("MM/dd/yyyy HH:mm")
            );
            Assert.NotNull(result.Staffs);
            Assert.Equal(expectedAppointment.Staffs.Count, result.Staffs.Count);
            for (int i = 0; i < expectedAppointment.Staffs.Count; i++)
            {
                Assert.Equal(expectedAppointment.Staffs[i], result.Staffs[i]);
            }
        }

    }

}