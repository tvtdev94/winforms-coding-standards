using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagement.Models;

/// <summary>
/// Represents a customer entity in the system.
/// </summary>
public class Customer
{
    /// <summary>
    /// Gets or sets the unique identifier for the customer.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the customer's full name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer's email address.
    /// </summary>
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer's phone number.
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the customer's address.
    /// </summary>
    [MaxLength(200)]
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the customer's city.
    /// </summary>
    [MaxLength(50)]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the customer's country.
    /// </summary>
    [MaxLength(50)]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the customer is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the customer was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the customer was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of orders associated with this customer.
    /// </summary>
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>
    /// Returns a string representation of the customer.
    /// </summary>
    public override string ToString()
    {
        return $"{Name} ({Email})";
    }
}
