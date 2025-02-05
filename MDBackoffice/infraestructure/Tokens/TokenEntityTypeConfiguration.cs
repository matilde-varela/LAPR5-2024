using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MDBackoffice.Domain.Tokens;

namespace MDBackoffice.Infrastructure.Tokens
{
    internal class TokenEntityTypeConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            
            builder.HasKey(b => b.Id);

            builder.Property(t => t.UserId)
                   .IsRequired();

            builder.Property(t => t.ExpirationTime)
                   .IsRequired();

            builder.Property(t => t.Active)
                   .HasColumnName("HasExpired")
                   .IsRequired();


            builder.OwnsOne(o => o.TokenType, tokenTypeBuilder =>
            {
                tokenTypeBuilder.WithOwner();

                tokenTypeBuilder.OwnsOne(t => t.TypeDenomination, type =>
                {
                    type.Property(t => t.Denomination)
                        .IsRequired()
                        .HasColumnName("TokenTypeDenomination");
                });
                tokenTypeBuilder.OwnsOne(t => t.ExpirationDurationHours, type =>
                {
                    type.Property(t => t.Hours)
                        .IsRequired()
                        .HasColumnName("ExpirationDurationInHours");
                });
            });

            builder.ToTable("Token");
        }

    }
}