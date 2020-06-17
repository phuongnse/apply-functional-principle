namespace ApplyFunctionalPrinciple.Logic.Model
{
    public interface IEmailGateway
    {
        bool SendPromotionNotification(string email, CustomerStatus newStatus);
    }
}