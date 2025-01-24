using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SharedKernel.Authorization.Extensions;

namespace CleanTemplate.Api.Controllers;

[ApiController]
// [Authorize]
public class ApiController : ControllerBase
{
    #region user
    protected bool UserIsAdmin(out Error error)
    {
        error = default;
        if (HttpContext.User.IsAdmin())
        {
            return true;
        }
        error = Error.Forbidden("User is not Authorized to perform this action");
        return false;
    }
    #endregion user

    #region problem
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    protected IActionResult ProblemBadRequest(string error)
    {
        return Problem(statusCode: StatusCodes.Status400BadRequest, title: error);
    }


    private IActionResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Problem(statusCode: statusCode, title: error.Description);
    }

    private IActionResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();

        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(
                error.Code,
                error.Description);
        }

        return ValidationProblem(modelStateDictionary);
    }
    #endregion problem
}
