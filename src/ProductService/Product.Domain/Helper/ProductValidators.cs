namespace ProductService.Product.Domain.Helper
{
    public static class ProductValidators
    {
        public static bool IsValidPrice(decimal price)
        {
            return price >= 0;
        }
        public static bool IsValidStock(int stock)
        {
            return stock >= 0;
        }
        public static bool IsValidName(string? name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }
    }
}
