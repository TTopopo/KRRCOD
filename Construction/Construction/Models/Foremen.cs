using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Construction.Models
{
    public class Foremen
    {
        [Key]
        public int ForemenID { get; set; }


        [Display(Name = "Имя"), Required, MaxLength(50)]
        public string Name { get; set; }


        [Display(Name = "Фамилия"), Required, MaxLength(100)]
        public string LastName { get; set; }


        [Display(Name = "Номер телефона"), Required, MaxLength(100)]
        public string PhoneNamber { get; set; }


        [Display(Name = "Квалификация"), Required, MaxLength(100)]
        public string Qualification { get; set; }


        [Display(Name = "Специализация "), Required, MaxLength(100)]
        public string Specialization { get; set; }


        [Display(Name = "Навыки"), Required, MaxLength(20)]
        public string Skills { get; set; }
    }
}
