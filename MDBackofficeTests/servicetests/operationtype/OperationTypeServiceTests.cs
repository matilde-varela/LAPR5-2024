using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using Moq;
using Xunit;

namespace MDBackofficeTests.servicetests.operationtype
{
    public class OperationTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<LogService> _logServiceMock = new(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<IOperationTypeRepository> _repoMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<OperationTypeRecordService> _opRecordService;
        private readonly OperationTypeService _service;
        private readonly Mock<ISpecializationRepository> _specializationRepo = new Mock<ISpecializationRepository>();        

        public OperationTypeServiceTests()
        {
            _opRecordService = new Mock<OperationTypeRecordService>(_unitOfWorkMock.Object, _logServiceMock.Object, new Mock<IOperationTypeRecordRepository>().Object);

            _service = new OperationTypeService(_unitOfWorkMock.Object, _repoMock.Object,_logServiceMock.Object,_opRecordService.Object, _specializationRepo.Object);
        }

        [Fact]
        public async Task FilterOperationTypes_ReturnsCorrectOperationTypeDtos()
        {
            // Arrange
            var queryParameters = new OperationTypeQueryParametersDto
            {
                queryFilters = new List<OperationTypeListingFilterParametersDto>
                {
                    new OperationTypeListingFilterParametersDto
                    {
                        Name = "test type 1",
                        Specialization = "251010300",
                        Status = "Active"
                    }
                }
            };

            var phasesDto = new List<PhaseDto>
            {
                new PhaseDto {
                            Description = "descrip",
                            Duration = 25
                            },
                new PhaseDto {
                            Description = "descrip2",
                            Duration = 50
                            },
                new PhaseDto {
                            Description = "descrip3",
                            Duration = 25
                            }
            };

            var reqStaffDto = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "251010300"
                }
            };

            var operationTypes = new List<Mock<OperationType>>
            {
                new Mock<OperationType>("test type 1", 100, true, reqStaffDto,phasesDto),
            };
       

            var expectedDtos = new List<OperationTypeDto>
            {
                new OperationTypeDto{Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff =reqStaffDto,Phases = phasesDto }
            };

            var operationTypeObjects = operationTypes.Select(mock => mock.Object).ToList();
            _repoMock.Setup(repo => repo.FilterOperationTypes(queryParameters))
                    .ReturnsAsync(operationTypeObjects);
           
            // Act
            var result = await _service.FilterOperationTypes(queryParameters);
  
            // Assert
            Assert.Equal(expectedDtos.Count, result.Count);
            for (int i = 0; i < expectedDtos.Count; i++)
            {
                Assert.Equal(expectedDtos[i].Name, result[i].Name);
                Assert.Equal(expectedDtos[i].EstimatedDuration, result[i].EstimatedDuration);
                Assert.Equal(expectedDtos[i].Status, result[i].Status);                

            }
            _repoMock.Verify(repo => repo.FilterOperationTypes(queryParameters), Times.Once);
        }


        [Fact]
        public async Task InactivateAsync_ReturnsOperationTypeDTO()
        {
            // Arrange
            var operationTypeId = "test type 1";
             var phasesDto = new List<PhaseDto>
            {
                new PhaseDto {
                            Description = "descrip",
                            Duration = 25
                            },
                new PhaseDto {
                            Description = "descrip2",
                            Duration = 50
                            },
                new PhaseDto {
                            Description = "descrip3",
                            Duration = 25
                            }
            };

            var reqStaffDto = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "251010300"
                }
            };

            var operationTypeMock = new Mock<OperationType>("test type 1", 100, true, reqStaffDto,phasesDto);

            var expectedDto = new OperationTypeDto {Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff = reqStaffDto,Phases = phasesDto };

            _repoMock.Setup(repo => repo.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(operationTypeMock.Object);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.InactivateAsync(operationTypeId);

            // Assert 
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Name, result.Name);
            Assert.Equal(expectedDto.EstimatedDuration, result.EstimatedDuration);
            Assert.NotEqual(expectedDto.Status, result.Status);
        }

        [Fact]
        public async Task EditOperationType_ReturnsOkResult()
        {
            // Arrange
             var phasesDto = new List<PhaseDto>
            {
                new PhaseDto {
                            Description = "descrip",
                            Duration = 25
                            },
                new PhaseDto {
                            Description = "descrip2",
                            Duration = 50
                            },
                new PhaseDto {
                            Description = "descrip3",
                            Duration = 25
                            }
            };

            var reqStaffDto = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "251010300"
                }
            };

            var recordID = "record 1";

            var operationType = new Mock<OperationType>("test type 1", 100, true, reqStaffDto,phasesDto);
            var expectedDto = new OperationTypeDto {Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff = reqStaffDto,Phases = phasesDto };
            var recordDto = new OperationTypeRecordDto(recordID, 1, "2024-10-27", operationType.Object.Id.Value,
             operationType.Object.Name.OperationName, operationType.Object.EstimatedDuration.TotalDurationMinutes,
              operationType.Object.Status.Active, reqStaffDto, phasesDto);

            var editDto = new EditOpTypeDto
            {
                Id = operationType.Object.Id.Value.ToString(),
                Name = "NEW NAME",
                EstimatedDuration = 300,
                Status = true
            };

            _repoMock.Setup(r => r.GetByIdWithStaffAsync(operationType.Object.Id)).ReturnsAsync(operationType.Object);
            _opRecordService.Setup(r =>r.AddAsync(operationType.Object)).ReturnsAsync(recordDto);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.EditOperationType(editDto);


            // Assert
            operationType.Verify(o => o.ChangeEstimatedDuration(300), Times.Once);
            operationType.Verify(o => o.ChangeName("NEW NAME"), Times.Once);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.AtLeastOnce);
            _opRecordService.Verify(r => r.AddAsync(operationType.Object), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ReturnsOperationDto()
        {
            // Arrange
            var operationTypeId = "test type 1";
            var phasesDto = new List<PhaseDto>
    {
        new PhaseDto {
            Description = "descrip",
            Duration = 25
        },
        new PhaseDto {
            Description = "descrip2",
            Duration = 50
        },
        new PhaseDto {
            Description = "descrip3",
            Duration = 25
        }
    };
            var reqStaffDto = new List<RequiredStaffDto>
    {
        new RequiredStaffDto{
            StaffQuantity = 1,
            Function = "doctor",
            Specialization = "251010300"
        }
    };

            var Specialization1 = new Specialization("251010300","Something","Something");

            var dto = new OperationTypeDto { Name = "test type 1", EstimatedDuration = 100, Status = true, RequiredStaff = reqStaffDto, Phases = phasesDto };
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _specializationRepo.Setup(u => u.FindByDenomination("251010300")).ReturnsAsync(Specialization1);
            // Act
            var result = await _service.AddAsync(dto); // Await the result

            // Assert
            Assert.IsType<OperationTypeDto>(result); // Now this checks the correct type
        }


    }
}

