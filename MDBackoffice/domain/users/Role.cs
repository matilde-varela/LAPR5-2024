using Microsoft.AspNetCore.Identity;

namespace MDBackoffice.Domain.Users
{
    public class Role : IdentityRole
    {
        // Parameterless constructor is required for EF Core
        public Role() : base() { }

        // You can add additional properties here if needed
        public Role(string roleName) : base(roleName) { }
    }
}