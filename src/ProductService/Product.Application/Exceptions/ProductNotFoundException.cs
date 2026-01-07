namespace ProductService.Product.Application.Exceptions
{
    public class ProductNotFoundException : ProductApplicationException
    {
        public ProductNotFoundException(Guid productId) 
            : base($"Product with ID {productId} was not found.")
        {
        }
    }
}
