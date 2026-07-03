using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class RemoveVendorHandler : RemoveAgentSubtypeHandlerBase<Vendor, RemoveVendor>
{
    public RemoveVendorHandler(IVendorRepository repository)
        : base(repository, id => new VendorNotFoundException(id))
    {
    }
}
