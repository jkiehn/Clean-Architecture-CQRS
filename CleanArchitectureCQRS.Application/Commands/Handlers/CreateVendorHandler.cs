using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class CreateVendorHandler : CreateAgentSubtypeHandlerBase<Vendor, CreateVendor>
{
    public CreateVendorHandler(IVendorRepository repository, IVendorReadService readService)
        : base(
            repository,
            readService,
            email => new VendorAlreadyExistsException(email),
            command => new Vendor(command.Id, command.Name, command.Email))
    {
    }
}
