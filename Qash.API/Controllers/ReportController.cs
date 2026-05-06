using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qash.API.Features.Reports.DTOs;
using Qash.API.Features.Reports.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Qash.API.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public class ReportController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<MonthlySummaryQuery> _monthlySummaryValidator;
    private readonly IValidator<CategoryBreakdownQuery> _categoryBreakdownValidator;
    private readonly IValidator<IncomeVsExpenseQuery> _incomeVsExpenseValidator;
    private readonly IValidator<SpendingTrendQuery> _spendingTrendValidator;
    private readonly IValidator<DateRangeSummaryQuery> _dateRangeSummaryValidator;

    public ReportController(
        IMediator mediator,
        IValidator<MonthlySummaryQuery> monthlySummaryValidator,
        IValidator<CategoryBreakdownQuery> categoryBreakdownValidator,
        IValidator<IncomeVsExpenseQuery> incomeVsExpenseValidator,
        IValidator<SpendingTrendQuery> spendingTrendValidator,
        IValidator<DateRangeSummaryQuery> dateRangeSummaryValidator)
    {
        _mediator = mediator;
        _monthlySummaryValidator = monthlySummaryValidator;
        _categoryBreakdownValidator = categoryBreakdownValidator;
        _incomeVsExpenseValidator = incomeVsExpenseValidator;
        _spendingTrendValidator = spendingTrendValidator;
        _dateRangeSummaryValidator = dateRangeSummaryValidator;
    }

    [HttpGet("monthly-summary")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var query = new MonthlySummaryQuery(userId.Value, year, month);
        var validationErrors = await ValidateAsync(query, _monthlySummaryValidator);

        if (validationErrors is not null)
            return BadRequest(validationErrors);

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("category-breakdown")]
    public async Task<IActionResult> GetCategoryBreakdown([FromQuery] int year, [FromQuery] int month)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var query = new CategoryBreakdownQuery(userId.Value, year, month);
        var validationErrors = await ValidateAsync(query, _categoryBreakdownValidator);

        if (validationErrors is not null)
            return BadRequest(validationErrors);

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("income-vs-expense")]
    public async Task<IActionResult> GetIncomeVsExpense([FromQuery] int year)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var query = new IncomeVsExpenseQuery(userId.Value, year);
        var validationErrors = await ValidateAsync(query, _incomeVsExpenseValidator);

        if (validationErrors is not null)
            return BadRequest(validationErrors);

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("spending-trend")]
    public async Task<IActionResult> GetSpendingTrend([FromQuery] int days)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var query = new SpendingTrendQuery(userId.Value, days);
        var validationErrors = await ValidateAsync(query, _spendingTrendValidator);

        if (validationErrors is not null)
            return BadRequest(validationErrors);

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("date-range-summary")]
    public async Task<IActionResult> GetDateRangeSummary([FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var query = new DateRangeSummaryQuery(userId.Value, fromUtc, toUtc);
        var validationErrors = await ValidateAsync(query, _dateRangeSummaryValidator);

        if (validationErrors is not null)
            return BadRequest(validationErrors);

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    private static async Task<object?> ValidateAsync<T>(T request, IValidator<T> validator)
    {
        var validationResult = await validator.ValidateAsync(request!);

        if (validationResult.IsValid)
            return null;

        return new
        {
            Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToArray()
        };
    }

    private Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return null;

        return userId;
    }
}