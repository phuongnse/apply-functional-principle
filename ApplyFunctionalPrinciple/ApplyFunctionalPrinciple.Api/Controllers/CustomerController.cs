using ApplyFunctionalPrinciple.Api.Models;
using ApplyFunctionalPrinciple.Logic.Common;
using ApplyFunctionalPrinciple.Logic.Model;
using ApplyFunctionalPrinciple.Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ApplyFunctionalPrinciple.Api.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerRepository _customerRepository;
        private readonly IEmailGateway _emailGateway;

        public CustomerController(UnitOfWork unitOfWork, IEmailGateway emailGateway) : base(unitOfWork)
        {
            _customerRepository = new CustomerRepository(unitOfWork);
            _emailGateway = emailGateway;
        }

        [HttpPost]
        [Route("customers")]
        public IActionResult Create(CreateCustomerModel createCustomerModel)
        {
            var customerNameResult = CustomerName.Create(createCustomerModel.Name);
            var primaryEmailResult = Email.Create(createCustomerModel.PrimaryEmail);
            var secondaryEmailResult = GetSecondaryEmail(createCustomerModel.SecondaryEmail);
            var industryResult = Industry.Get(createCustomerModel.Industry);
            var result = Result.Combine(customerNameResult, primaryEmailResult, secondaryEmailResult, industryResult);

            if (result.IsFailure)
                return Error(result.Error);

            var customer = new Customer(
                customerNameResult.Value,
                primaryEmailResult.Value,
                secondaryEmailResult.Value,
                industryResult.Value);

            _customerRepository.Save(customer);

            return Ok();
        }

        [HttpPut]
        [Route("customers/{id}")]
        public IActionResult Update(UpdateCustomerModel model)
        {
            var maybeCustomer = _customerRepository.GetById(model.Id);

            if (maybeCustomer.HasNoValue)
                return Error("Customer with such Id is not found: " + model.Id);

            var industryResult = Industry.Get(model.Industry);

            if (industryResult.IsFailure)
                return Error(industryResult.Error);

            var customer = maybeCustomer.Value;
            var industry = industryResult.Value;

            customer.UpdateIndustry(industry);

            return Ok();
        }

        [HttpDelete]
        [Route("customers/{id}/emailing")]
        public IActionResult DisableEmailing(long id)
        {
            var maybeCustomer = _customerRepository.GetById(id);

            if (maybeCustomer.HasNoValue)
                return Error("Customer with such Id is not found: " + id);

            var customer = maybeCustomer.Value;

            customer.DisableEmailing();

            return Ok();
        }

        [HttpGet]
        [Route("customers/{id}")]
        public IActionResult Get(long id)
        {
            var maybeCustomer = _customerRepository.GetById(id);

            if (maybeCustomer.HasNoValue)
                return Error("Customer with such Id is not found: " + id);

            var customer = maybeCustomer.Value;

            var customerDto = new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                PrimaryEmail = customer.PrimaryEmail,
                SecondaryEmail = customer.SecondaryEmail.HasValue ? customer.SecondaryEmail.Value.Value : null,
                Industry = customer.EmailSetting.Industry.Name,
                EmailCampaign = customer.EmailSetting.EmailCampaign,
                Status = customer.Status
            };

            return Ok(customerDto);
        }

        [HttpPost]
        [Route("customers/{id}/promotion")]
        public IActionResult Promote(long id)
        {
            var maybeCustomer = _customerRepository.GetById(id);

            if (maybeCustomer.HasNoValue)
                return Error("Customer with such Id is not found: " + id);

            var customer = maybeCustomer.Value;

            if (!customer.CanBePromoted())
                return Error("The customer has the highest status possible");

            customer.Promote();

            var sendPromotionNotificationResult =
                _emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status);

            return sendPromotionNotificationResult.IsFailure ? Error(sendPromotionNotificationResult.Error) : Ok();
        }

        private static Result<Maybe<Email>> GetSecondaryEmail(string secondaryEmail)
        {
            if (secondaryEmail == null)
                return Result.Ok<Maybe<Email>>(null);

            var emailResult = Email.Create(secondaryEmail);

            return emailResult.IsFailure
                ? Result.Fail<Maybe<Email>>(emailResult.Error)
                : Result.Ok<Maybe<Email>>(emailResult.Value);
        }
    }
}