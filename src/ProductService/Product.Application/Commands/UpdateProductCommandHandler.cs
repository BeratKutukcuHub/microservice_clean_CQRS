using AbstractionBlocks.Common.Exception.Logger;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductService.Product.Application.Exceptions;
using ProductService.Product.Domain;
namespace ProductService.Product.Application.Commands
{
    public record UpdateProductCommand(
        Guid Id,
        string? Name,
        string? Description,
        decimal? Price,
        int? Stock,
        string? Category) : IRequest<Guid>;
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<UpdateProductCommandHandler> _logger;
        public UpdateProductCommandHandler(
            IUnitOfWork uow,
            ILoggerService<UpdateProductCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new ProductNotFoundException(request.Id);
            product.UpdateProduct(
                request.Name,
                request.Description,
                request.Price,
                request.Stock,
                request.Category);
            await _uow.ProductRepository.UpdateAsync(product);
            _logger.Information("Product.Updated", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = product.Id,
                ProductName = product.Name
            });
            return product.Id;
        }
    }
}
