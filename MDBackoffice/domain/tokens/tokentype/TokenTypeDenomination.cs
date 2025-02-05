using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Tokens
{
        public class TokenTypeDenomination : IValueObject {

        public string Denomination { get; set; }

            private TokenTypeDenomination(){}

            public TokenTypeDenomination(string name) {

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Token type denomination cannot be null or empty.");
            }
                this.Denomination = name;
            }

            public void ChangeDenomination(string name){
                this.Denomination = name;
            }

        }

}