using System;
using System.Text.RegularExpressions;
using ApplyFunctional.Api.Models;
using ApplyFunctional.Logic.Common;
using ApplyFunctional.Logic.Model;
using ApplyFunctional.Logic.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApplyFunctional.Api.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly CustomerRepository _customerRepository;
        private readonly IEmailGateway _emailGateway;
        private readonly IndustryRepository _industryRepository;
        private readonly UnitOfWork _unitOfWork;

        public CustomerController(UnitOfWork unitOfWork, IEmailGateway emailGateway)
        {
            _unitOfWork = unitOfWork;
            _customerRepository = new CustomerRepository(unitOfWork);
            _industryRepository = new IndustryRepository(unitOfWork);
            _emailGateway = emailGateway;
        }

        [HttpPost]
        [Route("customers")]
        public IActionResult Create(CreateCustomerModel createCustomerModel)
        {
            try
            {
                ValidateName(createCustomerModel.Name);
                ValidateEmail(createCustomerModel.PrimaryEmail, "Primary email");
                if (createCustomerModel.SecondaryEmail != null)
                    ValidateEmail(createCustomerModel.SecondaryEmail, "Secondary email");

                var industry = _industryRepository.GetByName(createCustomerModel.Industry);
                if (industry == null)
                    throw new BusinessException("Industry name is invalid: " + createCustomerModel.Industry);

                var customer = new Customer(createCustomerModel.Name, createCustomerModel.PrimaryEmail,
                    createCustomerModel.SecondaryEmail, industry);
                _customerRepository.Save(customer);

                _unitOfWork.Commit();
                return Ok(Envelope.Ok());
            }
            catch (BusinessException businessException)
            {
                return BadRequest(Envelope.Error(businessException.Message));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error(exception.Message));
            }
        }

        private static void ValidateEmail(string email, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessException(fieldName + " should not be empty");
            if (email.Length > 256)
                throw new BusinessException(fieldName + " is too long");
            if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
                throw new BusinessException(fieldName + " is invalid");
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessException("Customer name should not be empty");
            if (name.Length > 200)
                throw new BusinessException("Customer name is too long");
        }

        [HttpPut]
        [Route("customers/{id}")]
        public IActionResult Update(UpdateCustomerModel model)
        {
            try
            {
                var customer = _customerRepository.GetById(model.Id);
                if (customer == null)
                    throw new BusinessException("Customer with such Id is not found: " + model.Id);

                var industry = _industryRepository.GetByName(model.Industry);
                if (industry == null)
                    throw new BusinessException("Industry name is invalid: " + model.Industry);

                customer.UpdateIndustry(industry);

                _unitOfWork.Commit();
                return Ok(Envelope.Ok());
            }
            catch (BusinessException businessException)
            {
                return BadRequest(Envelope.Error(businessException.Message));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error(exception.Message));
            }
        }

        [HttpDelete]
        [Route("customers/{id}/emailing")]
        public IActionResult DisableEmailing(long id)
        {
            try
            {
                var customer = _customerRepository.GetById(id);
                if (customer == null)
                    throw new BusinessException("Customer with such Id is not found: " + id);

                customer.DisableEmailing();

                _unitOfWork.Commit();
                return Ok(Envelope.Ok());
            }
            catch (BusinessException businessException)
            {
                return BadRequest(Envelope.Error(businessException.Message));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error(exception.Message));
            }
        }

        [HttpGet]
        [Route("customers/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var customer = _customerRepository.GetById(id);
                if (customer == null)
                    throw new BusinessException("Customer with such Id is not found: " + id);

                return Ok(Envelope.Ok(customer));
            }
            catch (BusinessException businessException)
            {
                return BadRequest(Envelope.Error(businessException.Message));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error(exception.Message));
            }
        }

        [HttpPost]
        [Route("customers/{id}/promotion")]
        public IActionResult Promote(long id)
        {
            try
            {
                var customer = _customerRepository.GetById(id);
                if (customer == null)
                    throw new BusinessException("Customer with such Id is not found: " + id);

                if (!customer.CanBePromoted())
                    throw new BusinessException("The customer has the highest status possible");

                customer.Promote();
                _emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status);

                _unitOfWork.Commit();
                return Ok(Envelope.Ok());
            }
            catch (BusinessException businessException)
            {
                return BadRequest(Envelope.Error(businessException.Message));
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error(exception.Message));
            }
        }
    }
}