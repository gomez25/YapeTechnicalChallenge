using Microsoft.AspNetCore.Mvc;
using TransactionService.Domain.DTOs;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Api.Controllers
{
    [Route("api/[controller]")]
  [ApiController]
  public class TransactionController(ITransactionService transactionService) : ControllerBase
  {
    private readonly ITransactionService _transactionService = transactionService;

    [HttpPost]
    public async Task<IActionResult> AddTransaction([FromBody] AddTransactionDto transaction)
    {
      var result = await _transactionService.AddTransactionAsync(transaction);
      if (result.IsSuccess)
      {
        return Ok(result);
      }
      return StatusCode(result.StatusCode, result);
    }
  }
}
