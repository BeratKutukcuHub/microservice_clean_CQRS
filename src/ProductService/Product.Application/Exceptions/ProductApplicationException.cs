namespace ProductService.Product.Application.Exceptions
{
    public class ProductApplicationException : Exception
    {
        public ProductApplicationException(string message) : base(message)
        {
        }
        public ProductApplicationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
