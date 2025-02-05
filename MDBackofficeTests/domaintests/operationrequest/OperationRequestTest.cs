using System;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using Moq;
using Xunit;

namespace MDBackofficeTests.domaintests.operationrequest;

public class OperationRequestTests
{

    [Theory]
    [InlineData("TestCode", "2024-12-31", "Elective", "2023-10-25", "N202400001", "Test operation request", "202410000001", "Test Operation")]
    [InlineData("TestCode2", "2024-12-10", "Emergency", "2023-10-03", "D202300333", "Test operation request 2", "202410000023", "Test Operation 2")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Urgent", "2023-10-03", "O202300333", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    public void WhenPassingCorrectData_ThenOperationRequestInstantiated(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
    {

        new OperationRequest(
         code,
         deadLineDate,
         priority,
         dateOfRequest,
         staffId,
         description,
         patientId,
         operationTypeId
     );
    }

    [Theory]
    [InlineData("TestCode", "sdfjbsdfib", "Elective", "2024-10-25", "N202400001", "Test operation request", "202410000001", "Test Operation")]
    [InlineData("TestCode2", " ", "Emergency", "2023-10-03", "D202300333", "Test operation request 2", "202410000023", "Test Operation 2")]
    [InlineData("TestCode2", "", "Emergency", "2023-10-03", "D202300333", "Test operation request 2", "202410000023", "Test Operation 2")]
    [InlineData("TestCode2", null, "Emergency", "2023-10-03", "D202300333", "Test operation request 2", "202410000023", "Test Operation 2")]
    [InlineData("TestCode2", "2022-10-03", "Emergency", "2023-10-03", "D202300333", "Test operation request 2", "202410000023", "Test Operation 2")]

    public void WhenPassingInvalidDeadlineDate_ThenThrowsException(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
               new OperationRequest(
                code,
                deadLineDate,
                priority,
                dateOfRequest,
                staffId,
                description,
                patientId,
                operationTypeId
            )
        );
    }

    [Theory]
    [InlineData("TestCode", "2024-12-31", "svsgfghgfjfs", "2023-10-25", "N202400001", "Test operation request", "202410000001", "Test Operation")]
    [InlineData("TestCode2", "2024-12-10", " ", "2023-10-03", "D202300333", "Test operation request 2", "202410000023", "Test Operation 2")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "", "2023-10-03", "O202300333", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", null, "2023-10-03", "O202300333", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    public void WhenPassingInvalidPriority_ThenThrowsException(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
               new OperationRequest(
                code,
                deadLineDate,
                priority,
                dateOfRequest,
                staffId,
                description,
                patientId,
                operationTypeId
            )
        );
    }

    [Theory]
    [InlineData("TestCode", "2024-12-31", "Elective", "sdvfdbsbsfvadv", "N202400001", "Test operation request", "202410000001", "Test Operation")]
    [InlineData("TestCode2", "2023-12-10", "Elective", " ", "D202300333", "Test operation request 2", "202410000023", "Test Operation 2")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2023-12-10", "Elective", " ", "O202300333", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2023-12-10", "Elective", null, "O202300333", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2023-12-10", "Elective", "2024-10-10", "O202300333", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    public void WhenPassingInvalidDateOfRequest_ThenThrowsException(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
               new OperationRequest(
                code,
                deadLineDate,
                priority,
                dateOfRequest,
                staffId,
                description,
                patientId,
                operationTypeId
            )
        );
    }

    [Theory]
    [InlineData("TestCode", "2024-12-10", "Elective", "2023-12-31", "sdgdfsdsv", "Test operation request", "202410000001", "Test Operation")]
    [InlineData("TestCode2", "2024-12-10", "Elective", "2023-12-31", "  ", "Test operation request 2", "202410000023", "Test Operation 2")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Elective", "2023-12-31", "H202300333", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Elective", "2023-12-31", "O20230", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Elective", "2023-12-31", "O", "asfasfoh operation rsafjkhaaequest 2", "202410000100", "Test Operation 3sfsduiagd osb")]
    public void WhenPassingInvalidStaffId_ThenThrowsException(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
               new OperationRequest(
                code,
                deadLineDate,
                priority,
                dateOfRequest,
                staffId,
                description,
                patientId,
                operationTypeId
            )
        );
    }
    [Theory]
    [InlineData("TestCode", "2024-12-10", "Elective", "2023-12-31", "N202400001", " ", "202410000001", "Test Operation")]
    [InlineData("TestCode2", "2024-12-10", "Elective", "2023-12-31", "N202400001", "", "202410000023", "Test Operation 2")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Elective", "2023-12-31", "N202400001", null, "202410000100", "Test Operation 3sfsduiagd osb")]
    public void WhenPassingInvalidDescription_ThenThrowsException(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
               new OperationRequest(
                code,
                deadLineDate,
                priority,
                dateOfRequest,
                staffId,
                description,
                patientId,
                operationTypeId
            )
        );
    }

    [Theory]
    [InlineData("TestCode", "2024-12-10", "Elective", "2023-12-31", "N202400001", "Test Description", "sdgdfsdsv", "Test Operation")]
    [InlineData("TestCode2", "2024-12-10", "Elective", "2023-12-31", "N202400001", "Test Description", "202400", "Test Operation 2")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Elective", "2023-12-31", "N202400001", "Test Description", " ", "Test Operation 3sfsduiagd osb")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Elective", "2023-12-31", "N202400001", "Test Description", "", "Test Operation 3sfsduiagd osb")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Elective", "2023-12-31", "N202400001", "Test Description", null, "Test Operation 3sfsduiagd osb")]
    public void WhenPassingInvalidPatientId_ThenThrowsException(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
                 new OperationRequest(
                  code,
                  deadLineDate,
                  priority,
                  dateOfRequest,
                  staffId,
                  description,
                  patientId,
                  operationTypeId
              )
          );
        Assert.True(exception is BusinessRuleValidationException || exception is NullReferenceException,
            $"Expected exception of type BusinessRuleValidationException or NullReferenceException, but got {exception.GetType()}.");
    }


    [Theory]
    [InlineData("TestCode", "2024-12-10", "Elective", "2023-12-31", "N202400001", "Test Description", "sdgdfsdsv", "  ")]
    [InlineData("TestCode2", "2024-12-10", "Elective", "2023-12-31", "N202400001", "Test Description", "202400", "")]
    [InlineData("sdfsdf-sdsdfsfd-sfsdgfsfd", "2024-12-10", "Elective", "2023-12-31", "N202400001", "Test Description", " ", null)]
    public void WhenPassingInvalidOperationTypeId_ThenThrowsException(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
                 new OperationRequest(
                  code,
                  deadLineDate,
                  priority,
                  dateOfRequest,
                  staffId,
                  description,
                  patientId,
                  operationTypeId
              )
          );
        Assert.True(exception is BusinessRuleValidationException || exception is NullReferenceException,
            $"Expected exception of type BusinessRuleValidationException or NullReferenceException, but got {exception.GetType()}.");
    }

    





}


