using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int? CategoryId { get; set; }
        
        [Required(ErrorMessage = "Product Name cannot be empty!!")]
        [Display(Name = "Product Name")]
        [MinLength(2, ErrorMessage = "Product Name needs to be at least 2 characters!!")]
        [MaxLength(40, ErrorMessage = "Product Name is limited to 40 characters!!")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product Weight cannot be empty!!")]
        [MinLength(2, ErrorMessage = "Product Weight needs to be at least 2 characters!!")]
        [MaxLength(20, ErrorMessage = "Product Weight is limited to 20 characters!!")]
        public string Weight { get; set; }

        [Required(ErrorMessage = "Product Unit Price is required!!")]
        [Display(Name = "Unit Price")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Product Unit Price must be a positive number!!")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Product Units in Stock is required!!")]
        [Display(Name = "Units in Stock")]
        [Range(0, int.MaxValue, ErrorMessage = "Product Units in Stock has to be a positive integer!")]
        public int UnitsInStock { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
