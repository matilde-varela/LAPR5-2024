using MDBackoffice.Controllers;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using System.Collections.Generic;
using Xunit;

namespace MDBackofficeTests.domaintests.operationtype;

public class OperationTypeTests
{

    [Theory]
    [MemberData(nameof(OperationTypeTestValidData))]
    public void WhenPassingCorrectData_ThenOperationTypeInstantiated(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        );
    }

    [Theory]
    [MemberData(nameof(OperationTypeTestInvalidNameBusinessException))]
    public void WhenPassingInvalidName_ThenThrowsBusinessException(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
           new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        )
           );
    }

    [Theory]
    [MemberData(nameof(OperationTypeTestInvalidNameNullException))]
    public void WhenPassingInvalidName_ThenThrowsNullException(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        Assert.Throws<NullReferenceException>(() =>
           new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        )
           );
    }

    [Theory]
    [MemberData(nameof(OperationTypeTestInvalidTotalDuration))]
    public void WhenPassingInvalidTotalDuration_ThenThrowsException(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
           new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        )
           );
    }

    [Theory]
    [MemberData(nameof(OperationTypeTestInvalidNumberOfStaff))]
    public void WhenPassingInvalidNumberOfStaff_ThenThrowsException(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
           new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        )
           );
    }

    [Theory]
    [MemberData(nameof(OperationTypeTestInvalidFunction))]
    public void WhenPassingInvalidFunction_ThenThrowsException(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
           new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        )
           );
    }

    [Theory]
    [MemberData(nameof(OperationTypeTestInvalidSpecializationBusinessException))]
    public void WhenPassingInvalidSpecialization_ThenThrowsExceptionBusinessException(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
           new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        )
           );
    }

    [Theory]
    [MemberData(nameof(OperationTypeTestInvalidSpecializationNullException))]
    public void WhenPassingInvalidSpecialization_ThenThrowsExceptionNullException(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        Assert.Throws<NullReferenceException>(() =>
           new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        )
           );
    }

    [Theory]
    [MemberData(nameof(OperationTypeTestInvalidPhaseDuration))]
    public void WhenPassingInvalidPhaseDuration_ThenThrowsException(
        string name,
        int duration,
        bool status,
        List<RequiredStaffDto> staff,
        List<PhaseDto> phases)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
           new OperationType(
            name,
            duration,
            status,
            staff,
            phases
        )
           );
    }

    public static IEnumerable<object[]> OperationTypeTestValidData()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = "25784120" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };

        yield return new object[]
        {
        "Heart Surgery",
        230,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 2, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 3, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Pre-Op", Duration = 40 },
            new PhaseDto { Description = "Surgery", Duration = 150 },
            new PhaseDto { Description = "Recovery", Duration = 40 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidNameBusinessException()
    {
        yield return new object[]
        {
        "",
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841202" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = "25841202" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "",
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidNameNullException()
    {
        yield return new object[]
        {
        null,
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = "2584120" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        null,
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidTotalDuration()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        -20,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        0,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidStatus()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        120,
        false,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        135,
        false,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidNumberOfStaff()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = -1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = -2, Function = "nurse", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 0, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 0, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };

        yield return new object[]
        {
        "Heart Surgery",
        230,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = -2, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 0, Function = "nurse", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Pre-Op", Duration = 40 },
            new PhaseDto { Description = "Surgery", Duration = 150 },
            new PhaseDto { Description = "Recovery", Duration = 40 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidFunction()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = null, Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 1, Function = null, Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };

        yield return new object[]
        {
        "Heart Surgery",
        230,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 2, Function = "Sun", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 3, Function = "re", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Pre-Op", Duration = 40 },
            new PhaseDto { Description = "Surgery", Duration = 150 },
            new PhaseDto { Description = "Recovery", Duration = 40 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidSpecializationBusinessException()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = "" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "" },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = "" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidSpecializationNullException()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = null },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = null }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 30 },
            new PhaseDto { Description = "Operation", Duration = 60 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = null },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = null }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 20 },
            new PhaseDto { Description = "Surgery", Duration = 80 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidPhaseDescription()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "", Duration = 30 },
            new PhaseDto { Description = "", Duration = 60 },
            new PhaseDto { Description = null, Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "", Duration = 20 },
            new PhaseDto { Description = "", Duration = 80 },
            new PhaseDto { Description = "", Duration = 35 }
        }
        };

        yield return new object[]
        {
        "Heart Surgery",
        230,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 2, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 3, Function = "nurse", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = null, Duration = 40 },
            new PhaseDto { Description = null, Duration = 150 },
            new PhaseDto { Description = null, Duration = 40 }
        }
        };
    }
    public static IEnumerable<object[]> OperationTypeTestInvalidPhaseDuration()
    {
        yield return new object[]
        {
        "Hip Replacement Surgery",
        120,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 2, Function = "nurse", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = -30 },
            new PhaseDto { Description = "Operation", Duration = 0 },
            new PhaseDto { Description = "Recovery", Duration = 30 }
        }
        };

        yield return new object[]
        {
        "ACL Reconstruction Surgery",
        135,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 1, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 1, Function = "assistant", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Preparation", Duration = 0 },
            new PhaseDto { Description = "Surgery", Duration = 0 },
            new PhaseDto { Description = "Post-Op", Duration = 35 }
        }
        };

        yield return new object[]
        {
        "Heart Surgery",
        230,
        true,
        new List<RequiredStaffDto>
        {
            new RequiredStaffDto { StaffQuantity = 2, Function = "doctor", Specialization = "25841209" },
            new RequiredStaffDto { StaffQuantity = 3, Function = "nurse", Specialization = "25841209" }
        },
        new List<PhaseDto>
        {
            new PhaseDto { Description = "Pre-Op", Duration = -40 },
            new PhaseDto { Description = "Surgery", Duration = -150 },
            new PhaseDto { Description = "Recovery", Duration = 40 }
        }
        };
    }
}