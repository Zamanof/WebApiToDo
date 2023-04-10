using FluentValidation;
using System.Text.RegularExpressions;
using TO_DO.DTOs.Auth;

namespace TO_DO.DTOs.Validation;

public static class SharedValidators
{
    public static bool BeValidatePassword(string arg)
    {
        return new Regex(@"\d").IsMatch(arg)
            && new Regex(@"[a-z]").IsMatch(arg)
            && new Regex(@"[A-Z]").IsMatch(arg)
            && new Regex(@"[\,@;:]").IsMatch(arg);
    }
}


public static class ValidationRulesExtentions
{
    public static IRuleBuilderOptions<T, string> Password<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        bool mustContainLowerCase = true,
        bool mustContainUpperCase = true,
        bool mustContainDigit = true,
        bool mustContainNonAlphaNumeric = true
        )
    {
        IRuleBuilderOptions<T, string> options = null;
        if (mustContainLowerCase)
        {
            options = ruleBuilder.Must(password => new Regex(@"[a-z]").IsMatch(password));
        }
        if (mustContainUpperCase)
        {
            options = ruleBuilder.Must(password => new Regex(@"[A-Z]").IsMatch(password));
        }
        if (mustContainDigit)
        {
            options = ruleBuilder.Must(password => new Regex(@"[0-9]").IsMatch(password));
        }


        return options;
    }
    
}
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        //RuleFor(x=> x.Password).NotEmpty().MinimumLength(8);
        //RuleFor(x => x.Password).Must(SharedValidators.BeValidatePassword).NotEmpty();
        RuleFor(x=>x.Password).Password(mustContainLowerCase:false).NotEmpty();
        
    }

    //private bool BeValidatePassword(string arg)
    //{
    //    return new Regex(@"\d").IsMatch(arg)
    //        && new Regex(@"[a-z]").IsMatch(arg)
    //        && new Regex(@"[A-Z]").IsMatch(arg)
    //        && new Regex(@"[\,@;:]").IsMatch(arg);
    //}
}
