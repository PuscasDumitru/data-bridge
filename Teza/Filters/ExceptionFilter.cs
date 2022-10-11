using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Teza.Models;

namespace Teza.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var result = new ObjectResult(new ErrorModel()
            {
                Success = false,
                Error = context.Exception.Message
            });
            context.Result = result;
        }
    }
}
