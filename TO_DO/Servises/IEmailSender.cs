namespace TO_DO.Servises;

public interface IEmailSender
{
    Task SendEmail(string to, string subject, string body);
}
