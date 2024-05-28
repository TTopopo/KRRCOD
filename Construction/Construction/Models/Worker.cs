using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Construction.Models
{
    public class Worker
    {
        [Key]
        public int WorkerID { get; set; }


        [Display(Name = "Имя"), Required, MaxLength(50)]
        public string Name { get; set; }


        [Display(Name = "Фамилия"), Required, MaxLength(100)]
        public string LastName { get; set; }


        [Display(Name = "Номер телефона"), Required, MaxLength(20)]
        public string PhoneNamber { get; set; }


        [Display(Name = "Должность"), Required, MaxLength(20)]
        public string Position { get; set; }


        [Display(Name = "Опыт"), Required, MaxLength(20)]
        public string Experience { get; set; }
        public int? ForemenId { get; set; }
        public Foremen? Foremen { get; set; }
    }
}
