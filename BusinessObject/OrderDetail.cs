using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Order detail Unit Price is required!!")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Order detail Unit Price must be a positive number!!")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Order detail Quantity is required!!")]
        [Range(0, int.MaxValue, ErrorMessage = "Order detail Quantity has to be a positive integer!")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Order detail Discount is required!!")]
        [Range(0, double.MaxValue, ErrorMessage = "Order detail Quantity has to be a positive number!")]
        public double Discount { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
