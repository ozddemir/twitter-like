using FluentValidation;
using Keove.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Keove.Api.Helper.validators.auth
{
    public class UserRegisterDtoValidator : AbstractValidator<UserLoginDto>
    {
        public UserRegisterDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress().WithMessage("Please enter an valid email");

        }
    }
}
