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
                return true;
            }).WithMessage("Email in use");

            RuleFor(x => x.Username).MustAsync(async (username, cancellation) =>
            {
                return true;
            }).WithMessage("username in use");
        }
    }
}