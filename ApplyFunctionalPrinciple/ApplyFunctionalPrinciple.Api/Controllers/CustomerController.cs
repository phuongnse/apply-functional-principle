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
            var customerResult = _customerRepository
                .GetById(model.Id)
                .ToResult("Customer with such Id is not found: " + model.Id);

            var industryResult = Industry.Get(model.Industry);

            return Result
                .Combine(customerResult, industryResult)
                .OnSuccess(() => customerResult.Value.UpdateIndustry(industryResult.Value))
                .OnBoth(result => result.IsFailure ? Error(result.Error) : Ok());
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
            return _customerRepository
                .GetById(id)
                .ToResult("Customer with such Id is not found: " + id)
                .OnSuccess(customer => new CustomerDto
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    PrimaryEmail = customer.PrimaryEmail,
                    SecondaryEmail = customer.SecondaryEmail.HasValue ? customer.SecondaryEmail.Value.Value : null,
                    Industry = customer.EmailSetting.Industry.Name,
                    EmailCampaign = customer.EmailSetting.EmailCampaign,
                    Status = customer.Status
                })
                .OnBoth(result => result.IsFailure ? Error(result.Error) : Ok(result.Value));
        }

        [HttpPost]
        [Route("customers/{id}/promotion")]
        public IActionResult Promote(long id)
        {
            return _customerRepository
                .GetById(id)
                .ToResult("Customer with such Id is not found: " + id)
                .Ensure(customer => customer.CanBePromoted(), "The customer has the highest status possible")
                .OnSuccess(customer => customer.Promote())
                .OnSuccess(customer => _emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status))
                .OnBoth(result => result.IsFailure ? Error(result.Error) : Ok());
        }

        private static Result<Maybe<Email>> GetSecondaryEmail(string secondaryEmail)
        {
            if (secondaryEmail == null)
                return Result.Ok<Maybe<Email>>(null);

            return Email
                .Create(secondaryEmail)
                .OnSuccess(email => (Maybe<Email>) email);
        }
    }
}