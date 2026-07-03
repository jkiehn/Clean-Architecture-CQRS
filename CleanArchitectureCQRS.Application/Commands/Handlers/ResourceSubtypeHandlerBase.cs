using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal abstract class CreateResourceSubtypeHandlerBase<TResource, TCommand> : ICommandHandler<TCommand>
    where TResource : Resource
    where TCommand : class, ICreateResourceSubtypeCommand
{
    private readonly IResourceRepository<TResource> _repository;
    private readonly IResourceSubtypeReadService _readService;
    private readonly Func<string, Exception> _alreadyExistsExceptionFactory;
    private readonly Func<TCommand, TResource> _resourceFactory;

    protected CreateResourceSubtypeHandlerBase(
        IResourceRepository<TResource> repository,
        IResourceSubtypeReadService readService,
        Func<string, Exception> alreadyExistsExceptionFactory,
        Func<TCommand, TResource> resourceFactory)
    {
        _repository = repository;
        _readService = readService;
        _alreadyExistsExceptionFactory = alreadyExistsExceptionFactory;
        _resourceFactory = resourceFactory;
    }

    public async Task HandleAsync(TCommand command)
    {
        if (await _readService.ExistsByNameAsync(command.Name))
        {
            throw _alreadyExistsExceptionFactory(command.Name);
        }

        await _repository.AddAsync(_resourceFactory(command));
    }
}

internal abstract class UpdateResourceSubtypeHandlerBase<TResource, TCommand> : ICommandHandler<TCommand>
    where TResource : Resource
    where TCommand : class, IUpdateResourceSubtypeCommand
{
    private readonly IResourceRepository<TResource> _repository;
    private readonly IResourceSubtypeReadService _readService;
    private readonly Func<string, Exception> _alreadyExistsExceptionFactory;
    private readonly Func<Guid, Exception> _notFoundExceptionFactory;

    protected UpdateResourceSubtypeHandlerBase(
        IResourceRepository<TResource> repository,
        IResourceSubtypeReadService readService,
        Func<string, Exception> alreadyExistsExceptionFactory,
        Func<Guid, Exception> notFoundExceptionFactory)
    {
        _repository = repository;
        _readService = readService;
        _alreadyExistsExceptionFactory = alreadyExistsExceptionFactory;
        _notFoundExceptionFactory = notFoundExceptionFactory;
    }

    public async Task HandleAsync(TCommand command)
    {
        var resource = await _repository.GetAsync(command.Id);

        if (resource is null)
        {
            throw _notFoundExceptionFactory(command.Id);
        }

        if (await _readService.ExistsByNameAsync(command.Name, command.Id))
        {
            throw _alreadyExistsExceptionFactory(command.Name);
        }

        resource.UpdateDetails(command.Name);
        await _repository.UpdateAsync(resource);
    }
}

internal abstract class RemoveResourceSubtypeHandlerBase<TResource, TCommand> : ICommandHandler<TCommand>
    where TResource : Resource
    where TCommand : class, IRemoveResourceSubtypeCommand
{
    private readonly IResourceRepository<TResource> _repository;
    private readonly Func<Guid, Exception> _notFoundExceptionFactory;

    protected RemoveResourceSubtypeHandlerBase(IResourceRepository<TResource> repository, Func<Guid, Exception> notFoundExceptionFactory)
    {
        _repository = repository;
        _notFoundExceptionFactory = notFoundExceptionFactory;
    }

    public async Task HandleAsync(TCommand command)
    {
        var resource = await _repository.GetAsync(command.Id);

        if (resource is null)
        {
            throw _notFoundExceptionFactory(command.Id);
        }

        await _repository.DeleteAsync(resource);
    }
}
