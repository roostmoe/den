using System.Security.Claims;
using Den.Application.Budgets;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Den.Api.Controllers.Budgets;

[ApiController]
[Route("v1/budgets")]
[Authorize]
public class BudgetsController(
    IBudgetService budgetService,
    IValidator<CreateBudgetRequest> createValidator,
    IValidator<UpdateBudgetRequest> updateValidator,
    ILogger<BudgetsController> logger
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<BudgetResponse>>> GetAll()
    {
        var budgets = await budgetService.GetAllAsync();
        return Ok(budgets);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetResponse>> GetById(Guid id)
    {
        var budget = await budgetService.GetByIdAsync(id);
        return budget switch
        {
            null => NotFound(new { error = "budget not found" }),
            _ => Ok(budget)
        };
    }

    [HttpPost]
    [Authorize(Policy = "EditorOrHigher")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BudgetResponse>> Create([FromBody] CreateBudgetRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "invalid token" });
        }

        var validationResult = await createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var budget = await budgetService.CreateAsync(request, userId.Value);
        return CreatedAtAction(nameof(GetById), new { id = budget.Id }, budget);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "EditorOrHigher")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetResponse>> Update(Guid id, [FromBody] UpdateBudgetRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "invalid token" });
        }

        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var budget = await budgetService.UpdateAsync(id, request, userId.Value);
        return budget switch
        {
            null => NotFound(new { error = "budget not found" }),
            _ => Ok(budget)
        };
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "EditorOrHigher")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { error = "invalid token" });
        }

        var deleted = await budgetService.DeleteAsync(id, userId.Value);
        return deleted ? NoContent() : NotFound(new { error = "budget not found" });
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            logger.LogDebug("Invalid token: {Reason}", new { Reason = "Token has no subject ID" });
            return null;
        }

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
