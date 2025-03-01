using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Features.Finances.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LifeSync.API.Features.FinanceTransactions
{
    [ApiController]
    [Authorize]
    [Route("api/finances")]
    public class FinancesController : ControllerBase
    {
        private readonly ITransactionsSearchService _transactionsSearchService;
        private readonly IExpenseTransactionsManagement _expenseTransactionsManagement;
        private readonly IIncomeTransactionsManagement _incomeTransactionsManagement;

        public FinancesController(
            ITransactionsSearchService transactionsSearchService,
            IExpenseTransactionsManagement expenseTransactionsManagement,
            IIncomeTransactionsManagement incomeTransactionsManagement)
        {
            _transactionsSearchService = transactionsSearchService;
            _expenseTransactionsManagement = expenseTransactionsManagement;
            _incomeTransactionsManagement = incomeTransactionsManagement;
        }

        [HttpGet("transactions", Name = nameof(GetTransactions))]
        [AllowAnonymous]
        public async Task<IActionResult> GetTransactions([FromQuery] GetUserFinancialTransactionsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _transactionsSearchService.GetUserFinancialTransactionsAsync(userId, request);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("transactions/expense", Name = nameof(GetExpenseTransactions))]
        public async Task<IActionResult> GetExpenseTransactions([FromQuery] GetUserExpenseTransactionsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _expenseTransactionsManagement.GetUserExpenseTransactionsAsync(userId, request);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        [HttpPost("transactions/expense", Name = nameof(AddExpense))]
        public async Task<IActionResult> AddExpense(AddExpenseDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _expenseTransactionsManagement.AddExpenseAsync(userId, request);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result.Data);
        }

        [HttpGet("transactions/income", Name = nameof(GetIncomeTransactions))]
        public async Task<IActionResult> GetIncomeTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _incomeTransactionsManagement.GetUserIncomesAsync(userId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Data);
        }

        [HttpPost("transactions/income", Name = nameof(AddIncome))]
        public async Task<IActionResult> AddIncome(AddIncomeDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _incomeTransactionsManagement.AddIncomeAsync(userId, request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Data);
        }
    }
}
