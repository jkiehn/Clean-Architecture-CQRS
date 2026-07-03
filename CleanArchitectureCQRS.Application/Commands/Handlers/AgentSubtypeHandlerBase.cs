using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal abstract class CreateAgentSubtypeHandlerBase<TAgent, TCommand> : ICommandHandler<TCommand>
    where TAgent : Agent
    where TCommand : class, ICreateAgentSubtypeCommand
{
    private readonly IAgentRepository<TAgent> _repository;
    private readonly IAgentSubtypeReadService _readService;
    private readonly Func<string, Exception> _alreadyExistsExceptionFactory;
    private readonly Func<TCommand, TAgent> _agentFactory;

    protected CreateAgentSubtypeHandlerBase(
        IAgentRepository<TAgent> repository,
        IAgentSubtypeReadService readService,
        Func<string, Exception> alreadyExistsExceptionFactory,
        Func<TCommand, TAgent> agentFactory)
    {
        _repository = repository;
        _readService = readService;
        _alreadyExistsExceptionFactory = alreadyExistsExceptionFactory;
        _agentFactory = agentFactory;
    }

    public async Task HandleAsync(TCommand command)
    {
        if (await _readService.ExistsByEmailAsync(command.Email))
        {
            throw _alreadyExistsExceptionFactory(command.Email);
        }

        await _repository.AddAsync(_agentFactory(command));
    }
}

internal abstract class UpdateAgentSubtypeHandlerBase<TAgent, TCommand> : ICommandHandler<TCommand>
    where TAgent : Agent
    where TCommand : class, IUpdateAgentSubtypeCommand
{
    private readonly IAgentRepository<TAgent> _repository;
    private readonly IAgentSubtypeReadService _readService;
    private readonly Func<string, Exception> _alreadyExistsExceptionFactory;
    private readonly Func<Guid, Exception> _notFoundExceptionFactory;

    protected UpdateAgentSubtypeHandlerBase(
        IAgentRepository<TAgent> repository,
        IAgentSubtypeReadService readService,
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
        var agent = await _repository.GetAsync(command.Id);

        if (agent is null)
        {
            throw _notFoundExceptionFactory(command.Id);
        }

        if (await _readService.ExistsByEmailAsync(command.Email, command.Id))
        {
            throw _alreadyExistsExceptionFactory(command.Email);
        }

        agent.UpdateDetails(command.Name, command.Email);
        await _repository.UpdateAsync(agent);
    }
}

internal abstract class RemoveAgentSubtypeHandlerBase<TAgent, TCommand> : ICommandHandler<TCommand>
    where TAgent : Agent
    where TCommand : class, IRemoveAgentSubtypeCommand
{
    private readonly IAgentRepository<TAgent> _repository;
    private readonly Func<Guid, Exception> _notFoundExceptionFactory;

    protected RemoveAgentSubtypeHandlerBase(IAgentRepository<TAgent> repository, Func<Guid, Exception> notFoundExceptionFactory)
    {
        _repository = repository;
        _notFoundExceptionFactory = notFoundExceptionFactory;
    }

    public async Task HandleAsync(TCommand command)
    {
        var agent = await _repository.GetAsync(command.Id);

        if (agent is null)
        {
            throw _notFoundExceptionFactory(command.Id);
        }

        await _repository.DeleteAsync(agent);
    }
}
