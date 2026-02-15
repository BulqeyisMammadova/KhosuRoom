using FluentValidation;
using KhosuRoom.Business.Dtos.SubmissionDtos;

namespace KhosuRoom.Business.Validators.SubmissionValidators;

public class SubmissionSubmitDtoValidator : AbstractValidator<SubmissionSubmitDto>
{
    public SubmissionSubmitDtoValidator()
    {
        RuleFor(x => x.Text).MaximumLength(8000);

        RuleForEach(x => x.Files)
            .Must(f => f.Length > 0)
            .WithMessage("File cannot be empty.");
    }
}