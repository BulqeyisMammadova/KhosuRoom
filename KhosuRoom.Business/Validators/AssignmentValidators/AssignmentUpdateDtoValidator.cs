using FluentValidation;
using KhosuRoom.Business.Dtos.AssignmentDtos;

namespace KhosuRoom.Business.Validators.AssignmentValidators;

public class AssignmentUpdateDtoValidator : AbstractValidator<AssignmentUpdateDto>
{
    public AssignmentUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.DueDate)
            .NotEmpty();
    }
}