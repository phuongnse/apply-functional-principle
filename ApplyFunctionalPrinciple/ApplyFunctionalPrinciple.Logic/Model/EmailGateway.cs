using System.Net.Mail;
using ApplyFunctionalPrinciple.Logic.Common;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class EmailGateway : IEmailGateway
    {
        public Result SendPromotionNotification(string email, CustomerStatus newStatus)
        {
            var message = new MailMessage("noreply@northwind.com", email, "Congratulations!",
                "You've been promoted to " + newStatus);

            using var client = new SmtpClient();

            try
            {
                client.Send(message);

                return Result.Ok();
            }
            catch (SmtpException)
            {
                return Result.Fail("Unable to send the email");
            }
        }
    }
}