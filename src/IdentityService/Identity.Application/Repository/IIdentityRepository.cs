using AbstractionBlocks.Common.Pagination;
using AbstractionBlocks.Common.Application.Repository;
using IdentityService.Identity.Domain;
namespace IdentityService.Identity.Application.Repository
{
    public interface IIdentityRepository : IRepository<IdentityUser>
    {
        Task<PaginationResponse<IdentityUser>?> GetAllPagination(PaginationValue paginationValue);
    }
}
