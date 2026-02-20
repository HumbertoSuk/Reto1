using System.ComponentModel.DataAnnotations;

namespace Reto1.Models;

public class PaymentFormVm
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime PaidOn { get; set; }

    [Required]
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Merchant { get; set; } = string.Empty;

    [StringLength(40)]
    public string? Category { get; set; }

    [StringLength(400)]
    public string? Notes { get; set; }
}