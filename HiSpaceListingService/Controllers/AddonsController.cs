using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiSpaceListingService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AddonsController : ControllerBase
	{
		private readonly HiSpaceListingContext _context;

		public AddonsController(HiSpaceListingContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Gets the list of all Images.
		/// </summary>
		/// <returns>The list of Images.</returns>
		// GET: api/Addons/GetImagesByListingId/1
		[HttpGet("GetImagesByListingId/{ListingID}")]
		public async Task<ActionResult<IEnumerable<ListingImages>>> GetImagesByListingId(int ListingID)
		{
			return await _context.ListingImagess.Where(d => d.ListingId == ListingID).ToListAsync();
		}

		// GET: api/Addons/GetImageByListingImagesID/1
		[HttpGet("GetImageByListingImagesID/{ListingImagesID}")]
		public async Task<ActionResult<ListingImages>> GetImageByListingImagesID(int ListingImagesID)
		{
			var listingImages = await _context.ListingImagess.FirstOrDefaultAsync(d => d.ListingImagesId == ListingImagesID);

			if (listingImages == null)
			{
				return NotFound();
			}

			return listingImages;
		}

		//DELETE: api/Addons/DeleteImage/1
		[HttpDelete("DeleteImage/{ListingImagesID}")]
		public async Task<ActionResult<ListingImages>> DeleteImage(int ListingImagesID)
		{
			var image = await _context.ListingImagess.FindAsync(ListingImagesID);
			if(image == null)
			{
				return NotFound();
			}
			_context.ListingImagess.Remove(image);
			await _context.SaveChangesAsync();
			return image;
		}

		/// <summary>
		/// Post the ListingImages.
		/// </summary>
		/// <returns>Add new Image.</returns>
		// POST: api/Addons/CreateImage
		[HttpPost("CreateImage")]
		public async Task<ActionResult<ListingImages>> CreateImage([FromBody] ListingImages listingImages)
		{
			_context.ListingImagess.Add(listingImages);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetImageByListingImagesID", new { ListingImagesId = listingImages.ListingImagesId }, listingImages);
		}

		// PUT: api/Addons/UpdateImage/1
		[HttpPut("UpdateImage/{ListingImagesId}")]
		public async Task<IActionResult> UpdateImage(int ListingImagesId, [FromBody] ListingImages listingImages)
		{
			if (ListingImagesId != listingImages.ListingImagesId || listingImages == null)
			{
				return BadRequest();
			}

			_context.Entry(listingImages).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!WorkingHoursExists(ListingImagesId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetImageByListingImagesID", new { ListingImagesId = listingImages.ListingImagesId }, listingImages);
		}

		// GET: api/Addons/GetWoringHoursByListingID/1
		[HttpGet("GetWoringHoursByListingID/{ListingId}")]
		public async Task<ActionResult<WorkingHours>> GetWoringHoursByListingID(int ListingId)
		{
			var workingHours = await _context.WorkingHourss.FirstOrDefaultAsync(d => d.ListingId == ListingId);

			if (workingHours == null)
			{
				return NotFound();
			}

			return workingHours;
		}

		/// <summary>
		/// Post the WorkingHours.
		/// </summary>
		/// <returns>The list of WorkingHours.</returns>
		// POST: api/Addons/AddCreateHours
		[HttpPost("AddCreateHours")]
		public async Task<ActionResult<WorkingHours>> AddCreateHours([FromBody] WorkingHours workingHours)
		{
			_context.WorkingHourss.Add(workingHours);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetWoringHoursByListingID", new { ListingId = workingHours.ListingId }, workingHours);
		}

		// PUT: api/Addons/UpdateHours/1
		[HttpPut("UpdateHours/{ListingId}")]
		public async Task<IActionResult> UpdateHours(int ListingId, [FromBody] WorkingHours workingHours)
		{
			if (ListingId != workingHours.ListingId || workingHours == null)
			{
				return BadRequest();
			}

			_context.Entry(workingHours).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!WorkingHoursExists(ListingId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		private bool WorkingHoursExists(int ListingId)
		{
			return _context.WorkingHourss.Any(e => e.ListingId == ListingId);
		}

	}
}