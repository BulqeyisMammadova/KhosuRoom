using FluentValidation;
using KhosuRoom.Business.Dtos.ChatDtos;

namespace KhosuRoom.Business.Validators.ChatValidators;

internal class SendMessageDtoValidator : AbstractValidator<SendMessageDto>
{
    public SendMessageDtoValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(2000);

      
    }
}
