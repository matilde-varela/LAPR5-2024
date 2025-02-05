using System.Threading.Tasks;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;
using System;
using Microsoft.AspNetCore.Identity;
using MDBackoffice.Domain.Users;

namespace MDBackoffice.Domain.Tokens
{
    public class TokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenRepository _repo;
        private readonly UserManager<User> _userManager;

        public TokenService(IUnitOfWork unitOfWork, ITokenRepository repo, UserManager<User> userManager)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._userManager = userManager;
        }

        public async Task<List<TokenDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();
            
            List<TokenDto> listDto = list.ConvertAll<TokenDto>(token => new TokenDto{TokenId = token.Id.AsString(), UserId = token.UserId, ExpirationTime = token.ExpirationTime.ToString(), Code = token.Id.AsString(), TokenType = ToDto(token.TokenType), Active = token.Active});

            return listDto;
        }

        public async Task<TokenDto> GetByIdAsync(TokenId id)
        {
            var token = await this._repo.GetByIdAsync(id);
            
            if(token == null)
                return null;

            return new TokenDto{TokenId = token.Id.AsString(), UserId = token.UserId.ToString(), ExpirationTime = token.ExpirationTime.ToString(), Code = token.Id.AsString(), TokenType = ToDto(token.TokenType), Active = token.Active};
        }

        public async Task<TokenDto> AddAsync(TokenDto dto)
        {
            var Token = new Token(dto.UserId, DateTime.Parse(dto.ExpirationTime), new TokenType(dto.TokenType.Name, dto.TokenType.ExpirationDuration));

            await this._repo.AddAsync(Token);
            await this._unitOfWork.CommitAsync();

            return new TokenDto {TokenId = Token.Id.AsString(), UserId = Token.UserId.ToString(), ExpirationTime = Token.ExpirationTime.ToString(), Code = Token.Id.AsString(), TokenType = dto.TokenType, Active = Token.Active};
        }

        public async Task<TokenDto> UpdateAsync(TokenDto dto)
        {
            var Token = await this._repo.GetByIdAsync(new TokenId(dto.Code)); 

            if (Token == null)
                return null;   

            Token.ChangeUser(dto.UserId);
            Token.ChangeExpirationTime(DateTime.Parse(dto.ExpirationTime));
            Token.ChangeTokenType(new TokenType(dto.TokenType.Name, dto.TokenType.ExpirationDuration));
            
            await this._unitOfWork.CommitAsync();

            return new TokenDto { TokenId = Token.Id.AsString(), UserId = Token.UserId.ToString(), ExpirationTime = Token.ExpirationTime.ToString(), Code = Token.Id.AsString(), TokenType = dto.TokenType, Active = Token.Active };
        }
        
        public virtual async Task<TokenDto> InactivateAsync(string id)
        {
            TokenId tokenId = new TokenId(id);
            var token = await this._repo.GetByIdAsync(tokenId); 

            if (token == null)
                return null;   

            token.MarkAsInative();
            
            await this._unitOfWork.CommitAsync();

            return new TokenDto {TokenId = token.Id.AsString(), UserId = token.UserId.ToString(), ExpirationTime = token.ExpirationTime.ToString(), Code = token.Id.AsString(), TokenType = ToDto(token.TokenType), Active = token.Active };
        }

         public async Task<TokenDto> DeleteAsync(TokenId id)
        {
            var Token = await this._repo.GetByIdAsync(id); 

            if (Token == null)
                return null;   

            if (Token.Active)
                throw new BusinessRuleValidationException("It is not possible to delete an active Token.");
            
            this._repo.Remove(Token);
            await this._unitOfWork.CommitAsync();

            return new TokenDto {TokenId = Token.Id.AsString(), UserId = Token.UserId.ToString(), ExpirationTime = Token.ExpirationTime.ToString(), Code = Token.Id.AsString(), TokenType = ToDto(Token.TokenType), Active = Token.Active };
        }


        public TokenTypeDto ToDto(TokenType tokenType){
            return new TokenTypeDto( tokenType.ExpirationDurationHours.Hours, tokenType.TypeDenomination.Denomination );
        }

        public virtual async Task<bool> ConfirmEmailToken(string userId, string token)
        {
            User user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public virtual async Task<string> GeneratePasswordResetTokenAsync(User user){
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<TokenDto> CreateAccountDeletionToken(string username)
        {
            var token = new Token(username, DateTime.Now.AddHours(TokenType.AccountDeletion.ExpirationDurationHours.Hours), TokenType.AccountDeletion);

            return await AddAsync(new TokenDto{TokenId = token.Id.AsString(), UserId = token.UserId, ExpirationTime = token.ExpirationTime.ToString(), Code = token.Id.AsString(), TokenType = ToDto(TokenType.AccountDeletion), Active = token.Active});   
        }


        public virtual async Task<bool> TokenExistsById(string token)
        {
            var tokenDto = await GetByIdAsync(new TokenId(token));
            if (tokenDto == null)
            {
                return false;
            }
            return true;
        }

        public virtual async Task<bool> IsTokenExpired(string token)
        {
            TokenDto tokenDto = await GetByIdAsync(new TokenId(token));   
            var expirationDate = DateTime.Parse(tokenDto.ExpirationTime);

           // Compare (DateTime t1, DateTime t2);
           // Less than zero    | t1 is earlier than t2.
           // Zero              | t1 is the same as t2.
           // Greater than zero | t1 is later than t2.
           if (DateTime.Compare(expirationDate, DateTime.Now) < 0)
               return true;
            
            return false;
        }
        public virtual async Task<bool> IsTokenActive(string token)
        {
            TokenDto tokenDto = await GetByIdAsync(new TokenId(token)); 
            return tokenDto.Active; 
        }
    }

}