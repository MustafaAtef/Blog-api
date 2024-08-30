namespace BlogApi.ServiceContracts {
    public interface IEmailService {
        Task SendActivationEmail(string toEmail, string token, DateTime tokenExpiration);
        Task SendResetPassowrdEmail(string toEmail, string token, DateTime tokenExpiration);
    }
}
