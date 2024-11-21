using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TokenViewModel
    {
        public string PasswordToken { get; set; }
        public string Email { get; set; }
        public int RequestId { get; set; }
        public DateTime ExpirationDate { get; set; }


    }
}
