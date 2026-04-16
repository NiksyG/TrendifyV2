using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendifyV1.ViewModels.CheckoutViewModel
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Полето 'Имена' е задължително.")]
        [Display(Name = "Имена")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Полето 'Телефон' е задължително.")]
        [Phone(ErrorMessage = "Невалиден телефонен номер.")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Полето 'Град' е задължително.")]
        [Display(Name = "Град")]
        public string City { get; set; }

        [Required(ErrorMessage = "Полето 'Адрес за доставка' е задължително.")]
        [Display(Name = "Адрес за доставка (или офис)")]
        public string Address { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "CashOnDelivery"; 

        public decimal TotalAmount { get; set; }
    }
}
