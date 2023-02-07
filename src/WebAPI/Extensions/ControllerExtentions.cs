using System;
using System.Linq;
using System.Security.Claims;
using Application.DTOs;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Extensions;

public static class ControllerExtentions
{
    public static Guid GetUserId(this ControllerBase controller)
    {
        string str = controller.User.Claims.Where(
            x => x.Type.Equals(ClaimTypes.NameIdentifier)
        ).FirstOrDefault().Value;

        return new Guid(str);
    }

    public static ActionResult<CommandResponse> CommandResponse(this ControllerBase controller,
        CommandResponse response)
    {
        ActionResult<CommandResponse> result;
        if (response.Successfull)
        {
            result = controller.Ok(response);
        }
        else
        {
            if (IsServerError(response))
                result = controller.StatusCode(StatusCodes.Status500InternalServerError, response);
            else
                result = controller.BadRequest(response);
        }

        return result;
    }

    public static bool IsServerError(CommandResponse response)
    {
        return response.ResultMessage.Equals(ExceptionMessages.ServerError);
    }
}