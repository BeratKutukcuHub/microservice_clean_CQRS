using AbstractionBlocks.Common.Exception;

namespace IdentityService.Identity.Infrastructure.Exceptions
{
    public abstract class InfrastructureException : BaseDomainException
    {
        protected InfrastructureException(string message, int statusCode = 500, string errorCode = "INFRASTRUCTURE_ERROR")
            : base(message, statusCode, errorCode)
        {
        }
    }

    public class DatabaseOperationException : InfrastructureException
    {
        public DatabaseOperationException(string message, Exception innerException)
            : base(message, 500, "DATABASE_ERROR")
        {
        }

        public DatabaseOperationException(string message)
            : base(message, 500, "DATABASE_ERROR")
        {
        }
    }
}

