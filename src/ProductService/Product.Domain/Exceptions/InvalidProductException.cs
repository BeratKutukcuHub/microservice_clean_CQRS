namespace ProductService.Product.Domain.Exceptions
{
    public class InvalidProductException : ProductDomainException
    {
        public InvalidProductException() 
            : base("Product validation failed. Please check product properties.")
        {
        }
        public InvalidProductException(string message) : base(message)
        {
        }
    }
}
