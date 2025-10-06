using CleanArchTemplate.Domain.Resources;
using System.ComponentModel.DataAnnotations;

namespace CleanArchTemplate.Domain.Entities;

public class ProductEntity : BaseEntity
{
    [Required]
    [MaxLength(EntitiesConstants.Product_Name_Max_Length)]
    public string Name { get; private set; }

    [Required]
    [Range(EntitiesConstants.Product_UnitPrice_Min_Value, double.MaxValue)]
    public double UnitPrice { get; private set; }

    public ProductEntity(string name, double unitPrice)
    {
        SetName(name);
        SetUnitPrice(unitPrice);
    }

    public void ChangeName(string name)
    {
        SetName(name);
    }

    public void ChangeUnitPrice(double unitPrice)
    {
        SetUnitPrice(unitPrice);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        if (name.Length > EntitiesConstants.Product_Name_Max_Length)
            throw new ArgumentException($"Product name cannot exceed {EntitiesConstants.Product_Name_Max_Length} characters.", nameof(name));

        Name = name;
    }

    private void SetUnitPrice(double unitPrice)
    {
        if (unitPrice < EntitiesConstants.Product_UnitPrice_Min_Value)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), $"Unit price must be at least {EntitiesConstants.Product_UnitPrice_Min_Value}.");
        UnitPrice = unitPrice;
    }
}
