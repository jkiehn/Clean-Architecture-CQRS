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

public class ItemCommandHandlersTests
{
    [Fact]
    public async Task CreateItem_Should_Throw_When_Name_Already_Exists()
    {
        var repository = Substitute.For<IItemRepository>();
        var readService = Substitute.For<IItemReadService>();
        readService.ExistsByNameAsync("Widget", null).Returns(true);
        ICommandHandler<CreateItem> handler = new CreateItemHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new CreateItem(Guid.NewGuid(), "Widget")));

        exception.ShouldBeOfType<ItemAlreadyExistsException>();
        await repository.DidNotReceive().AddAsync(Arg.Any<Item>());
    }

    [Fact]
    public async Task CreateItem_Should_Add_Item_On_Success()
    {
        var repository = Substitute.For<IItemRepository>();
        var readService = Substitute.For<IItemReadService>();
        readService.ExistsByNameAsync("Widget", null).Returns(false);
        ICommandHandler<CreateItem> handler = new CreateItemHandler(repository, readService);
        var itemId = Guid.NewGuid();

        await handler.HandleAsync(new CreateItem(itemId, "Widget"));

        await repository.Received(1).AddAsync(Arg.Is<Item>(item => item.Id.Value == itemId));
    }

    [Fact]
    public async Task UpdateItem_Should_Throw_When_Item_Not_Found()
    {
        var repository = Substitute.For<IItemRepository>();
        var readService = Substitute.For<IItemReadService>();
        repository.GetAsync(Arg.Any<ResourceId>()).Returns((Item?)null);
        ICommandHandler<UpdateItem> handler = new UpdateItemHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new UpdateItem(Guid.NewGuid(), "Widget")));

        exception.ShouldBeOfType<ItemNotFoundException>();
    }

    [Fact]
    public async Task UpdateItem_Should_Throw_When_Name_Belongs_To_Another_Item()
    {
        var repository = Substitute.For<IItemRepository>();
        var readService = Substitute.For<IItemReadService>();
        var itemId = Guid.NewGuid();
        repository.GetAsync(itemId).Returns(new Item(itemId, "Widget"));
        readService.ExistsByNameAsync("Taken Name", itemId).Returns(true);
        ICommandHandler<UpdateItem> handler = new UpdateItemHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new UpdateItem(itemId, "Taken Name")));

        exception.ShouldBeOfType<ItemAlreadyExistsException>();
        await repository.DidNotReceive().UpdateAsync(Arg.Any<Item>());
    }

    [Fact]
    public async Task UpdateItem_Should_Update_Item_On_Success()
    {
        var repository = Substitute.For<IItemRepository>();
        var readService = Substitute.For<IItemReadService>();
        var itemId = Guid.NewGuid();
        repository.GetAsync(itemId).Returns(new Item(itemId, "Widget"));
        readService.ExistsByNameAsync("Widget Updated", itemId).Returns(false);
        ICommandHandler<UpdateItem> handler = new UpdateItemHandler(repository, readService);

        await handler.HandleAsync(new UpdateItem(itemId, "Widget Updated"));

        await repository.Received(1).UpdateAsync(Arg.Is<Item>(item => item.Id.Value == itemId));
    }

    [Fact]
    public async Task RemoveItem_Should_Throw_When_Item_Not_Found()
    {
        var repository = Substitute.For<IItemRepository>();
        ICommandHandler<RemoveItem> handler = new RemoveItemHandler(repository);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new RemoveItem(Guid.NewGuid())));

        exception.ShouldBeOfType<ItemNotFoundException>();
    }

    [Fact]
    public async Task RemoveItem_Should_Delete_Item_On_Success()
    {
        var repository = Substitute.For<IItemRepository>();
        var item = new Item(Guid.NewGuid(), "Widget");
        repository.GetAsync(item.Id).Returns(item);
        ICommandHandler<RemoveItem> handler = new RemoveItemHandler(repository);

        await handler.HandleAsync(new RemoveItem(item.Id));

        await repository.Received(1).DeleteAsync(item);
    }
}
