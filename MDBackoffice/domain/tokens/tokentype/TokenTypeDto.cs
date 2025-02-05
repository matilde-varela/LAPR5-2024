namespace MDBackoffice.Domain.Tokens
{
    public class TokenTypeDto(int expirationDuration, string name)
    {
        public int ExpirationDuration { get; set; } = expirationDuration;
        public string Name { get; set; } = name;
    }
}
