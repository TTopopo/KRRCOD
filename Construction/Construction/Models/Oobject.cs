using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Construction.Models
{
    public class Oobject
    {
        [Key]
        public int OobjectID { get; set; }


        [Display(Name = "Название"), Required, MaxLength(50)]
        public string Title { get; set; } 


        [Display(Name = "Адрес"), Required, MaxLength(130)]
        public string Adress { get; set; }


        [Display(Name = "Тип"), Required, MaxLength(50)]
        public string Type { get; set; }


        [Display(Name = "Статус"), Required, MaxLength(50)]
        public string Status { get; set; }

        [Display(Name = "Фото")]
        public string? Photo { get; set; } 

        public int? ForemenId { get; set; }
        public Foremen? Foremen { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
