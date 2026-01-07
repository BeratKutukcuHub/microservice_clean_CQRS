using System.Text.Json.Serialization;
using AbstractionBlocks.Common.Domain;
using ProductService.Product.Domain.Exceptions;
using ProductService.Product.Domain.Helper;
namespace ProductService.Product.Domain
{
    public class Product : Entity, IAggregateRoot
    {
        public Product() { }
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public string? Category { get; private set; }
        public bool IsActive { get; private set; } = true;
        private List<IEventDomain> _events = new List<IEventDomain>();
        [JsonIgnore]
        public IReadOnlyList<IEventDomain> Events => _events;
        private Product(
            Guid id,
            string name,
            string description,
            decimal price,
            int stock,
            string? category)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            CreateById = id;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Category = category;
            IsActive = true;
        }
        public static Product Create(
            string name,
            string description,
            decimal price,
            int stock,
            string? category = null)
        {
            if (!ProductValidators.IsValidName(name))
                throw new InvalidProductException("Product name cannot be empty.");
            if (!ProductValidators.IsValidPrice(price))
                throw new InvalidProductException($"Product price cannot be negative. Provided: {price}");
            if (!ProductValidators.IsValidStock(stock))
                throw new InvalidProductException($"Product stock cannot be negative. Provided: {stock}");
            return new Product(
                Guid.NewGuid(),
                name,
                description ?? string.Empty,
                price,
                stock,
                category);
        }
        public Product UpdateProduct(
            string? name,
            string? description,
            decimal? price,
            int? stock,
            string? category)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!ProductValidators.IsValidName(name))
                    throw new InvalidProductException("Product name cannot be empty.");
                Name = name;
            }
            if (description != null)
            {
                Description = description;
            }
            if (price.HasValue)
            {
                if (!ProductValidators.IsValidPrice(price.Value))
                    throw new InvalidProductException($"Product price cannot be negative. Provided: {price.Value}");
                Price = price.Value;
            }
            if (stock.HasValue)
            {
                if (!ProductValidators.IsValidStock(stock.Value))
                    throw new InvalidProductException($"Product stock cannot be negative. Provided: {stock.Value}");
                Stock = stock.Value;
            }
            if (category != null)
            {
                Category = category;
            }
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = Id;
            return this;
        }
        public void UpdateStock(int quantity)
        {
            if (!ProductValidators.IsValidStock(quantity))
                throw new InvalidProductException($"Stock quantity cannot be negative. Provided: {quantity}");
            Stock = quantity;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = Id;
        }
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = Id;
        }
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = Id;
        }
        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = Id;
        }
    }
}
