using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniERP.DAL.Entities;

public enum MovementType
{
    Entry,  // Stok Giriş
    Exit    // Stok Çıkış
}

public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public MovementType MovementType { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime MovementDate { get; set; } = DateTime.Now;

    // Navigation
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }
}
