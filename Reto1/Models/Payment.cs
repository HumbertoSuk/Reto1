using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Reto1.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reto1.Models
{

    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaidOn { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999999999.99)]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(120, MinimumLength = 2)]
        [Display(Name = "Merchant / Concept")]
        public string Merchant { get; set; } = string.Empty;

        [ValidateNever]
        [StringLength(140)]
        public string MerchantNormalized { get; private set; } = string.Empty;

        [StringLength(400)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [StringLength(40)]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public void RecalculateNormalized()
            => MerchantNormalized = PaymentNormalizer.NormalizeMerchant(Merchant);
    }
}
