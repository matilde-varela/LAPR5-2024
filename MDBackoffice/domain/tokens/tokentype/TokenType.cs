using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Tokens
{
        public class TokenType : IValueObject {

        public TokenTypeDenomination TypeDenomination { get; set; }
        public TokenTypeExpirationDuration ExpirationDurationHours { get; set; }

        public static TokenType AccountDeletion { get; } = new TokenType("Account Deletion", 24);
        public static TokenType PasswordReset { get; } = new TokenType("Password Reset", 1);

            private TokenType(){}

            public TokenType(string name, int expirationHours) {
                this.TypeDenomination = new TokenTypeDenomination(name);
                this.ExpirationDurationHours = new TokenTypeExpirationDuration(expirationHours);
            }

            public void ChangeTypeDenomination(string name){
                this.TypeDenomination = new TokenTypeDenomination(name);
            }

            public void ChangeExpirationDuration(int duration){
                this.ExpirationDurationHours = new TokenTypeExpirationDuration(duration);
            }

        }

}