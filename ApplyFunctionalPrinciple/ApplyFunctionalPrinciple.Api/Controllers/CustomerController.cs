using ApplyFunctionalPrinciple.Api.Models;
using ApplyFunctionalPrinciple.Logic.Model;
using ApplyFunctionalPrinciple.Logic.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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

            var industry = _industryRepository.GetByName(createCustomerModel.Industry);
            if (industry == null)
                return Error("Industry name is invalid: " + createCustomerModel.Industry);

            var customer = new Customer(
                customerNameResult.Value,
                primaryEmailResult.Value,
                // this is small hack because we can not use result inside secondary email check
                createCustomerModel.SecondaryEmail == null ? null : (Email) createCustomerModel.SecondaryEmail,
                industry);

            _customerRepository.Save(customer);

            return Ok();
        }

        [HttpPut]
        [Route("customers/{id}")]
        public IActionResult Update(UpdateCustomerModel model)
        {
            var customer = _customerRepository.GetById(model.Id);
            if (customer == null)
                return Error("Customer with such Id is not found: " + model.Id);

            var industry = _industryRepository.GetByName(model.Industry);
            if (industry == null)
                return Error("Industry name is invalid: " + model.Industry);

            customer.UpdateIndustry(industry);

            return Ok();
        }

        [HttpDelete]
        [Route("customers/{id}/emailing")]
        public IActionResult DisableEmailing(long id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null)
                return Error("Customer with such Id is not found: " + id);

            customer.DisableEmailing();

            return Ok();
        }

        [HttpGet]
        [Route("customers/{id}")]
        public IActionResult Get(long id)
        {
            var customer = _customerRepository.GetById(id);

            return customer == null ? 
                Error("Customer with such Id is not found: " + id) : 
                Ok(customer);
        }

        [HttpPost]
        [Route("customers/{id}/promotion")]
        public IActionResult Promote(long id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null)
                return Error("Customer with such Id is not found: " + id);

            if (!customer.CanBePromoted())
                return Error("The customer has the highest status possible");

            customer.Promote();

            return !_emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status) ? 
                Error("Unable to send a notification email") : 
                Ok();
        }
    }
}