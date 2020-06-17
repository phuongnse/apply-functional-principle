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
            var nameError = ValidateName(createCustomerModel.Name);
            if (!string.IsNullOrEmpty(nameError))
                return Error(nameError);

            var primaryEmailError = ValidateEmail(createCustomerModel.PrimaryEmail, "Primary email");
            if (!string.IsNullOrEmpty(primaryEmailError))
                return Error(primaryEmailError);

            if (createCustomerModel.SecondaryEmail != null)
            {
                var secondaryEmailError = ValidateEmail(createCustomerModel.SecondaryEmail, "Secondary email");
                if (!string.IsNullOrEmpty(secondaryEmailError))
                    return Error(secondaryEmailError);
            }

            var industry = _industryRepository.GetByName(createCustomerModel.Industry);
            if (industry == null)
                return Error("Industry name is invalid: " + createCustomerModel.Industry);

            var customer = new Customer(
                createCustomerModel.Name, 
                createCustomerModel.PrimaryEmail,
                createCustomerModel.SecondaryEmail, 
                industry);

            _customerRepository.Save(customer);

            return Ok();
        }

        private static string ValidateEmail(string email, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(email))
                return fieldName + " should not be empty";
            if (email.Length > 256)
                return fieldName + " is too long";
            if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
                return fieldName + " is invalid";
            return string.Empty;
        }

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Customer name should not be empty";
            if (name.Length > 200)
                return "Customer name is too long";
            return string.Empty;
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
            if (customer == null)
                return Error("Customer with such Id is not found: " + id);

            return Ok();
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

            _emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status);

            return Ok();
        }
    }
}