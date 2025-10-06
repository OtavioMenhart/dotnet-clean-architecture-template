using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Resources;

namespace CleanArchTemplate.UnitTests.Domain.Entities;

    public class ProductEntityTests
    {
        [Fact]
        public void Constructor_SetsProperties_WhenValid()
        {
            var name = "Valid Name";
            var unitPrice = EntitiesConstants.Product_UnitPrice_Min_Value + 1;

            var product = new ProductEntity(name, unitPrice);

            Assert.Equal(name, product.Name);
            Assert.Equal(unitPrice, product.UnitPrice);
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.True((DateTime.UtcNow - product.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Constructor_Throws_WhenNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new ProductEntity("", EntitiesConstants.Product_UnitPrice_Min_Value + 1));
        }

        [Fact]
        public void Constructor_Throws_WhenNameIsTooLong()
        {
            var longName = new string('A', EntitiesConstants.Product_Name_Max_Length + 1);
            Assert.Throws<ArgumentException>(() => new ProductEntity(longName, EntitiesConstants.Product_UnitPrice_Min_Value + 1));
        }

        [Fact]
        public void Constructor_Throws_WhenUnitPriceIsTooLow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProductEntity("Valid", EntitiesConstants.Product_UnitPrice_Min_Value - 0.01));
        }

        [Fact]
        public void ChangeName_UpdatesName_WhenValid()
        {
            var product = new ProductEntity("Old", EntitiesConstants.Product_UnitPrice_Min_Value + 1);
            var newName = "New Name";
            product.ChangeName(newName);
            Assert.Equal(newName, product.Name);
        }

        [Fact]
        public void ChangeName_Throws_WhenInvalid()
        {
            var product = new ProductEntity("Old", EntitiesConstants.Product_UnitPrice_Min_Value + 1);
            Assert.Throws<ArgumentException>(() => product.ChangeName(""));
            var longName = new string('B', EntitiesConstants.Product_Name_Max_Length + 1);
            Assert.Throws<ArgumentException>(() => product.ChangeName(longName));
        }

        [Fact]
        public void ChangeUnitPrice_UpdatesUnitPrice_WhenValid()
        {
            var product = new ProductEntity("Name", EntitiesConstants.Product_UnitPrice_Min_Value + 1);
            var newPrice = EntitiesConstants.Product_UnitPrice_Min_Value + 10;
            product.ChangeUnitPrice(newPrice);
            Assert.Equal(newPrice, product.UnitPrice);
        }

        [Fact]
        public void ChangeUnitPrice_Throws_WhenInvalid()
        {
            var product = new ProductEntity("Name", EntitiesConstants.Product_UnitPrice_Min_Value + 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => product.ChangeUnitPrice(EntitiesConstants.Product_UnitPrice_Min_Value - 0.01));
        }
    }
