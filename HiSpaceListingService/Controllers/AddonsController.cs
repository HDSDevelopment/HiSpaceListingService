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

		// GET: api/Addons/GetAmenityByAmenityId/1
		[HttpGet("GetAmenityByAmenityId/{AmenityID}")]
		public async Task<ActionResult<Amenity>> GetAmenityByAmenityId(int AmenityID)
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

			return CreatedAtAction("GetAmenityByAmenityId", new { AmenityId = amenity.AmenityId }, amenity);
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

			return CreatedAtAction("GetAmenityByAmenityId", new { AmenityId = amenity.AmenityId }, amenity);
		}
		private bool AmenitysExists(int AmenityId)
		{
			return _context.Amenitys.Any(e => e.AmenityId == AmenityId);
		}

		/// <summary>
		/// Gets the list of all Facility.
		/// </summary>
		/// <returns>The list of Facility.</returns>
		// GET: api/Addons/GetFacilityByListingId/1
		[HttpGet("GetFacilityByListingId/{ListingID}")]
		public async Task<ActionResult<IEnumerable<Facility>>> GetFacilityByListingId(int ListingID)
		{
			return await _context.Facilitys.Where(d => d.ListingId == ListingID).ToListAsync();
		}

		// GET: api/Addons/GetFacilityByFacilityId/1
		[HttpGet("GetFacilityByFacilityId/{FacilityID}")]
		public async Task<ActionResult<Facility>> GetFacilityByFacilityId(int FacilityID)
		{
			var facility = await _context.Facilitys.FirstOrDefaultAsync(d => d.FacilityId == FacilityID);

			if (facility == null)
			{
				return NotFound();
			}

			return facility;
		}

		//DELETE: api/Addons/DeleteFacility/1
		[HttpDelete("DeleteFacility/{FacilityID}")]
		public async Task<ActionResult<Facility>> DeleteFacility(int FacilityID)
		{
			var facility = await _context.Facilitys.FindAsync(FacilityID);
			if (facility == null)
			{
				return NotFound();
			}
			_context.Facilitys.Remove(facility);
			await _context.SaveChangesAsync();
			return facility;
		}

		/// <summary>
		/// Post the Facility.
		/// </summary>
		/// <returns>Add new Facility.</returns>
		// POST: api/Addons/CreateFacility
		[HttpPost("CreateFacility")]
		public async Task<ActionResult<Facility>> CreateFacility([FromBody] Facility facility)
		{
			_context.Facilitys.Add(facility);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetFacilityByFacilityId", new { FacilityId = facility.FacilityId }, facility);
		}

		// PUT: api/Addons/UpdateFacility/1
		[HttpPut("UpdateFacility/{FacilityId}")]
		public async Task<IActionResult> UpdateFacility(int FacilityId, [FromBody] Facility facility)
		{
			if (FacilityId != facility.FacilityId || facility == null)
			{
				return BadRequest();
			}

			_context.Entry(facility).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FacilityExists(FacilityId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetFacilityByFacilityId", new { FacilityId = facility.FacilityId }, facility);
		}
		private bool FacilityExists(int FacilityId)
		{
			return _context.Facilitys.Any(e => e.FacilityId == FacilityId);
		}

		/// <summary>
		/// Post the Health check.
		/// </summary>
		/// <returns>Add new Health check.</returns>
		// POST: api/Addons/CreateHealthCheck
		[HttpPost("CreateHealthCheck")]
		public async Task<ActionResult<HealthCheck>> CreateHealthCheck([FromBody] HealthCheck healthCheck)
		{
			_context.HealthChecks.Add(healthCheck);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetHealthCheckByHealthCheckId", new { HealthCheckId = healthCheck.HealthCheckId }, healthCheck);
		}

		// GET: api/Addons/GetHealthCheckByHealthCheckId/1
		[HttpGet("GetHealthCheckByHealthCheckId/{HealthCheckID}")]
		public async Task<ActionResult<HealthCheck>> GetHealthCheckByHealthCheckId(int HealthCheckID)
		{
			var healthCheck = await _context.HealthChecks.FirstOrDefaultAsync(d => d.HealthCheckId == HealthCheckID);

			if (healthCheck == null)
			{
				return NotFound();
			}

			return healthCheck;
		}

		/// <summary>
		/// Gets the list of all Health Check.
		/// </summary>
		/// <returns>The list of Health Check.</returns>
		// GET: api/Addons/GetHealthCheckByListingId/1
		[HttpGet("GetHealthCheckByListingId/{ListingID}")]
		public async Task<ActionResult<HealthCheck>> GetHealthCheckByListingId(int ListingID)
		{
			var healthChecks = await _context.HealthChecks.FirstOrDefaultAsync(d => d.ListingId == ListingID);

			if (healthChecks == null)
			{
				return NotFound();
			}

			return healthChecks;
		}

		// PUT: api/Addons/UpdateHealthCheck/1
		[HttpPut("UpdateHealthCheck/{HealthCheckId}")]
		public async Task<IActionResult> UpdateHealthCheck(int HealthCheckId, [FromBody] HealthCheck healthCheck)
		{
			if (HealthCheckId != healthCheck.HealthCheckId || healthCheck == null)
			{
				return BadRequest();
			}

			_context.Entry(healthCheck).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!HealthCheckExists(HealthCheckId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetHealthCheckByHealthCheckId", new { HealthCheckId = healthCheck.HealthCheckId }, healthCheck);
		}
		private bool HealthCheckExists(int HealthCheckId)
		{
			return _context.HealthChecks.Any(e => e.HealthCheckId == HealthCheckId);
		}

		/// <summary>
		/// Post the Green building check.
		/// </summary>
		/// <returns>Add new Green building check.</returns>
		// POST: api/Addons/CreateGreenBuildingCheck
		[HttpPost("CreateGreenBuildingCheck")]
		public async Task<ActionResult<GreenBuildingCheck>> CreateGreenBuildingCheck([FromBody] GreenBuildingCheck greenBuildingCheck)
		{
			_context.GreenBuildingChecks.Add(greenBuildingCheck);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetGreenBuildingCheckByGreenBuildingCheckId", new { GreenBuildingCheckId = greenBuildingCheck.GreenBuildingCheckId }, greenBuildingCheck);
		}

		// GET: api/Addons/GetGreenBuildingCheckByGreenBuildingCheckId/1
		[HttpGet("GetGreenBuildingCheckByGreenBuildingCheckId/{GreenBuildingCheckID}")]
		public async Task<ActionResult<GreenBuildingCheck>> GetGreenBuildingCheckByGreenBuildingCheckId(int GreenBuildingCheckID)
		{
			var greenBuilding = await _context.GreenBuildingChecks.FirstOrDefaultAsync(d => d.GreenBuildingCheckId == GreenBuildingCheckID);

			if (greenBuilding == null)
			{
				return NotFound();
			}

			return greenBuilding;
		}

		/// <summary>
		/// Gets the list of all Green Bjuilding Check.
		/// </summary>
		/// <returns>The list of Green Bjuilding Check.</returns>
		// GET: api/Addons/GetGreenBuildingCheckByListingId/1
		[HttpGet("GetGreenBuildingCheckByListingId/{ListingID}")]
		public async Task<ActionResult<GreenBuildingCheck>> GetGreenBuildingCheckByListingId(int ListingID)
		{
			var greenBuilding = await _context.GreenBuildingChecks.FirstOrDefaultAsync(d => d.ListingId == ListingID);

			if (greenBuilding == null)
			{
				return NotFound();
			}

			return greenBuilding;
		}

		// PUT: api/Addons/UpdateGreenBuildingCheck/1
		[HttpPut("UpdateGreenBuildingCheck/{GreenBuildingCheckID}")]
		public async Task<IActionResult> UpdateGreenBuildingCheck(int GreenBuildingCheckID, [FromBody] GreenBuildingCheck greenBuildingCheck)
		{
			if (GreenBuildingCheckID != greenBuildingCheck.GreenBuildingCheckId || greenBuildingCheck == null)
			{
				return BadRequest();
			}

			_context.Entry(greenBuildingCheck).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!GreenBuildingCheckExists(GreenBuildingCheckID))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetGreenBuildingCheckByGreenBuildingCheckId", new { GreenBuildingCheckId = greenBuildingCheck.GreenBuildingCheckId }, greenBuildingCheck);
		}
		private bool GreenBuildingCheckExists(int GreenBuildingCheckID)
		{
			return _context.GreenBuildingChecks.Any(e => e.GreenBuildingCheckId == GreenBuildingCheckID);
		}
	}
}