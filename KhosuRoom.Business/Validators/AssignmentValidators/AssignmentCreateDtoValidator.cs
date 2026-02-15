using FluentValidation;
using KhosuRoom.Business.Dtos.AssignmentDtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KhosuRoom.Business.Validators.AssignmentValidators;

internal class AssignmentCreateDtoValidator : AbstractValidator<AssignmentCreateDto>
{
    public AssignmentCreateDtoValidator()
    {
        RuleFor(x=> x.Title)
            .NotEmpty().WithMessage("Assignment title is required.")
            .MaximumLength(200).WithMessage("Assignment title must not exceed 200 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.DueDate)
            .NotEmpty();

        RuleForEach(x => x.Files)
            .Must(f => f.Length > 0)
            .WithMessage("File cannot be empty.");
    }
}
