namespace MDBackoffice.Domain.Tokens{
    public class TokenDto{

            public string TokenId {get; set;}

            public string UserId { get; set; }

            public string ExpirationTime { get; set; }

            public string Code { get; set; }

            public TokenTypeDto TokenType { get; set; }

            public bool Active { get; set; }

    }
}
