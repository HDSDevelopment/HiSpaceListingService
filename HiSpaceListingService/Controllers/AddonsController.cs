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
				if (!ListingImagesExists(ListingImagesId))
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
		private bool ListingImagesExists(int ListingImagesId)
		{
			return _context.ListingImagess.Any(e => e.ListingImagesId == ListingImagesId);
		}

		/// <summary>
		/// Gets the list of all Project.
		/// </summary>
		/// <returns>The list of Project.</returns>
		// GET: api/Addons/GetProjectByListingId/1
		[HttpGet("GetProjectByListingId/{ListingID}")]
		public async Task<ActionResult<IEnumerable<REProfessionalMaster>>> GetProjectByListingId(int ListingID)
		{
			return await _context.REProfessionalMasters.Where(d => d.ListingId == ListingID).ToListAsync();
		}

		// GET: api/Addons/GetImageByREProfessionalMasterId/1
		[HttpGet("GetImageByREProfessionalMasterId/{REProfessionalMasterID}")]
		public async Task<ActionResult<REProfessionalMaster>> GetImageByREProfessionalMasterId(int REProfessionalMasterID)
		{
			var project = await _context.REProfessionalMasters.FirstOrDefaultAsync(d => d.REProfessionalMasterId == REProfessionalMasterID);

			if (project == null)
			{
				return NotFound();
			}

			return project;
		}

		//DELETE: api/Addons/DeleteProject/1
		[HttpDelete("DeleteProject/{REProfessionalMasterID}")]
		public async Task<ActionResult<REProfessionalMaster>> DeleteProject(int REProfessionalMasterID)
		{
			var project = await _context.REProfessionalMasters.FindAsync(REProfessionalMasterID);
			if (project == null)
			{
				return NotFound();
			}
			_context.REProfessionalMasters.Remove(project);
			await _context.SaveChangesAsync();
			return project;
		}

		/// <summary>
		/// Post the ReProfessionalProjects.
		/// </summary>
		/// <returns>Add new project.</returns>
		// POST: api/Addons/CreateProject
		[HttpPost("CreateProject")]
		public async Task<ActionResult<REProfessionalMaster>> CreateProject([FromBody] REProfessionalMaster rEProfessionalMaster)
		{
			_context.REProfessionalMasters.Add(rEProfessionalMaster);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetImageByREProfessionalMasterId", new { REProfessionalMasterId = rEProfessionalMaster.REProfessionalMasterId }, rEProfessionalMaster);
		}

		// PUT: api/Addons/UpdateProject/1
		[HttpPut("UpdateProject/{REProfessionalMasterId}")]
		public async Task<IActionResult> UpdateProject(int REProfessionalMasterId, [FromBody] REProfessionalMaster rEProfessionalMaster)
		{
			if (REProfessionalMasterId != rEProfessionalMaster.REProfessionalMasterId || rEProfessionalMaster == null)
			{
				return BadRequest();
			}

			_context.Entry(rEProfessionalMaster).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProjectExists(REProfessionalMasterId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetImageByREProfessionalMasterId", new { REProfessionalMasterId = rEProfessionalMaster.REProfessionalMasterId }, rEProfessionalMaster);
		}
		private bool ProjectExists(int REProfessionalMasterId)
		{
			return _context.REProfessionalMasters.Any(e => e.REProfessionalMasterId == REProfessionalMasterId);
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


		/// <summary>
		/// Gets the list of all Amenity.
		/// </summary>
		/// <returns>The list of Amenity.</returns>
		// GET: api/Addons/GetAmenityByListingId/1
		[HttpGet("GetAmenityByListingId/{ListingID}")]
		public async Task<ActionResult<IEnumerable<Amenity>>> GetAmenityByListingId(int ListingID)
		{
			return await _context.Amenitys.Where(d => d.ListingId == ListingID).ToListAsync();
		}

		// GET: api/Addons/GetImageByAmenityId/1
		[HttpGet("GetImageByAmenityId/{AmenityID}")]
		public async Task<ActionResult<Amenity>> GetImageByAmenityId(int AmenityID)
		{
			var amenity = await _context.Amenitys.FirstOrDefaultAsync(d => d.AmenityId == AmenityID);

			if (amenity == null)
			{
				return NotFound();
			}

			return amenity;
		}

		//DELETE: api/Addons/DeleteAmenity/1
		[HttpDelete("DeleteAmenity/{AmenityID}")]
		public async Task<ActionResult<Amenity>> DeleteAmenity(int AmenityID)
		{
			var amenity = await _context.Amenitys.FindAsync(AmenityID);
			if (amenity == null)
			{
				return NotFound();
			}
			_context.Amenitys.Remove(amenity);
			await _context.SaveChangesAsync();
			return amenity;
		}

		/// <summary>
		/// Post the Amenity.
		/// </summary>
		/// <returns>Add new Amenity.</returns>
		// POST: api/Addons/CreateAmenity
		[HttpPost("CreateAmenity")]
		public async Task<ActionResult<Amenity>> CreateAmenity([FromBody] Amenity amenity)
		{
			_context.Amenitys.Add(amenity);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetImageByAmenityId", new { AmenityId = amenity.AmenityId }, amenity);
		}


		// PUT: api/Addons/UpdateAmenity/1
		[HttpPut("UpdateAmenity/{AmenityId}")]
		public async Task<IActionResult> UpdateAmenity(int AmenityId, [FromBody] Amenity amenity)
		{
			if (AmenityId != amenity.AmenityId || amenity == null)
			{
				return BadRequest();
			}

			_context.Entry(amenity).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!AmenitysExists(AmenityId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetImageByAmenityId", new { AmenityId = amenity.AmenityId }, amenity);
		}
		private bool AmenitysExists(int AmenityId)
		{
			return _context.Amenitys.Any(e => e.AmenityId == AmenityId);
		}
	}
}