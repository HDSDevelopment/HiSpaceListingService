using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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


		//Get Enquiry list
		[HttpGet]
		[Route("GetEnquiryListByUserIdAndUserType/{UserId}/{UserType}/{Type}")]
		public async Task<ActionResult<IEnumerable<EnquiryListResponse>>> GetEnquiryListByUserIdAndUserType(int UserId, int UserType, string Type)
		{
			List<EnquiryListResponse> enquiryTable = new List<EnquiryListResponse>();
			if(UserType == 2 || (UserType == 1 && Type == "My"))
			{
				var enquiries = await _context.Enquiries.Where(d => d.Sender_UserId == UserId).ToListAsync();

				foreach (var item in enquiries)
				{
					EnquiryListResponse lst = new EnquiryListResponse();

					lst.Enquiry = new Enquiry();
					lst.Enquiry = item;
					lst.PropertyName = _context.Listings.Where(d => d.ListingId == item.ListingId).Select(d => d.Name).First();
					lst.Type = _context.Listings.Where(d => d.ListingId == item.ListingId).Select(d => d.ListingType).First();
					lst.OperatorName = _context.Users.Where(d => d.UserId == item.Listing_UserId).Select(d => d.CompanyName).First();

					enquiryTable.Add(lst);
				}
			}
			else if(UserType == 1 && Type == "User")
			{
				var enquiries = await _context.Enquiries.Where(d => d.Listing_UserId == UserId).ToListAsync();

				foreach (var item in enquiries)
				{
					EnquiryListResponse lst = new EnquiryListResponse();

					lst.Enquiry = new Enquiry();
					lst.Enquiry = item;
					lst.PropertyName = _context.Listings.Where(d => d.ListingId == item.ListingId).Select(d => d.Name).First();
					lst.Type = _context.Listings.Where(d => d.ListingId == item.ListingId).Select(d => d.ListingType).First();
					lst.OperatorName = _context.Users.Where(d => d.UserId == item.Listing_UserId).Select(d => d.CompanyName).First();

					enquiryTable.Add(lst);
				}
			}
			

			return enquiryTable;
		}

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
		//[HttpPost]
		//[Route("AddListing")]
		//public async Task<ActionResult<Listing>> AddListing([FromBody] Listing listing)
		//{
		//	//listing.CreatedDateTime = DateTime.Now;

		//	_context.Listings.Add(listing);
		//	await _context.SaveChangesAsync();

		//	return CreatedAtAction("GetListingByListingId", new { ListingId = listing.ListingId }, listing);
		//}
		[HttpPost]
		[Route("AddListing")]
		public async Task<ActionResult<Listing>> AddListing([FromBody] Listing listing)
		{
			//listing.CreatedDateTime = DateTime.Now;

			_context.Listings.Add(listing);
			await _context.SaveChangesAsync();

			if ((listing.ListingType == "Commercial") || (listing.ListingType == "Co-Working") && (_context.Listings.Any(e => e.UserId == listing.UserId && e.ListingType == "RE-Professional") == false))
			{
				Listing autoEntry = new Listing();
				autoEntry.ListingType = "RE-Professional";
				autoEntry.UserId = listing.UserId;

				_context.Listings.Add(autoEntry);
				await _context.SaveChangesAsync();
			}

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

		/// <summary>
		/// Get property list
		/// </summary>
		/// <returns>The list of Properties.</returns>
		// GET: api/Listing/GetPropertyListByUserIDAndListingID
		[HttpGet("GetPropertyListByUserIDAndListingID/{UserID}/{ListingID}")]
		public async Task<ActionResult<PropertyListListerResponse>> GetPropertyListByUserIDAndListingID(int UserID, int ListingID)
		{
			PropertyListListerResponse vModel = new PropertyListListerResponse();
			var properties = await _context.Listings.Where(m => m.Status == true && m.UserId == UserID && m.ListingId == ListingID).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			foreach (var item in properties)
			{
				PropertyDetailResponse property = new PropertyDetailResponse();
				property.SpaceListing = item;
				//property.SpaceUser = _context.Users.SingleOrDefault(d => d.UserId == item.UserId);
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

			//geting linked re-prof
			var linkedREProf = (from l in _context.Listings
								from r in _context.REProfessionalMasters
								where (l.UserId == UserID &&
								(l.ListingType == "Commercial" || l.ListingType == "Co-Working") &&
								(l.CMCW_ReraId == r.PropertyReraId
								 || l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_GatNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber) && (r.LinkingStatus == "Approved"))
								select new
								{
									l.ListingId,
									r.REProfessionalMasterId,
									l.UserId,
									r.ProjectRole,
									r.ProjectName,
									r.OperatorName,
									r.ImageUrl,
									r.LinkingStatus
								}).ToList();

			vModel.LinkedREPRofessionals = new List<LinkedREPRofessionals>();
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
				REProf.OperatorName = linked.OperatorName;
				REProf.LinkingStatus = linked.LinkingStatus;
				REProf.ImageUrl = linked.ImageUrl;
				REProf.REFirstName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_FirstName).First();
				REProf.RELastName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_LastName).First();
				vModel.LinkedREPRofessionals.Add(REProf);
			}

			vModel.LinkedREProfCount = vModel.LinkedREPRofessionals.Count;

			return vModel;
		}

		// GET: api/Listing/GetLinkedReProfessionalListByUserIDForApproval
		[Route("GetLinkedReProfessionalListByUserIDForApproval/{UserID}")]
		[HttpGet]
		public async Task<ActionResult<List<LinkedREPRofessionals>>> GetLinkedReProfessionalListByUserIDForApproval(int UserID)
		{
			List<LinkedREPRofessionals> vModel = new List<LinkedREPRofessionals>();
			var properties = await _context.Listings.Where(m => m.Status == true && m.UserId == UserID).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			//geting linked re-prof
			var linkedREProf = (from l in _context.Listings
								from r in _context.REProfessionalMasters
								where (l.UserId == UserID &&
								(l.ListingType == "Commercial" || l.ListingType == "Co-Working") &&
								(((l.CMCW_ReraId != null && r.PropertyReraId != null) && (l.CMCW_ReraId == r.PropertyReraId))
								|| ((l.CMCW_CTSNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_GatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_GatNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_MilkatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_PlotNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_SurveyNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_PropertyTaxBillNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber))
								 ))
								select new
								{
									l.ListingId,
									r.REProfessionalMasterId,
									l.UserId,
									r.ProjectRole,
									r.ProjectName,
									r.OperatorName,
									r.ImageUrl,
									r.LinkingStatus

								}).ToList();
			foreach (var linked in linkedREProf)
			{
				LinkedREPRofessionals REProf = new LinkedREPRofessionals();
				var GetListingIdOnReProfessional = _context.REProfessionalMasters.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId).Select(d => d.ListingId).First();
				REProf.Property_ListingId = linked.ListingId;
				REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
				var GetREListing_UserId = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.UserId).First();
				REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
				REProf.UserId = linked.UserId;
				REProf.ProjectRole = linked.ProjectRole;
				REProf.ProjectName = linked.ProjectName;
				REProf.OperatorName = linked.OperatorName;
				REProf.LinkingStatus = linked.LinkingStatus;
				REProf.ImageUrl = linked.ImageUrl;
				REProf.RE_UserName = _context.Users.Where(d => d.UserId == GetREListing_UserId).Select(d => d.CompanyName).First();
				REProf.RE_Address = _context.Users.Where(d => d.UserId == GetREListing_UserId).Select(d => d.Address).First();
				//REProf. = linked.ImageUrl;
				REProf.REFirstName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_FirstName).First();
				REProf.RELastName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_LastName).First();
				vModel.Add(REProf);
			}
			return vModel;
		}

		// GET: api/Listing/GetPropertyAndLinkedReProfessionalListByUserID
		[Route("GetPropertyAndLinkedReProfessionalListByUserID/{UserID}")]
		[HttpGet]
		public async Task<ActionResult<PropertyListListerResponse>> GetPropertyAndLinkedReProfessionalListByUserID(int UserID)
		{
			PropertyListListerResponse vModel = new PropertyListListerResponse();
			var properties = await _context.Listings.Where(m => m.Status == true && m.UserId == UserID).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			foreach (var item in properties)
			{
				PropertyDetailResponse property = new PropertyDetailResponse();
				property.SpaceListing = item;
				//property.SpaceUser = _context.Users.SingleOrDefault(d => d.UserId == item.UserId);
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

			//geting linked re-prof
			var linkedREProf = (from l in _context.Listings
								from r in _context.REProfessionalMasters
								where (l.UserId == UserID &&
								(l.ListingType == "Commercial" || l.ListingType == "Co-Working") &&
								(l.CMCW_ReraId == r.PropertyReraId
								 || l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_GatNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber
								 || l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber) && (r.LinkingStatus == "Approved"))
								select new
								{
									l.ListingId,
									r.REProfessionalMasterId,
									l.UserId,
									r.ProjectRole,
									r.ProjectName,
									r.ImageUrl,
									r.OperatorName,
									r.LinkingStatus
								}).ToList();

			vModel.LinkedREPRofessionals = new List<LinkedREPRofessionals>();
			foreach (var linked in linkedREProf)
			{
				LinkedREPRofessionals REProf = new LinkedREPRofessionals();
				var GetListingIdOnReProfessional = _context.REProfessionalMasters.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId).Select(d => d.ListingId).First();
				REProf.Property_ListingId = linked.ListingId;
				REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
				REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
				REProf.UserId = linked.UserId;
				REProf.ProjectRole = linked.ProjectRole;
				REProf.OperatorName = linked.ProjectRole;
				REProf.LinkingStatus = linked.LinkingStatus;
				REProf.ProjectName = linked.ProjectName;
				REProf.ImageUrl = linked.ImageUrl;
				REProf.REFirstName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_FirstName).First();
				REProf.RELastName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_LastName).First();
				vModel.LinkedREPRofessionals.Add(REProf); 
			}

			vModel.LinkedREProfCount = vModel.LinkedREPRofessionals.Count;

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

		//GetPropertyListCommercialAndCoworking/searchCriteria?ListingType=&CMCW_PropertyFor= 
		[Route("GetPropertyListCommercialAndCoworking")]
		[HttpPost]
		public async Task<ActionResult<List<PropertyDetailResponse>>> GetPropertyListCommercialAndCoworking([FromBody] PropertySearchCriteria searchCriteria)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			IEnumerable<Listing> properties = await _context.Listings.ToListAsync();			

			if(searchCriteria.IsValidListingType())
				properties = from prop in properties
								where prop.ListingType == searchCriteria.ListingType
								orderby prop.CreatedDateTime descending
								select prop;

            if (properties != null && searchCriteria.IsValidCMCW_PropertyFor())
                properties = from prop in properties
							where prop.CMCW_PropertyFor == searchCriteria.CMCW_PropertyFor
							select prop;
                												

            if (properties != null && searchCriteria.IsValidCoworkingType())
                properties = from prop in properties
							where prop.CoworkingType == searchCriteria.CoworkingType
							select prop;

			if (properties != null && searchCriteria.IsValidCommercialType())
            	properties = from prop in properties
							where prop.CommercialType == searchCriteria.CommercialType
							select prop;																				

            if (properties != null && !string.IsNullOrEmpty(searchCriteria.Locality))
                properties = from prop in properties
							where prop.locality == searchCriteria.Locality
							select prop;

			if (properties != null && searchCriteria.IsValidPriceMin())
                properties = from prop in properties
							where prop.PriceMin >= searchCriteria.PriceMin
							select prop;

			if (properties != null && searchCriteria.IsValidPriceMax())
                properties = from prop in properties
							where prop.PriceMax <= searchCriteria.PriceMax
							select prop;
            
						if(properties == null)
					 			return BadRequest();
						 
			foreach (var item in properties)
			{
			PropertyDetailResponse property = new PropertyDetailResponse();
			property.SpaceListing = item;
		property.SpaceUser = await _context.Users.SingleOrDefaultAsync(d => d.UserId == item.UserId);
				
		List<Amenity> amenities = await _context.Amenitys.ToListAsync();			 
		property.AvailableAmenities = (from amenity in amenities
								where amenity.ListingId == item.ListingId && amenity.Status == true 
								select amenity)
								.Count();

		List<Facility> facilities = await _context.Facilitys.ToListAsync();
		property.AvailableFacilities = (from facility in facilities 
								where facility.ListingId == item.ListingId && facility.Status == true 
								select facility)
								.Count();
		
IEnumerable<REProfessionalMaster> professionals = await _context.REProfessionalMasters.ToListAsync();	
	property.AvailableProjects = (from professional in professionals
						where professional.ListingId == item.ListingId && professional.Status == true 
						select professional)					
						.Count();
	
IEnumerable<ListingImages> images = await _context.ListingImagess.ToListAsync();
	property.ListingImagesList = (from image in images
								where image.ListingId == item.ListingId
								select image)
								.ToList();

IEnumerable<HealthCheck> healthChecks = await _context.HealthChecks.ToListAsync();				
	property.AvailableHealthCheck = (from healthCheck in healthChecks 
						where healthCheck.ListingId == item.ListingId && healthCheck.Status == true 
						select healthCheck)															.Count();

IEnumerable<GreenBuildingCheck> greenBldingChecks = await _context.GreenBuildingChecks.ToListAsync();
	property.AvailableGreenBuildingCheck = (from GBC in greenBldingChecks 
										where GBC.ListingId == item.ListingId && GBC.Status == true 
										select GBC)
										.Count();

													//geting linked re-prof			
		var linkedREProf = (from r in _context.REProfessionalMasters
							where (r.PropertyReraId  == item.CMCW_ReraId
								 || r.PropertyAdditionalIdNumber == item.CMCW_CTSNumber
								 || r.PropertyAdditionalIdNumber == item.CMCW_MilkatNumber
								 || r.PropertyAdditionalIdNumber == item.CMCW_PlotNumber
								 || r.PropertyAdditionalIdNumber == item.CMCW_SurveyNumber
								 || r.PropertyAdditionalIdNumber == item.CMCW_PropertyTaxBillNumber
								 || r.PropertyAdditionalIdNumber == item.CMCW_GatNumber) && (r.LinkingStatus == "Approved")
								select new
								{
									item.ListingId,
									r.REProfessionalMasterId,
									item.UserId,
									r.ProjectRole,
									r.ProjectName,
									r.ImageUrl,
									r.OperatorName,
									r.LinkingStatus
								}).ToList();								

				property.LinkedREProf = new List<LinkedREPRofessionals>();
				foreach (var linked in linkedREProf)
				{
					LinkedREPRofessionals REProf = new LinkedREPRofessionals();
					var GetListingIdOnReProfessional = _context.REProfessionalMasters.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId).Select(d => d.ListingId).First();
					REProf.Property_ListingId = linked.ListingId;
					REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
					REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
					REProf.UserId = linked.UserId;
					REProf.ProjectRole = linked.ProjectRole;
					REProf.OperatorName = linked.OperatorName;
					REProf.LinkingStatus = linked.LinkingStatus;
					REProf.ProjectName = linked.ProjectName;
					REProf.ImageUrl = linked.ImageUrl;
					REProf.REFirstName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_FirstName).First();
					REProf.RELastName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_LastName).First();
					property.LinkedREProf.Add(REProf);
				}

				property.LinkedREProfCount = property.LinkedREProf.Count;
				vModel.Add(property);
			}
			if(vModel.Count > 0)
			return vModel;

			return NotFound();
		}

		[Route("GetAllOperatorList")]
		[HttpGet]
		public ActionResult<List<PropertyOperatorResponse>> GetAllOperatorList()
		{
			List<PropertyOperatorResponse> listoperators = new List<PropertyOperatorResponse>();

			var users = (from u in _context.Users
						 where u.UserType == 1 && u.Status == true
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
							   (((l.CMCW_ReraId != null && r.PropertyReraId != null) && (l.CMCW_ReraId == r.PropertyReraId))
							   || ((l.CMCW_CTSNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_GatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_GatNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_MilkatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_PlotNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_SurveyNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_PropertyTaxBillNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber))
								))
									select new
									{
										l.ListingId,
										r.REProfessionalMasterId,
										l.UserId,
										r.ProjectRole,
										r.ProjectName,
										r.ImageUrl,
										r.OperatorName,
										r.LinkingStatus
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
					REProf.OperatorName = linked.OperatorName;
					REProf.LinkingStatus = linked.LinkingStatus;
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
				//geting linked Operator
				var linkedOperator = (from l in _context.Listings
									from r in _context.REProfessionalMasters
									where (
									(l.ListingType == "Commercial" || l.ListingType == "Co-Working") &&
									(l.CMCW_ReraId == r.PropertyReraId
									 || l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_GatNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber
									 || l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber) && (r.LinkingStatus == "Approved") && (r.ListingId == p.Listing.ListingId))
									select new
									{
										l.ListingId,
										r.REProfessionalMasterId,
										l.UserId,
										r.ProjectRole,
										r.ProjectName,
										r.ImageUrl,
										r.OperatorName,
										r.LinkingStatus
									}).ToList();
				p.LinkedOpr = new List<LinkedOperators>();
				foreach (var linked in linkedOperator)
				{
					LinkedOperators Oper = new LinkedOperators();
					var GetListingIdOnReProfessional = _context.REProfessionalMasters.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId).Select(d => d.ListingId).First();
					//var GetListingIdOnpro = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.UserId).First();
					Oper.Property_ListingId = linked.ListingId;
					Oper.ReProfessional_ListingId = GetListingIdOnReProfessional;
					Oper.REProfessionalMasterId = linked.REProfessionalMasterId;
					Oper.UserId = linked.UserId;
					Oper.ProjectRole = linked.ProjectRole;
					Oper.ProjectName = linked.ProjectName;
					Oper.PropertyName = _context.Listings.Where(d => d.ListingId == linked.ListingId).Select(d => d.Name).First();
					Oper.CompanyName = _context.Users.Where(d => d.UserId == linked.UserId).Select(d => d.CompanyName).First();
					Oper.Doc_CompanyLogo = _context.Users.Where(d => d.UserId == linked.UserId).Select(d => d.Doc_CompanyLogo).First();

					p.LinkedOpr.Add(Oper);
				}

				ppl.Add(p);
			}

			return ppl;

		}

		//[HttpGet("GetPeopleDetailByListingID/{ListingID}")]
		//public async Task<ActionResult<PeopleDetailResponse>> GetPeopleDetailByListingID(int ListingID)
		//{
		//	PeopleDetailResponse peopleDetails = new PeopleDetailResponse();
		//	var property = await _context.Listings.SingleOrDefaultAsync(m => m.ListingId == ListingID && m.ListingType == "RE-Professional");
		//	if (property == null)
		//	{
		//		return NotFound();
		//	}

		//	peopleDetails.Listing = property;
		//	peopleDetails.User = _context.Users.SingleOrDefault(d => d.UserId == property.UserId);
		//	peopleDetails.REProfessionalMasters = _context.REProfessionalMasters.Where(d => d.ListingId == property.ListingId).ToList();
		//	peopleDetails.ProjectCount = _context.REProfessionalMasters.Where(d => d.ListingId == property.ListingId).Count();

		//	return peopleDetails;
		//}


		//User level Listing check
		//GET: api/Listing/UserLevelListApprove/1/1
		[HttpGet("UserLevelListApprove/{ListingId}/{Status}")]
		public ActionResult<bool> UserLevelListApprove(int ListingId, bool Status)
		{
			bool result = true;
			if (ListingId == 0)
			{
				result = false;
			}
			try
			{
				var listing = _context.Listings.SingleOrDefault(d => d.ListingId == ListingId);
				if (listing != null)
				{
					listing.Status = Status;
					_context.Entry(listing).State = EntityState.Modified;
					_context.SaveChangesAsync();
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				result = false;
			}
			return result;
		}


		//Admin level Listing check
		//GET: api/Listing/AdminLevelListApprove/1/1
		[HttpGet("AdminLevelListApprove/{ListingId}/{Status}")]
		public ActionResult<bool> AdminLevelListApprove(int ListingId, bool Status)
		{
			bool result = true;
			if (ListingId == 0)
			{
				result = false;
			}
			try
			{
				var listing = _context.Listings.SingleOrDefault(d => d.ListingId == ListingId);
				if (listing != null)
				{
					listing.AdminStatus = Status;
					_context.Entry(listing).State = EntityState.Modified;
					_context.SaveChangesAsync();
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				result = false;
			}
			return result;
		}

		//operator search Criteria
		[Route("GetOperatorList")]
		[HttpPost]
		public async Task<ActionResult<IEnumerable<PropertyOperatorResponse>>> GetOperatorList([FromBody] OperatorSearchCriteria searchCriteria)
		{			
			List<PropertyOperatorResponse> listoperators = new List<PropertyOperatorResponse>();

			IEnumerable<User> userGroup = await _context.Users.ToListAsync();
			
				userGroup = from user in userGroup 
							where user.UserType == 1 && user.Status == true
							select user;

			 if(userGroup!= null && !string.IsNullOrEmpty(searchCriteria.OperatorName))
			 	userGroup = from user in userGroup
			 				where user.CompanyName == searchCriteria.OperatorName
							select user;
			
				if(userGroup!= null && !string.IsNullOrEmpty(searchCriteria.CityName))
			 	userGroup = from user in userGroup
			 				where user.City == searchCriteria.CityName
							select user;			

			if(userGroup != null)
			{
			foreach (var item in userGroup)
			{
				PropertyOperatorResponse operatorResponse = new PropertyOperatorResponse();

				operatorResponse.Operator = new User();
				operatorResponse.Operator = item;

                    IEnumerable<Listing> listingsByUserID = await (from listing in _context.Listings
                                                                   where listing.UserId == item.UserId
                                                                   select listing).ToListAsync();

                    operatorResponse.TotalCommercial = (from listing in listingsByUserID
														where listing.ListingType == "Commercial"
														select listing)
														.Count();
				
				operatorResponse.TotalCoWorking = (from listing in listingsByUserID
													where listing.ListingType == "Co-Working"
													select listing)
													.Count();
				
				operatorResponse.TotalREProfessional = (from listing in listingsByUserID
														where listing.ListingType == "RE-Professional"
														select listing)
														.Count();
																	
				//getting roles
				IEnumerable<int> REProfessionalIDGroup = from listing in listingsByUserID
					where listing.Status == true  && listing.ListingType == "RE-Professional"
                    select listing.ListingId;

                    operatorResponse.roles = null;

				if (REProfessionalIDGroup != null)
				{
					operatorResponse.roles = new List<string>();
					foreach (var REProfessionalID in REProfessionalIDGroup)
					{
						operatorResponse.roles = await _context.REProfessionalMasters
												.Where(d => d.ListingId == REProfessionalID)
												.Select(d => d.ProjectRole)
												.Distinct()
												.ToListAsync();
					}
				}					
				
				//geting linked re-prof
				var linkedREProfGroup = (from listing in listingsByUserID
									from ReProf in _context.REProfessionalMasters
									where ((listing.ListingType == "Commercial" || listing.ListingType == "Co-Working") && 
									(listing.CMCW_ReraId == ReProf.PropertyReraId
									 || listing.CMCW_CTSNumber == ReProf.PropertyAdditionalIdNumber
									 || listing.CMCW_GatNumber == ReProf.PropertyAdditionalIdNumber
									 || listing.CMCW_MilkatNumber == ReProf.PropertyAdditionalIdNumber
									 || listing.CMCW_PlotNumber == ReProf.PropertyAdditionalIdNumber
									 || listing.CMCW_SurveyNumber == ReProf.PropertyAdditionalIdNumber
									 || listing.CMCW_PropertyTaxBillNumber == ReProf.PropertyAdditionalIdNumber) && (ReProf.LinkingStatus == "Approved"))
									select new
									{
										listing.ListingId,
										ReProf.REProfessionalMasterId,
										listing.UserId,
										ReProf.ProjectRole,
										ReProf.ProjectName,
										ReProf.ImageUrl,
										ReProf.OperatorName,
										ReProf.LinkingStatus
									}).ToList();
			
				operatorResponse.LinkedREProf = new List<LinkedREPRofessionals>();
				foreach (var linked in linkedREProfGroup)
				{
					LinkedREPRofessionals REProf = new LinkedREPRofessionals();
					var ListingIdOnReProfessional = _context.REProfessionalMasters
								.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId)
								.Select(d => d.ListingId)
								.First();
					REProf.Property_ListingId = linked.ListingId;
					REProf.ReProfessional_ListingId = ListingIdOnReProfessional;
					REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
					REProf.UserId = linked.UserId;
					REProf.ProjectRole = linked.ProjectRole;
					REProf.OperatorName = linked.OperatorName;
					REProf.LinkingStatus = linked.LinkingStatus;
					REProf.ProjectName = linked.ProjectName;
					REProf.ImageUrl = linked.ImageUrl;
					REProf.REFirstName = _context.Listings
										.Where(d => d.ListingId == ListingIdOnReProfessional)
										.Select(d => d.RE_FirstName)
										.First();
					REProf.RELastName = _context.Listings
										.Where(d => d.ListingId == ListingIdOnReProfessional)
										.Select(d => d.RE_LastName)
										.First();
					operatorResponse.LinkedREProf.Add(REProf);
				}

				operatorResponse.LinkedREProfCount = operatorResponse.LinkedREProf.Count;
				listoperators.Add(operatorResponse);
			}
		}
			if( listoperators.Count > 0) 
			 return listoperators; 
			
			return NotFound();
		}

		[Route("GetPeopleList")]
		[HttpPost]
		public async Task<ActionResult<List<PropertyPeopleResponse>>> GetPeopleList([FromBody] PeopleSearchCriteria searchCriteria)
		{
					IEnumerable<Listing> listings = await _context.Listings.ToListAsync();
					IEnumerable<User> operators = await _context.Users.ToListAsync();
					IEnumerable<REProfessionalMaster> projects;
					projects = await _context.REProfessionalMasters.ToListAsync();			

			var professionals = (from listing in listings
											join operatr in operators 
											on listing.UserId equals operatr.UserId
											where listing.ListingType == "RE-Professional"
											select new
											{
												operatr,
												listing
												});			

			if(professionals != null && !string.IsNullOrEmpty(searchCriteria.FirstName))
			professionals = from professional in professionals
								where professional.listing.RE_FirstName == searchCriteria.FirstName
								select professional;

			if(professionals != null && !string.IsNullOrEmpty(searchCriteria.LastName))
			professionals = from professional in professionals
								where professional.listing.RE_LastName == searchCriteria.LastName
								select professional;

			if(professionals != null && !string.IsNullOrEmpty(searchCriteria.Locality))
			professionals = from professional in professionals
								where professional.listing.locality == searchCriteria.Locality
								select professional;

			if (professionals == null)			
				return NotFound();			

			List<PropertyPeopleResponse> professionalsResponse = new List<PropertyPeopleResponse>();
			PropertyPeopleResponse professionalResponse;

			foreach (var item in professionals)
			{
				professionalResponse = new PropertyPeopleResponse();

				professionalResponse.Operator = new User();
				professionalResponse.Operator = item.operatr;
				//p.Operator.UserId = item.a.UserId;
				//p.Operator.UserType = item.a.UserType;
				//p.ListingId = item.ListingId;
				professionalResponse.Listing = (from listing in listings
										where listing.ListingId == item.listing.ListingId
										select listing)
										.SingleOrDefault();
				
				professionalResponse.Projects = (from project in projects
							  			where project.ListingId == item.listing.ListingId
							  			select project).ToList();

				professionalResponse.Roles = (from project in professionalResponse.Projects
											select project.ProjectRole)
											.Distinct()
											.ToList();				

				professionalResponse.TotalProjects = professionalResponse.Projects.Count();
				//geting linked Operator
				var linkedOperators = (from listing in listings
									from project in projects
									where (
									(listing.ListingType == "Commercial" || listing.ListingType == "Co-Working") &&
									(listing.CMCW_ReraId == project.PropertyReraId
									 || listing.CMCW_CTSNumber == project.PropertyAdditionalIdNumber
									 || listing.CMCW_GatNumber == project.PropertyAdditionalIdNumber
									 || listing.CMCW_MilkatNumber == project.PropertyAdditionalIdNumber
									 || listing.CMCW_PlotNumber == project.PropertyAdditionalIdNumber
									 || listing.CMCW_SurveyNumber == project.PropertyAdditionalIdNumber
									 || listing.CMCW_PropertyTaxBillNumber == project.PropertyAdditionalIdNumber) && (project.LinkingStatus == "Approved") && (project.ListingId == professionalResponse.Listing.ListingId))
									select new
									{
										listing.ListingId,
										project.REProfessionalMasterId,
										listing.UserId,
										project.ProjectRole,
										project.ProjectName,
										project.ImageUrl,
										project.OperatorName,
										project.LinkingStatus
									}).ToList();
				professionalResponse.LinkedOpr = new List<LinkedOperators>();
				
				LinkedOperators Oper;
				foreach (var linked in linkedOperators)
				{
					Oper = new LinkedOperators();
					int listingIdOnReProfessional = (from project in projects
													where project.REProfessionalMasterId == linked.REProfessionalMasterId 
													select project.ListingId)
													.First();
					//var GetListingIdOnpro = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.UserId).First();
					Oper.Property_ListingId = linked.ListingId;
					Oper.ReProfessional_ListingId = listingIdOnReProfessional;
					Oper.REProfessionalMasterId = linked.REProfessionalMasterId;
					Oper.UserId = linked.UserId;
					Oper.ProjectRole = linked.ProjectRole;
					Oper.ProjectName = linked.ProjectName;
					
					Oper.PropertyName = (from listing in listings
										where listing.ListingId == linked.ListingId
										select listing.Name)
										.First();
					
					Oper.CompanyName = (from operatr in operators
										where operatr.UserId == linked.UserId
										select operatr.CompanyName)
										.First();
					
					Oper.Doc_CompanyLogo = (from operatr in operators
											where operatr.UserId == linked.UserId
											select operatr.Doc_CompanyLogo)
											.First();

					professionalResponse.LinkedOpr.Add(Oper);
				}
				professionalsResponse.Add(professionalResponse);
			}

			if(professionalsResponse.Count > 0)
				return professionalsResponse;
					return NotFound();
		}


	}
}