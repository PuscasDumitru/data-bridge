using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Teza.Models;

namespace Teza.Filters
{
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var result = new ObjectResult(new ErrorModel()
            {
                success = false,
                error = context.Exception.Message
            });

            context.HttpContext.Response.ContentType = "application/json";
            context.Result = result;
        }
    }
}
