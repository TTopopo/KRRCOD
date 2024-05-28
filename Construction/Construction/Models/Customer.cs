using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Construction.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }


        [Display(Name = "Имя"), Required, MaxLength(50)]
        public string Name { get; set; }


        [Display(Name = "Фамилия"), Required, MaxLength(100)]
        public string LastName { get; set; }


        [Display(Name = "Номер телефона"), Required, MaxLength(20)]
        public string PhoneNamber { get; set; }
    }
}
