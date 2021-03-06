﻿using System;
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
using HiSpaceListingService.Utilities;
//using StackExchange.Profiling;

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

			if (UserType == 2 || (UserType == 1 && Type == "My"))
			{
				var enquiries = await _context.Enquiries.AsNoTracking()
															.Where(d => d.Sender_UserId == UserId)
															.ToListAsync();

				await AddToEnquiryTable(enquiryTable, enquiries);
			}
			else if (UserType == 1 && Type == "User")
			{
				var enquiries = await _context.Enquiries.AsNoTracking()
															.Where(d => d.Listing_UserId == UserId)
															.ToListAsync();

				await AddToEnquiryTable(enquiryTable, enquiries);
			}
			return enquiryTable;
		}

		async Task AddToEnquiryTable(List<EnquiryListResponse> enquiryTable, List<Enquiry> enquiries)
		{
			EnquiryListResponse list;
			var listings = await (from listing in _context.Listings.AsNoTracking()
								  select new { listing.ListingId, listing.Name, listing.ListingType }).ToListAsync();

			var users = await (from user in _context.Users.AsNoTracking()
							   select new { user.UserId, user.CompanyName })
								.ToListAsync();

			foreach (var item in enquiries)
			{
				list = new EnquiryListResponse();

				list.Enquiry = new Enquiry();
				list.Enquiry = item;
				list.PropertyName = listings.Where(d => d.ListingId == item.ListingId).Select(d => d.Name)
											.First();
				list.Type = listings.Where(d => d.ListingId == item.ListingId)
									.Select(d => d.ListingType)
									.First();
				list.OperatorName = users.Where(d => d.UserId == item.Listing_UserId)
									.Select(d => d.CompanyName)
									.First();

				enquiryTable.Add(list);
			}
		}


		[HttpGet]
		[Route("GetListingsByUserId/{UserId}")]
		public async Task<ActionResult<IEnumerable<ListingTableResponse>>> GetListingsByUserId(int UserId)
		{
			List<ListingTableResponse> listingTable = new List<ListingTableResponse>();

			var listings = await _context.Listings.AsNoTracking()
								.Where(d => d.UserId == UserId && d.DeletedStatus == false).ToListAsync();

			List<GreenBuildingCheck> GBCs = await (from GBC in _context.GreenBuildingChecks
													.AsNoTracking()
												   select GBC)
											.ToListAsync();

			List<int> healthChecks = await (from hc in _context.HealthChecks
													   .AsNoTracking()
											select hc.ListingId)
													.ToListAsync();

			List<int> workingHours = await (from wh in _context.WorkingHourss
																.AsNoTracking()
											select wh.ListingId)
														.ToListAsync();

			List<int> listingImages = await (from image in _context.ListingImagess
														.AsNoTracking()
											 select image.ListingId)
														.ToListAsync();

			List<int> amenities = await (from amenity in _context.Amenitys.AsNoTracking()
										 select amenity.ListingId)
												.ToListAsync();

			List<int> facilities = await (from facility in _context.Facilitys.AsNoTracking()
										  select facility.ListingId)
													.ToListAsync();

			List<int> projects = await (from project in _context.REProfessionalMasters
										.AsNoTracking()
										select project.ListingId)
										.ToListAsync();

			ListingTableResponse list;

			foreach (var item in listings)
			{
				list = new ListingTableResponse();

				list.Listings = new Listing();
				list.Listings = item;

				list.GBC = GBCs.SingleOrDefault(d => d.ListingId == item.ListingId);

				list.TotalHealthCheck = healthChecks.Where(d => d == item.ListingId).Count();

				list.TotalGreenBuildingCheck = GBCs.Where(d => d.ListingId == item.ListingId)
													.Count();

				list.TotalWorkingHours = workingHours.Where(d => d == item.ListingId).Count();

				list.TotalListingImages = listingImages.Where(d => d == item.ListingId).Count();

				list.TotalAmenities = amenities.Where(d => d == item.ListingId)
														.Count();

				list.TotalFacilities = facilities.Where(d => d == item.ListingId)
														.Count();

				list.TotalProjects = projects.Where(d => d == item.ListingId)
													.Count();

				listingTable.Add(list);
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
			return await _context.Listings.AsNoTracking().ToListAsync();
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
				autoEntry.CreatedDateTime = DateTime.Now;

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
		public async Task<IActionResult> UpdateListingByListingId(int ListingId, [FromBody] Listing listing)
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
			return _context.Listings.AsNoTracking().Any(e => e.ListingId == ListingId);
		}

		/// <summary>
		/// Delete the Listing by ListingId.
		/// </summary>
		/// <returns>The Listings list.</returns>
		[HttpGet]
		[Route("DeleteListing/{ListingId}")]
		public async Task<ActionResult<bool>> DeleteListing(int ListingId)
		{
			var result = true;
			var listing = await _context.Listings.FindAsync(ListingId);
			if (listing == null)
			{
				return NotFound();
			}
			listing.DeletedStatus = true;

			_context.Entry(listing).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			var reProfessionalMaster = await _context.REProfessionalMasters.Where(m => m.ListingId == ListingId).ToListAsync();
			foreach (var item in reProfessionalMaster)
			{
				item.DeletedStatus = true;
				_context.Entry(item).State = EntityState.Modified;
			}
			await _context.SaveChangesAsync();

			//var amenities = await _context.Amenitys.Where(m => m.ListingId == ListingId).ToListAsync();
			//foreach (var item in amenities)
			//{
			//	_context.Amenitys.Remove(item);
			//}
			//await _context.SaveChangesAsync();

			//var facilities = await _context.Facilitys.Where(m => m.ListingId == ListingId).ToListAsync();
			//foreach (var item in facilities)
			//{
			//	_context.Facilitys.Remove(item);
			//}
			//await _context.SaveChangesAsync();

			//var listingImages = await _context.ListingImagess.Where(m => m.ListingId == ListingId).ToListAsync();
			//foreach (var item in listingImages)
			//{
			//	_context.ListingImagess.Remove(item);
			//}
			//await _context.SaveChangesAsync();

			//var workingHours = await _context.WorkingHourss.Where(m => m.ListingId == ListingId).FirstOrDefaultAsync();
			//_context.WorkingHourss.Remove(workingHours);
			//await _context.SaveChangesAsync();

			//var healthCheck = await _context.HealthChecks.Where(m => m.ListingId == ListingId).FirstOrDefaultAsync();
			//_context.HealthChecks.Remove(healthCheck);
			//await _context.SaveChangesAsync();

			//var greenBuildingCheck = await _context.GreenBuildingChecks.Where(m => m.ListingId == ListingId).FirstOrDefaultAsync();
			//_context.GreenBuildingChecks.Remove(greenBuildingCheck);
			//await _context.SaveChangesAsync();



			return result;
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
			var properties = await _context.Listings.AsNoTracking()
									.Where(m => m.Status == true && m.DeletedStatus == false).OrderByDescending(d => d.CreatedDateTime)
									.ToListAsync();

			PropertyDetailResponse property;
			var users = await _context.Users.AsNoTracking().ToListAsync();
			List<int> amenities = await (from PA in _context.Amenitys.AsNoTracking()
										 where PA.Status == true
										 select PA.ListingId)
									.ToListAsync();

			List<int> facilities = await (from PF in _context.Facilitys.AsNoTracking()
										  where PF.Status == true
										  select PF.ListingId)
									.ToListAsync();

			List<int> projects = await (from project in _context.REProfessionalMasters.AsNoTracking()
										where project.Status == true
										select project.ListingId)
									.ToListAsync();

			List<ListingImages> images = await _context.ListingImagess.AsNoTracking().ToListAsync();

			List<int> AHCs = await (from AHC in _context.HealthChecks.AsNoTracking()
									where AHC.Status == true
									select AHC.ListingId)
								.ToListAsync();

			List<int> GBCs = await (from GBC in _context.GreenBuildingChecks.AsNoTracking()
									where GBC.Status == true
									select GBC.ListingId)
								.ToListAsync();

			foreach (var item in properties)
			{
				property = new PropertyDetailResponse();
				property.SpaceListing = item;
				property.SpaceUser = users.SingleOrDefault(d => d.UserId == item.UserId);

				property.AvailableAmenities = (from PA in amenities
											   where PA == item.ListingId
											   select PA).Count();

				property.AvailableFacilities = (from listingId in facilities
												where listingId == item.ListingId
												select listingId)
												.Count();

				property.AvailableProjects = (from listingId in projects
											  where listingId == item.ListingId
											  select listingId)
												.Count();

				property.ListingImagesList = (from image in images
											  where image.ListingId == item.ListingId
											  select image)
												.ToList();

				property.AvailableHealthCheck = (from listingId in AHCs
												 where listingId == item.ListingId
												 select listingId)
												.Count();

				property.AvailableGreenBuildingCheck = (from listingId in GBCs
														where listingId == item.ListingId
														select listingId)
														.Count();
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

			List<Listing> listings = await (from listing in _context.Listings.AsNoTracking()
											where listing.Status == true
											select listing).ToListAsync();

			List<Listing> properties = listings
							.Where(m => m.UserId == UserID && m.DeletedStatus == false)
							.OrderByDescending(d => d.CreatedDateTime)
							.ToList();

			List<User> users = await _context.Users.AsNoTracking().ToListAsync();

			IEnumerable<int> amenities = from amenity in _context.Amenitys.AsNoTracking()
										 where amenity.Status == true
										 select amenity.ListingId;

			IEnumerable<int> facilities = from facility in _context.Facilitys.AsNoTracking()
										  where facility.Status == true
										  select facility.ListingId;

			IEnumerable<int> projects = from project in _context.REProfessionalMasters.AsNoTracking()
										where project.Status == true
										select project.ListingId;

			IEnumerable<int> AHCs = from AHC in _context.HealthChecks.AsNoTracking()
									where AHC.Status == true
									select AHC.ListingId;

			IEnumerable<GreenBuildingCheck> GBCs = from GBC in _context.GreenBuildingChecks.AsNoTracking()
												   select GBC;

			PropertyDetailResponse property;

			foreach (var item in properties)
			{
				property = new PropertyDetailResponse();
				property.SpaceListing = item;

				property.SpaceUser = users.SingleOrDefault(d => d.UserId == item.UserId);

				property.AvailableAmenities = (from listingId in amenities
											   where listingId == item.ListingId
											   select listingId).Count();

				property.AvailableFacilities = (from listingId in facilities
												where listingId == item.ListingId
												select listingId)
												.Count();

				property.AvailableProjects = (from listingId in projects
											  where listingId == item.ListingId
											  select listingId)
												.Count();

				property.ListingImagesList = _context.ListingImagess
											.Where(d => d.ListingId == item.ListingId)
											.ToList();

				property.AvailableHealthCheck = (from listingId in AHCs
												 where listingId == item.ListingId
												 select listingId).Count();

				property.AvailableGreenBuildingCheck = (from gbc in GBCs
														where gbc.ListingId == item.ListingId && gbc.Status == true
														select gbc.ListingId).Count();

				// Adding additional detail
				property.GreenBuildingCheckDetails = GBCs.SingleOrDefault(d => d.ListingId == item.ListingId);
				vModel.PropertyDetail.Add(property);
			}
			vModel.SpaceUser = users.SingleOrDefault(m => m.UserId == UserID);

			vModel.PropertyCount = (from listing in listings
									where listing.UserId == UserID
									select listing.ListingId)
									.Count();

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

			List<Listing> listings = await _context.Listings.AsNoTracking()
											.ToListAsync();

			List<Listing> properties = listings.Where(m => m.Status == true &&
											m.AdminStatus == true &&
											m.UserId == UserID &&
											m.ListingId == ListingID)
									.OrderByDescending(d => d.CreatedDateTime)
									.ToList();

			List<User> users = await _context.Users.AsNoTracking().ToListAsync();

			List<int> amenities = await (from amenity in _context.Amenitys.AsNoTracking()
										 where amenity.Status == true
										 select amenity.ListingId)
											.ToListAsync();

			List<int> facilities = await (from facility in _context.Facilitys.AsNoTracking()
										  where facility.Status == true
										  select facility.ListingId)
											.ToListAsync();

			List<REProfessionalMaster> projects = await (from project in _context.REProfessionalMasters
															.AsNoTracking()
														 select project)
														.ToListAsync();

			List<ListingImages> images = await _context.ListingImagess.AsNoTracking()
												.ToListAsync();

			IEnumerable<int> AHCs = from AHC in _context.HealthChecks.AsNoTracking()
									where AHC.Status == true
									select AHC.ListingId;

			IEnumerable<GreenBuildingCheck> GBCs = from GBC in _context.GreenBuildingChecks.AsNoTracking()
												   select GBC;

			PropertyDetailResponse property;

			foreach (var item in properties)
			{
				property = new PropertyDetailResponse();
				property.SpaceListing = item;
				//property.SpaceUser = _context.Users.SingleOrDefault(d => d.UserId == item.UserId);
				property.AvailableAmenities = (from listingId in amenities
											   where listingId == item.ListingId
											   select listingId)
												.Count();

				property.AvailableFacilities = (from listingId in facilities
												where listingId == item.ListingId
												select listingId)
												.Count();

				property.AvailableProjects = (from project in projects
											  where project.ListingId == item.ListingId
											  select project.ListingId)
												.Count();

				property.ListingImagesList = (from image in images
											  where image.ListingId == item.ListingId
											  select image)
											.ToList();

				property.AvailableHealthCheck = (from listingId in AHCs
												 where listingId == item.ListingId
												 select listingId).Count();

				property.AvailableGreenBuildingCheck = (from GBC in GBCs
														where GBC.ListingId == item.ListingId
														&& GBC.Status == true
														select GBC.ListingId).Count();

				// Adding additional detail
				property.GreenBuildingCheckDetails = GBCs.SingleOrDefault(d => d.ListingId == item.ListingId);

				vModel.PropertyDetail.Add(property);
			}
			vModel.SpaceUser = users.SingleOrDefault(m => m.UserId == UserID);

			vModel.PropertyCount = listings.Where(d => d.UserId == UserID && d.Status == true)
											.ToList()
											.Count();

			//geting linked re-prof
			var linkedREProf = (from l in listings
								from r in projects
								where (l.UserId == UserID && l.DeletedStatus == false &&
								(l.ListingType == "Commercial" || l.ListingType == "Co-Working") &&
							   (((l.CMCW_ReraId != null && r.PropertyReraId != null) && (l.CMCW_ReraId == r.PropertyReraId))
							   || ((l.CMCW_CTSNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_GatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_GatNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_MilkatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_PlotNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_SurveyNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_PropertyTaxBillNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber))
								) && (r.LinkingStatus == "Approved") && r.DeletedStatus == false)
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
				var GetListingIdOnReProfessional = projects.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId).Select(d => d.ListingId).First();
				REProf.Property_ListingId = linked.ListingId;
				REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
				REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
				REProf.UserId = linked.UserId;
				REProf.ProjectRole = linked.ProjectRole;
				REProf.ProjectName = linked.ProjectName;
				REProf.OperatorName = linked.OperatorName;
				REProf.LinkingStatus = linked.LinkingStatus;
				REProf.ImageUrl = linked.ImageUrl;
				var REName = (from listing in listings
							  where listing.ListingId == GetListingIdOnReProfessional
							  select new { listing.RE_FirstName, listing.RE_LastName })
											.First();

				REProf.REFirstName = REName.RE_FirstName;
				REProf.RELastName = REName.RE_LastName;
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

			List<Listing> listings = await _context.Listings.AsNoTracking().ToListAsync();

			List<Listing> properties = listings.Where(m => m.Status == true &&
																m.UserId == UserID)
													.OrderByDescending(d => d.CreatedDateTime)
													.ToList();

			List<REProfessionalMaster> projects = await _context.REProfessionalMasters.AsNoTracking().ToListAsync();

			List<User> users = await _context.Users.AsNoTracking().ToListAsync();

			//geting linked re-prof
			var linkedREProf = (from l in listings
								from r in projects
								where ((l.UserId == UserID) && (l.DeletedStatus == false) &&
								(l.ListingType == "Commercial" || l.ListingType == "Co-Working") &&
								(((l.CMCW_ReraId != null && r.PropertyReraId != null) && (l.CMCW_ReraId == r.PropertyReraId))
								|| ((l.CMCW_CTSNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_GatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_GatNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_MilkatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_PlotNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_SurveyNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber))
								|| ((l.CMCW_PropertyTaxBillNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber)))
								&& (r.DeletedStatus == false)
								 )
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
				var GetListingIdOnReProfessional = projects.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId)
													.Select(d => d.ListingId)
													.First();

				REProf.Property_ListingId = linked.ListingId;
				REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;

				var GetREListing_UserId = listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.UserId)
														.First();

				REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
				REProf.UserId = linked.UserId;
				REProf.ProjectRole = linked.ProjectRole;
				REProf.ProjectName = linked.ProjectName;
				REProf.OperatorName = linked.OperatorName;
				REProf.LinkingStatus = linked.LinkingStatus;
				REProf.ImageUrl = linked.ImageUrl;

				var userNameAndAddress = (from user in users
										  where user.UserId == GetREListing_UserId
										  select new { user.CompanyName, user.Address })
										.First();

				REProf.RE_UserName = userNameAndAddress.CompanyName;
				REProf.RE_Address = userNameAndAddress.Address;
				//REProf. = linked.ImageUrl;

				var REName = (from listing in listings
							  where listing.ListingId == GetListingIdOnReProfessional
							  select new { listing.RE_FirstName, listing.RE_LastName })
							.First();

				REProf.REFirstName = REName.RE_FirstName;
				REProf.RELastName = REName.RE_LastName;

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

			List<Listing> listings = await _context.Listings.AsNoTracking().ToListAsync();

			List<Listing> properties = listings.Where(m => m.Status == true &&
															m.AdminStatus == true &&
															m.UserId == UserID &&
															m.DeletedStatus == false)
															.OrderByDescending(d => d.CreatedDateTime)
															.ToList();

			List<int> amenities = await (from amenity in _context.Amenitys.AsNoTracking()
										 where amenity.Status == true
										 select amenity.ListingId)
								.ToListAsync();

			List<int> facilities = await (from facility in _context.Facilitys.AsNoTracking()
										  where facility.Status == true
										  select facility.ListingId)
								.ToListAsync();

			List<REProfessionalMaster> projects = await _context.REProfessionalMasters.AsNoTracking()
														.ToListAsync();

			List<ListingImages> images = await _context.ListingImagess.AsNoTracking().ToListAsync();

			List<int> AHCs = await (from AHC in _context.HealthChecks.AsNoTracking()
									where AHC.Status == true
									select AHC.ListingId)
									.ToListAsync();

			List<GreenBuildingCheck> GBCs = await _context.GreenBuildingChecks.AsNoTracking()
												.ToListAsync();

			List<User> users = await _context.Users.ToListAsync();


			PropertyDetailResponse property;

			foreach (var item in properties)
			{
				property = new PropertyDetailResponse();
				property.SpaceListing = item;
				//property.SpaceUser = _context.Users.SingleOrDefault(d => d.UserId == item.UserId);

				property.AvailableAmenities = (from listingId in amenities
											   where listingId == item.ListingId
											   select listingId).Count();

				property.AvailableFacilities = (from listingId in facilities
												where listingId == item.ListingId
												select listingId).Count();

				property.AvailableProjects = (from project in projects
											  where project.ListingId == item.ListingId &&
											  project.Status == true
											  select project.ListingId).Count();

				property.ListingImagesList = (from image in images
											  where image.ListingId == item.ListingId
											  select image).ToList();

				property.AvailableHealthCheck = (from listingId in AHCs
												 where listingId == item.ListingId
												 select listingId).Count();

				property.AvailableGreenBuildingCheck = (from GBC in GBCs
														where GBC.ListingId == item.ListingId &&
														GBC.Status == true
														select GBC.ListingId).Count();

				// Adding additional detail
				property.GreenBuildingCheckDetails = GBCs.SingleOrDefault(d => d.ListingId == item.ListingId);
				vModel.PropertyDetail.Add(property);
			}
			vModel.SpaceUser = users.SingleOrDefault(m => m.UserId == UserID);
			vModel.PropertyCount = listings.Where(d => d.UserId == UserID &&
													d.Status == true &&
													d.AdminStatus == true)
											.Count();

			//geting linked re-prof
			var linkedREProf = (from l in listings
								from r in projects
								where (l.UserId == UserID && l.DeletedStatus == false &&
							   (l.ListingType == "Commercial" || l.ListingType == "Co-Working") &&
							   (((l.CMCW_ReraId != null && r.PropertyReraId != null) && (l.CMCW_ReraId == r.PropertyReraId))
							   || ((l.CMCW_CTSNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_GatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_GatNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_MilkatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_PlotNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_SurveyNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber))
							   || ((l.CMCW_PropertyTaxBillNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber))
								)
								&& (r.LinkingStatus == "Approved") && r.DeletedStatus == false)
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
				var GetListingIdOnReProfessional = projects.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId)
													.Select(d => d.ListingId)
													.First();

				REProf.Property_ListingId = linked.ListingId;
				REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
				REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
				REProf.UserId = linked.UserId;
				REProf.ProjectRole = linked.ProjectRole;
				REProf.OperatorName = linked.ProjectRole;
				REProf.LinkingStatus = linked.LinkingStatus;
				REProf.ProjectName = linked.ProjectName;
				REProf.ImageUrl = linked.ImageUrl;

				var REName = listings.Where(d => d.ListingId == GetListingIdOnReProfessional)
										.Select(d => new { d.RE_FirstName, d.RE_LastName })
										.First();

				REProf.REFirstName = REName.RE_FirstName;
				REProf.RELastName = REName.RE_LastName;

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
			IEnumerable<Listing> activeProperties = await _context.Listings.ToListAsync();
			activeProperties = GetActiveListing(activeProperties);

			var property = (from prop in activeProperties
							where prop.ListingId == ListingID
							select prop)
							.SingleOrDefault();

			if (property == null)
			{
				return NotFound();
			}

			propertyDetails.Listing = property;
			propertyDetails.User = _context.Users.SingleOrDefault(d => d.UserId == property.UserId);
			propertyDetails.WorkingHours = _context.WorkingHourss.SingleOrDefault(d => d.ListingId == property.ListingId);
			propertyDetails.Amenities = _context.Amenitys.Where(d => d.Status == true && d.ListingId == property.ListingId).ToList();
			propertyDetails.Facilities = _context.Facilitys.Where(d => d.Status == true && d.ListingId == property.ListingId).ToList();
			propertyDetails.ListingImages = _context.ListingImagess.Where(d => d.Status == true && d.ListingId == property.ListingId).ToList();

			List<REProfessionalMaster> projects = await _context.REProfessionalMasters.ToListAsync();
			propertyDetails.REProfessionalMasters = (from project in projects
													 where project.Status == true && project.ListingId == property.ListingId
													 select project)
							.ToList();

			var propertyCount = (from prop in activeProperties
								 where prop.UserId == property.UserId && prop.Status == true
								 select prop)
								.ToList();

			propertyDetails.ListerPropertyCount = propertyCount.Count();
			propertyDetails.HealthCheck = _context.HealthChecks.SingleOrDefault(d => d.ListingId == property.ListingId);
			propertyDetails.GreenBuildingCheck = _context.GreenBuildingChecks.SingleOrDefault(d => d.ListingId == property.ListingId);
			// Adding additional field
			propertyDetails.ProjectCount = _context.REProfessionalMasters.Where(d => d.ListingId == property.ListingId).Count();



			var linkedREProf = (from r in projects
								where (((r.PropertyReraId == property.CMCW_ReraId && property.CMCW_ReraId != null)
								|| (r.PropertyAdditionalIdNumber == property.CMCW_CTSNumber && property.CMCW_CTSNumber != null)
							|| (r.PropertyAdditionalIdNumber == property.CMCW_MilkatNumber && property.CMCW_MilkatNumber != null)
							|| (r.PropertyAdditionalIdNumber == property.CMCW_PlotNumber && property.CMCW_PlotNumber != null)
							|| (r.PropertyAdditionalIdNumber == property.CMCW_SurveyNumber && property.CMCW_SurveyNumber != null)
							|| (r.PropertyAdditionalIdNumber == property.CMCW_PropertyTaxBillNumber && property.CMCW_PropertyTaxBillNumber != null)
								|| (r.PropertyAdditionalIdNumber == property.CMCW_GatNumber && property.CMCW_GatNumber != null))
									 && (r.LinkingStatus == "Approved") && (r.DeletedStatus == false))
								select new
								{
									property.ListingId,
									r.REProfessionalMasterId,
									property.UserId,
									r.ProjectRole,
									r.ProjectName,
									r.ImageUrl,
									r.OperatorName,
									r.LinkingStatus
								}).ToList();

			propertyDetails.LinkedREProf = new List<LinkedREPRofessionals>();
			foreach (var linked in linkedREProf)
			{
				LinkedREPRofessionals REProf = new LinkedREPRofessionals();
				var GetListingIdOnReProfessional = projects
						.Where(d => d.REProfessionalMasterId == linked.REProfessionalMasterId)
						.Select(d => d.ListingId)
						.First();
				REProf.Property_ListingId = linked.ListingId;
				REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
				REProf.REProfessionalMasterId = linked.REProfessionalMasterId;
				REProf.UserId = linked.UserId;
				REProf.ProjectRole = linked.ProjectRole;
				REProf.OperatorName = linked.OperatorName;
				REProf.LinkingStatus = linked.LinkingStatus;
				REProf.ProjectName = linked.ProjectName;
				REProf.ImageUrl = linked.ImageUrl;
				REProf.REFirstName = activeProperties
									.Where(d => d.ListingId == GetListingIdOnReProfessional)
									.Select(d => d.RE_FirstName)
									.First();
				REProf.RELastName = activeProperties
										.Where(d => d.ListingId == GetListingIdOnReProfessional)
										.Select(d => d.RE_LastName)
										.First();
				propertyDetails.LinkedREProf.Add(REProf);
			}
			propertyDetails.LinkedREProfCount = propertyDetails.LinkedREProf.Count;

			//foreach(var amenity in amenities)
			//{

			//}
			return propertyDetails;
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
		[Route("GetOperatorList/{currentPageNumber}")]
		[HttpPost]
		public async Task<ActionResult> GetOperatorList(int currentPageNumber, [FromBody] OperatorSearchCriteria searchCriteria)
		{
			IQueryable<User> operatorQuery = from REOperator in _context.Users.AsQueryable()
											 select REOperator;

			operatorQuery = from REOperator in operatorQuery
							where REOperator.UserType == 1 &&
										REOperator.Status == true
							select REOperator;

			if (!string.IsNullOrEmpty(searchCriteria.OperatorName))
				operatorQuery = from REOperator in operatorQuery
								where REOperator.CompanyName == searchCriteria.OperatorName
								select REOperator;

			if (!string.IsNullOrEmpty(searchCriteria.CityName))
				operatorQuery = from REOperator in operatorQuery
								where REOperator.City == searchCriteria.CityName
								select REOperator;

			List<int> operatorIds = await (from REOperator in operatorQuery.AsNoTracking()
										   select REOperator.UserId)
																		.ToListAsync();

			PaginationModel<PropertyOperatorResponse> model = await OperatorPaginationModelGenerator.GetPagedModelFromOperatorIds(_context,operatorIds,currentPageNumber);
			return Ok(model);
			
		}

		[Route("GetOperatorListWithFavorites/{userId}/{currentPageNumber}")]
		[HttpPost]
		public async Task<ActionResult> GetOperatorListWithFavorites(int userId, int currentPageNumber, [FromBody] OperatorSearchCriteria searchCriteria)
		{
			try
			{
				ActionResult actionResult = await GetOperatorList(currentPageNumber, searchCriteria);
				PaginationModel<PropertyOperatorResponse> response = await ActionResultUtility
															.GetOperatorPageResponse(userId,
																				actionResult,
																				_context);
				if (response != null)
					return Ok(response);

				return NotFound();
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}
			return NotFound();
		}

		//[Route("GetPeopleList")]
		//[HttpPost]
		//public async Task<ActionResult> GetPeopleList([FromBody] PeopleSearchCriteria searchCriteria)
		//{
		//	IEnumerable<Listing> listings = await _context.Listings.ToListAsync();
		//	IEnumerable<User> operators = await _context.Users.ToListAsync();
		//	IEnumerable<REProfessionalMaster> projects;
		//	projects = await _context.REProfessionalMasters.ToListAsync();

		//	var professionals = (from listing in listings
		//						 join operatr in operators
		//						 on listing.UserId equals operatr.UserId
		//						 where listing.ListingType == "RE-Professional" && listing.Status == true && listing.AdminStatus == true && listing.DeletedStatus == false
		//						 select new
		//						 {
		//							 operatr,
		//							 listing
		//						 });
		//	if (professionals != null && !string.IsNullOrEmpty(searchCriteria.Role))
		//	{
		//		professionals = (from professional in professionals
		//						 join project in projects
		//						 on professional.listing.ListingId equals project.ListingId
		//						 where project.ProjectRole == searchCriteria.Role
		//						 select professional).Distinct();

		//		if (professionals == null)
		//			return NotFound();
		//	}

		//	if (professionals != null && !string.IsNullOrEmpty(searchCriteria.FirstName))
		//		professionals = from professional in professionals
		//						where professional.listing.RE_FirstName == searchCriteria.FirstName
		//						select professional;

		//	if (professionals != null && !string.IsNullOrEmpty(searchCriteria.LastName))
		//		professionals = from professional in professionals
		//						where professional.listing.RE_LastName == searchCriteria.LastName
		//						select professional;

		//	if (professionals != null && !string.IsNullOrEmpty(searchCriteria.Locality))
		//		professionals = from professional in professionals
		//						where professional.listing.locality == searchCriteria.Locality
		//						select professional;

		//	if (professionals == null)
		//		return NotFound();

		//	List<PropertyPeopleResponse> professionalsResponse = new List<PropertyPeopleResponse>();
		//	PropertyPeopleResponse professionalResponse;

		//	foreach (var item in professionals)
		//	{
		//		professionalResponse = new PropertyPeopleResponse();

		//		professionalResponse.Operator = new User();
		//		professionalResponse.Operator = item.operatr;
		//		//p.Operator.UserId = item.a.UserId;
		//		//p.Operator.UserType = item.a.UserType;
		//		//p.ListingId = item.ListingId;
		//		professionalResponse.Listing = (from listing in listings
		//										where listing.ListingId == item.listing.ListingId
		//										select listing)
		//								.SingleOrDefault();

		//		professionalResponse.Projects = (from project in projects
		//										 where project.ListingId == item.listing.ListingId
		//										 select project).ToList();

		//		professionalResponse.Roles = (from project in professionalResponse.Projects
		//									  select project.ProjectRole)
		//									.Distinct()
		//									.ToList();

		//		professionalResponse.TotalProjects = professionalResponse.Projects.Count();
		//		//geting linked Operator
		//		var linkedOperators = (from listing in listings
		//							   from project in projects
		//							   where (
		//							   (listing.ListingType == "Commercial" || listing.ListingType == "Co-Working") && listing.DeletedStatus == false &&
		//								(((listing.CMCW_ReraId != null && project.PropertyReraId != null) && (listing.CMCW_ReraId == project.PropertyReraId))
		//						   || ((listing.CMCW_CTSNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_GatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_MilkatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_PlotNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_SurveyNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_PropertyTaxBillNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == project.PropertyAdditionalIdNumber))
		//							)
		//								&& (project.LinkingStatus == "Approved") && (project.ListingId == professionalResponse.Listing.ListingId) && project.DeletedStatus == false)
		//							   select new
		//							   {
		//								   listing.ListingId,
		//								   project.REProfessionalMasterId,
		//								   listing.UserId,
		//								   project.ProjectRole,
		//								   project.ProjectName,
		//								   project.ImageUrl,
		//								   project.OperatorName,
		//								   project.LinkingStatus
		//							   }).ToList();
		//		professionalResponse.LinkedOpr = new List<LinkedOperators>();

		//		LinkedOperators Oper;
		//		foreach (var linked in linkedOperators)
		//		{
		//			Oper = new LinkedOperators();
		//			int listingIdOnReProfessional = (from project in projects
		//											 where project.REProfessionalMasterId == linked.REProfessionalMasterId
		//											 select project.ListingId)
		//											.First();
		//			//var GetListingIdOnpro = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.UserId).First();
		//			Oper.Property_ListingId = linked.ListingId;
		//			Oper.ReProfessional_ListingId = listingIdOnReProfessional;
		//			Oper.REProfessionalMasterId = linked.REProfessionalMasterId;
		//			Oper.UserId = linked.UserId;
		//			Oper.ProjectRole = linked.ProjectRole;
		//			Oper.ProjectName = linked.ProjectName;

		//			Oper.PropertyName = (from listing in listings
		//								 where listing.ListingId == linked.ListingId
		//								 select listing.Name)
		//								.First();

		//			Oper.CompanyName = (from operatr in operators
		//								where operatr.UserId == linked.UserId
		//								select operatr.CompanyName)
		//								.First();

		//			Oper.Doc_CompanyLogo = (from operatr in operators
		//									where operatr.UserId == linked.UserId
		//									select operatr.Doc_CompanyLogo)
		//									.First();

		//			professionalResponse.LinkedOpr.Add(Oper);
		//		}
		//		professionalsResponse.Add(professionalResponse);
		//	}

		//	if (professionalsResponse.Count > 0)
		//		return Ok(professionalsResponse);
		//	return NotFound();
		//}

		[Route("GetPeopleListPaged/{currentPageNumber}")]
		[HttpPost]
		public async Task<ActionResult> GetPeopleListPaged(int currentPageNumber, [FromBody] PeopleSearchCriteria searchCriteria)
		{
			IEnumerable<REProfessionalMaster> projects = await _context.REProfessionalMasters.AsNoTracking()
																												.ToListAsync();

			IQueryable<Listing> peopleQuery = from listing in _context.Listings.AsQueryable()
											  where listing.ListingType == "RE-Professional" &&
														  listing.Status == true &&
														  listing.AdminStatus == true &&
														  listing.DeletedStatus == false
											  select listing;

			if (!string.IsNullOrEmpty(searchCriteria.FirstName))
				peopleQuery = from professional in peopleQuery
							  where professional.RE_FirstName == searchCriteria.FirstName
							  select professional;

			if (!string.IsNullOrEmpty(searchCriteria.LastName))
				peopleQuery = from professional in peopleQuery
							  where professional.RE_LastName == searchCriteria.LastName
							  select professional;

			if (!string.IsNullOrEmpty(searchCriteria.Locality))
				peopleQuery = from professional in peopleQuery
							  where professional.locality == searchCriteria.Locality
							  select professional;


			if (!string.IsNullOrEmpty(searchCriteria.Role))
				peopleQuery = (from professional in peopleQuery
							   join project in projects
							   on professional.ListingId equals project.ListingId
							   where project.ProjectRole == searchCriteria.Role
							   select professional)
																		.Distinct();

			List<int> allPeopleIds = await (from professional in peopleQuery.AsTracking()
											select professional.ListingId)
																			.ToListAsync();

			if (allPeopleIds.Count() > 0)
			{
				PaginationModel<PropertyPeopleResponse> response = await PeoplePaginationModelGenerator
																															.GetPagedModelFromPropertyIds(_context,
																																												allPeopleIds,
																																										currentPageNumber);
				return Ok(response);
			}

			return NotFound();
		}


		IEnumerable<Listing> GetActiveListing(IEnumerable<Listing> listings)
		{
			return from listing in listings
				   where listing.DeletedStatus == false
				   select listing;
		}

		[HttpGet]
		[Route("GetListingsWithCompletionPercentByUserId/{UserId}")]
		public async Task<ActionResult<ListingCompletionPercentResponse>> GetListingsWithCompletionPercentByUserId(int UserId)
		{
			var listings = await _context.Listings.AsNoTracking()
								.Where(d => d.UserId == UserId && d.DeletedStatus == false).ToListAsync();

			List<GreenBuildingCheck> GBCs = await (from GBC in _context.GreenBuildingChecks
													.AsNoTracking()
												   select GBC)
											.ToListAsync();

			List<int> healthChecks = await (from hc in _context.HealthChecks
													   .AsNoTracking()
											select hc.ListingId)
													.ToListAsync();

			List<int> workingHours = await (from wh in _context.WorkingHourss
																.AsNoTracking()
											select wh.ListingId)
														.ToListAsync();

			List<int> listingImages = await (from image in _context.ListingImagess
														.AsNoTracking()
											 select image.ListingId)
														.ToListAsync();

			List<int> amenities = await (from amenity in _context.Amenitys.AsNoTracking()
										 select amenity.ListingId)
												.ToListAsync();

			List<int> facilities = await (from facility in _context.Facilitys.AsNoTracking()
										  select facility.ListingId)
													.ToListAsync();

			List<int> projects = await (from project in _context.REProfessionalMasters
										.AsNoTracking()
										select project.ListingId)
										.ToListAsync();

			ListingCompletionPercentResponse response = new ListingCompletionPercentResponse();
			response.ListingsWithCompletionPercent = new List<ListingItemCompletionPercentResponse>();

			ListingItemCompletionPercentResponse list;

			int completedFormFieldsCount;
			int completedDataFieldsCount;
			int totalCompletedCount;

			int totalDataFields;
			int totalFormFields;
			int totalFields;

			foreach (var item in listings)
			{
				list = new ListingItemCompletionPercentResponse();
				list.Listings = new Listing();
				list.Listings = item;

				string listType = item.ListingType;

				if (listType == "Commercial" || listType == "Co-Working")
				{
					totalDataFields = 8;
					totalFormFields = 16;

					list.GBC = GBCs.SingleOrDefault(d => d.ListingId == item.ListingId);




					list.TotalHealthCheck = healthChecks.Where(d => d == item.ListingId).Count();

					list.TotalGreenBuildingCheck = GBCs.Where(d => d.ListingId == item.ListingId).Count();

					list.TotalAmenities = amenities.Where(d => d == item.ListingId).Count();

					list.TotalFacilities = facilities.Where(d => d == item.ListingId).Count();
				}
				else
				{
					totalDataFields = 3;
					totalFormFields = 5;
				}

				totalFields = totalFormFields + totalDataFields;

				list.TotalWorkingHours = workingHours.Where(d => d == item.ListingId).Count();
				list.TotalListingImages = listingImages.Where(d => d == item.ListingId).Count();
				list.TotalProjects = projects.Where(d => d == item.ListingId).Count();

				completedDataFieldsCount = GetCompletedDataFieldsCount(list);
				completedFormFieldsCount = GetCompletedFormFieldsCount(list);
				totalCompletedCount = completedDataFieldsCount + completedFormFieldsCount;

				list.PercentCompleted = (int)((float)totalCompletedCount * 100) / (totalFields);
				response.ListingsWithCompletionPercent.Add(list);
			}

			int numberOfEntries = response.ListingsWithCompletionPercent.Count();
			int averagePercent = 0;

			if (numberOfEntries != 0)
			{
				decimal sumOfPercents = response.ListingsWithCompletionPercent.
												Sum(n => n.PercentCompleted);
				averagePercent = (int)(sumOfPercents / numberOfEntries);
			}

			response.OverallPercentCompleted = averagePercent;
			return response;
		}

		int GetCompletedFormFieldsCount(ListingItemCompletionPercentResponse listItem)
		{
			int completedCount = 0;
			int requiredCount = 4;

			if (listItem == null)
				return completedCount;

			Listing listing = listItem.Listings;

			string listType = listing.ListingType;

			if (listType == "Commercial" || listType == "Co-Working")
			{
				completedCount = requiredCount;

				completedCount = GetCompletedFieldsCount(listing.Name, completedCount);
				completedCount = GetCompletedFieldsCount(listing.CMCW_PropertyFor, completedCount);
				completedCount = GetCompletedFieldsCount(listing.CommercialType, completedCount);
				completedCount = GetCompletedFieldsCount(listing.CommercialInfraType, completedCount);
				completedCount = GetCompletedFieldsCount(listing.SpaceSize, completedCount);
				completedCount = GetCompletedFieldsCount(listing.PriceMin, completedCount);
				completedCount = GetCompletedFieldsCount(listing.PriceMax, completedCount);
				completedCount = GetCompletedFieldsCount(listing.SpaceSize, completedCount);
				completedCount = GetCompletedFieldsCount(listing.BuildYear, completedCount);
				completedCount = GetCompletedFieldsCount(listing.RecentInnovation, completedCount);
				completedCount = GetCompletedFieldsCount(listing.CurrentOccupancy, completedCount);

				if (!string.IsNullOrEmpty(listing.CMCW_ReraId) ||
					!string.IsNullOrEmpty(listing.CMCW_SurveyNumber) ||
					!string.IsNullOrEmpty(listing.CMCW_PropertyTaxBillNumber) ||
					!string.IsNullOrEmpty(listing.CMCW_CTSNumber) ||
					!string.IsNullOrEmpty(listing.CMCW_MilkatNumber) ||
					!string.IsNullOrEmpty(listing.CMCW_GatNumber) ||
					!string.IsNullOrEmpty(listing.CMCW_PlotNumber))
					completedCount++;
			}
			else
			{
				requiredCount = 3;
				completedCount = requiredCount;
			}
			completedCount = GetCompletedFieldsCount(listing.Description, completedCount);

			if (listing.RE_Warehouse ||
				listing.RE_Office ||
				listing.RE_Retail ||
				listing.RE_Coworking ||
				listing.RE_PropertyManagement)
				completedCount++;

			return completedCount;
		}

		int GetCompletedDataFieldsCount(ListingItemCompletionPercentResponse list)
		{
			int completedCount = 0;

			if (list == null)
				return completedCount;

			string listType = list.Listings.ListingType;

			if (listType == "Commercial" || listType == "Co-Working")
			{

				if (list.GBC != null)
					completedCount++;

				completedCount = GetCompletedFieldsCount(list.TotalHealthCheck, completedCount);

				completedCount = GetCompletedFieldsCount(list.TotalGreenBuildingCheck, completedCount);

				completedCount = GetCompletedFieldsCount(list.TotalAmenities, completedCount);

				completedCount = GetCompletedFieldsCount(list.TotalFacilities, completedCount);
			}

			completedCount = GetCompletedFieldsCount(list.TotalProjects, completedCount);

			completedCount = GetCompletedFieldsCount(list.TotalWorkingHours, completedCount);

			completedCount = GetCompletedFieldsCount(list.TotalListingImages, completedCount);

			return completedCount;
		}

		int GetCompletedFieldsCount(int fieldData, int previousCount)
		{
			return fieldData > 0 ? ++previousCount : previousCount;
		}

		int GetCompletedFieldsCount(int? fieldData, int previousCount)
		{
			if (fieldData != null)
				return fieldData > 0 ? ++previousCount : previousCount;

			return previousCount;
		}

		int GetCompletedFieldsCount(decimal? fieldData, int previousCount)
		{
			if (fieldData != null)
				return fieldData > 0 ? ++previousCount : previousCount;

			return previousCount;
		}

		int GetCompletedFieldsCount(string fieldData, int previousCount)
		{
			return !string.IsNullOrEmpty(fieldData) ? ++previousCount : previousCount;
		}

		int GetCompletedFieldsCount(DateTime? fieldData, int previousCount)
		{
			if (fieldData != null)
				return ++previousCount;

			return previousCount;
		}

		//add bookmarks for listing
		[Route("AddUserListing")]
		[HttpPost]
		public async Task<IActionResult> AddUserListing([FromBody] UserListing userListing)
		{
			int userId;
			bool isUserExists = false;

			int listingId;
			bool isListingExists = false;

			if (ModelState.IsValid)
			{
				try
				{
					userId = userListing.UserId;
					isUserExists = await _context.Users.AsNoTracking()
										.AnyAsync(u => u.UserId == userId);
					if (!isUserExists)
						return NotFound(userListing);
					listingId = userListing.ListingId;
					isListingExists = await _context.Listings.AsNoTracking()
											.AnyAsync(l => l.ListingId == listingId);
					if (!isListingExists)
						return NotFound(userListing);

					await _context.UserListings.AddAsync(userListing);
					int recordsAffected = await _context.SaveChangesAsync();

					if (recordsAffected > 0)
						return Ok(userListing);
				}
				catch (Exception ex)
				{
					return StatusCode(StatusCodes.Status500InternalServerError, userListing);
				}
			}
			return BadRequest(userListing);
		}

		//add bookmarks for operator
		[Route("AddUserOperator")]
		[HttpPost]
		public async Task<IActionResult> AddUserOperator([FromBody] UserOperator userOperator)
		{
			int userId;
			bool isUserExists = false;

			int opertorId;
			bool isListingExists = false;

			if (ModelState.IsValid)
			{
				try
				{
					userId = userOperator.UserId;
					isUserExists = await _context.Users.AsNoTracking()
										.AnyAsync(u => u.UserId == userId);
					if (!isUserExists)
						return NotFound(userOperator);
					opertorId = userOperator.OperatorId;
					isListingExists = await _context.Users.AsNoTracking()
											.AnyAsync(l => l.UserId == opertorId);
					if (!isListingExists)
						return NotFound(userOperator);

					await _context.UserOperators.AddAsync(userOperator);
					int recordsAffected = await _context.SaveChangesAsync();

					if (recordsAffected > 0)
						return Ok(userOperator);
				}
				catch (Exception ex)
				{
					return StatusCode(StatusCodes.Status500InternalServerError, userOperator);
				}
			}
			return BadRequest(userOperator);
		}

		//remove bookmarks for listing
		[Route("DeleteUserListing")]
		[HttpPost]
		public async Task<IActionResult> DeleteUserListing([FromBody] UserListing userListing)
		{
			int userListingId;
			bool isUserListingExists = false;

			if (ModelState.IsValid)
			{
				try
				{
					userListingId = userListing.Id;
					isUserListingExists = await _context.UserListings.AsNoTracking()
													.AnyAsync(u => u.Id == userListingId);

					if (!isUserListingExists)
						return NotFound(userListingId);

					_context.UserListings.Remove(userListing);
					int recordsAffected = await _context.SaveChangesAsync();

					if (recordsAffected > 0)
						return Ok();
				}
				catch (Exception ex)
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			return BadRequest();
		}

		//remove bookmarks for Operator
		[Route("DeleteUserOperator")]
		[HttpPost]
		public async Task<IActionResult> DeleteUserOperator([FromBody] UserOperator userOperator)
		{
			int userOperatorId;
			bool isUserListingExists = false;

			if (ModelState.IsValid)
			{
				try
				{
					userOperatorId = userOperator.Id;
					isUserListingExists = await _context.UserOperators.AsNoTracking()
													.AnyAsync(u => u.Id == userOperatorId);

					if (!isUserListingExists)
						return NotFound(userOperatorId);

					_context.UserOperators.Remove(userOperator);
					int recordsAffected = await _context.SaveChangesAsync();

					if (recordsAffected > 0)
						return Ok();
				}
				catch (Exception ex)
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			return BadRequest();
		}

		//GET: api/Listing/GetLatestPropertiesCommercialAndCoworking
		[Route("GetLatestPropertiesCommercialAndCoworking")]
		[HttpGet]
		public async Task<ActionResult> GetLatestPropertiesCommercialAndCoworking()
		{
			const int count = 10;

			List<int> top10PropIds = await (from listing in _context.Listings.AsNoTracking()
											where listing.Status == true &&
											listing.AdminStatus == true &&
											listing.ListingType != "RE-Professional" && listing.DeletedStatus == false
											where listing.CreatedDateTime != null
											orderby listing.CreatedDateTime descending
											select listing.ListingId)
																																		  .Take(count)
																																		  .ToListAsync();

			if (top10PropIds.Count() <= 0)
				return NotFound();

			List<PropertyDetailResponse> vModel = await GetPropertyDetailResponseList(top10PropIds);

			if (vModel.Count > 0)
				return Ok(vModel);

			return NotFound();
		}

		async Task<List<PropertyDetailResponse>> GetPropertyDetailResponseList(List<int> PropertyIds)

		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();

			List<Listing> properties = await GetPropertiesFromPropertyIds(PropertyIds);

			List<User> propertiesUsers = await GetUsersForProperties(properties);

			List<int> amenitiesPropertyIds = await GetPropertyIdsOfAmenties(PropertyIds);

			List<int> facilitiesPropertyIds = await GetPropertyIdsOfFacilities(PropertyIds);

			List<ListingImages> images = await GetImagesOfProperties(PropertyIds);

			List<int> healthChecksPropertyIds = await GetPropertyIdsOfHealthChecks(PropertyIds);

			List<int> gbcsPropertyIds = await GetPropertyIdsOfGreenBuildingChecks(PropertyIds);

			List<REProfessionalMaster> projects = await _context.REProfessionalMasters
																													.AsNoTracking()
																													.ToListAsync();
			PropertyDetailResponse property;

			foreach (var item in properties)

			{
				property = new PropertyDetailResponse();
				property.SpaceListing = item;
				property.SpaceUser = GetUserForProperty(propertiesUsers, item.UserId);

				property.AvailableAmenities = GetAmenitiesCountForProperty(amenitiesPropertyIds, item.ListingId);

				property.AvailableFacilities = GetFacilitiesCountForProperty(facilitiesPropertyIds, item.ListingId);

				property.AvailableProjects = GetProjectsCountForProperty(projects, item.ListingId);

				property.ListingImagesList = GetImagesForProperty(images, item.ListingId);

				property.AvailableHealthCheck = GetHealthChecksCountForProperty(healthChecksPropertyIds, item.ListingId);

				property.AvailableGreenBuildingCheck = GetGreenBuildingChecksCountForProperty(gbcsPropertyIds, item.ListingId);

				//geting linked re-prof
				List<RelatedREProfessional> RelatedREProf = GetRelatedREProfessionals(projects, item);

				property.LinkedREProf = new List<LinkedREPRofessionals>();

				foreach (RelatedREProfessional professional in RelatedREProf)

				{
					LinkedREPRofessionals REProf = SetRelatedREProfessional(projects, professional);
					property.LinkedREProf.Add(REProf);

				}

				property.LinkedREProfCount = property.LinkedREProf.Count;
				vModel.Add(property);

			}
			return vModel;

		}

		List<RelatedREProfessional> GetRelatedREProfessionals(List<REProfessionalMaster> projects, Listing property)
		{
			List<RelatedREProfessional> RelatedREProf = new List<RelatedREProfessional>();
			RelatedREProf = (from r in projects
							 where (
							 (((r.PropertyReraId != null && property.CMCW_ReraId != null) && (r.PropertyReraId == property.CMCW_ReraId))
							 || ((r.PropertyAdditionalIdNumber != null && property.CMCW_CTSNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_CTSNumber))
							 || ((r.PropertyAdditionalIdNumber != null && property.CMCW_MilkatNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_MilkatNumber))
							 || ((r.PropertyAdditionalIdNumber != null && property.CMCW_PlotNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_PlotNumber))
							 || ((r.PropertyAdditionalIdNumber != null && property.CMCW_SurveyNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_SurveyNumber))
							 || ((r.PropertyAdditionalIdNumber != null && property.CMCW_PropertyTaxBillNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_PropertyTaxBillNumber))
							 || ((r.PropertyAdditionalIdNumber != null && property.CMCW_GatNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_GatNumber)))
							 && (r.LinkingStatus == "Approved") && (r.DeletedStatus == false))
							 select new RelatedREProfessional
							 {
								 ListingId = property.ListingId,
								 ProjectId = r.REProfessionalMasterId,
								 UserId = property.UserId,
								 ProjectRole = r.ProjectRole,
								 ProjectName = r.ProjectName,
								 ImageUrl = r.ImageUrl,
								 OperatorName = r.OperatorName,
								 LinkingStatus = r.LinkingStatus
							 }).ToList();
			return RelatedREProf;
		}

		LinkedREPRofessionals SetRelatedREProfessional(IEnumerable<REProfessionalMaster> projects, RelatedREProfessional professional)
		{
			LinkedREPRofessionals REProf = new LinkedREPRofessionals();
			var GetListingIdOnReProfessional = projects.Where(d => d.REProfessionalMasterId ==
																								professional.ProjectId)
																											.Select(d => d.ListingId)
																											.First();
			REProf.Property_ListingId = professional.ListingId;
			REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
			REProf.REProfessionalMasterId = professional.ProjectId;
			REProf.UserId = professional.UserId;
			REProf.ProjectRole = professional.ProjectRole;
			REProf.OperatorName = professional.OperatorName;
			REProf.LinkingStatus = professional.LinkingStatus;
			REProf.ProjectName = professional.ProjectName;
			REProf.ImageUrl = professional.ImageUrl;

			var REName = (from listing in _context.Listings.AsNoTracking()
						  where listing.ListingId == GetListingIdOnReProfessional
						  select new { listing.RE_FirstName, listing.RE_LastName })
														.First();

			REProf.REFirstName = REName.RE_FirstName;
			REProf.RELastName = REName.RE_LastName;
			return REProf;
		}

		async Task<List<Listing>> GetPropertiesFromPropertyIds(IEnumerable<int> propertyIds)
		{
			List<Listing> properties = await (from property in _context.Listings.AsNoTracking()
											  select property)
																							.Where(n => propertyIds.Contains(n.ListingId))
																							.ToListAsync();
			return properties;
		}

		async Task<List<User>> GetUsersForProperties(List<Listing> properties)
		{
			IEnumerable<int> userIdsForProperties = from REProperty in properties
													select REProperty.UserId;

			List<User> users = await (from user in _context.Users.AsNoTracking()
									  select user)
																			.Where(n => userIdsForProperties.Contains(n.UserId))
																			.ToListAsync();
			return users;
		}

		User GetUserForProperty(List<User> propertiesUsers, int userId)
		{
			User propertyUser = (from user in propertiesUsers
								 where user.UserId == userId
								 select user)
																		.SingleOrDefault();
			return propertyUser;
		}

		async Task<List<int>> GetPropertyIdsOfAmenties(IEnumerable<int> propertyIds)
		{
			List<int> amenitiesPropertyIds = await (from amenity in _context.Amenitys.AsNoTracking()
													where amenity.Status == true
													select amenity.ListingId)
																									.Where(n => propertyIds.Contains(n))
																									.ToListAsync();
			return amenitiesPropertyIds;
		}

		async Task<List<int>> GetPropertyIdsOfFacilities(IEnumerable<int> propertyIds)
		{
			List<int> facilitiesPropertyIds = await (from facility in _context.Facilitys.AsNoTracking()
													 where facility.Status == true
													 select facility.ListingId)
																									.Where(n => propertyIds.Contains(n))
																									.ToListAsync();
			return facilitiesPropertyIds;
		}

		async Task<List<int>> GetPropertyIdsOfHealthChecks(IEnumerable<int> propertyIds)
		{
			List<int> healthChecks = await (from healthCheck in _context.HealthChecks.AsNoTracking()
											where healthCheck.Status == true
											select healthCheck.ListingId)
																									.Where(n => propertyIds.Contains(n))
																									.ToListAsync();
			return healthChecks;
		}

		async Task<List<ListingImages>> GetImagesOfProperties(IEnumerable<int> propertyIds)
		{
			List<ListingImages> images = await _context.ListingImagess.AsNoTracking()
																							.Where(n => propertyIds.Contains(n.ListingId))
																							.ToListAsync();
			return images;
		}

		async Task<List<int>> GetPropertyIdsOfGreenBuildingChecks(IEnumerable<int> propertyIds)
		{
			List<int> gbcsPropIds = await (from gbc in _context.GreenBuildingChecks.AsNoTracking()
										   where gbc.Status == true
										   select gbc.ListingId)
																									.Where(n => propertyIds.Contains(n))
																									.ToListAsync();
			return gbcsPropIds;
		}

		int GetAmenitiesCountForProperty(List<int> amenitiesPropertyIds, int propertyId)
		{
			int amenitiesCount = (from amenityPropertyId in amenitiesPropertyIds
								  where amenityPropertyId == propertyId
								  select amenityPropertyId)
																											.Count();
			return amenitiesCount;
		}

		int GetFacilitiesCountForProperty(List<int> facilitiesPropertyIds, int propertyId)
		{
			int facilitiesCount = (from facilitypropertyId in facilitiesPropertyIds
								   where facilitypropertyId == propertyId
								   select facilitypropertyId)
																											.Count();
			return facilitiesCount;
		}

		int GetProjectsCountForProperty(IEnumerable<REProfessionalMaster> projects, int propertyId)
		{
			int projectsCount = (from project in projects
								 where project.ListingId == propertyId && project.Status == true
								 select project.ListingId)
																									.Count();
			return projectsCount;
		}

		List<ListingImages> GetImagesForProperty(List<ListingImages> propertiesImages, int propertyId)
		{
			List<ListingImages> propertyImages = (from propertyImage in propertiesImages
												  where propertyImage.ListingId == propertyId
												  select propertyImage)
																									.ToList();
			return propertyImages;
		}

		int GetHealthChecksCountForProperty(List<int> healthChecksPropertyIds, int propertyId)
		{
			int healthChecksCount = (from propertyHealthCheckId in healthChecksPropertyIds
									 where propertyHealthCheckId == propertyId
									 select propertyHealthCheckId)
																									.Count();
			return healthChecksCount;
		}

		int GetGreenBuildingChecksCountForProperty(List<int> gbcsPropertyIds, int propertyId)
		{
			int gbcChecksCount = (from propertyGBCId in gbcsPropertyIds
								  where propertyGBCId == propertyId
								  select propertyGBCId).Count();
			return gbcChecksCount;
		}

		//GetPropertyListCommercialAndCoworking/searchCriteria?ListingType=&CMCW_PropertyFor= 
		[Route("GetPropertyListCommercialAndCoworking/{currentPageNumber}")]
		[HttpPost]
		public async Task<ActionResult> GetPropertyListCommercialAndCoworking(int currentPageNumber, [FromBody] PropertySearchCriteria searchCriteria)
		{

			IQueryable<Listing> propertiesQuery = from property in _context.Listings.AsQueryable()
												  where property.Status == true &&
															  property.AdminStatus == true &&
															  property.ListingType != "RE-Professional" &&
															  property.DeletedStatus == false
												  select property;

			if (searchCriteria.IsValidListingType())
				propertiesQuery = from property in propertiesQuery
								  where property.ListingType == searchCriteria.ListingType
								  orderby property.CreatedDateTime descending
								  select property;

			if (searchCriteria.IsValidCMCW_PropertyFor())
				propertiesQuery = from property in propertiesQuery
								  where property.CMCW_PropertyFor == searchCriteria.CMCW_PropertyFor
								  select property;

			if (searchCriteria.IsValidCoworkingType())
				propertiesQuery = from property in propertiesQuery
								  where property.CoworkingType == searchCriteria.CoworkingType
								  select property;

			if (searchCriteria.IsValidCommercialType())
				propertiesQuery = from property in propertiesQuery
								  where property.CommercialType == searchCriteria.CommercialType
								  select property;

			if (!string.IsNullOrEmpty(searchCriteria.Locality))
				propertiesQuery = from property in propertiesQuery
								  where property.locality == searchCriteria.Locality
								  select property;

			//if (searchCriteria.IsValidPriceMin())
			//if (searchCriteria.PriceMin != 0)
			//	propertiesQuery = from property in propertiesQuery
			//					  where property.PriceMin >= searchCriteria.PriceMin
			//					  select property;
			if (searchCriteria.PriceMin == 0)
				propertiesQuery = from property in propertiesQuery
								  where property.PriceMin == null ||
								  property.PriceMin >= 0
								  select property;
			else
				propertiesQuery = from property in propertiesQuery
								  where property.PriceMin >= searchCriteria.PriceMin
								  select property;

			//if (searchCriteria.IsValidPriceMax())
			if (searchCriteria.PriceMax != 0)
				propertiesQuery = from property in propertiesQuery
								  where property.PriceMax <= searchCriteria.PriceMax ||
								  property.PriceMax == null
								  select property;

			if (searchCriteria.IsValidPerformGBC())
				propertiesQuery = from property in propertiesQuery
								  join GBC in _context.GreenBuildingChecks
									  on property.ListingId equals GBC.ListingId
								  select property;

			if (searchCriteria.IsValidPerformHealthCheck())
				propertiesQuery = from property in propertiesQuery
								  join healthCheck in _context.HealthChecks
									  on property.ListingId equals healthCheck.ListingId
								  select property;

			if (searchCriteria.IsValidHour())
				propertiesQuery = from property in propertiesQuery
								  where property.RentalHour == searchCriteria.IsPerformHour
								  select property;

			if (searchCriteria.IsValidDay())
				propertiesQuery = from property in propertiesQuery
								  where property.RentalDay == searchCriteria.IsPerformDay
								  select property;

			if (searchCriteria.IsValidMonth())
				propertiesQuery = from property in propertiesQuery
								  where property.RentalMonth == searchCriteria.IsPerformMonth
								  select property;

			List<int> propertiesIds = await (from property in propertiesQuery.AsNoTracking()
											 select property.ListingId).ToListAsync();

			PaginationModel<PropertyDetailResponse> model = await PropertyPaginationModelGenerator
																												.GetPagedModelFromPropertyIds(_context,
																																							propertiesIds,
																																							currentPageNumber);
			return Ok(model);

		}

		//GET: api/Listing/GetPropertiesCommercialAndCoworkingWithFavoritesByUserId/347
		[Route("GetPropertiesCommercialAndCoworkingWithFavoritesByUserId/{userId}")]
		[HttpGet]
		public async Task<ActionResult> GetPropertiesCommercialAndCoworkingWithFavoritesByUserId(int userId)
		{
			try
			{
				ActionResult actionResult = await GetLatestPropertiesCommercialAndCoworking();
				List<UserListing> userListings = await (from userListing in _context.UserListings
														where userListing.UserId == userId
														select userListing).ToListAsync();

				Listing spaceListing = null;
				PaginationModel<PropertyDetailResponse> response = new PaginationModel<PropertyDetailResponse>();

				if (actionResult is OkObjectResult objectResult)
				{
					ObjectResult result = (ObjectResult)actionResult;
					//List<PropertyDetailResponse> response = (List<PropertyDetailResponse>)result.Value;
					response.CurrentPageData = (List<PropertyDetailResponse>)result.Value;

					foreach (UserListing userListing in userListings)
					{
						spaceListing = (from property in response.CurrentPageData
										where property.SpaceListing != null &&
										property.SpaceListing.ListingId == userListing.ListingId
										select property.SpaceListing).SingleOrDefault();
						if (spaceListing != null)
						{
							spaceListing.IsFavorite = true;
							spaceListing.FavoriteId = userListing.Id;
						}
					}
					return Ok(response);
				}
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}


		//GET: api/Listing/GetPropertiesCommercialAndCoworking
		[Route("GetPropertiesCommercialAndCoworkingWithFavorites")]
		[HttpGet]
		public async Task<ActionResult> GetPropertiesCommercialAndCoworkingWithFavorites()
		{
			try
			{
				ActionResult actionResult = await GetLatestPropertiesCommercialAndCoworking();

				PaginationModel<PropertyDetailResponse>  response = new PaginationModel<PropertyDetailResponse>();
				if (actionResult is OkObjectResult objectResult)
				{
					ObjectResult result = (ObjectResult)actionResult;
					response.CurrentPageData = (List<PropertyDetailResponse>)result.Value;
					return Ok(response);
				}
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}

		//GET: api/Listing/GetLatestOperatorList
		[Route("GetLatestOperatorList")]
		[HttpGet]
		public async Task<ActionResult> GetLatestOperatorList()
		{
			const int count = 10;
			//PaginationModel<PropertyOperatorResponse> response = new PaginationModel<PropertyOperatorResponse>();
			List<PropertyOperatorResponse> listoperators = new List<PropertyOperatorResponse>();

			List<int> top10UserIds = await (from u in _context.Users.AsNoTracking()
											where u.UserType == 1 && u.Status == true && u.CreatedDateTime != null
											orderby u.CreatedDateTime descending
											select u.UserId)
																			.Take(count)
																			.ToListAsync();


			List<User> userGroup = await (from user in _context.Users.AsNoTracking()
										  select user)
																				.Where(n => top10UserIds.Contains(n.UserId))
																				.ToListAsync();


			/* var userGroup = await (from u in _context.Users.AsNoTracking()
								   where u.UserType == 1 && u.Status == true
								   orderby u.CreatedDateTime descending
								   select u).Take(count).ToListAsync();*/

			PropertyOperatorResponse operatorResponse;

			List<Listing> listings = await (from listing in _context.Listings.AsNoTracking()
											where listing.DeletedStatus == false
											select listing).ToListAsync();

			List<REProfessionalMaster> projects = await _context.REProfessionalMasters.AsNoTracking()
																										.ToListAsync();

			if (userGroup.Count != 0)
			{
				foreach (var item in userGroup)
				{
					operatorResponse = new PropertyOperatorResponse();

					operatorResponse.Operator = new User();
					operatorResponse.Operator = item;

					List<Listing> listingsByUserID = (from listing in listings
													  where listing.UserId == item.UserId
													  select listing)
																								.ToList();

					operatorResponse.TotalCommercial = (from listing in listingsByUserID
														where listing.ListingType == "Commercial" && listing.Status == true && listing.AdminStatus == true
														select listing)
																												.Count();

					operatorResponse.TotalCoWorking = (from listing in listingsByUserID
													   where listing.ListingType == "Co-Working" &&
													   listing.Status == true && listing.AdminStatus == true
													   select listing)
																												.Count();

					operatorResponse.TotalREProfessional = (from listing in listingsByUserID
															where listing.ListingType == "RE-Professional" && listing.Status == true && listing.AdminStatus == true
															select listing)
																													.Count();

					//getting roles
					IEnumerable<int> REProfessionalIDGroup = from listing in listingsByUserID
															 where listing.Status == true && listing.ListingType == "RE-Professional"
															 select listing.ListingId;

					operatorResponse.roles = null;

					if (REProfessionalIDGroup != null)
					{
						operatorResponse.roles = new List<string>();
						foreach (var REProfessionalID in REProfessionalIDGroup)
						{
							operatorResponse.roles = projects
							.Where(d => d.ListingId == REProfessionalID && d.DeletedStatus == false)
							.Select(d => d.ProjectRole)
							.Distinct()
							.ToList();
						}
					}

					//geting linked re-prof
					var linkedREProfGroup = (from listing in listingsByUserID
											 from ReProf in projects
											 where ((listing.ListingType == "Commercial" || listing.ListingType == "Co-Working")
											 && (((listing.CMCW_ReraId != null && ReProf.PropertyReraId != null)
											 && (listing.CMCW_ReraId == ReProf.PropertyReraId))
											 || ((listing.CMCW_CTSNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == ReProf.PropertyAdditionalIdNumber))
											 || ((listing.CMCW_GatNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == ReProf.PropertyAdditionalIdNumber))
											 || ((listing.CMCW_MilkatNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == ReProf.PropertyAdditionalIdNumber))
											 || ((listing.CMCW_PlotNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == ReProf.PropertyAdditionalIdNumber))
											 || ((listing.CMCW_SurveyNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == ReProf.PropertyAdditionalIdNumber))
											 || ((listing.CMCW_PropertyTaxBillNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == ReProf.PropertyAdditionalIdNumber))
											 )
											 && (ReProf.LinkingStatus == "Approved") && ReProf.DeletedStatus == false)
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
						var ListingIdOnReProfessional = projects
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
						var REName = (from listing in listings
									  where listing.ListingId == ListingIdOnReProfessional
									  select new { listing.RE_FirstName, listing.RE_LastName })
						.First();
						REProf.REFirstName = REName.RE_FirstName;
						REProf.RELastName = REName.RE_LastName;
						operatorResponse.LinkedREProf.Add(REProf);
					}

					operatorResponse.LinkedREProfCount = operatorResponse.LinkedREProf.Count;
					listoperators.Add(operatorResponse);
				}
			}
			if (listoperators.Count > 0)
			{
				//response.CurrentPageData = listoperators;
				return Ok(listoperators);
			}
				

			return NotFound();
		}

		//GET: api/Listing/GetLatestOperatorListWithFavorites/
		[Route("GetLatestOperatorListWithFavorites")]
		[HttpGet]
		public async Task<ActionResult> GetLatestOperatorListWithFavorites()
		{
			try
			{
				ActionResult actionResult = await GetLatestOperatorList();
				PaginationModel<PropertyOperatorResponse> response = new PaginationModel<PropertyOperatorResponse>();
				//List<PropertyOperatorResponse> response = await ActionResultUtility.GetPropertyOperatorResponses
				//																				(userId,
				//																				actionResult,
				//																				_context);
				if (actionResult is OkObjectResult objectResult)
				{
					ObjectResult result = (ObjectResult)actionResult;
					response.CurrentPageData = (List<PropertyOperatorResponse>)result.Value;
					return Ok(response);
				}
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}

		//GET: api/Listing/GetLatestOperatorListWithFavoritesByUserId/1
		[Route("GetLatestOperatorListWithFavoritesByUserId/{userId}")]
		[HttpGet]
		public async Task<ActionResult<PaginationModel<PropertyOperatorResponse>>> GetLatestOperatorListWithFavoritesByUserId(int userId)
		{
			try
			{
				ActionResult actionResult = await GetLatestOperatorList();
				PaginationModel<PropertyOperatorResponse> result = new PaginationModel<PropertyOperatorResponse>();
				List<PropertyOperatorResponse> response = await ActionResultUtility.GetPropertyOperatorResponses
																								(userId,
																								actionResult,
																								_context);
				if (response != null)
				{
					result.CurrentPageData = response;
					return Ok(result);
				}

				return NotFound();
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}


		//GET: api/Listing/GetLatestPeopleList
		[Route("GetLatestPeopleList")]
		[HttpGet]
		public async Task<ActionResult> GetLatestPeopleList()
		{
			//PaginationModel<PropertyPeopleResponse> response = new PaginationModel<PropertyPeopleResponse>();
			int count = 10;
			IEnumerable<Listing> listings = await _context.Listings.ToListAsync();
			IEnumerable<User> operators = await _context.Users.ToListAsync();
			IEnumerable<REProfessionalMaster> projects;
			projects = await _context.REProfessionalMasters.ToListAsync();

			var professionals = (from listing in listings
								 join operatr in operators
								 on listing.UserId equals operatr.UserId
								 where listing.ListingType == "RE-Professional" && listing.Status == true && listing.AdminStatus == true && listing.DeletedStatus == false
								 orderby listing.CreatedDateTime
								 select new
								 {
									 operatr,
									 listing
								 }).TakeLast(count);

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
									   (listing.ListingType == "Commercial" || listing.ListingType == "Co-Working") && listing.DeletedStatus == false &&
									 (((listing.CMCW_ReraId != null && project.PropertyReraId != null) && (listing.CMCW_ReraId == project.PropertyReraId))
									   || ((listing.CMCW_CTSNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == project.PropertyAdditionalIdNumber))
									   || ((listing.CMCW_GatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == project.PropertyAdditionalIdNumber))
									   || ((listing.CMCW_MilkatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == project.PropertyAdditionalIdNumber))
									   || ((listing.CMCW_PlotNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == project.PropertyAdditionalIdNumber))
									   || ((listing.CMCW_SurveyNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == project.PropertyAdditionalIdNumber))
									   || ((listing.CMCW_PropertyTaxBillNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == project.PropertyAdditionalIdNumber))
									 )
									 && (project.LinkingStatus == "Approved") && (project.ListingId == professionalResponse.Listing.ListingId) && (project.DeletedStatus == false))
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

			if (professionalsResponse.Count > 0)
			{
				//response.CurrentPageData = professionalsResponse;
				return Ok(professionalsResponse);
			}
			return NotFound();
		}

		//refactor completed---------------------------------------------------// 

		//GET: api/Listing/GetPeopleWithFavorites
		[Route("GetPeopleWithFavorites")]
		[HttpGet]
		public async Task<ActionResult> GetPeopleWithFavorites()
		{
			try
			{
				ActionResult actionResult = await GetLatestPeopleList();

				PaginationModel<PropertyPeopleResponse> response = new PaginationModel<PropertyPeopleResponse>();
				if (actionResult is OkObjectResult objectResult)
				{
					ObjectResult result = (ObjectResult)actionResult;
					//List<PropertyPeopleResponse> response = (List<PropertyPeopleResponse>)result.Value;
					response.CurrentPageData = (List<PropertyPeopleResponse>)result.Value;

					return Ok(response);
				}
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}

		//GET: api/Listing/GetPeopleWithFavoritesByUserId/347
		[Route("GetPeopleWithFavoritesByUserId/{userId}")]
		[HttpGet]
		public async Task<ActionResult> GetPeopleWithFavoritesByUserId(int userId)
		{
			try
			{
				ActionResult actionResult = await GetLatestPeopleList();
				List<UserListing> userListings = await (from userListing in _context.UserListings
														where userListing.UserId == userId
														select userListing).ToListAsync();

				Listing spaceListing = null;
				PaginationModel<PropertyPeopleResponse> response = new PaginationModel<PropertyPeopleResponse>();
				if (actionResult is OkObjectResult objectResult)
				{
					ObjectResult result = (ObjectResult)actionResult;
					//List<PropertyPeopleResponse> response = (List<PropertyPeopleResponse>)result.Value;
					response.CurrentPageData = (List<PropertyPeopleResponse>)result.Value;

					foreach (UserListing userListing in userListings)
					{
						spaceListing = (from property in response.CurrentPageData
										where property.Listing != null &&
										property.Listing.ListingId == userListing.ListingId
										select property.Listing).SingleOrDefault();
						if (spaceListing != null)
						{
							spaceListing.IsFavorite = true;
							spaceListing.FavoriteId = userListing.Id;
						}
					}
					return Ok(response);
				}
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}

		//GET: api/Listing/GetPropertiesCommercialAndCoworkingWithFavoritesBySearch/
		[Route("GetPropertiesCommercialAndCoworkingWithFavoritesBySearch/{currentPageNumber}")]
		[HttpPost]
		public async Task<ActionResult> GetPropertiesCommercialAndCoworkingWithFavoritesBySearch( int currentPageNumber, [FromBody] PropertyUserSearchCriteria criteria)
		{
			try
			{
				ActionResult actionResult = await GetPropertyListCommercialAndCoworking(currentPageNumber, criteria);
				PaginationModel<PropertyDetailResponse> response = await ActionResultUtility.GetPropertyPageResponse(criteria.UserId,actionResult,_context);

				if (response != null)
					return Ok(response);

				return NotFound();
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}

		//GET: api/Listing/GetPeopleWithFavoritesBySearch/1
		//[Route("GetPeopleWithFavoritesBySearch/{UserId}")]
		//[HttpPost]
		//public async Task<ActionResult> GetPeopleWithFavoritesBySearch(int UserId, [FromBody] PeopleSearchCriteria criteria)
		//{
		//	try
		//	{
		//		ActionResult actionResult = await GetPeopleList(criteria);

		//		List<UserListing> userListings = await (from userListing in _context.UserListings
		//												where userListing.UserId == UserId
		//												select userListing).ToListAsync();

		//		Listing spaceListing = null;

		//		if (actionResult is OkObjectResult objectResult)
		//		{
		//			ObjectResult result = (ObjectResult)actionResult;
		//			List<PropertyPeopleResponse> response = (List<PropertyPeopleResponse>)result.Value;

		//			foreach (UserListing userListing in userListings)
		//			{
		//				spaceListing = (from property in response
		//								where property.Listing != null &&
		//								property.Listing.ListingId == userListing.ListingId
		//								select property.Listing).SingleOrDefault();
		//				if (spaceListing != null)
		//				{
		//					spaceListing.IsFavorite = true;
		//					spaceListing.FavoriteId = userListing.Id;
		//				}
		//			}
		//			return Ok(response);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		StatusCode(StatusCodes.Status500InternalServerError);
		//	}

		//	return NotFound();
		//}

		//GET: api/Listing/GetPeopleWithFavoritesBySearchPaged/1/1
		[Route("GetPeopleWithFavoritesBySearchPaged/{UserId}/{currentPageNumber}")]
		[HttpPost]
		public async Task<ActionResult> GetPeopleWithFavoritesBySearchPaged(int UserId, int currentPageNumber, [FromBody] PeopleSearchCriteria criteria)
		{
			try
			{
				ActionResult actionResult = await GetPeopleListPaged(currentPageNumber, criteria);

				List<UserListing> userListings = await (from userListing in _context.UserListings
														where userListing.UserId == UserId
														select userListing).ToListAsync();

				Listing spaceListing = null;

				if (actionResult is OkObjectResult objectResult)
				{
					ObjectResult result = (ObjectResult)actionResult;
					PaginationModel<PropertyPeopleResponse> response = (PaginationModel<PropertyPeopleResponse>)result.Value;

					foreach (UserListing userListing in userListings)
					{
						spaceListing = (from professional in response.CurrentPageData
										where professional.Listing != null &&
													professional.Listing.ListingId == userListing.ListingId
										select professional.Listing)
																			.SingleOrDefault();

						if (spaceListing != null)
						{
							spaceListing.IsFavorite = true;
							spaceListing.FavoriteId = userListing.Id;
						}
					}
					return Ok(response);
				}
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}

		//[Route("GetAllPropertyListCommercialAndCoworking")]
		[HttpGet]
		public async Task<ActionResult> GetAllPropertyListCommercialAndCoworking()

		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();

			List<Listing> properties = await _context.Listings.Where(m => m.Status == true &&
																																   m.AdminStatus == true &&
																																   m.ListingType != "RE-Professional" &&
																																   m.DeletedStatus == false)
																																   .ToListAsync();

			if (properties == null)
				return BadRequest();

			PropertyDetailResponse property;

			foreach (var item in properties)

			{
				property = new PropertyDetailResponse();
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
												 select healthCheck).Count();

				IEnumerable<GreenBuildingCheck> greenBldingChecks = await _context.GreenBuildingChecks.ToListAsync();
				property.AvailableGreenBuildingCheck = (from GBC in greenBldingChecks
														where GBC.ListingId == item.ListingId && GBC.Status == true
														select GBC)
													.Count();

				//geting linked re-prof			
				var linkedREProf = (from r in _context.REProfessionalMasters
									where (
									(((r.PropertyReraId != null && item.CMCW_ReraId != null) && (r.PropertyReraId == item.CMCW_ReraId))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_CTSNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_CTSNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_MilkatNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_MilkatNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_PlotNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_PlotNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_SurveyNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_SurveyNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_PropertyTaxBillNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_PropertyTaxBillNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_GatNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_GatNumber)))
										 && (r.LinkingStatus == "Approved") && (r.DeletedStatus == false))
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
			if (vModel.Count > 0)
				return Ok(vModel);

			return NotFound();

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

		[Route("GetAllOperatorList/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetAllOperatorList(int currentPageNumber)
		{

			List<int> allOperatorIds = await (from REOperator in _context.Users
											  where REOperator.UserType == 1 && REOperator.Status == true
											  select REOperator.UserId).ToListAsync();

			PaginationModel<PropertyOperatorResponse> model = await OperatorPaginationModelGenerator.GetPagedModelFromOperatorIds(_context, allOperatorIds, currentPageNumber);

			return Ok(model);

		}

		[Route("GetAllOperatorListWithFavorites/{userId}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetAllOperatorListWithFavorites(int userId, int currentPageNumber)
		{
			try
			{
				ActionResult actionResult = await GetAllOperatorList(currentPageNumber);
				PaginationModel<PropertyOperatorResponse> response = await ActionResultUtility.GetOperatorPageResponse
																								(userId,
																								actionResult,
																								_context);
				if (response != null)
					return Ok(response);

				return NotFound();
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();

		}


		[Route("GetAllOperatorListPaged/")]
		[HttpGet]
		public async Task<ActionResult<List<PropertyOperatorResponse>>> GetAllOperatorListPaged()
		{
			List<PropertyOperatorResponse> listoperators = new List<PropertyOperatorResponse>();

			var userGroup = (from u in _context.Users
							 where u.UserType == 1 && u.Status == true
							 select u).ToList();

			if (userGroup != null)
			{
				foreach (var item in userGroup)
				{
					PropertyOperatorResponse operatorResponse = new PropertyOperatorResponse();

					operatorResponse.Operator = new User();
					operatorResponse.Operator = item;

					IEnumerable<Listing> listingsByUserID = await (from listing in _context.Listings
																   where listing.UserId == item.UserId && listing.DeletedStatus == false
																   select listing).ToListAsync();

					operatorResponse.TotalCommercial = (from listing in listingsByUserID
														where listing.ListingType == "Commercial" && listing.Status == true && listing.AdminStatus == true && listing.DeletedStatus == false
														select listing)
														.Count();

					operatorResponse.TotalCoWorking = (from listing in listingsByUserID
													   where listing.ListingType == "Co-Working" && listing.Status == true && listing.AdminStatus == true && listing.DeletedStatus == false
													   select listing)
														.Count();

					operatorResponse.TotalREProfessional = (from listing in listingsByUserID
															where listing.ListingType == "RE-Professional" && listing.Status == true && listing.AdminStatus == true && listing.DeletedStatus == false
															select listing)
															.Count();

					//getting roles
					IEnumerable<int> REProfessionalIDGroup = from listing in listingsByUserID
															 where listing.Status == true && listing.ListingType == "RE-Professional" && listing.DeletedStatus == false
															 select listing.ListingId;

					operatorResponse.roles = null;

					if (REProfessionalIDGroup != null)
					{
						operatorResponse.roles = new List<string>();
						foreach (var REProfessionalID in REProfessionalIDGroup)
						{
							operatorResponse.roles = await _context.REProfessionalMasters
													.Where(d => d.ListingId == REProfessionalID && d.DeletedStatus == false)
													.Select(d => d.ProjectRole)
													.Distinct()
													.ToListAsync();
						}
					}

					//geting linked re-prof
					var linkedREProfGroup = (from listing in listingsByUserID
											 from ReProf in _context.REProfessionalMasters
											 where ((listing.ListingType == "Commercial" || listing.ListingType == "Co-Working") && listing.DeletedStatus == false &&
											  (((listing.CMCW_ReraId != null && ReProf.PropertyReraId != null) && (listing.CMCW_ReraId == ReProf.PropertyReraId))
										 || ((listing.CMCW_CTSNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == ReProf.PropertyAdditionalIdNumber))
										 || ((listing.CMCW_GatNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == ReProf.PropertyAdditionalIdNumber))
										 || ((listing.CMCW_MilkatNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == ReProf.PropertyAdditionalIdNumber))
										 || ((listing.CMCW_PlotNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == ReProf.PropertyAdditionalIdNumber))
										 || ((listing.CMCW_SurveyNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == ReProf.PropertyAdditionalIdNumber))
										 || ((listing.CMCW_PropertyTaxBillNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == ReProf.PropertyAdditionalIdNumber))
										  )
											  && (ReProf.LinkingStatus == "Approved") && ReProf.DeletedStatus == false)
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
			if (listoperators.Count > 0)
				return listoperators;

			return NotFound();

		}

		//[Route("GetAllPeopleList")]
		//[HttpGet]
		//public async Task<ActionResult> GetAllPeopleList()
		//{
		//	IEnumerable<Listing> listings = await _context.Listings.ToListAsync();
		//	IEnumerable<User> operators = await _context.Users.ToListAsync();
		//	IEnumerable<REProfessionalMaster> projects;
		//	projects = await _context.REProfessionalMasters.ToListAsync();

		//	var professionals = (from listing in listings
		//						 join operatr in operators
		//						 on listing.UserId equals operatr.UserId
		//						 where listing.ListingType == "RE-Professional" && listing.Status == true && listing.AdminStatus == true && listing.DeletedStatus == false
		//						 select new
		//						 {
		//							 operatr,
		//							 listing
		//						 });
		//	if (professionals == null)
		//		return NotFound();

		//	List<PropertyPeopleResponse> professionalsResponse = new List<PropertyPeopleResponse>();
		//	PropertyPeopleResponse professionalResponse;

		//	foreach (var item in professionals)
		//	{
		//		professionalResponse = new PropertyPeopleResponse();

		//		professionalResponse.Operator = new User();
		//		professionalResponse.Operator = item.operatr;
		//		//p.Operator.UserId = item.a.UserId;
		//		//p.Operator.UserType = item.a.UserType;
		//		//p.ListingId = item.ListingId;
		//		professionalResponse.Listing = (from listing in listings
		//										where listing.ListingId == item.listing.ListingId
		//										select listing)
		//								.SingleOrDefault();

		//		professionalResponse.Projects = (from project in projects
		//										 where project.ListingId == item.listing.ListingId
		//										 select project).ToList();

		//		professionalResponse.Roles = (from project in professionalResponse.Projects
		//									  select project.ProjectRole)
		//									.Distinct()
		//									.ToList();

		//		professionalResponse.TotalProjects = professionalResponse.Projects.Count();
		//		//geting linked Operator
		//		var linkedOperators = (from listing in listings
		//							   from project in projects
		//							   where (
		//							   (listing.ListingType == "Commercial" || listing.ListingType == "Co-Working") && listing.DeletedStatus == false &&
		//								(((listing.CMCW_ReraId != null && project.PropertyReraId != null) && (listing.CMCW_ReraId == project.PropertyReraId))
		//						   || ((listing.CMCW_CTSNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_GatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_MilkatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_PlotNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_SurveyNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == project.PropertyAdditionalIdNumber))
		//						   || ((listing.CMCW_PropertyTaxBillNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == project.PropertyAdditionalIdNumber))
		//							)
		//								&& (project.LinkingStatus == "Approved") && (project.ListingId == professionalResponse.Listing.ListingId) && (project.DeletedStatus == false))
		//							   select new
		//							   {
		//								   listing.ListingId,
		//								   project.REProfessionalMasterId,
		//								   listing.UserId,
		//								   project.ProjectRole,
		//								   project.ProjectName,
		//								   project.ImageUrl,
		//								   project.OperatorName,
		//								   project.LinkingStatus
		//							   }).ToList();
		//		professionalResponse.LinkedOpr = new List<LinkedOperators>();

		//		LinkedOperators Oper;
		//		foreach (var linked in linkedOperators)
		//		{
		//			Oper = new LinkedOperators();
		//			int listingIdOnReProfessional = (from project in projects
		//											 where project.REProfessionalMasterId == linked.REProfessionalMasterId
		//											 select project.ListingId)
		//											.First();
		//			//var GetListingIdOnpro = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.UserId).First();
		//			Oper.Property_ListingId = linked.ListingId;
		//			Oper.ReProfessional_ListingId = listingIdOnReProfessional;
		//			Oper.REProfessionalMasterId = linked.REProfessionalMasterId;
		//			Oper.UserId = linked.UserId;
		//			Oper.ProjectRole = linked.ProjectRole;
		//			Oper.ProjectName = linked.ProjectName;

		//			Oper.PropertyName = (from listing in listings
		//								 where listing.ListingId == linked.ListingId
		//								 select listing.Name)
		//								.First();

		//			Oper.CompanyName = (from operatr in operators
		//								where operatr.UserId == linked.UserId
		//								select operatr.CompanyName)
		//								.First();

		//			Oper.Doc_CompanyLogo = (from operatr in operators
		//									where operatr.UserId == linked.UserId
		//									select operatr.Doc_CompanyLogo)
		//									.First();

		//			professionalResponse.LinkedOpr.Add(Oper);
		//		}
		//		professionalsResponse.Add(professionalResponse);
		//	}

		//	if (professionalsResponse.Count > 0)
		//		return Ok(professionalsResponse);
		//	return NotFound();
		//}


		[Route("GetAllPeopleListPaged/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetAllPeopleListPaged(int currentPageNumber)
		{
			//IEnumerable<Listing> listings = await _context.Listings.ToListAsync();
			IEnumerable<User> operators = await _context.Users.ToListAsync();
			IEnumerable<REProfessionalMaster> projects;
			projects = await _context.REProfessionalMasters.ToListAsync();

			List<int> allPeopleIds = await (from listing in _context.Listings.AsNoTracking()
											where listing.ListingType == "RE-Professional" &&
														listing.Status == true &&
														listing.AdminStatus == true &&
														listing.DeletedStatus == false
											orderby listing.CreatedDateTime descending
											select listing.ListingId).ToListAsync();

			if (allPeopleIds.Count() == 0)
				return NotFound();

			PaginationModel<PropertyPeopleResponse> model = await PeoplePaginationModelGenerator.GetPagedModelFromPropertyIds(_context,allPeopleIds,currentPageNumber);
			return Ok(model);
		}


		//[Route("GetAllPeopleListWithFavorites/{userId}")]
		//[HttpGet]
		//public async Task<ActionResult> GetAllPeopleListWithFavorites(int userId)
		//{
		//	try
		//	{
		//		ActionResult actionResult = await GetAllPeopleListPaged(1);
		//		List<PropertyPeopleResponse> response = await ActionResultUtility.GetPropertyPeopleResponses(userId,actionResult,_context);

		//		if (response != null)
		//			return Ok(response);

		//		return NotFound();
		//	}
		//	catch (Exception ex)
		//	{
		//		StatusCode(StatusCodes.Status500InternalServerError);
		//	}

		//	return NotFound();
		//}
		[Route("GetAllPeopleListWithFavoritesPaged/{userId}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetAllPeopleListWithFavoritesPaged(int userId, int currentPageNumber)
		{
			try
			{
				ActionResult actionResult = await GetAllPeopleListPaged(currentPageNumber);
				PaginationModel<PropertyPeopleResponse> response = await ActionResultUtility.GetPeoplePageResponse
																								(userId,
																								actionResult,
																								_context);
				if (response != null)
					return Ok(response);

				return NotFound();
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}

		[Route("GetAllPropertyListCommercialAndCoworkingWithFavorites/{userId}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetAllPropertyListCommercialAndCoworkingWithFavorites(int userId, int currentPageNumber)
		{
			try
			{
				ActionResult actionResult = await GetAllPropertyListCommercialAndCoworkingPaged(currentPageNumber);
				PaginationModel<PropertyDetailResponse> response = await ActionResultUtility.GetPropertyPageResponse
																									  (userId,
																										actionResult,
																										_context);

				if (response != null)
					return Ok(response);

				return NotFound();
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}



		//pagination calls
		[Route("GetAllPropertyListCommercialAndCoworkingPaged/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetAllPropertyListCommercialAndCoworkingPaged(int currentPageNumber)
		{
			List<int> allPropertyIds = await (from listing in _context.Listings.AsNoTracking()
											  where listing.Status == true &&
														  listing.AdminStatus == true &&
														  listing.ListingType != "RE-Professional" &&
														  listing.DeletedStatus == false
											  orderby listing.ListingId
											  select listing.ListingId)
																															.ToListAsync();

			if (allPropertyIds.Count <= 0)
				NotFound();

			PaginationModel<PropertyDetailResponse> pagedModel = await PropertyPaginationModelGenerator.GetPagedModelFromPropertyIds(_context,
																																		allPropertyIds,
																																		currentPageNumber);

			/*await GetPagedModelFromPropertyIds(
																																		allPropertyIds,
																																		currentPageNumber);*/
			return Ok(pagedModel);
		}

		//async Task<PaginationModel<PropertyDetailResponse>> GetPagedModelFromPropertyIds(
		//																															  List<int> allPropertyIds,
		//																															  int currentPageNumber)
		//{
		//	PaginationModel<PropertyDetailResponse> pagedModel = new PaginationModel<PropertyDetailResponse>();
		//	pagedModel.Count = allPropertyIds.Count;
		//	pagedModel.CurrentPage = currentPageNumber;

		//	List<int> currentPagePropertyIds = GetCurrentPagePropertyIds(currentPageNumber,
		//																																			allPropertyIds,
		//																																			pagedModel.PageSize);
		//	List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
		//	vModel = await PropertyPaginationModelGenerator.GetPropertyDetailResponseList(_context, currentPagePropertyIds);
		//	pagedModel.CurrentPageData = vModel;

		//	return pagedModel;
		//}

		//List<int> GetCurrentPagePropertyIds(int currentPageNumber, List<int> propertyIds, int pageSize)
		//{
		//	int countToSkip = (currentPageNumber - 1) * pageSize;
		//	propertyIds = propertyIds.
		//													Skip(countToSkip)
		//													.Take(pageSize)
		//													.ToList();
		//	return propertyIds;
		//}

	}
}
