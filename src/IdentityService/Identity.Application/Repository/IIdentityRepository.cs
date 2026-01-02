using AbstractionBlocks.CommonApplication.Pagination;
using AbstractionBlocks.CommonDomain.Repository;
using IdentityService.Identity.Domain;

namespace IdentityService.Identity.Application.Repository
{
    public interface IIdentityRepository : IRepository<IdentityUser>
    {
        Task<PaginationResponse<IdentityUser>?> GetAllPagination(PaginationValue paginationValue);
    }
}
