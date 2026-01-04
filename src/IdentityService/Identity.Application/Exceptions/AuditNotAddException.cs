using AbstractionBlocks.Common.Exception;

namespace IdentityService.Application.Exceptions
{
    public class AuditNotAddException : ConflictException
    {
        public AuditNotAddException(string message) : base(message)
        {
            
        }
    }
}