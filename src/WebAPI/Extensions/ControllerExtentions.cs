using Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace WebAPI.Extensions
{
    public static class ControllerExtentions
    {
        public static Guid GetUserId(this ControllerBase controller)
        {
            string str = controller.User.Claims.Where(
                x => x.Type.Equals(ClaimTypes.NameIdentifier)
                ).FirstOrDefault().Value;

            return new Guid(str);
        }

        public static ActionResult<CommandResponse> CommandResponse(this ControllerBase controller, CommandResponse response)
        {
            ActionResult<CommandResponse> result;
            if (response.Successfull)
            {
                result = controller.Ok(response);
            }
            else
            {
                if(response.IsServerError)
                {
                    result = controller.StatusCode(StatusCodes.Status500InternalServerError, response);
                }
                else
                {
                    result = controller.BadRequest(response);
                }
            }
            return result;
        }
    }
}
