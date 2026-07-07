using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Contract : Commitment
{
    private ContractDepartmentCode _departmentCode = default!;
    private ParticipationId _internalParticipationId = default!;
    private AgentId _responsibleEmployeeId = default!;
    private ParticipationId _externalParticipationId = default!;
    private AgentId _vendorId = default!;

    protected Contract()
    {
    }

    protected Contract(
        CommitmentId id,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        decimal prepaidAmount,
        ContractDepartmentCode departmentCode,
        AgentId responsibleEmployeeId,
        AgentId vendorId)
        : base(id, NormalizeDate(startDate), NormalizeDate(endDate), prepaidAmount)
    {
        if (prepaidAmount <= 0)
        {
            throw new ContractInvalidException();
        }

        _internalParticipationId = new ParticipationId(Guid.NewGuid());
        _externalParticipationId = new ParticipationId(Guid.NewGuid());
        UpdateContractDetails(startDate, endDate, prepaidAmount, departmentCode, responsibleEmployeeId, vendorId);
    }

    public void UpdateContractDetails(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        decimal prepaidAmount,
        ContractDepartmentCode departmentCode,
        AgentId responsibleEmployeeId,
        AgentId vendorId)
    {
        if (prepaidAmount <= 0)
        {
            throw new ContractInvalidException();
        }

        UpdateTemporalDetails(NormalizeDate(startDate), NormalizeDate(endDate), prepaidAmount);
        _departmentCode = departmentCode;
        _responsibleEmployeeId = responsibleEmployeeId;
        _vendorId = vendorId;
    }

    public InternalCommitmentParticipation GetInternalParticipation()
        => CreateInternalParticipation(_internalParticipationId, _responsibleEmployeeId, Id);

    public ExternalCommitmentParticipation GetExternalParticipation()
        => CreateExternalParticipation(_externalParticipationId, _vendorId, Id);

    public IReadOnlyList<ContractMonthlyExpense> CalculateMonthlyExpenses(DaysInMonth daysInMonth = DaysInMonth.Calculated)
    {
        if (EndWhen is null || Amount is null)
        {
            throw new ContractInvalidException();
        }

        var start = DateOnly.FromDateTime(When.Date);
        var end = DateOnly.FromDateTime(EndWhen.Value.Date);
        var monthStarts = new List<DateOnly>();
        var cursor = new DateOnly(start.Year, start.Month, 1);
        var lastMonth = new DateOnly(end.Year, end.Month, 1);

        while (cursor <= lastMonth)
        {
            monthStarts.Add(cursor);
            cursor = cursor.AddMonths(1);
        }

        var coveredDaysByMonth = monthStarts
            .Select(monthStart => new
            {
                MonthStart = monthStart,
                CoveredDays = CalculateCoveredDays(start, end, monthStart, daysInMonth)
            })
            .Where(item => item.CoveredDays > 0)
            .ToList();

        var totalDays = coveredDaysByMonth.Sum(item => item.CoveredDays);

        if (totalDays <= 0)
        {
            throw new ContractInvalidException();
        }

        var pricePerDay = Amount.Value / totalDays;
        var results = new List<ContractMonthlyExpense>(coveredDaysByMonth.Count);
        decimal allocated = 0m;

        for (var i = 0; i < coveredDaysByMonth.Count; i++)
        {
            var month = coveredDaysByMonth[i];
            var coveredDays = month.CoveredDays;

            var rawAmount = pricePerDay * coveredDays;
            var roundedAmount = i == coveredDaysByMonth.Count - 1
                ? Amount.Value - allocated
                : Math.Round(rawAmount, 2, MidpointRounding.AwayFromZero);

            allocated += roundedAmount;

            results.Add(new ContractMonthlyExpense(
                month.MonthStart,
                _departmentCode.Value,
                roundedAmount,
                coveredDays,
                Math.Round(pricePerDay, 8, MidpointRounding.AwayFromZero)));
        }

        return results;
    }

    protected ContractDepartmentCode GetDepartmentCode()
        => _departmentCode;

    protected AgentId GetResponsibleEmployeeId()
        => _responsibleEmployeeId;

    protected AgentId GetVendorId()
        => _vendorId;

    private static DateTimeOffset NormalizeDate(DateTimeOffset value)
        => new(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);

    private static int CalculateCoveredDays(DateOnly contractStart, DateOnly contractEnd, DateOnly monthStart, DaysInMonth daysInMonth)
    {
        var calendarMonthEnd = new DateOnly(monthStart.Year, monthStart.Month, DateTime.DaysInMonth(monthStart.Year, monthStart.Month));
        var coveredStart = contractStart > monthStart ? contractStart : monthStart;
        var coveredEnd = contractEnd < calendarMonthEnd ? contractEnd : calendarMonthEnd;

        if (coveredEnd < coveredStart)
        {
            return 0;
        }

        return daysInMonth switch
        {
            DaysInMonth.Calculated => coveredEnd.DayNumber - coveredStart.DayNumber + 1,
            DaysInMonth.ThirtyDays => CalculateThirtyDayCoveredDays(coveredStart, coveredEnd, monthStart, calendarMonthEnd),
            _ => throw new ContractInvalidException()
        };
    }

    private static int CalculateThirtyDayCoveredDays(DateOnly coveredStart, DateOnly coveredEnd, DateOnly monthStart, DateOnly calendarMonthEnd)
    {
        var monthEndDay = 30;
        var startDay = coveredStart == monthStart
            ? 1
            : Math.Min(coveredStart.Day, monthEndDay);
        var endDay = coveredEnd == calendarMonthEnd
            ? monthEndDay
            : Math.Min(coveredEnd.Day, monthEndDay);

        return endDay - startDay + 1;
    }

    protected abstract InternalCommitmentParticipation CreateInternalParticipation(ParticipationId id, AgentId employeeId, CommitmentId contractId);
    protected abstract ExternalCommitmentParticipation CreateExternalParticipation(ParticipationId id, AgentId vendorId, CommitmentId contractId);
}
