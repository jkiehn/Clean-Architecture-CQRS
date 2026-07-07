using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class ItContract : Contract
{
    private ContractServiceName _serviceName = default!;

    public ItContract()
    {
    }

    public ItContract(
        CommitmentId id,
        ContractServiceName serviceName,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        decimal prepaidAmount,
        ContractDepartmentCode departmentCode,
        AgentId responsibleEmployeeId,
        AgentId vendorId)
        : base(id, startDate, endDate, prepaidAmount, departmentCode, responsibleEmployeeId, vendorId)
    {
        _serviceName = serviceName;
    }

    public void UpdateDetails(
        ContractServiceName serviceName,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        decimal prepaidAmount,
        ContractDepartmentCode departmentCode,
        AgentId responsibleEmployeeId,
        AgentId vendorId)
    {
        _serviceName = serviceName;
        UpdateContractDetails(startDate, endDate, prepaidAmount, departmentCode, responsibleEmployeeId, vendorId);
    }

    public string GetServiceName()
        => _serviceName.Value;

    protected override InternalCommitmentParticipation CreateInternalParticipation(ParticipationId id, AgentId employeeId, CommitmentId contractId)
        => new ItContractInternalParticipation(id, employeeId, contractId);

    protected override ExternalCommitmentParticipation CreateExternalParticipation(ParticipationId id, AgentId vendorId, CommitmentId contractId)
        => new ItContractExternalParticipation(id, vendorId, contractId);
}
