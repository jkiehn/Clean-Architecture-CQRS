using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class UpdateVendorHandler : UpdateAgentSubtypeHandlerBase<Vendor, UpdateVendor>
{
    public UpdateVendorHandler(IVendorRepository repository, IVendorReadService readService)
        : base(
            repository,
            readService,
            email => new VendorAlreadyExistsException(email),
            id => new VendorNotFoundException(id))
    {
    }
}
