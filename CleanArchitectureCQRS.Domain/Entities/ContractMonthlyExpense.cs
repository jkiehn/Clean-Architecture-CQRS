namespace CleanArchitectureCQRS.Domain.Entities;

public sealed record ContractMonthlyExpense(
    DateOnly Month,
    string DepartmentCode,
    decimal Amount,
    int CoveredDays,
    decimal PricePerDay);
