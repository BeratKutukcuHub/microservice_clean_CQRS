using AbstractionBlocks.Common.Pagination;
using MailNotification.Application.Queries;
using MediatR;
namespace MailNotification.Application.Queries
{
    public class GetMailHistoryQueryHandler : IRequestHandler<GetMailHistoryQuery, PaginationResponse<MailLogDto>>
    {
        public Task<PaginationResponse<MailLogDto>> Handle(GetMailHistoryQuery request, CancellationToken cancellationToken)
        {
            var data = new List<MailLogDto>();
            return Task.FromResult(PaginationResponse<MailLogDto>.Create(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                0,
                0,
                data));
        }
    }
}
