using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using HiSpaceListingService.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiSpaceListingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : ControllerBase
    {
		private readonly HiSpaceListingContext _context;

		public ListingController(HiSpaceListingContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Gets the list of all Listings by userId.
		/// </summary>
		/// <returns>The list of Listings.</returns>
		// GET: api/Listing/GetListingsByUserId/1
		//[HttpGet]
		//[Route("GetListingsByUserId/{UserId}")]
		//public async Task<ActionResult<IEnumerable<Listing>>> GetListingsByUserId(int UserId)
		//{
		//	return await _context.Listings.Where(d => d.UserId == UserId).ToListAsync();
		//}
		[HttpGet]
		[Route("GetListingsByUserId/{UserId}")]
		public async Task<ActionResult<IEnumerable<ListingTableResponse>>> GetListingsByUserId(int UserId)
		{
			List<ListingTableResponse> listingTable = new List<ListingTableResponse>();
			var listings = await _context.Listings.Where(d => d.UserId == UserId).ToListAsync();

			foreach (var item in listings)
			{
				ListingTableResponse lst = new ListingTableResponse();

				lst.Listings = new Listing();
				lst.Listings = item;

				lst.GBC = _context.GreenBuildingChecks.SingleOrDefault(d => d.ListingId == item.ListingId);
				lst.TotalHealthCheck = _context.HealthChecks.Where(d => d.ListingId == item.ListingId).Count();
				lst.TotalGreenBuildingCheck = _context.GreenBuildingChecks.Where(d => d.ListingId == item.ListingId).Count();
				lst.TotalWorkingHours = _context.WorkingHourss.Where(d => d.ListingId == item.ListingId).Count();
				lst.TotalListingImages = _context.ListingImagess.Where(d => d.ListingId == item.ListingId).Count();
				lst.TotalAmenities = _context.Amenitys.Where(d => d.ListingId == item.ListingId).Count();
				lst.TotalFacilities = _context.Facilitys.Where(d => d.ListingId == item.ListingId).Count();
				lst.TotalProjects = _context.REProfessionalMasters.Where(d => d.ListingId == item.ListingId).Count();

				listingTable.Add(lst);
			}

			return listingTable;
		}
		/// <summary>
		/// Gets the list of all Listings.
		/// </summary>
		/// <returns>The list of Listings.</returns>
		// GET: api/Listing/GetListings
		[HttpGet]
		[Route("GetListings")]
		public async Task<ActionResult<IEnumerable<Listing>>> GetListings()
		{
			return await _context.Listings.ToListAsync();
		}

		/// <summary>
		/// Gets the Listing by ListingId.
		/// </summary>
		/// <returns>The Listing by ListingId.</returns>
		// GET: api/Listing/GetListingByListingId/1
		//[HttpGet("GetListingByListingId/{ListingId}")]
		[HttpGet]
		[Route("GetListingByListingId/{ListingId}")]
		public async Task<ActionResult<Listing>> GetListingByListingId(int ListingId)
		{
			var listing = await _context.Listings.FindAsync(ListingId);

			if (listing == null)
			{
				return NotFound();
			}

			return listing;
		}

		/// <summary>
		/// Add the Listing.
		/// </summary>
		/// <returns>The Listing by ListingId.</returns>
		// POST: api/Listing/AddListing
		[HttpPost]
		[Route("AddListing")]
		public async Task<ActionResult<Listing>> AddListing([FromBody] Listing listing)
		{
			//listing.CreatedDateTime = DateTime.Now;

			_context.Listings.Add(listing);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetListingByListingId", new { ListingId = listing.ListingId }, listing);
		}

		/// <summary>
		/// Update the Listing by ListingId.
		/// </summary>
		/// <returns>The Listing by ListingId.</returns>
		// PUT: api/Listing/UpdateListingByListingId/1
		[HttpPut]
		[Route("UpdateListingByListingId/{ListingId}")]
		public async Task<IActionResult> UpdateListingByListingId(int ListingId, [FromBody]  Listing listing)
		{
			if (ListingId != listing.ListingId || listing == null)
			{
				return BadRequest();
			}

			using (var trans = _context.Database.BeginTransaction())
			{
				try
				{
					_context.Entry(listing).State = EntityState.Modified;

					try
					{
						await _context.SaveChangesAsync();
					}
					catch (DbUpdateConcurrencyException)
					{
						if (!ListingExists(ListingId))
						{
							return NotFound();
						}
						else
						{
							throw;
						}
					}

					trans.Commit();
				}
				catch (Exception err)
				{
					trans.Rollback();
				}
			}

			return NoContent();
		}

		private bool ListingExists(int ListingId)
		{
			return _context.Listings.Any(e => e.ListingId == ListingId);
		}

		/// <summary>
		/// Delete the Listing by ListingId.
		/// </summary>
		/// <returns>The Listings list.</returns>
		// POST: api/Listing/DeleteListing
		//[HttpPost]
		//[Route("DeleteListing/{ListingId}")]
		//public async Task<ActionResult<IEnumerable<Listing>>> DeleteListing(int ListingId)
		//{
		//	var listing = await _context.Listings.FindAsync(ListingId);
		//	_context.Listings.Remove(listing);
		//	await _context.SaveChangesAsync();
		//	return CreatedAtAction("GetUsers", listing);
		//}
		[HttpPost]
		[Route("DeleteListing/{ListingId}")]
		public async Task<ActionResult<Listing>> DeleteListing(int ListingId)
		{
			var listing = await _context.Listings.FindAsync(ListingId);
			if (listing == null)
			{
				return NotFound();
			}
			_context.Listings.Remove(listing);
			await _context.SaveChangesAsync();

			var amenities = await _context.Amenitys.Where(m => m.ListingId == ListingId).ToListAsync();
			foreach (var item in amenities)
			{
				_context.Amenitys.Remove(item);
			}
			await _context.SaveChangesAsync();

			var facilities = await _context.Facilitys.Where(m => m.ListingId == ListingId).ToListAsync();
			foreach (var item in facilities)
			{
				_context.Facilitys.Remove(item);
			}
			await _context.SaveChangesAsync();

			var listingImages = await _context.ListingImagess.Where(m => m.ListingId == ListingId).ToListAsync();
			foreach (var item in listingImages)
			{
				_context.ListingImagess.Remove(item);
			}
			await _context.SaveChangesAsync();

			var workingHours = await _context.WorkingHourss.Where(m => m.ListingId == ListingId).FirstOrDefaultAsync();
			_context.WorkingHourss.Remove(workingHours);
			await _context.SaveChangesAsync();

			var healthCheck = await _context.HealthChecks.Where(m => m.ListingId == ListingId).FirstOrDefaultAsync();
			_context.HealthChecks.Remove(healthCheck);
			await _context.SaveChangesAsync();

			var greenBuildingCheck = await _context.GreenBuildingChecks.Where(m => m.ListingId == ListingId).FirstOrDefaultAsync();
			_context.GreenBuildingChecks.Remove(greenBuildingCheck);
			await _context.SaveChangesAsync();

			var reProfessionalMaster = await _context.REProfessionalMasters.Where(m => m.ListingId == ListingId).ToListAsync();
			foreach (var item in reProfessionalMaster)
			{
				_context.REProfessionalMasters.Remove(item);
			}
			await _context.SaveChangesAsync();

			return listing;
		}

		// GET: api/Addons/GetWoringHoursByWoringHoursID/1
		[HttpGet("GetWoringHoursByWoringHoursID/{WoringHoursID}")]
		public async Task<ActionResult<WorkingHours>> GetWoringHoursByWoringHoursID(int WoringHoursID)
		{
			var workingHours = await _context.WorkingHourss.FindAsync(WoringHoursID);

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

			return CreatedAtAction("GetWoringHoursByWoringHoursID", new { WoringHoursID = workingHours.WorkingHoursId }, workingHours);
		}

		/// <summary>
		/// Get property list
		/// </summary>
		/// <returns>The list of Properties.</returns>
		// GET: api/Listing/GetPropertyList
		[HttpGet("GetPropertyList")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetPropertyList()
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			var properties = await _context.Listings.Where(m => m.Status == true).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			foreach(var item in properties)
			{
				PropertyDetailResponse property = new PropertyDetailResponse();
				property.SpaceListing = item;
				property.SpaceUser = _context.Users.SingleOrDefault(d => d.UserId == item.UserId);
				property.AvailableAmenities = (from PA in _context.Amenitys where PA.ListingId == item.ListingId && PA.Status == true select new Amenity() { AmenityId = PA.AmenityId, Name = PA.Name}).ToList().Count();
				property.AvailableFacilities = (from PF in _context.Facilitys where PF.ListingId == item.ListingId && PF.Status == true select new Facility() {  FacilityId = PF.FacilityId, Name = PF.Name }).ToList().Count();
				property.AvailableProjects = (from PF in _context.REProfessionalMasters where PF.ListingId == item.ListingId && PF.Status == true select new REProfessionalMaster() { REProfessionalMasterId = PF.REProfessionalMasterId, ProjectName = PF.ProjectName }).ToList().Count();
				property.ListingImagesList = _context.ListingImagess.Where(d => d.ListingId == item.ListingId).ToList();
				property.AvailableHealthCheck = (from AHC in _context.HealthChecks where AHC.ListingId == item.ListingId && AHC.Status == true select new HealthCheck() { HealthCheckId = AHC.HealthCheckId }).ToList().Count();
				
				property.AvailableGreenBuildingCheck = (from GBC in _context.GreenBuildingChecks where GBC.ListingId == item.ListingId && GBC.Status == true select new GreenBuildingCheck() { GreenBuildingCheckId = GBC.GreenBuildingCheckId }).ToList().Count();
				vModel.Add(property);
			}
			
			return vModel;
		}

		/// <summary>
		/// Get property list
		/// </summary>
		/// <returns>The list of Properties.</returns>
		// GET: api/Listing/GetPropertyListByUserID
		[HttpGet("GetPropertyListByUserID/{UserID}")]
		public async Task<ActionResult<PropertyListListerResponse>> GetPropertyListByUserID(int UserID)
		{
			PropertyListListerResponse vModel = new PropertyListListerResponse();
			var properties = await _context.Listings.Where(m => m.Status == true && m.UserId == UserID).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			foreach (var item in properties)
			{
				PropertyDetailResponse property = new PropertyDetailResponse();
				property.SpaceListing = item;
				property.SpaceUser = _context.Users.SingleOrDefault(d => d.UserId == item.UserId);
				property.AvailableAmenities = (from PA in _context.Amenitys where PA.ListingId == item.ListingId && PA.Status == true select new Amenity() { AmenityId = PA.AmenityId, Name = PA.Name }).ToList().Count();
				property.AvailableFacilities = (from PF in _context.Facilitys where PF.ListingId == item.ListingId && PF.Status == true select new Facility() { FacilityId = PF.FacilityId, Name = PF.Name }).ToList().Count();
				property.AvailableProjects = (from PF in _context.REProfessionalMasters where PF.ListingId == item.ListingId && PF.Status == true select new REProfessionalMaster() { REProfessionalMasterId = PF.REProfessionalMasterId, ProjectName = PF.ProjectName }).ToList().Count();
				property.ListingImagesList = _context.ListingImagess.Where(d => d.ListingId == item.ListingId).ToList();
				property.AvailableHealthCheck = (from AHC in _context.HealthChecks where AHC.ListingId == item.ListingId && AHC.Status == true select new HealthCheck() { HealthCheckId = AHC.HealthCheckId }).ToList().Count();
				property.AvailableGreenBuildingCheck = (from GBC in _context.GreenBuildingChecks where GBC.ListingId == item.ListingId && GBC.Status == true select new GreenBuildingCheck() { GreenBuildingCheckId = GBC.GreenBuildingCheckId }).ToList().Count();

				// Adding additional detail
				property.GreenBuildingCheckDetails = _context.GreenBuildingChecks.SingleOrDefault(d => d.ListingId == item.ListingId);
				vModel.PropertyDetail.Add(property);
			}
			vModel.SpaceUser = _context.Users.SingleOrDefault(m => m.UserId == UserID);
			vModel.PropertyCount = _context.Listings.Where(d => d.UserId == UserID && d.Status == true).ToList().Count();

			return vModel;
		}

		//GET: api/Listing/GetPropertyDetailByListingID/1
		[HttpGet("GetPropertyDetailByListingID/{ListingID}")]
		public async Task<ActionResult<PropertyDetailViewModelResponse>> GetPropertyDetailByListingID(int ListingID)
		{
			PropertyDetailViewModelResponse propertyDetails = new PropertyDetailViewModelResponse();
			var property = await _context.Listings.SingleOrDefaultAsync(m => m.ListingId == ListingID);
			if(property == null)
			{
				return NotFound();
			}

			propertyDetails.Listing = property;
			propertyDetails.User = _context.Users.SingleOrDefault(d => d.UserId == property.UserId);
			propertyDetails.WorkingHours = _context.WorkingHourss.SingleOrDefault(d => d.ListingId == property.ListingId);
			propertyDetails.Amenities = _context.Amenitys.Where(d => d.Status == true && d.ListingId == property.ListingId).ToList();
			propertyDetails.Facilities = _context.Facilitys.Where(d => d.Status == true && d.ListingId == property.ListingId).ToList();
			propertyDetails.ListingImages = _context.ListingImagess.Where(d => d.Status == true && d.ListingId == property.ListingId).ToList();
			propertyDetails.REProfessionalMasters = _context.REProfessionalMasters.Where(d => d.Status == true && d.ListingId == property.ListingId).ToList();
			var propertyCount = _context.Listings.Where(d => d.UserId == property.UserId && d.Status == true).ToList();
			propertyDetails.ListerPropertyCount = propertyCount.Count();
			propertyDetails.HealthCheck = _context.HealthChecks.SingleOrDefault(d => d.ListingId == property.ListingId);
			propertyDetails.GreenBuildingCheck = _context.GreenBuildingChecks.SingleOrDefault(d => d.ListingId == property.ListingId);
			// Adding additional field
			propertyDetails.ProjectCount = _context.REProfessionalMasters.Where(d => d.ListingId == property.ListingId).Count();
			//foreach(var amenity in amenities)
			//{

			//}
			return propertyDetails;
		}

		[Route("GetAllPropertyList")]
		[HttpGet]
		public ActionResult<List<Listing>> GetAllPropertyList()
		{
			List<Listing> l = new List<Listing>();
			l = (from r in _context.Listings
				 select r).ToList();
			if (l == null)
			{
				return NotFound();
			}

			return l;

		}

		[Route("GetAllPropertyListCommercialAndCoworking")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetAllPropertyListCommercialAndCoworking()
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			var properties = await _context.Listings.Where(m => m.Status == true && m.ListingType != "RE-Professional").OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			foreach (var item in properties)
			{
				PropertyDetailResponse property = new PropertyDetailResponse();
				property.SpaceListing = item;
				property.SpaceUser = _context.Users.SingleOrDefault(d => d.UserId == item.UserId);
				property.AvailableAmenities = (from PA in _context.Amenitys where PA.ListingId == item.ListingId && PA.Status == true select new Amenity() { AmenityId = PA.AmenityId, Name = PA.Name }).ToList().Count();
				property.AvailableFacilities = (from PF in _context.Facilitys where PF.ListingId == item.ListingId && PF.Status == true select new Facility() { FacilityId = PF.FacilityId, Name = PF.Name }).ToList().Count();
				property.AvailableProjects = (from PF in _context.REProfessionalMasters where PF.ListingId == item.ListingId && PF.Status == true select new REProfessionalMaster() { REProfessionalMasterId = PF.REProfessionalMasterId, ProjectName = PF.ProjectName }).ToList().Count();
				property.ListingImagesList = _context.ListingImagess.Where(d => d.ListingId == item.ListingId).ToList();
				property.AvailableHealthCheck = (from AHC in _context.HealthChecks where AHC.ListingId == item.ListingId && AHC.Status == true select new HealthCheck() { HealthCheckId = AHC.HealthCheckId }).ToList().Count();

				property.AvailableGreenBuildingCheck = (from GBC in _context.GreenBuildingChecks where GBC.ListingId == item.ListingId && GBC.Status == true select new GreenBuildingCheck() { GreenBuildingCheckId = GBC.GreenBuildingCheckId }).ToList().Count();
				vModel.Add(property);
			}

			return vModel;
		}

		[Route("GetAllOperatorList")]
		[HttpGet]
		public ActionResult<List<PropertyOperatorResponse>> GetAllOperatorList()
		{
			List<PropertyOperatorResponse> listoperators = new List<PropertyOperatorResponse>();

			var users = (from u in _context.Users
						 where u.UserId != 0 && u.UserStatus == "Completed"
						 select u).ToList();
			if (users == null)
			{
				return NotFound();
			}

			foreach (var item in users)
			{
				PropertyOperatorResponse op = new PropertyOperatorResponse();

				op.Operator = new User();
				op.Operator = item;

				op.TotalCommercial = _context.Listings.Where(d => d.ListingType == "Commercial" && d.UserId == item.UserId).Count();
				op.TotalCoWorking = _context.Listings.Where(d => d.ListingType == "Co-Working" && d.UserId == item.UserId).Count();
				op.TotalREProfessional = _context.Listings.Where(d => d.ListingType == "RE-Professional" && d.UserId == item.UserId).Count();
				//geting roles
				var GetListingIdByUsingUser = (from l in _context.Listings
									where (l.UserId == item.UserId && l.Status == true && l.ListingType == "RE-Professional")
									select new
									{
										l.ListingId
									}).ToList();
				if (GetListingIdByUsingUser != null)
				{
					op.roles = new List<string>();
					foreach (var id in GetListingIdByUsingUser)
					{
						op.roles = _context.REProfessionalMasters.Where(d => d.ListingId == id.ListingId).Select(d => d.ProjectRole).Distinct().ToList();
					}
				}
				else
				{
					op.roles = null;
				}
				//geting linked re-prof
				var linkedREProf = (from l in _context.Listings
									from r in _context.REProfessionalMasters
									where (l.UserId == item.UserId && 
									(l.ListingType == "Commercial" || l.ListingType == "Co-Working") && 
									(l.CMCW_ReraId == r.PropertyReraId
									 || l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_GatNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber))
									select new
									{
										l.ListingId,
										r.REProfessionalMasterId,
										l.UserId,
										r.ProjectRole,
										r.ProjectName,
										r.ImageUrl
									}).ToList();

				op.LinkedREProf = new List<LinkedREPRofessionals>();
				foreach (var linked in linkedREProf)
				{
					LinkedREPRofessionals REProf = new LinkedREPRofessionals();
					var GetListingIdOnReProfessional = _context.REProfessionalMasters.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId).Select(d => d.ListingId).First();
					REProf.Property_ListingId = linked.ListingId;
					REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
					REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
					REProf.UserId = linked.UserId;
					REProf.ProjectRole = linked.ProjectRole;
					REProf.ProjectName = linked.ProjectName;
					REProf.ImageUrl = linked.ImageUrl;
					REProf.REFirstName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_FirstName).First();
					REProf.RELastName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_LastName).First();
					op.LinkedREProf.Add(REProf);
				}

				op.LinkedREProfCount = op.LinkedREProf.Count;
				listoperators.Add(op);
			}

			return listoperators;

		}

		[Route("GetAllPeopleList")]
		[HttpGet]
		public ActionResult<List<PropertyPeopleResponse>> GetAllPeopleList()
		{
			List<PropertyPeopleResponse> ppl = new List<PropertyPeopleResponse>();
			var users = (from r in _context.Listings
						 join a in _context.Users on r.UserId equals a.UserId
						 where r.ListingType == "RE-Professional"
						 select new
						 {
							 a,
							 r.ListingId
						 }).ToList();
			if (users == null)
			{
				return NotFound();
			}

			foreach (var item in users)
			{
				PropertyPeopleResponse p = new PropertyPeopleResponse();

				p.Operator = new User();
				p.Operator = item.a;
				//p.Operator.UserId = item.a.UserId;
				//p.Operator.UserType = item.a.UserType;
				//p.ListingId = item.ListingId;
				p.Listing = _context.Listings.SingleOrDefault(d => d.ListingId == item.ListingId);
				p.Projects = (from r in _context.REProfessionalMasters
							  where r.ListingId == item.ListingId
							  select r).ToList();

				p.TotalProjects = p.Projects.Count();
				ppl.Add(p);
			}

			return ppl;

		}
		[HttpGet("GetPeopleDetailByListingID/{ListingID}")]
		public async Task<ActionResult<PeopleDetailResponse>> GetPeopleDetailByListingID(int ListingID)
		{
			PeopleDetailResponse peopleDetails = new PeopleDetailResponse();
			var property = await _context.Listings.SingleOrDefaultAsync(m => m.ListingId == ListingID && m.ListingType == "RE-Professional");
			if (property == null)
			{
				return NotFound();
			}

			peopleDetails.Listing = property;
			peopleDetails.User = _context.Users.SingleOrDefault(d => d.UserId == property.UserId);
			peopleDetails.REProfessionalMasters = _context.REProfessionalMasters.Where(d => d.ListingId == property.ListingId).ToList();
			peopleDetails.ProjectCount = _context.REProfessionalMasters.Where(d => d.ListingId == property.ListingId).Count();

			return peopleDetails;
		}
	}
}