using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.CQRS.Commands;
using FluentValidation;
using FluentValidation.Results;

namespace WebAPI.Validators;

public class UpdateCredentialsCommandValidator : AbstractValidator<UpdateCredentialsCommand>
{
    private readonly IUserRepo _repo;

    public UpdateCredentialsCommandValidator(IUserRepo repo)
    {
        _repo = repo;
    }

    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateCredentialsCommand> context,
        CancellationToken cancellation = default)
    {
        RuleFor(command => command.Username)
            .NotEmpty().WithMessage("Username can't be empty")
            .MinimumLength(2).WithMessage("Username is too short")
            .MaximumLength(32).WithMessage("Username is too long")
            .MustAsync(async (username, cancellation) =>
                await BeAvailable(username, cancellation)).WithMessage("Username is already taken");
        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password can't be empty")
            .MinimumLength(6).WithMessage("Password is too short")
            .MaximumLength(64).WithMessage("Password is too long");
        return base.ValidateAsync(context, cancellation);
    }

    protected async Task<bool> BeAvailable(string username, CancellationToken cancellation)
    {
        return !await _repo.CheckIfUsernameIsTaken(username, cancellation);
    }
}