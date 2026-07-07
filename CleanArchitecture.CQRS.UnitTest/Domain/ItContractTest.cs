using CleanArchitectureCQRS.Domain.Entities;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class ItContractTest
{
    [Fact]
    public void CalculateMonthlyExpenses_Prorrates_Start_And_End_Months_And_Preserves_Total()
    {
        var contract = new ItContract(
            Guid.NewGuid(),
            "Microsoft 365 E5",
            new DateTimeOffset(2026, 1, 15, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 14, 17, 0, 0, TimeSpan.Zero),
            590m,
            "FIN",
            Guid.NewGuid(),
            Guid.NewGuid());

        var expenses = contract.CalculateMonthlyExpenses();

        expenses.Count.ShouldBe(3);
        expenses[0].Month.ShouldBe(new DateOnly(2026, 1, 1));
        expenses[0].CoveredDays.ShouldBe(17);
        expenses[0].Amount.ShouldBe(170m);
        expenses[1].CoveredDays.ShouldBe(28);
        expenses[1].Amount.ShouldBe(280m);
        expenses[2].CoveredDays.ShouldBe(14);
        expenses[2].Amount.ShouldBe(140m);
        expenses.Sum(item => item.Amount).ShouldBe(590m);
    }

    [Fact]
    public void CalculateMonthlyExpenses_With_ThirtyDay_Mode_Keeps_Full_Months_Equal()
    {
        var contract = new ItContract(
            Guid.NewGuid(),
            "Microsoft 365 E5",
            new DateTimeOffset(2024, 1, 15, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 4, 14, 17, 0, 0, TimeSpan.Zero),
            900m,
            "FIN",
            Guid.NewGuid(),
            Guid.NewGuid());

        var expenses = contract.CalculateMonthlyExpenses(DaysInMonth.ThirtyDays);

        expenses.Count.ShouldBe(4);
        expenses[0].CoveredDays.ShouldBe(16);
        expenses[1].CoveredDays.ShouldBe(30);
        expenses[2].CoveredDays.ShouldBe(30);
        expenses[3].CoveredDays.ShouldBe(14);
        expenses[1].Amount.ShouldBe(expenses[2].Amount);
        expenses[1].Amount.ShouldBe(300m);
        expenses.Sum(item => item.Amount).ShouldBe(900m);
    }
}
