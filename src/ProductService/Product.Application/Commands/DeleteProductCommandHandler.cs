using AbstractionBlocks.Common.Exception.Logger;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductService.Product.Application.Exceptions;
namespace ProductService.Product.Application.Commands
{
    public record DeleteProductCommand(Guid Id) : IRequest<bool>;
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<DeleteProductCommandHandler> _logger;
        public DeleteProductCommandHandler(
            IUnitOfWork uow,
            ILoggerService<DeleteProductCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new ProductNotFoundException(request.Id);
            product.SoftDelete();
            await _uow.ProductRepository.UpdateAsync(product);
            _logger.Information("Product.Deleted", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = product.Id,
                ProductName = product.Name
            });
            return true;
        }
    }
}
