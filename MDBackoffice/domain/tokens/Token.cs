using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Tokens
{
    public class Token : Entity<TokenId>, IAggregateRoot
    {

        public string UserId { get; set; }

        public DateTime ExpirationTime { get; set; }

        public TokenType TokenType { get; set; }

        public bool Active{ get; set; }

        private Token()
        {
            // for ORM
        }

        public Token(string user, DateTime expiration, TokenType type)
        {
            this.Id = new TokenId(Guid.NewGuid());
            this.UserId = user;
            this.ExpirationTime = expiration;
            this.TokenType = type;
            this.Active = true;
        }

         public void ChangeUser(string newUser)
        {
            if (!this.Active)
                throw new BusinessRuleValidationException("It is not possible to change the description to an expired token.");
            this.UserId = newUser;
        }

        public void ChangeExpirationTime(DateTime newDate)
        {
            if (!this.Active)
                throw new BusinessRuleValidationException("It is not possible to change the description to an expired token.");
            this.ExpirationTime = newDate;
        }

         public void ChangeTokenType(TokenType newType)
        {
            if (!this.Active)
                throw new BusinessRuleValidationException("It is not possible to change the description to an expired token.");
            this.TokenType = newType;
        }
      
        public void MarkAsInative()
        {
            this.Active = false;
        }
    }
}