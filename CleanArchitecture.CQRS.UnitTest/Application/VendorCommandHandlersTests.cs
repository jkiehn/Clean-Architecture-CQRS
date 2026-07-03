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

public class VendorCommandHandlersTests
{
    [Fact]
    public async Task CreateVendor_Should_Throw_When_Email_Already_Exists()
    {
        var repository = Substitute.For<IVendorRepository>();
        var readService = Substitute.For<IVendorReadService>();
        readService.ExistsByEmailAsync("sales@northwind.example", null).Returns(true);
        ICommandHandler<CreateVendor> handler = new CreateVendorHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new CreateVendor(Guid.NewGuid(), "Northwind", "sales@northwind.example")));

        exception.ShouldBeOfType<VendorAlreadyExistsException>();
        await repository.DidNotReceive().AddAsync(Arg.Any<Vendor>());
    }

    [Fact]
    public async Task CreateVendor_Should_Add_Vendor_On_Success()
    {
        var repository = Substitute.For<IVendorRepository>();
        var readService = Substitute.For<IVendorReadService>();
        readService.ExistsByEmailAsync("sales@northwind.example", null).Returns(false);
        ICommandHandler<CreateVendor> handler = new CreateVendorHandler(repository, readService);
        var vendorId = Guid.NewGuid();

        await handler.HandleAsync(new CreateVendor(vendorId, "Northwind", "sales@northwind.example"));

        await repository.Received(1).AddAsync(Arg.Is<Vendor>(vendor => vendor.Id.Value == vendorId));
    }

    [Fact]
    public async Task UpdateVendor_Should_Throw_When_Vendor_Not_Found()
    {
        var repository = Substitute.For<IVendorRepository>();
        var readService = Substitute.For<IVendorReadService>();
        repository.GetAsync(Arg.Any<AgentId>()).Returns((Vendor?)null);
        ICommandHandler<UpdateVendor> handler = new UpdateVendorHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new UpdateVendor(Guid.NewGuid(), "Northwind", "sales@northwind.example")));

        exception.ShouldBeOfType<VendorNotFoundException>();
    }

    [Fact]
    public async Task UpdateVendor_Should_Throw_When_Email_Belongs_To_Another_Vendor()
    {
        var repository = Substitute.For<IVendorRepository>();
        var readService = Substitute.For<IVendorReadService>();
        var vendorId = Guid.NewGuid();
        repository.GetAsync(vendorId).Returns(new Vendor(vendorId, "Northwind", "sales@northwind.example"));
        readService.ExistsByEmailAsync("taken@example.com", vendorId).Returns(true);
        ICommandHandler<UpdateVendor> handler = new UpdateVendorHandler(repository, readService);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new UpdateVendor(vendorId, "Northwind", "taken@example.com")));

        exception.ShouldBeOfType<VendorAlreadyExistsException>();
        await repository.DidNotReceive().UpdateAsync(Arg.Any<Vendor>());
    }

    [Fact]
    public async Task UpdateVendor_Should_Update_Vendor_On_Success()
    {
        var repository = Substitute.For<IVendorRepository>();
        var readService = Substitute.For<IVendorReadService>();
        var vendorId = Guid.NewGuid();
        repository.GetAsync(vendorId).Returns(new Vendor(vendorId, "Northwind", "sales@northwind.example"));
        readService.ExistsByEmailAsync("contact@contoso.example", vendorId).Returns(false);
        ICommandHandler<UpdateVendor> handler = new UpdateVendorHandler(repository, readService);

        await handler.HandleAsync(new UpdateVendor(vendorId, "Contoso Supply", "contact@contoso.example"));

        await repository.Received(1).UpdateAsync(Arg.Is<Vendor>(vendor => vendor.Id.Value == vendorId));
    }

    [Fact]
    public async Task RemoveVendor_Should_Throw_When_Vendor_Not_Found()
    {
        var repository = Substitute.For<IVendorRepository>();
        ICommandHandler<RemoveVendor> handler = new RemoveVendorHandler(repository);

        var exception = await Record.ExceptionAsync(() =>
            handler.HandleAsync(new RemoveVendor(Guid.NewGuid())));

        exception.ShouldBeOfType<VendorNotFoundException>();
    }

    [Fact]
    public async Task RemoveVendor_Should_Delete_Vendor_On_Success()
    {
        var repository = Substitute.For<IVendorRepository>();
        var vendor = new Vendor(Guid.NewGuid(), "Northwind", "sales@northwind.example");
        repository.GetAsync(vendor.Id).Returns(vendor);
        ICommandHandler<RemoveVendor> handler = new RemoveVendorHandler(repository);

        await handler.HandleAsync(new RemoveVendor(vendor.Id));

        await repository.Received(1).DeleteAsync(vendor);
    }
}
