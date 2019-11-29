﻿using FluentValidation;

namespace Esquio.UI.Api.Features.Flags.Details
{
    public class DetailsFeatureRequestValidator
        : AbstractValidator<DetailsFeatureRequest>
    {
        public DetailsFeatureRequestValidator()
        {
            this.RuleFor(rf => rf.FeatureName)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(200);

            this.RuleFor(rf => rf.ProductName)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(200);
        }
    }
}
