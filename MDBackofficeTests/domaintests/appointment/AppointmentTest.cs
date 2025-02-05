using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.Shared;
using Xunit;

namespace MDBackofficeTests.domaintests.appointments;

public class AppointmentTests
{

    [Theory]
    [InlineData("TestCode123", "R101", "13:00", "14:00", "2023-10-25", "2023-10-25")]
    [InlineData("TestCode456", "R102", "14:00", "16:00", "2023-11-01", "2023-11-01")]
    [InlineData("TestCode789", "R203", "15:00", "18:00", "2023-09-15", "2023-09-17")]
    public void WhenPassingValidData_ThenAppointmentInstantiated(string code, string roomNumber, string startTime, string endTime,
    string startDate, string endDate)
    {
        var appointment = new Appointment(
            new OperationRequestId(code),
            roomNumber,
            startTime,
            endTime,
            startDate,
            endDate
        );

        Assert.NotNull(appointment);
    }


    [Theory]
    [InlineData("TestCode123", "", "13:00", "14:00", "2023-10-25", "2023-10-25")]
    [InlineData("TestCode789", "R2", "15:00", "18:00", "2023-09-15", "2023-09-17")]
    [InlineData("TestCode789", "R2034", "15:00", "18:00", "2023-09-15", "2023-09-17")]
    public void WhenPassingInvalidRoomNumber_ThenThrowsBusinessException(string code, string roomNumber, string startTime, string endTime,
    string startDate, string endDate)
    {
    Assert.Throws<BusinessRuleValidationException>(() =>
            new Appointment(
            new OperationRequestId(code),
            roomNumber,
            startTime,
            endTime,
            startDate,
            endDate
        ));
    }

    [Theory]
    [InlineData("TestCode456", null, "14:00", "16:00", "2023-11-01", "2023-11-01")]
    public void WhenPassingInvalidRoomNumber_ThenThrowsException(string code, string roomNumber, string startTime, string endTime,
    string startDate, string endDate)
    {
    Assert.Throws<NullReferenceException>(() =>
            new Appointment(
            new OperationRequestId(code),
            roomNumber,
            startTime,
            endTime,
            startDate,
            endDate
        ));
    }


    [Theory]
    [InlineData("TestCode123", "R101", "13:00", "18:00", "2023-10-25", "2023-09-15")]
    [InlineData("TestCode456", "R102", "14:00", "19:00", "2023-11-01", "2023-10-01")]
    [InlineData("TestCode789", "R203", "12:00", "17:00", "2023-09-15", "2023-09-10")]
    public void WhenPassingInvalidDateInterval_ThenThrowsException(string code, string roomNumber, string startTime, string endTime,
    string startDate, string endDate)
    {
    Assert.Throws<BusinessRuleValidationException>(() =>
            new Appointment(
            new OperationRequestId(code),
            roomNumber,
            startTime,
            endTime,
            startDate,
            endDate
        ));
    }


    [Theory]
    [InlineData("TestCode123", "R101", "13:00 00", "12:00", "2023-10-25", "2024-12-31")] //invalid format
    [InlineData("TestCode456", "R102", "16:00", "14:00", "2024-12-01", null)] // valid start time < end time
    public void WhenPassingInvalidTimeInterval_ThenThrowsException(string code, string roomNumber, string startTime, string endTime,
    string startDate, string endDate)
    {
    Assert.Throws<BusinessRuleValidationException>(() =>
            new Appointment(
            new OperationRequestId(code),
            roomNumber,
            startTime,
            endTime,
            startDate,
            endDate
        ));
    }
   
}