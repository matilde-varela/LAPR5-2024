using MDBackoffice.Domain.Tokens;
using MDBackoffice.Infrastructure.Shared;

namespace MDBackoffice.Infrastructure.Tokens
{
    public class TokenRepository : BaseRepository<Token, TokenId>, ITokenRepository
    {
    
        public TokenRepository(MDBackofficeDbContext context):base(context.Tokens)
        {
           
        }


    }
}