using Asp.Versioning;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Payments.Commands.RecordPayment;
using KGSystem.Application.Payments.Commands.UpdatePayment;
using KGSystem.Application.Payments.Dtos;
using KGSystem.Application.Payments.Queries.GetChildPayments;
using KGSystem.Application.Payments.Queries.GetPayments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class PaymentsController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayments(
        [FromQuery] int? childId,
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentsQuery(childId, month, year, status, fromDate, toDate);
        var result = await dispatcher.Send<IReadOnlyList<PaymentDto>>(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet("child/{childId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildPayments(int childId, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<IReadOnlyList<PaymentDto>>(new GetChildPaymentsQuery(childId), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant")]
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentCommand command, CancellationToken cancellationToken)
    {
        var paymentId = await dispatcher.Send<int>(command, cancellationToken);
        return CreatedAtAction(nameof(GetChildPayments), new { childId = command.EnrollmentId }, paymentId);
    }

    [Authorize(Roles = "Accountant")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentCommand command, CancellationToken cancellationToken)
    {
        await dispatcher.Send<Unit>(command with { Id = id }, cancellationToken);
        return NoContent();
    }
}
