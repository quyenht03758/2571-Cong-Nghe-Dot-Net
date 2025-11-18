using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindLib;

/// <summary>
/// Supplier entity - represents a supplier in the Northwind database
/// </summary>
[Table("Suppliers")]
public class Supplier
{
    /// <summary>
    /// Supplier ID - Primary Key
    /// </summary>
    [Key]
    public int SupplierID { get; set; }

    /// <summary>
    /// Company Name
    /// </summary>
    [Required]
    [StringLength(40)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Contact Name
    /// </summary>
    [StringLength(30)]
    public string? ContactName { get; set; }

    /// <summary>
    /// Contact Title
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
    /// Postal Code
    /// </summary>
    [StringLength(10)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country
    /// </summary>
    [StringLength(15)]
    public string? Country { get; set; }

    /// <summary>
    /// Phone
    /// </summary>
    [StringLength(24)]
    public string? Phone { get; set; }

    /// <summary>
    /// Fax
    /// </summary>
    [StringLength(24)]
    public string? Fax { get; set; }

    /// <summary>
    /// Home Page
    /// </summary>
    public string? HomePage { get; set; }

    /// <summary>
    /// Navigation property - Products supplied by this supplier
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
