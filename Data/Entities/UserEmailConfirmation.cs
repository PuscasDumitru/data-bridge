using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class UserEmailConfirmation
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string EmailConfirmationToken { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
