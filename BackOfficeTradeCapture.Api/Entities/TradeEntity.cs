using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackOfficeTradeCapture.Api.Entities;
public class TradeEntity
{
    [Key]
    public int Id { get; set; }  // Internal PK

    [Required]
    [MaxLength(50)]
    public string ExternalId { get; set; } = default!;  // Unique trade identifier

    [Required]
    [MaxLength(50)]
    public string Account { get; set; } = default!;     // Trade account

    [Required]
    [MaxLength(50)]
    public string Symbol { get; set; } = default!;      // Instrument symbol

    [Required]
    [MaxLength(4)]
    public string Side { get; set; } = default!;        // BUY or SELL

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }              // Quantity traded

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Price { get; set; }                 // Price per unit

    [Required]
    public DateTimeOffset TradeTime { get; set; }      // Original trade timestamp

    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = default!;   // Original trade currency

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal NotionalBase { get; set; }          // Quantity * Price * Rate

    [Required]
    [MaxLength(3)]
    public string BaseCurrency { get; set; } = default!;  // Base currency (e.g., EUR)
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;  // When stored
}

