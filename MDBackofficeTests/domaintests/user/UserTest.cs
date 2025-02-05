using MDBackoffice.Domain.Users;
using Xunit;

namespace MDBackofficeTests.domaintests.user;

public class UserTests
{

    [Fact]
    public void WhenUserIsInstantiated_PropertiesAreSetCorrectly()
    {
        var email = "test@example.com";
        var userName = "testUser";

        var user = new User { UserName = userName, Email = email };

        Assert.Equal(userName, user.UserName);
        Assert.Equal(email, user.Email);
        Assert.False(user.Status);
    }


    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void WhenChangeStatusIsCalled_StatusIsUpdated(bool status, bool update)
    {
        var user = new User { Status = status };
        user.changeStatus(update);
        Assert.Equal(update, !status);
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhenUserIsInstantiated_StatusIsSetCorrectly(bool initialStatus)
    {
        var user = new User { Status = initialStatus };
        Assert.Equal(initialStatus, user.Status);
    }

    [Fact]
    public void WhenUserPropertiesAreUpdated_PropertiesReflectChanges()
    {
        var user = new User { UserName = "Maria", Email = "maria@example.com" };

        user.UserName = "Ana";
        user.Email = "ana@example.com";

        Assert.Equal("Ana", user.UserName);
        Assert.Equal("ana@example.com", user.Email);
    }

}

