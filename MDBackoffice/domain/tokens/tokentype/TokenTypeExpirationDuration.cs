using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Tokens
{
        public class TokenTypeExpirationDuration : IValueObject {

        public int Hours { get; set; }

            private TokenTypeExpirationDuration(){}

            public TokenTypeExpirationDuration(int time) {
                if (time < 0)
                {
                    throw new ArgumentException("The duration in hours of a token type cannot be negative.");
                }
                this.Hours = time;
            }

            public void ChangeHours(int time){
                this.Hours = time;
            }

        }

}