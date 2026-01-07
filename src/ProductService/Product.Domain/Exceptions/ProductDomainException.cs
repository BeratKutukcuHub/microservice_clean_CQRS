namespace ProductService.Product.Domain.Exceptions
{
    public class ProductDomainException : Exception
    {
        public ProductDomainException(string message) : base(message)
        {
        }
        public ProductDomainException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
