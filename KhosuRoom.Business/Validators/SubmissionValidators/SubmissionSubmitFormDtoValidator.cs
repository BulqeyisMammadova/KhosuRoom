using FluentValidation;
using KhosuRoom.Business.Dtos.SubmissionDtos;
using KhosuRoom.Business.Helpers;


namespace KhosuRoom.Business.Validators.SubmissionValidators;

public class SubmissionSubmitFormDtoValidator : AbstractValidator<SubmissionSubmitFormDto>
{
    public SubmissionSubmitFormDtoValidator()
    {
        RuleFor(x => x.AssignmentId).NotEmpty();

        RuleFor(x => x.Comment)
            .MaximumLength(2000);

        RuleFor(x => x.Files)
            .Must(f => f == null || f.Count <= FileValidationHelper.MaxFiles)
            .WithMessage($"Maximum {FileValidationHelper.MaxFiles} files allowed.");

        RuleForEach(x => x.Files)
            .Must(FileValidationHelper.IsAllowed)
            .WithMessage("Invalid file type or file too large (max 10MB).");
    }
}