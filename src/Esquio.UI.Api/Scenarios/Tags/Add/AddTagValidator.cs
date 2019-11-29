﻿using FluentValidation;

namespace Esquio.UI.Api.Features.Tags.Add
{
    public class AddTagValidator 
        : AbstractValidator<AddTagRequest>
    {
        public AddTagValidator()
        {
            this.RuleFor(rf => rf.Tag)
                .NotNull()
                .Matches(ApiConstants.Constraints.NamesRegularExpression);
        }
    }
}
