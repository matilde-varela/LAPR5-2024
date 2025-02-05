using Microsoft.AspNetCore.Identity;

namespace MDBackoffice.Domain.Users
{

    public class User : IdentityUser
    {
        public virtual bool Status { get; set; }

        public virtual void changeStatus(bool status)
        {
            this.Status = status;
        }
    }
}