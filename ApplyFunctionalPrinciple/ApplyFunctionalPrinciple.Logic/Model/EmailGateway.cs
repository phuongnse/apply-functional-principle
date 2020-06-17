using System.Net.Mail;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class EmailGateway : IEmailGateway
    {
        public bool SendPromotionNotification(string email, CustomerStatus newStatus)
        {
            var message = new MailMessage(
                "noreply@northwind.com", 
                email, 
                "Congratulations!", 
                "You've been promoted to " + newStatus);

            using var client = new SmtpClient();

            try
            {
                client.Send(message);
                return true;
            }
            catch (SmtpException)
            {
                return false;
            }
        }
    }
}