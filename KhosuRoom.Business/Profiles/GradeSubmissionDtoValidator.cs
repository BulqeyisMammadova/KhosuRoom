using FluentValidation;
using KhosuRoom.Business.Dtos.SubmissionDtos;


namespace KhosuRoom.Business.Validators.AssignmentValidators;

public class GradeSubmissionDtoValidator : AbstractValidator<GradeSubmissionDto>
{
    public GradeSubmissionDtoValidator()
    {
        RuleFor(x => x.Grade).InclusiveBetween(0, 100);
        RuleFor(x => x.Feedback).MaximumLength(2000);
    }
}