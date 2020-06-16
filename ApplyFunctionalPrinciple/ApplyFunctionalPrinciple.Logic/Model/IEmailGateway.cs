namespace ApplyFunctionalPrinciple.Logic.Model
{
    public interface IEmailGateway
    {
        void SendPromotionNotification(string email, CustomerStatus newStatus);
    }
}