using FluentValidation;
using Keove.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Keove.Api.Helper.validators.auth
{
    public class TokenRequestDtoValidator : AbstractValidator<TokenRequestDto>
    {
        public TokenRequestDtoValidator()
        {
            RuleFor(x => x.Token).NotEmpty().NotNull();
            RuleFor(x => x.RefreshToken).NotEmpty().NotNull();

        }
    }
}
