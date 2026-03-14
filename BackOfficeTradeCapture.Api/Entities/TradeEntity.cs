using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackOfficeTradeCapture.Api.Entities;
public class TradeEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string ExternalId { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string Account { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string Symbol { get; set; } = default!;

    [Required]
    [MaxLength(4)]
    public string Side { get; set; } = default!;

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Price { get; set; }

    [Required]
    public DateTimeOffset TradeTime { get; set; } 

    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = default!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal NotionalBase { get; set; }

    [Required]
    [MaxLength(3)]
    public string BaseCurrency { get; set; } = default!; 
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; 
}

