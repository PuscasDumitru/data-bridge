using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teza.Models
{
    public class SuccessModel
    {
        public bool success { get; set; }
        public object data { get; set; }
        public string message { get; set; }
    }
}
