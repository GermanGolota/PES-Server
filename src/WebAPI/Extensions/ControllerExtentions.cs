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
    }
}
