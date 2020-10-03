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
	public class FilterController : ControllerBase
	{
		private readonly HiSpaceListingContext _context;

		public FilterController(HiSpaceListingContext context)
		{
			_context = context;
		}

		/// <summary>
		/// </summary>
		/// <returns>The list of Properties by its location.</returns>
		// GET: api/Listing/GetListingByLocation
		[HttpGet("GetListingPropertyByLocation/{Location}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingPropertyByLocation(string Location)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			IEnumerable<Listing> properties = await _context.Listings.Where(m => m.Status == true && m.AdminStatus == true && m.locality == Location && m.ListingType != "RE-Professional").OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			if (properties == null)
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
												 select healthCheck).Count();

				IEnumerable<GreenBuildingCheck> greenBldingChecks = await _context.GreenBuildingChecks.ToListAsync();
				property.AvailableGreenBuildingCheck = (from GBC in greenBldingChecks
														where GBC.ListingId == item.ListingId && GBC.Status == true
														select GBC)
													.Count();

				//geting linked re-prof			
				var linkedREProf = (from r in _context.REProfessionalMasters
									where (
									((r.PropertyReraId != null && item.CMCW_ReraId != null) && (r.PropertyReraId == item.CMCW_ReraId))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_CTSNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_CTSNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_MilkatNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_MilkatNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_PlotNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_PlotNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_SurveyNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_SurveyNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_PropertyTaxBillNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_PropertyTaxBillNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_GatNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_GatNumber))
										 && (r.LinkingStatus == "Approved"))
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
				return vModel;

			return NotFound();

		}

		/// <summary>
		/// Get property list by type
		/// </summary>
		/// <returns>The list of Properties by its type.</returns>
		// GET: api/Listing/GetListingByType
		[HttpGet("GetListingPropertyByType/{Type}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingPropertyByType(string Type)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			IEnumerable<Listing> properties = await _context.Listings.Where(m => m.Status == true && m.AdminStatus == true && m.ListingType == Type && m.ListingType != "RE-Professional").OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			if (properties == null)
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
												 select healthCheck).Count();

				IEnumerable<GreenBuildingCheck> greenBldingChecks = await _context.GreenBuildingChecks.ToListAsync();
				property.AvailableGreenBuildingCheck = (from GBC in greenBldingChecks
														where GBC.ListingId == item.ListingId && GBC.Status == true
														select GBC)
													.Count();

				//geting linked re-prof			
				var linkedREProf = (from r in _context.REProfessionalMasters
									where (
									((r.PropertyReraId != null && item.CMCW_ReraId != null) && (r.PropertyReraId == item.CMCW_ReraId))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_CTSNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_CTSNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_MilkatNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_MilkatNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_PlotNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_PlotNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_SurveyNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_SurveyNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_PropertyTaxBillNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_PropertyTaxBillNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_GatNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_GatNumber))
										 && (r.LinkingStatus == "Approved"))
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
				return vModel;

			return NotFound();
		}

		/// <summary>
		/// Get property list by user
		/// </summary>
		/// <returns>The list of Properties by its user.</returns>
		// GET: api/Listing/GetListingByUser
		[HttpGet("GetListingPropertyByUser/{User}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingPropertyByUser(int User)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			IEnumerable<Listing> properties = await _context.Listings.Where(m => m.Status == true && m.AdminStatus == true && m.UserId == User && m.ListingType != "RE-Professional").OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			if (properties == null)
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
												 select healthCheck).Count();

				IEnumerable<GreenBuildingCheck> greenBldingChecks = await _context.GreenBuildingChecks.ToListAsync();
				property.AvailableGreenBuildingCheck = (from GBC in greenBldingChecks
														where GBC.ListingId == item.ListingId && GBC.Status == true
														select GBC)
													.Count();

				//geting linked re-prof			
				var linkedREProf = (from r in _context.REProfessionalMasters
									where (
									((r.PropertyReraId != null && item.CMCW_ReraId != null) && (r.PropertyReraId == item.CMCW_ReraId))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_CTSNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_CTSNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_MilkatNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_MilkatNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_PlotNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_PlotNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_SurveyNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_SurveyNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_PropertyTaxBillNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_PropertyTaxBillNumber))
										 || ((r.PropertyAdditionalIdNumber != null && item.CMCW_GatNumber != null) && (r.PropertyAdditionalIdNumber == item.CMCW_GatNumber))
										 && (r.LinkingStatus == "Approved"))
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
				return vModel;

			return NotFound();
		}

		[Route("GetOperatorByUserId/{User}")]
		[HttpGet]
		public async Task<ActionResult<List<PropertyOperatorResponse>>> GetOperatorByUserId(int User)
		{
			List<PropertyOperatorResponse> listoperators = new List<PropertyOperatorResponse>();

			var userGroup = (from u in _context.Users
						 where u.UserId == User && u.Status == true
						 select u).ToList();
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
														where listing.ListingType == "Commercial" && listing.Status == true && listing.AdminStatus == true
														select listing)
														.Count();
				
				operatorResponse.TotalCoWorking = (from listing in listingsByUserID
													where listing.ListingType == "Co-Working" && listing.Status == true && listing.AdminStatus == true
													select listing)
													.Count();
				
				operatorResponse.TotalREProfessional = (from listing in listingsByUserID
														where listing.ListingType == "RE-Professional" && listing.Status == true && listing.AdminStatus == true
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
									 (((listing.CMCW_ReraId != null && ReProf.PropertyReraId != null) && (listing.CMCW_ReraId == ReProf.PropertyReraId))
								|| ((listing.CMCW_CTSNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == ReProf.PropertyAdditionalIdNumber))
								|| ((listing.CMCW_GatNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == ReProf.PropertyAdditionalIdNumber))
								|| ((listing.CMCW_MilkatNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == ReProf.PropertyAdditionalIdNumber))
								|| ((listing.CMCW_PlotNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == ReProf.PropertyAdditionalIdNumber))
								|| ((listing.CMCW_SurveyNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == ReProf.PropertyAdditionalIdNumber))
								|| ((listing.CMCW_PropertyTaxBillNumber != null && ReProf.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == ReProf.PropertyAdditionalIdNumber))
								 )
									 && (ReProf.LinkingStatus == "Approved"))
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

		[Route("GetPeopleByListingId/{ListingId}")]
		[HttpGet]
		public async Task<ActionResult<List<PropertyPeopleResponse>>> GetPeopleByListingId(int ListingId)
		{
			IEnumerable<Listing> listings = await _context.Listings.ToListAsync();
			IEnumerable<User> operators = await _context.Users.ToListAsync();
			IEnumerable<REProfessionalMaster> projects;
			projects = await _context.REProfessionalMasters.ToListAsync();

			var professionals = (from listing in listings
								 join operatr in operators
								 on listing.UserId equals operatr.UserId
								 where listing.ListingType == "RE-Professional" && listing.ListingId == ListingId
								 select new
								 {
									 operatr,
									 listing
								 });
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
										(((listing.CMCW_ReraId != null && project.PropertyReraId != null) && (listing.CMCW_ReraId == project.PropertyReraId))
								   || ((listing.CMCW_CTSNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_GatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_MilkatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_PlotNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_SurveyNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_PropertyTaxBillNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == project.PropertyAdditionalIdNumber))
									)
										&& (project.LinkingStatus == "Approved") && (project.ListingId == professionalResponse.Listing.ListingId))
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
				return professionalsResponse;
			return NotFound();

		}

	}
}