using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.MoneyTransfer.API.Data;
using Sample.MoneyTransfer.API.Domain.Models;
using Sample.MoneyTransfer.API.DTO;

namespace Sample.MoneyTransfer.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly MoneyKeepingContext context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(MoneyKeepingContext context, ILogger<AccountController> logger)
        {
            this.context = context;
            this._logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CreateAccountResponse>> CreateAccount(
            NewAccountRequest accountRequest)
        {
            var account = new Account() { AccountName = accountRequest.AccountName };

            try
            {
                context.Add(account);
                await context.SaveChangesAsync();


            }
            catch (DbUpdateException /* ex */)
            {
                return BadRequest();


            }
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id },
                new CreateAccountResponse() { AccountId=account.Id});
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(long id)
        {
            var account = await context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }
    }
}

