using System.ComponentModel.DataAnnotations;
using FluentValidation;
using RedisBloomBlazor.Data;
using StackExchange.Redis;

namespace RedisBloomBlazor.Validators
{
    public class BloomFilterValidator : AbstractValidator<CreateUserForm>
    {
        public BloomFilterValidator(IConnectionMultiplexer mux)
        {
            var db = mux.GetDatabase();
            RuleFor(x => x.Email).MustAsync(async (email, cancellation) =>
            {
                var exists = await db.ExecuteAsync("BF.EXISTS", "email-filter", email);
                return (int) exists != 1;
            }).WithMessage("Email in use");

            RuleFor(x => x.Username).MustAsync(async (username, cancellation) =>
            {
                var exists = await db.ExecuteAsync("BF.EXISTS", "username-filter", username);
                return (int) exists != 1;
            }).WithMessage("username in use");
        }
    }
}