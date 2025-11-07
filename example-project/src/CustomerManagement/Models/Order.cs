using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagement.Models;

/// <summary>
/// Represents an order placed by a customer.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order number (user-friendly identifier).
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the foreign key to the customer.
    /// </summary>
    [Required]
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the customer.
    /// </summary>
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    /// <summary>
    /// Gets or sets the order date.
    /// </summary>
    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the total amount of the order.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999.99)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the order status.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets optional notes about the order.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the order was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Returns a string representation of the order.
    /// </summary>
    public override string ToString()
    {
        return $"Order {OrderNumber} - {TotalAmount:C}";
    }
}
