namespace ProductService.Product.Application.Exceptions
{
    public class AuditNotAddException : ProductApplicationException
    {
        public AuditNotAddException(string message) : base(message)
        {
        }
    }
}
