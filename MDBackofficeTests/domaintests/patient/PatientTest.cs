using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
using System.Xml;
using Xunit;

namespace MDBackofficeTests.domaintests.patient;

public class PatientTests
{

    [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "9100087827", "ritabarbosa@email.com", "2004-12-15", "000001")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "FEMALE", "+351", "910000001", "9100087845", "anaguterres@example.com", "2004-12-15", "000002")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "fEMALE", "+351", "910000001", "9100087845", "anaguterres@example.com", "2004-12-15", "000005")]
    [InlineData("Test", "Second", "Test First Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "2006-04-09", "000003")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "MALE", "+351", "910000003", "91120871245", "lastTest@example.com", "2000-12-15", "000004")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "mALE", "+351", "910000003", "91120871245", "lastTest@example.com", "2000-12-15", "000004")]
    public void WhenPassingCorrectData_ThenPatientInstantiated(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        new Patient(
            firstName,
            lastName,
            fullName,
            address,
            gender,
            countryCode,
            phoneNumber,
            emergencyContact,
            email,
            dateBirth,
            seqNumber
        );
    }

    [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "9100087827", "ritabarbosa@email.com", "20/10/2004", "001")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "Female", "+351", "910000001", "9100087845", "anaguterres@example.com", "15/12/2004", "11000002")]
    [InlineData("Test", "Second", "Test First Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09/04/2006", "ABCdef")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "GhijkLetip")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "JkJ")]
    public void WhenPassingInvalidSeqNumber_ThenThrowsException(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Patient(
                firstName,
                lastName,
                fullName,
                address,
                gender,
                countryCode,
                phoneNumber,
                emergencyContact,
                email,
                dateBirth,
                seqNumber
            )
        );
    }

    [Theory]
    [InlineData("", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "9100087827", "ritabarbosa@email.com", "20/10/2004", "000001")]
    [InlineData("Ana", null, "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "Female", "+351", "910000001", "9100087845", "anaguterres@example.com", "15/12/2004", "000002")]
    [InlineData(null, "Second", "null", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09/04/2006", "000003")]
    [InlineData("João", "", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "000004")]
    [InlineData("João", "Last", "", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "000004")]
    [InlineData("João", "Last", null, "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "000004")]
    public void WhenPassingInvalidName_ThenThrowsException(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Patient(
                firstName,
                lastName,
                fullName,
                address,
                gender,
                countryCode,
                phoneNumber,
                emergencyContact,
                email,
                dateBirth,
                seqNumber
            )
        );
    }

    [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "", "Female", "+351", "910000000", "9100087827", "ritabarbosa@email.com", "20/10/2004", "P123456")]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", null, "Female", "+351", "910000000", "9100087827", "ritabarbosa@email.com", "20/10/2004", "P123456")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856", "Female", "+351", "910000001", "9100087845", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, Rua do Salmão", "Female", "+351", "910000001", "9100087845", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "4580-856, Rua do Salmão", "Female", "+351", "910000001", "9100087845", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Test", "Second", "Test First Second", " , 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09/04/2006", "P984321")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856,  ", "Male", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "P444321")]
    [InlineData("João", "Last", "João Last", "Portugal,  , Rua do Salmão", "Male", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "P444321")]
    public void WhenPassingInvalidAddress_ThenThrowsException(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Patient(
                firstName,
                lastName,
                fullName,
                address,
                gender,
                countryCode,
                phoneNumber,
                emergencyContact,
                email,
                dateBirth,
                seqNumber
            )
        );
    }

    [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "", "910000000", "910000000", "ritabarbosa@email.com", "20/10/2004", "P123456")]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", null, "910000000", "910000000", "ritabarbosa@email.com", "20/10/2004", "P123456")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "Female", "+351456", "910000001", "910000001", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "Female", "+35As", "910000001", "910000001", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "Female", "+351", "91000AB01", "910000001", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "Female", "51", "910000001", "910000001", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "Female", "+351", "123456789123456789", "910000001", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Test", "Second", "Test Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "9100000005", "", "test@example.com", "09/04/2006", "P984321")]
    [InlineData("Test", "Second", "Test Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "9100000005", null, "test@example.com", "09/04/2006", "P984321")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+4444444444", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "P444321")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "9112087124512345678", "lastTest@example.com", "15/12/2000", "P444321")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "911EFs678", "lastTest@example.com", "15/12/2000", "P444321")]
    public void WhenPassingInvalidPhone_ThenThrowsException(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Patient(
                firstName,
                lastName,
                fullName,
                address,
                gender,
                countryCode,
                phoneNumber,
                emergencyContact,
                email,
                dateBirth,
                seqNumber
            )
        );
    }

    [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "910000000", "ritabarbosa@email.com", "20/10/2004", "P123456")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Sardinha", "Female", "+351", "910000001", "910000001", "anaguterres@example.com", "15/12/2004", "P654321")]
    [InlineData("Test", "Second", "Test First Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "9104987845", "9104987845", "test@example.com", "09/04/2006", "P984321")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "9112087124576", "9112087124576", "lastTest@example.com", "15/12/2000", "P444321")]
    public void WhenPhoneNumberAndEmergencyContactAreSame_ThenThrowsException(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Patient(
                firstName,
                lastName,
                fullName,
                address,
                gender,
                countryCode,
                phoneNumber,
                emergencyContact,
                email,
                dateBirth,
                seqNumber
            )
        );
    }

    [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "910000000", "ritabarbosa@email.com", "40/10/2004", "P123456")]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "910000000", "ritabarbosa@email.com", "20/15/2004", "P123456")]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "910000000", "ritabarbosa@email.com", "data inválida", "P123456")]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "910000000", "ritabarbosa@email.com", "dez de janeiro", "P123456")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Sardinha", "Female", "+351", "910000001", "910000001", "anaguterres@example.com", "15-48-2004", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Sardinha", "Female", "+351", "910000001", "910000001", "anaguterres@example.com", "100-10-206", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Sardinha", "Female", "+351", "910000001", "910000001", "anaguterres@example.com", "16.12.2007", "P654321")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Sardinha", "Female", "+351", "910000001", "910000001", "anaguterres@example.com", "15*11*2001", "P654321")]
    [InlineData("Test", "Second", "Test Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09/04/2006 27:00", "P984321")]
    [InlineData("Test", "Second", "Test Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09/04/2006 12:100", "P984321")]
    [InlineData("Test", "Second", "Test Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09-04-2006 30:01", "P984321")]
    [InlineData("Test", "Second", "Test Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09-04-2006 09:50:00", "P984321")]
    [InlineData("Test", "Second", "Test Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09-04-2006 09:20:666", "P984321")]
    [InlineData("Test", "Second", "Test Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "09-04-2006 177:20:00", "P984321")]
    public void WhenPassingInvalidDateOfBirth_ThenThrowsException(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Patient(
                firstName,
                lastName,
                fullName,
                address,
                gender,
                countryCode,
                phoneNumber,
                emergencyContact,
                email,
                dateBirth,
                seqNumber
            )
        );
    }

    [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "910000000", "", "20/10/2004", "P123456")]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "910000000", null, "20/10/2004", "P123456")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Sardinha", "Female", "+351", "910000001", "910000001", "anaguterresexample.com", "15/12/2004", "P654321")]
    [InlineData("Test", "Second", "Test First Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@examplecom", "09/04/2006", "P984321")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "P444321")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "91120871245", "lastTest@example-com", "15/12/2000", "P444321")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Male", "+351", "910000003", "91120871245", "lastTest*example.com", "15/12/2000", "P444321")]
    public void WhenPassingInvalidEmail_ThenThrowsException(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Patient(
                firstName,
                lastName,
                fullName,
                address,
                gender,
                countryCode,
                phoneNumber,
                emergencyContact,
                email,
                dateBirth,
                seqNumber
            )
        );
    }


     [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "", "+351", "910000000", "9100087827", "ritabarbosa@email.com", "20/10/2004", "000001")]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", null, "+351", "910000000", "9100087827", "ritabarbosa@email.com", "20/10/2004", "000001")]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "F7male", "+351", "910000000", "9100087827", "ritabarbosa@email.com", "20/10/2004", "000001")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "Something", "+351", "910000001", "9100087845", "anaguterres@example.com", "15/12/2004", "000002")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "ALGO", "+351", "910000001", "9100087845", "anaguterres@example.com", "15/12/2004", "000002")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "FEMA5E", "+351", "910000001", "9100087845", "anaguterres@example.com", "15/12/2004", "000002")]
    [InlineData("Test", "Second", "Test First Second", "Portugal, 4580-856, Rua do Salmão", "M2LE", "+351", "910000002", "9104987845", "test@example.com", "09/04/2006", "000003")]
    [InlineData("Test", "Second", "Test First Second", "Portugal, 4580-856, Rua do Salmão", "M4le", "+351", "910000002", "9104987845", "test@example.com", "09/04/2006", "000003")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "M/le", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "000004")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Mal*e", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "000004")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "Ma-le", "+351", "910000003", "91120871245", "lastTest@example.com", "15/12/2000", "000004")]
    public void WhenPassingInvalidGender_ThenThrowsException(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            new Patient(
                firstName,
                lastName,
                fullName,
                address,
                gender,
                countryCode,
                phoneNumber,
                emergencyContact,
                email,
                dateBirth,
                seqNumber
            )
        );
    }


    [Fact]
    public void WhenChangingPhoneWithDifferentPhoneNumber_ThenUpdatesPhoneNumber()
    {
        // Arrange
        var countryCode = "+351";
        var phoneNumber = "987654321";
        var newPhone =    "+351 900001234";
        var entity = new Patient("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", countryCode, phoneNumber, "9100087827", "ritabarbosa@email.com", "2004-12-15", "000001");

        // Act
        entity.ChangePhone(newPhone);

        // Assert
        Assert.Equal(new Phone(newPhone), entity.PhoneNumber);
    }

    [Fact]
    public void WhenChangingPhoneWithIdenticalPhoneNumber_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var countryCode = "+351";
        var phoneNumber = "123456789";
        var entity = new Patient("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", countryCode, phoneNumber, "9100087827", "ritabarbosa@email.com", "2004-12-15", "000001");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => entity.ChangePhone("${countryCode} {phoneNumber}"));
    }
    [Fact]
    public void WhenChangingEmailWithDifferentPhoneNumber_ThenUpdatesEmail()
    {
        // Arrange
        var email = "test1@email.com";
        var newEmail = "update@email.com";
        var entity = new Patient("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "987654321", "9100087827", email, "2004-12-15", "000001");

        // Act
        entity.ChangeEmail(newEmail);

        // Assert
        Assert.Equal(new Email(newEmail), entity.Email);
    }

    [Fact]
    public void WhenChangingEmailWithIdenticalPhoneNumber_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var email = "test@email.com";
        var entity = new Patient("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "987654321", "9100087827", email, "2004-12-15", "000001");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => entity.ChangePhone(email));
    }

    [Fact]
    public void WhenChangingAddressWithDifferentAddress_ThenUpdatesAddress()
    {
        // Arrange
        var address = "Portugal, 4590-850, Rua da Sardinha";
        var newAddress = "Portugal, 4500-750, Rua das Flores";
        var entity = new Patient("Rita", "Barbosa", "Rita Barbosa", address, "Female", "+351", "987654321", "9100087827", "test@email.com", "2004-12-15", "000001");

        // Act
        entity.ChangeAddress(newAddress);

        // Assert
        Assert.Equal(new ResidentialAddress(newAddress), entity.Address);
    }

    [Fact]
    public void WhenChangingAddressWithIdenticalAddress_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var address = "Portugal, 4590-850, Rua da Sardinha";
        var entity = new Patient("Rita", "Barbosa", "Rita Barbosa", address, "Female", "+351", "987654321", "9100087827", "test@email.com", "2004-12-15", "000001");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => entity.ChangeAddress(address));
    }

    [Fact]
    public void WhenChangingNameWithDifferentName_ThenUpdatesName()
    {
        // Arrange
        var oldName = "Rita Barbosa";
        var newName = "Rita Silva";
        var entity = new Patient("Rita", "Barbosa", oldName, "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "987654321", "9100087827", "test@email.com", "2004-12-15", "000001");

        // Act
        entity.ChangeName(newName);

        // Assert
        Assert.Equal(new Name(newName), entity.Name);
    }

    [Fact]
    public void WhenChangingNameWithIdenticalName_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var name = "Rita Barbosa";
        var entity = new Patient("Rita", "Barbosa", name, "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "987654321", "9100087827", "test@email.com", "2004-12-15", "000001");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => entity.ChangeName(name));
    }

    [Fact]
    public void WhenChangingDateOfBirthWithDifferentDate_ThenUpdatesDateOfBirth()
    {
        // Arrange
        var oldDateOfBirth = "2004-12-15";
        var newDateOfBirth = "2003-08-05";
        var entity = new Patient("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "987654321", "9100087827", "test@email.com", oldDateOfBirth, "000001");

        // Act
        entity.ChangeDateBirth(newDateOfBirth);

        // Assert
        Assert.Equal(DateTime.Parse(newDateOfBirth), entity.DateBirth);
    }

    [Fact]
    public void WhenChangingDateOfBirthWithIdenticalDate_ThenThrowBusinessRuleValidationException()
    {
        // Arrange
        var dateOfBirth = "2004-12-15";
        var entity = new Patient("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "987654321", "9100087827", "test@email.com", dateOfBirth, "000001");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() =>    entity.ChangeDateBirth(dateOfBirth));
    }

    [Theory]
    [InlineData("Rita", "Barbosa", "Rita Barbosa", "Portugal, 4590-850, Rua da Sardinha", "Female", "+351", "910000000", "9100087827", "ritabarbosa@email.com", "2004-12-15", "000001")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "FEMALE", "+351", "910000001", "9100087845", "anaguterres@example.com", "2004-12-15", "000002")]
    [InlineData("Ana", "Guterres", "Ana Guterres", "Portugal, 4580-856, Rua da Faneca", "fEMALE", "+351", "910000001", "9100087845", "anaguterres@example.com", "2004-12-15", "000005")]
    [InlineData("Test", "Second", "Test First Second", "Portugal, 4580-856, Rua do Salmão", "Male", "+351", "910000002", "9104987845", "test@example.com", "2006-04-09", "000003")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "MALE", "+351", "910000003", "91120871245", "lastTest@example.com", "2000-12-15", "000004")]
    [InlineData("João", "Last", "João Last", "Portugal, 4580-856, Rua do Bacalhau", "mALE", "+351", "910000003", "91120871245", "lastTest@example.com", "2000-12-15", "000004")]
    public void Anonymize_ShouldRedactPatientInformation(string firstName, string lastName, string fullName, string address, string gender, string countryCode, string phoneNumber, string emergencyContact, string email, string dateBirth, string seqNumber)
    {
        // Arrange
        var patient = new Patient(
            firstName,
            lastName,
            fullName,
            address,
            gender,
            countryCode,
            phoneNumber,
            emergencyContact,
            email,
            dateBirth,
            seqNumber
        );

        // Act
        bool result = patient.Anonymize();

        // Assert
        Assert.True(result);
        Assert.Equal("[REDACTED]", patient.Name.FirstName);
        Assert.Equal("[REDACTED]", patient.Name.LastName);
        Assert.Equal("[REDACTED]", patient.Address.Country);
        Assert.Equal("[REDACTED]", patient.Address.PostalCode);
        Assert.Equal("[REDACTED]", patient.Address.Residence);
        Assert.Equal("[REDACTED]", patient.Email.EmailAddress);
        Assert.Equal("[REDACTED]", patient.PhoneNumber.PhoneNumber);
        Assert.Equal("[REDACTED]", patient.PhoneNumber.CountryCode);
        Assert.Equal("[REDACTED]", patient.EmergencyContact.PhoneNumber);
        Assert.Equal("[REDACTED]", patient.EmergencyContact.CountryCode);
        Assert.False(patient.Status);
    }

}
