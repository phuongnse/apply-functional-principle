using ApplyFunctionalPrinciple.Api.Models;
using ApplyFunctionalPrinciple.Logic.Model;
using ApplyFunctionalPrinciple.Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ApplyFunctionalPrinciple.Api.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerRepository _customerRepository;
        private readonly IndustryRepository _industryRepository;
        private readonly IEmailGateway _emailGateway;

        public CustomerController(UnitOfWork unitOfWork, IEmailGateway emailGateway) : base(unitOfWork)
        {
            _customerRepository = new CustomerRepository(unitOfWork);
            _industryRepository = new IndustryRepository(unitOfWork);
            _emailGateway = emailGateway;
        }

        [HttpPost]
        [Route("customers")]
        public IActionResult Create(CreateCustomerModel createCustomerModel)
        {
            var customerNameResult = CustomerName.Create(createCustomerModel.Name);

            if (customerNameResult.IsFailure)
                return Error(customerNameResult.Error);

            var primaryEmailResult = Email.Create(createCustomerModel.PrimaryEmail);
            
            if (primaryEmailResult.IsFailure)
                return Error(primaryEmailResult.Error);

            if (createCustomerModel.SecondaryEmail != null)
            {
                var secondaryEmailResult = Email.Create(createCustomerModel.SecondaryEmail);

                if (secondaryEmailResult.IsFailure)
                    return Error(secondaryEmailResult.Error);
            }

            var maybeIndustry = _industryRepository.GetByName(createCustomerModel.Industry);

            if (maybeIndustry.HasNoValue)
                return Error("Industry name is invalid: " + createCustomerModel.Industry);

            var customer = new Customer(
                customerNameResult.Value,
                primaryEmailResult.Value,
                // this is small hack because we can not use result inside secondary email check
                createCustomerModel.SecondaryEmail == null ? null : (Email) createCustomerModel.SecondaryEmail,
                maybeIndustry.Value);

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

            var maybeIndustry = _industryRepository.GetByName(model.Industry);

            if (maybeIndustry.HasNoValue)
                return Error("Industry name is invalid: " + model.Industry);

            maybeCustomer.Value.UpdateIndustry(maybeIndustry.Value);

            return Ok();
        }

        [HttpDelete]
        [Route("customers/{id}/emailing")]
        public IActionResult DisableEmailing(long id)
        {
            var maybeCustomer = _customerRepository.GetById(id);

            if (maybeCustomer.HasNoValue)
                return Error("Customer with such Id is not found: " + id);

            maybeCustomer.Value.DisableEmailing();

            return Ok();
        }

        [HttpGet]
        [Route("customers/{id}")]
        public IActionResult Get(long id)
        {
            var maybeCustomer = _customerRepository.GetById(id);

            if (maybeCustomer.HasNoValue)
                return Error("Customer with such Id is not found: " + id);

            var customerDto = new CustomerDto
            {
                Id = maybeCustomer.Value.Id,
                Name = maybeCustomer.Value.Name,
                PrimaryEmail = maybeCustomer.Value.PrimaryEmail,
                SecondaryEmail = 
                    maybeCustomer.Value.SecondaryEmail.HasValue 
                        ? maybeCustomer.Value.SecondaryEmail.Value.Value 
                        : null,
                Industry = maybeCustomer.Value.EmailSetting.Industry.Name,
                EmailCampaign = maybeCustomer.Value.EmailSetting.EmailCampaign,
                Status = maybeCustomer.Value.Status
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

            if (!maybeCustomer.Value.CanBePromoted())
                return Error("The customer has the highest status possible");

            maybeCustomer.Value.Promote();

            var sendPromotionNotificationResult = 
                _emailGateway.SendPromotionNotification(
                    maybeCustomer.Value.PrimaryEmail, 
                    maybeCustomer.Value.Status);

            if (sendPromotionNotificationResult.IsFailure)
                return Error(sendPromotionNotificationResult.Error);
                
            return Ok();
        }
    }
}