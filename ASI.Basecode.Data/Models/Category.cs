using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
    }
}
