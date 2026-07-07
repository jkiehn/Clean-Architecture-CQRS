namespace CleanArchitectureCQRS.Domain.Entities.Procurement;

public sealed record ContractMonthlyExpense(
    DateOnly Month,
    string DepartmentCode,
    decimal Amount,
    int CoveredDays,
    decimal PricePerDay);
