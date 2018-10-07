#region -    Using Statements    -

using DemoApi2.Application.People;
using FluentValidation;

#endregion

namespace DemoApi2.Presentation.People
{
    public class PersonRequestResourceValidator : AbstractValidator<PersonResource>
    {
        #region -    Constructors    -

        public PersonRequestResourceValidator()
        {
            RuleFor(p => p.FirstName)
                .NotEmpty()
                .NotNull()
                .MinimumLength(3);

            RuleFor(p => p.LastName)
                .NotEmpty()
                .NotNull()
                .MinimumLength(5);

            RuleFor(p => p.EmailAddress)
                .NotNull()
                .NotEmpty()
                .EmailAddress();
        }

        #endregion
    }
}
