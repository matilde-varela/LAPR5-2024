using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Shared;
using Xunit;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Specializations;

namespace MDBackofficeTests.domaintests.staff;

public class StaffTests
{
    [Theory]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "910000000", "Doctor", "25841809")]
    [InlineData("00002", "Portugal, 4440-440, Rua das Azeitonas", "12346", "Ana", "Guterres", "Ana Algo Guterres", "anaguterres@example.com", "+44", "91000000111", "Nurse", "25841900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example.com", "+2", "24510000001", "Intern", "25001900")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", "Matilde", "Varela", "Matilde Varela", "matildevarela@example.com", "+351", "7840567001", "Assistant", "25001907")]
    public void WhenPassingCorrectData_ThenStaffInstantiated(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string fullName, string email, string countryCode, string phoneNumber, string function, string specializationId)
    {
        new Staff(
            seqNumber,
            address,
            licenseNumber,
            firstName,
            lastName,
            fullName,
            email,
            countryCode,
            phoneNumber,
            function,
            specializationId
        );
    }

    [Theory]
    [InlineData("00001", "", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "910000000", "Doctor", "25841809")]
    [InlineData("00001", null, "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "910000000", "Doctor", "25841809")]
    [InlineData("00002", "Portugal, 4440-440", "12346", "Ana", "Guterres", "Ana Algo Guterres", "anaguterres@example.com", "+44", "91000000111", "Nurse", "25841900")]
    [InlineData("00002", "Portugal, Rua das Azeitonas", "12346", "Ana", "Guterres", "Ana Guterres", "anaguterres@example.com", "+44", "91000000111", "Nurse", "25841900")]
    [InlineData("00003", "5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example.com", "+2", "24510000001", "Intern", "25001900")]
    [InlineData("00003", " , 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example.com", "+2", "24510000001", "Intern", "25001900")]
    [InlineData("00004", "Portugal, 3780-452, ", "93734", "Matilde", "Varela", "Matilde Varela", "matildevarela@example.com", "+351", "7840567001", "Assistant", "25001907")]
    [InlineData("00004", "Portugal,  , Rua das Amoreiras", "93734", "Matilde", "Varela", "Matilde Varela", "matildevarela@example.com", "+351", "7840567001", "Assistant", "25001907")]
    public void WhenPassingInvalidAddress_ThenThrowsException(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string fullName, string email, string countryCode, string phoneNumber, string function, string specializationId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Staff(
                seqNumber,
                address,
                licenseNumber,
                firstName,
                lastName,
                fullName,
                email,
                countryCode,
                phoneNumber,
                function,
                specializationId
            )
        );
    }
    
    [Theory]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "910000000", "Doctor", "25841809")]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", null, "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "910000000", "Doctor", "25841809")]
    [InlineData("00002", "Portugal, 4440-440, Rua das Azeitonas", "12346", "Ana", "", "Ana Guterres", "anaguterres@example.com", "+44", "91000000111", "Nurse", "25841900")]
    [InlineData("00002", "Portugal, 4440-440, Rua das Azeitonas", "12346", "Ana", null, "Ana Guterres", "anaguterres@example.com", "+44", "91000000111", "Nurse", "25841900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "", "josesantos@example.com", "+2", "24510000001", "Intern", "25001900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", null, "josesantos@example.com", "+2", "24510000001", "Intern", "25001900")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", "", "Varela", "", "matildevarela@example.com", "+351", "7840567001", "Assistant", "25001907")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", null, "", "Matilde Varela", "matildevarela@example.com", "+351", "7840567001", "Assistant", "25001907")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", "", "Varela", null, "matildevarela@example.com", "+351", "7840567001", "Assistant", "25001907")]
    public void WhenPassingInvalidName_ThenThrowsException(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string fullName, string email, string countryCode, string phoneNumber, string function, string specializationId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Staff(
                seqNumber,
                address,
                licenseNumber,
                firstName,
                lastName,
                fullName,
                email,
                countryCode,
                phoneNumber,
                function,
                specializationId
            )
        );
    }


    [Theory]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "", "910000000", "Doctor", "25841809")]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", null, "910000000", "Doctor", "25841809")]
    [InlineData("00002", "Portugal, 4440-440, Rua das Azeitonas", "12346", "Ana", "Guterres", "Ana Guterres", "anaguterres@example.com", "+44", "", "Nurse", "25841900")]
    [InlineData("00002", "Portugal, 4440-440, Rua das Azeitonas", "12346", "Ana", "Guterres", "Ana Guterres", "anaguterres@example.com", "+44", null, "Nurse", "25841900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example.com", "2", "24510000001", "Intern", "25001900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example.com", "+2567854", "24510000001", "Intern", "25001900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example.com", "+2a3", "24510000001", "Intern", "25001900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example.com", "+bac", "24510000001", "Intern", "25001900")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", "Matilde", "Varela", "Matilde Varela", "matildevarela@example.com", "+351", "78405677799879001", "Assistant", "25001907")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", "Matilde", "Varela", "Matilde Varela", "matildevarela@example.com", "+351", "784", "Assistant", "25001907")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", "Matilde", "Varela", "Matilde Varela", "matildevarela@example.com", "+351", "7840Abd7001", "Assistant", "25001907")]
    public void WhenPassingInvalidPhone_ThenThrowsException(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string fullName, string email, string countryCode, string phoneNumber, string function, string specializationId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Staff(
                seqNumber,
                address,
                licenseNumber,
                firstName,
                lastName,
                fullName,
                email,
                countryCode,
                phoneNumber,
                function,
                specializationId
            )
        );
    }

    [Theory]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "", "+351", "910000000", "Doctor", "25841809")]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", null, "+351", "910000000", "Doctor", "25841809")]
    [InlineData("00002", "Portugal, 4440-440, Rua das Azeitonas", "12346", "Ana", "Guterres", "Ana Guterres", "anaguterresexample.com", "+44", "91000000111", "Nurse", "25841900")]
    [InlineData("00002", "Portugal, 4440-440, Rua das Azeitonas", "12346", "Ana", "Guterres", "Ana Guterres", "anaguterres@examplecom", "+44", "91000000111", "Nurse", "25841900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos-example.com", "+2", "24510000001", "Intern", "25001900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example*com", "+2", "24510000001", "Intern", "25001900")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", "Matilde", "Varela", "Matilde Varela", "matildevarela+example=com", "+351", "7840567001", "Assistant", "25001907")]
    public void WhenPassingInvalidEmail_ThenThrowsException(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string fullName, string email, string countryCode, string phoneNumber, string function, string specializationId)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Staff(
                seqNumber,
                address,
                licenseNumber,
                firstName,
                lastName,
                fullName,
                email,
                countryCode,
                phoneNumber,
                function,
                specializationId
            )
        );
    }


    [Theory]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "910000000", "", "25841809")]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "910000000", null, "25841809")]
    [InlineData("00002", "Portugal, 4440-440, Rua das Azeitonas", "12346", "Ana", "Guterres", "Ana Guterres", "anaguterres@example.com", "+44", "91000000111", "something", "25841900")]
    [InlineData("00003", "Portugal, 5550-220, Rua das Amoreiras", "56784", "José", "Santos", "José Santos", "josesantos@example.com", "+2", "24510000001", "a342", "25001900")]
    [InlineData("00004", "Portugal, 3780-452, Rua das Aguias", "93734", "Matilde", "Varela", "Matilde Varela", "matildevarela@example.com", "+351", "7840567001", "D", "25001907")]
    public void WhenPassingInvalidFunction_ThenThrowsException(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string fullName, string email, string countryCode, string phoneNumber, string function, string specializationId)
    {
         var exception = Assert.ThrowsAny<Exception>(() =>
            new Staff(
                seqNumber,
                address,
                licenseNumber,
                firstName,
                lastName,
                fullName,
                email,
                countryCode,
                phoneNumber,
                function,
                specializationId
            )
        );
        Assert.True(exception is BusinessRuleValidationException || exception is NullReferenceException,
            $"Expected exception of type BusinessRuleValidationException or NullReferenceException, but got {exception.GetType()}.");
    }

    [Theory]
    [InlineData("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "910000000", "Doctor", "")]
    [InlineData("00002", "Portugal, 4570-860, Rua das Oliveiras", "12346", "Ana", "Guterres", "Ana Guterres", "anaguterres@email.com", "+351", "910000000", "Doctor", null)]
    public void WhenPassingInvalidSpecialization_ThenThrowsException(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string fullName, string email, string countryCode, string phoneNumber, string function, string specializationId)
    {
         var exception = Assert.ThrowsAny<Exception>(() =>
            new Staff(
                seqNumber,
                address,
                licenseNumber,
                firstName,
                lastName,
                fullName,
                email,
                countryCode,
                phoneNumber,
                function,
                specializationId
            )
        );
        Assert.True(exception is BusinessRuleValidationException || exception is NullReferenceException,
            $"Expected exception of type BusinessRuleValidationException or NullReferenceException, but got {exception.GetType()}.");
    }


    [Fact]
    public void WhenChangingPhoneWithDifferentPhoneNumber_ThenUpdatesPhoneNumber()
    {
        // Arrange
        var countryCode = "+351";
        var phoneNumber = "987654321";
        var newPhone = "+351 900001234";
        var entity = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", countryCode, phoneNumber, "Doctor", "25841809");

        // Act
        entity.ChangePhone(newPhone);

        // Assert
        Assert.Equal(new Phone(newPhone), entity.Phone);
    }

    [Fact]
    public void WhenChangingPhoneWithIdenticalPhoneNumber_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var countryCode = "+351";
        var phoneNumber = "123456789";
        var entity = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", countryCode, phoneNumber, "Doctor", "25841809");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => entity.ChangePhone("${countryCode} {phoneNumber}"));
    }
    [Fact]
    public void WhenChangingEmailWithDifferentEmail_ThenUpdatesEmail()
    {
        // Arrange
        var oldEmail = "ritabarbosa@email.com";
        var newEmail = "rita.barbosa.new@email.com";
        var entity = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", oldEmail, "+351", "987654321", "Doctor", "25841809");

        // Act
        entity.ChangeEmail(newEmail);

        // Assert
        Assert.Equal(new Email(newEmail), entity.Email);
    }

    [Fact]
    public void WhenChangingEmailWithIdenticalEmail_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var email = "ritabarbosa@email.com";
        var entity = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841809");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => entity.ChangeEmail(email));
    }

    [Fact]
    public void WhenChangingAddressWithDifferentAddress_ThenUpdatesAddress()
    {
        // Arrange
        var oldAddress = "Portugal, 4570-860, Rua das Oliveiras";
        var newAddress = "Portugal, 4000-000, Rua Nova";
        var entity = new Staff("00001", oldAddress, "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "987654321", "Doctor", "25841809");

        // Act
        entity.ChangeAddress(newAddress);

        // Assert
        Assert.Equal(new ResidentialAddress(newAddress), entity.Address);
    }

    [Fact]
    public void WhenChangingAddressWithIdenticalAddress_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var address = "Portugal, 4570-860, Rua das Oliveiras";
        var entity = new Staff("00001", address, "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "987654321", "Doctor", "25841809");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => entity.ChangeAddress(address));
    }
    [Fact]
    public void WhenChangingSpecializationWithDifferentSpecialization_ThenUpdatesSpecialization()
    {
        // Arrange
        var oldSpecialization = "25841809";
        var newSpecialization = "25841100";
        var entity = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "987654321", "Doctor", oldSpecialization);

        // Act
        entity.ChangeSpecialization(newSpecialization);

        // Assert
        Assert.Equal(new SpecializationCode(newSpecialization), entity.SpecializationId);
    }

    [Fact]
    public void WhenChangingSpecializationWithIdenticalSpecialization_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var specialization = "25841809";
        var entity = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "987654321", "Doctor", specialization);

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => entity.ChangeSpecialization(specialization));
    }

    [Fact]
    public void WhenUpdatingSlotsWithDifferentSlots_ThenUpdatesSlots()
    {
        // Arrange
        var newSlots = new List<SlotsDto> { new SlotsDto("2024-12-20","2024-12-21", "19:00", "21:00") };
        var entity = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", "ritabarbosa@email.com", "+351", "987654321", "Doctor", "25841809");

        entity.AddSlot("10:00", "11:00", "2024-11-20");
        entity.AddSlot("09:00", "11:00", "2024-11-21");

        var expectedSlots = new List<Slot>
        {
            new Slot("19:00", "21:00", "2024-12-20", "2024-12-21")
        };

        // Act
        entity.ChangeSlots(newSlots);

        // Assert
        Assert.Equal(expectedSlots.Count, entity.Slots.Count);
        Assert.Equal(expectedSlots, entity.Slots);
    }

}
