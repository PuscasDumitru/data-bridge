using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Teza.Models;

namespace Teza.Extensions
{
    public static class ExceptionHandlerExtension
    {
        public static ErrorModel HandleException(Exception ex)
        {
            if (ex is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException != null && dbUpdateEx.InnerException.Message.Contains("unique constraint"))
                {
                    return new ErrorModel
                    {
                        error = "There's already an entity with such a name, choose another one.",
                        success = false
                    };
                }
            }

            if (ex.Message.Contains("Data.Enums.EntityType"))
            {
                return new ErrorModel
                {
                    error = "There's no such type of entity.",
                    success = false
                };
            }

            else if (ex.Message.Contains("Data.Enums.Roles"))
            {
                return new ErrorModel
                {
                    error = "There's no such role for users.",
                    success = false
                };
            }

            else if (ex.Message.Contains("Error converting"))
            {
                Regex regex = new Regex("Path '[^']*'", RegexOptions.IgnoreCase);
                var property = regex.Match(ex.Message).Value.Replace("Path", "");

                return new ErrorModel
                {
                    error = $"Incorrect value for{property}",
                    success = false
                };
            }

            return new ErrorModel
            {
                error = ex.Message,
                success = false
            };
        }
    }
}
