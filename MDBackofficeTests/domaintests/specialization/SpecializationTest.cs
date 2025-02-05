using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using System.Xml;
using Xunit;

namespace MDBackofficeTests.domaintests.specialization;

public class SpecializationTests
{

    [Theory]
    [InlineData("10101010", "orthopaedics", "Description 1")]
    [InlineData("10101001", "anesthesiology", "Description 2")]
    [InlineData("10101110", "instrumenting", "Description 3")]
    [InlineData("10101011", "circulating", "Description 4")]
    public void WhenPassingCorrectData_ThenSpecializationInstantiated(string code, string designation, string description)
    {
        new Specialization(
            code,
            designation,
            description
        );
    }

    [Theory]
    [InlineData("", "orthopaedics", "Description 1")]
    [InlineData("0001", "anesthesiology", "Description 3")]
    [InlineData("BLCO", "instrumenting", "Description 4")]
    public void WhenPassingInvalidCode_ThenThrowsException(string code, string designation, string description)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Specialization(
            code,
            designation,
            description
            )
        );
    }

    [Theory]
    [InlineData("10101011", "", "Description 1")]
    public void WhenPassingInvalidDesignation_ThenThrowsException(string code, string designation, string description)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Specialization(
            code,
            designation,
            description
            )
        );
    }

}