using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required!!")]
        [Display(Name = "Category Name")]
        [MinLength(2, ErrorMessage = "Category Name needs to be at least 2 characters!")]
        [MaxLength(40, ErrorMessage = "Category Name is limited to 40 characters!")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
