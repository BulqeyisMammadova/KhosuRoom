using FluentValidation;
using KhosuRoom.Business.Dtos.AssignmentDtos;
using KhosuRoom.Business.Helpers;


namespace KhosuRoom.Business.Validators.AssignmentValidators;

public class AssignmentCreateFormDtoValidator : AbstractValidator<AssignmentCreateFormDto>
{
    public AssignmentCreateFormDtoValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.Files)
            .Must(f => f == null || f.Count <= FileValidationHelper.MaxFiles)
            .WithMessage($"Maximum {FileValidationHelper.MaxFiles} files allowed.");

        RuleForEach(x => x.Files)
            .Must(FileValidationHelper.IsAllowed)
            .WithMessage("Invalid file type or file too large (max 10MB).");
    }
}
