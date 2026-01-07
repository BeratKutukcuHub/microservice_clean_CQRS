using System;
namespace AbstractionBlocks.Common.Application.Interfaces
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        Guid CorrelationId { get; }
    }
}
