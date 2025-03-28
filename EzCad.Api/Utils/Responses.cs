using EzCad.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EzCad.Api.Utils;

public static class Responses
{
    public static NotFoundObjectResult IdentityNotFound()
    {
        return new NotFoundObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Identity not found"
        });
    }
    
    public static NotFoundObjectResult JobNotFound()
    {
        return new NotFoundObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Job not found"
        });
    }

    public static NotFoundObjectResult RecordNotFound()
    {
        return new NotFoundObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Record not found"
        });
    }

    public static BadRequestObjectResult RoleAlreadyExists()
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Role already exists"
        });
    }

    public static BadRequestObjectResult RoleNotFound()
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Role not found"
        });
    }

    public static BadRequestObjectResult UserNotFound()
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "User not found"
        });
    }

    public static BadRequestObjectResult NoConfiguration()
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "No configuration detected"
        });
    }

    public static BadRequestObjectResult CannotModifyDefaultRole()
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Unable to make changes to this role as it is a default EZCad role"
        });
    }

    public static BadRequestObjectResult CadNotLinked()
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Your CAD account is not linked to your FiveM account"
        });
    }

    public static BadRequestObjectResult NotEnoughMoney()
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "You haven't got that much money"
        });
    }

    public static NotFoundObjectResult UserNotFoundInCad()
    {
        return new NotFoundObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Unable to find user in CAD"
        });
    }

    public static BadRequestObjectResult NoPrimaryIdentity()
    {
        return new BadRequestObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "User has no primary identity"
        });
    }

    public static NotFoundObjectResult TransactionNotFound()
    {
        return new NotFoundObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Transaction not found"
        });
    }

    public static NotFoundObjectResult VehicleNotFound()
    {
        return new NotFoundObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Vehicle not found"
        });
    }

    public static UnauthorizedObjectResult UserNotAuthorized()
    {
        return new UnauthorizedObjectResult(new ErrorResponse
        {
            Success = false,
            Message = "Not authorized"
        });
    }
}