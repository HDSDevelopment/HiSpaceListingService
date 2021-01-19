using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using HiSpaceListingService.Utilities;
using HiSpaceListingService.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HiSpaceListingService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InvestorController : Controller
	{
		private readonly HiSpaceListingContext _context;

		public InvestorController(HiSpaceListingContext context)
		{
			_context = context;
		}


		[HttpPost]
		[Route("AddInvestor")]
		public async Task<ActionResult<InvestorViewModel>> AddInvestor([FromBody] Investor investor)
		{
			_context.Investors.Add(investor);
			await _context.SaveChangesAsync();

			InvestorViewModel model = new InvestorViewModel(investor);
			model.Assign();

			EmailMessage email = new EmailMessage();
			var DetailsSentStatus = email.SendInvestorDetails(investor);

			if (DetailsSentStatus == true)
				model.IsSuccessMessageSent = email.SendInvestorSuccess(investor.FirstName, investor.LastName, investor.Email);

			return CreatedAtAction("GetInvestor", model);
		}

		[HttpGet]
		[Route("GetInvestor")]
		public async Task<ActionResult<Investor>> GetInvestor()
		{
			int id = 0;
			try
			{
				id = (from investor in _context.Investors.AsNoTracking() 
						  select investor.InvestorId)
						  .Max();

				if (id == 0)
					return NotFound();

				var latestInvestor = await _context.Investors.AsNoTracking()
											.FirstOrDefaultAsync(n => n.InvestorId == id);

				return Ok(latestInvestor);
			}
			catch (Exception ex)
			{

				//throw ex;
				return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");

			}

		}
	}
}
