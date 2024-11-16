using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category Name is required.")]
        public string Name { get; set; }
        public DateTime CategoryDateCreated { get; set; }
        [Required(ErrorMessage = "Category Description is required.")]
        public string Description { get; set; }
        public bool isDeleted { get; set; }
    }
}
