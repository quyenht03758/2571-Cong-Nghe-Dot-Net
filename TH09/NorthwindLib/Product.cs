using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindLib;

/// <summary>
/// Product entity - represents a product in the Northwind database
/// </summary>
[Table("Products")]
public class Product
{
    /// <summary>
    /// Product ID - Primary Key
    /// </summary>
    [Key]
    public int ProductID { get; set; }

    /// <summary>
    /// Product Name
    /// </summary>
    [Required]
    [StringLength(40)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Supplier ID - Foreign Key
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// Category ID - Foreign Key
    /// </summary>
    public int? CategoryID { get; set; }

    /// <summary>
    /// Quantity Per Unit
    /// </summary>
    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    /// <summary>
    /// Unit Price
    /// </summary>
    [Column(TypeName = "money")]
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Units In Stock
    /// </summary>
    public short? UnitsInStock { get; set; }

    /// <summary>
    /// Units On Order
    /// </summary>
    public short? UnitsOnOrder { get; set; }

    /// <summary>
    /// Reorder Level
    /// </summary>
    public short? ReorderLevel { get; set; }

    /// <summary>
    /// Discontinued
    /// </summary>
    public bool Discontinued { get; set; }

    /// <summary>
    /// Navigation property - Supplier that supplies this product
    /// </summary>
    [ForeignKey("SupplierID")]
    public virtual Supplier? Supplier { get; set; }
}
