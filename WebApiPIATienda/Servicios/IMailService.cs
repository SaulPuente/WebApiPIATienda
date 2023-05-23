namespace WebApiPIATienda.Servicios
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
