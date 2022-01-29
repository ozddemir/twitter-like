using FluentValidation;
using Keove.Dto.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Keove.Api.Helper.validators.post
{
    public class PostCreateDtoValidator : AbstractValidator<PostCreateDto>
    {
        public PostCreateDtoValidator()
        {
            RuleFor(x => x.Content).NotEmpty().NotNull().WithMessage("Content is required");
        }
    }
}
