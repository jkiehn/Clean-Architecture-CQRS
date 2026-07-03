using CleanArchitectureCQRS.Application.Commands;
using CleanArchitectureCQRS.Application.Commands.Handlers;
using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;
using NSubstitute;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Application;

public class CustomerCommandHandlersTests
{
    [Fact]
    public async Task CreateCustomer_Should_Throw_When_Email_Already_Exists()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var readService = Substitute.For<ICustomerReadService>();
        readService.ExistsByEmailAsync("alice@example.com", null).Returns(true);
        ICommandHandler<CreateCustomer> handler = new CreateCustomerHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new CreateCustomer(Guid.NewGuid(), "Alice", "alice@example.com")));

        exception.ShouldBeOfType<CustomerAlreadyExistsException>();
        await repository.DidNotReceive().AddAsync(Arg.Any<Customer>());
    }

    [Fact]
    public async Task CreateCustomer_Should_Add_Customer_On_Success()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var readService = Substitute.For<ICustomerReadService>();
        readService.ExistsByEmailAsync("alice@example.com", null).Returns(false);
        ICommandHandler<CreateCustomer> handler = new CreateCustomerHandler(repository, readService);
        var customerId = Guid.NewGuid();

        await handler.HandleAsync(new CreateCustomer(customerId, "Alice", "alice@example.com"));

        await repository.Received(1).AddAsync(Arg.Is<Customer>(customer => customer.Id.Value == customerId));
    }

    [Fact]
    public async Task UpdateCustomer_Should_Throw_When_Customer_Not_Found()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var readService = Substitute.For<ICustomerReadService>();
        repository.GetAsync(Arg.Any<AgentId>()).Returns((Customer?)null);
        ICommandHandler<UpdateCustomer> handler = new UpdateCustomerHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new UpdateCustomer(Guid.NewGuid(), "Alice", "alice@example.com")));

        exception.ShouldBeOfType<CustomerNotFoundException>();
    }

    [Fact]
    public async Task UpdateCustomer_Should_Throw_When_Email_Belongs_To_Another_Customer()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var readService = Substitute.For<ICustomerReadService>();
        var customerId = Guid.NewGuid();
        repository.GetAsync(customerId).Returns(new Customer(customerId, "Alice", "alice@example.com"));
        readService.ExistsByEmailAsync("taken@example.com", customerId).Returns(true);
        ICommandHandler<UpdateCustomer> handler = new UpdateCustomerHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new UpdateCustomer(customerId, "Alice", "taken@example.com")));

        exception.ShouldBeOfType<CustomerAlreadyExistsException>();
        await repository.DidNotReceive().UpdateAsync(Arg.Any<Customer>());
    }

    [Fact]
    public async Task UpdateCustomer_Should_Update_Customer_On_Success()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var readService = Substitute.For<ICustomerReadService>();
        var customerId = Guid.NewGuid();
        repository.GetAsync(customerId).Returns(new Customer(customerId, "Alice", "alice@example.com"));
        readService.ExistsByEmailAsync("updated@example.com", customerId).Returns(false);
        ICommandHandler<UpdateCustomer> handler = new UpdateCustomerHandler(repository, readService);

        await handler.HandleAsync(new UpdateCustomer(customerId, "Alice Updated", "updated@example.com"));

        await repository.Received(1).UpdateAsync(Arg.Is<Customer>(customer => customer.Id.Value == customerId));
    }

    [Fact]
    public async Task RemoveCustomer_Should_Throw_When_Customer_Not_Found()
    {
        var repository = Substitute.For<ICustomerRepository>();
        ICommandHandler<RemoveCustomer> handler = new RemoveCustomerHandler(repository);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new RemoveCustomer(Guid.NewGuid())));

        exception.ShouldBeOfType<CustomerNotFoundException>();
    }

    [Fact]
    public async Task RemoveCustomer_Should_Delete_Customer_On_Success()
    {
        var repository = Substitute.For<ICustomerRepository>();
        var customer = new Customer(Guid.NewGuid(), "Alice", "alice@example.com");
        repository.GetAsync(customer.Id).Returns(customer);
        ICommandHandler<RemoveCustomer> handler = new RemoveCustomerHandler(repository);

        await handler.HandleAsync(new RemoveCustomer(customer.Id));

        await repository.Received(1).DeleteAsync(customer);
    }
}
