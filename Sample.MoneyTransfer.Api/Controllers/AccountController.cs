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
        private readonly Gringotts  context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(Gringotts  context, ILogger<AccountController> logger)
        {
            this.context = context;
            this._logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CreateAccountResponse>> CreateAccount(
            NewAccountRequest accountRequest)
        {
            var account = new Account() { AccountName = accountRequest.AccountName,
                                          Balance=accountRequest.InitialFunds,
                                          Internal=accountRequest.Internal};

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
        public async Task<ActionResult<AccountDto>> GetAccount(long id, [FromQuery(Name = "all")] bool all)
        {
            var account = await context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }
            
            return new AccountDto { AccountId=account.Id, AccountName=account.AccountName};
        }
    }
}

