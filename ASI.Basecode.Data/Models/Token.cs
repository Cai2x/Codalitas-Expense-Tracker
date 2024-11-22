using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Token
    {
        public int RequestId { get; set; }
        public string Token1 { get; set; }
        public string Email { get; set; }
        public DateTime ExpirationDate { get; set; }

        public virtual User EmailNavigation { get; set; }
    }
}
