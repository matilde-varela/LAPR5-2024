
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Tokens
{
    public interface ITokenRepository: IRepository<Token, TokenId>
    {
    }
}