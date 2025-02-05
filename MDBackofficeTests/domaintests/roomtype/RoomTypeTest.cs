using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Domain.Shared;
using System.Xml;
using Xunit;

namespace MDBackofficeTests.domaintests.roomtype;

public class RoomTypeTests
{

    [Theory]
    [InlineData("BLCOP-T1", "Bloco Operatório", "Description 1")]
    [InlineData("ARMZM-T1", "Armazém de Equipamentos", "Description 2")]
    [InlineData("BLCOP-T2", "Bloco Operatório Joelho", "Description 3")]
    [InlineData("BLCOP-T3", "Bloco Operatório Mão", "Description 4")]
    public void WhenPassingCorrectData_ThenRoomTypeInstantiated(string code, string designation, string description)
    {
        new RoomType(
            code,
            designation,
            description
        );
    }

    [Theory]
    [InlineData("", "Bloco Operatório", "Description 1")]
    [InlineData("0001", "Bloco Operatório Joelho", "Description 3")]
    [InlineData("BLCOPT3", "Bloco Operatório Mão", "Description 4")]
    public void WhenPassingInvalidCode_ThenThrowsException(string code, string designation, string description)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new RoomType(
            code,
            designation,
            description
            )
        );
    }

    [Theory]
    [InlineData("BLCOP-T1", "", "Description 1")]
    public void WhenPassingInvalidDesignation_ThenThrowsException(string code, string designation, string description)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new RoomType(
            code,
            designation,
            description
            )
        );
    }

}