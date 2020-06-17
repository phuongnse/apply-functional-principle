using ApplyFunctionalPrinciple.Api.Models;
using ApplyFunctionalPrinciple.Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ApplyFunctionalPrinciple.Api.Controllers
{
    public class Controller : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        protected Controller(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected IActionResult Error(string errorMessage)
        {
            return BadRequest(errorMessage);
        }

        protected new IActionResult Ok()
        {
            _unitOfWork.Commit();

            return Ok(Envelope.Ok());
        }

        protected IActionResult Ok<T>(T result)
        {
            _unitOfWork.Commit();

            return Ok(Envelope.Ok(result));
        }
    }
}
