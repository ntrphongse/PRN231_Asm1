using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Member
    {
        public Member()
        {
            Orders = new HashSet<Order>();
        }

        public int MemberId { get; set; }

        [Required(ErrorMessage = "Email cannot be empty!!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Company Name cannot be empty!!")]
        [MinLength(2, ErrorMessage = "Company Name needs to be at least 2 characters!!")]
        [MaxLength(40, ErrorMessage = "Company Name is limited to 40 characters!!")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "City cannot be empty!!")]
        [MinLength(2, ErrorMessage = "City needs to be at least 2 characters!!")]
        [MaxLength(15, ErrorMessage = "City is limited to 15 characters!!")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country cannot be empty!!")]
        [MinLength(2, ErrorMessage = "Country needs to be at least 2 characters!!")]
        [MaxLength(15, ErrorMessage = "Country is limited to 15 characters!!")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Password cannot be empty!!")]
        [MinLength(6, ErrorMessage = "Password needs to be at least 6 characters!!")]
        [MaxLength(30, ErrorMessage = "Password is limited to 30 characters!!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
