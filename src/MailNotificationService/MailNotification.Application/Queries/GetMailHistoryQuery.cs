using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Pagination;
using MediatR;
namespace MailNotification.Application.Queries
{
    [Cache("MailHistory", 5)]
    public record GetMailHistoryQuery(PaginationValue Pagination) : IRequest<PaginationResponse<MailLogDto>>;
    public record MailLogDto(Guid Id, string To, string Subject, bool IsSent, DateTime SentAt);
}
