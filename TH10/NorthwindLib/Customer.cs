using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindLib;

/// <summary>
/// Customer entity
/// Represents a customer in the Northwind database
/// </summary>
[Table("Customers")]
public class Customer
{
    /// <summary>
    /// Customer ID (Primary Key)
    /// </summary>
    [Key]
    [StringLength(5)]
    [Column("CustomerID")]
    public string CustomerID { get; set; } = null!;

    /// <summary>
    /// Company name
    /// </summary>
    [Required]
    [StringLength(40)]
    public string CompanyName { get; set; } = null!;

    /// <summary>
    /// Contact name
    /// </summary>
    [StringLength(30)]
    public string? ContactName { get; set; }

    /// <summary>
    /// Contact title
    /// </summary>
    [StringLength(30)]
    public string? ContactTitle { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [StringLength(60)]
    public string? Address { get; set; }

    /// <summary>
    /// City
    /// </summary>
    [StringLength(15)]
    public string? City { get; set; }

    /// <summary>
    /// Region
    /// </summary>
    [StringLength(15)]
    public string? Region { get; set; }

    /// <summary>
    /// Postal code
    /// </summary>
    [StringLength(10)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country
    /// </summary>
    [StringLength(15)]
    public string? Country { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [StringLength(24)]
    public string? Phone { get; set; }

    /// <summary>
    /// Fax number
    /// </summary>
    [StringLength(24)]
    public string? Fax { get; set; }
}
