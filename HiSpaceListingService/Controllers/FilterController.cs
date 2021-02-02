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
using HiSpaceListingService.Utilities;

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

		//get the max price
		[HttpGet("GetMaxPrice/{SearchFor}/{Hour}/{Day}/{Month}")]
		public async Task<decimal?> GetMaxPrice(string SearchFor, bool Hour, bool Day, bool Month)
		{
			var Listings = await _context.Listings.AsNoTracking().ToListAsync();
			//var Price = Listings.AsEnumerable().Max(row => row.PriceMax).ToString();
			decimal? MaxValue = 0;
			if (SearchFor == "All" || SearchFor == "Sale")
			{
				SearchFor = "Sale";
				MaxValue = (from d in Listings
							where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
							select d.PriceMax).Max();
			}
			else
			{
				MaxValue = (from d in Listings
							where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
							select d.PriceMax).Max();
			}

			//if (SearchFor == "Rental")
			//{
			//	if (Hour == true && Day == true && Month == true)
			//		MaxValue = (from d in Listings
			//					where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
			//					select d.PriceMax).Max();

			//	else if (Hour == false && Day == false && Month == false)
			//		MaxValue = (from d in Listings
			//					where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
			//					select d.PriceMax).Max();

			//	else if ((Hour == true) && (Day == false && Month == false))
			//		MaxValue = (from d in Listings
			//					where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
			//					select d.PriceMax).Max();

			//	else if ((Day == true) && (Hour == false && Month == false))
			//		MaxValue = (from d in Listings
			//					where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
			//					select d.PriceMax).Max();

			//	else if ((Month == true) && (Hour == false && Day == false))
			//		MaxValue = (from d in Listings
			//					where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
			//					select d.PriceMax).Max();

			//	else if ((Hour == false) && (Day == true && Month == true))
			//		MaxValue = (from d in Listings
			//					where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
			//					select d.PriceMax).Max();

			//	else if ((Day == false) && (Hour == true && Month == true))
			//		MaxValue = (from d in Listings
			//					where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
			//					select d.PriceMax).Max();

			//	else if ((Month == false) && (Hour == true && Day == true))
			//		MaxValue = (from d in Listings
			//					where d.CMCW_PropertyFor == SearchFor && d.RentalHour == Hour && d.RentalDay == Day && d.RentalMonth == Month
			//					select d.PriceMax).Max();
			//}

			if (MaxValue == null)
				MaxValue = 100000;

			return MaxValue;

		}

		/// <summary>
		/// </summary>
		/// <returns>The list of Properties by its location.</returns>
		// GET: api/Listing/GetListingByLocation
		[HttpGet("GetListingPropertyByLocation/{Location}/{currentPageNumber}")]
		public async Task<ActionResult> GetListingPropertyByLocation(string Location, int currentPageNumber)
		{

			List<int> filteredPropertyIds = await (from property in _context.Listings.AsNoTracking()
												   where property.Status == true &&
															   property.AdminStatus == true &&
															   property.ListingType != "RE-Professional" &&
															   property.DeletedStatus == false &&
															   property.locality == Location
												   orderby property.CreatedDateTime descending
												   select property.ListingId)
																										.ToListAsync();

			if (filteredPropertyIds.Count <= 0)
				NotFound();

			PaginationModel<PropertyDetailResponse> pagedModel = await PropertyPaginationModelGenerator.GetPagedModelFromPropertyIds(_context,
																																			filteredPropertyIds,
																																			currentPageNumber);
			return Ok(pagedModel);
		}

		[Route("GetListingPropertyByLocationWithFavorites/{userId}/{Location}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetListingPropertyByLocationWithFavorites(int userId, string Location, int currentPageNumber)
		{
			try
			{
				ActionResult actionResult = await GetListingPropertyByLocation(Location, currentPageNumber);
				PaginationModel<PropertyDetailResponse> response = await ActionResultUtility.GetPropertyDetailResponses
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

		/// <summary>
		/// Get property list by type
		/// </summary>
		/// <returns>The list of Properties by its type.</returns>
		// GET: api/Listing/GetListingByType
		[HttpGet("GetListingPropertyByType/{ListingType}/{currentPageNumber}")]
		public async Task<ActionResult> GetListingPropertyByType(string ListingType, int currentPageNumber)
		{
			List<int> filteredPropertyIds = await (from property in _context.Listings.AsNoTracking()
												   where property.AdminStatus == true &&
												   property.ListingType != "RE-Professional" &&
												   property.ListingType == ListingType &&
												   property.DeletedStatus == false
												   orderby property.CreatedDateTime descending
												   select property.ListingId)
			.ToListAsync();

			PaginationModel<PropertyDetailResponse> pagedModel = await PropertyPaginationModelGenerator.GetPagedModelFromPropertyIds(_context,
			filteredPropertyIds,
			currentPageNumber);
			return Ok(pagedModel);
		}

		[Route("GetListingPropertyByTypeWithFavorites/{userId}/{ListingType}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetListingPropertyByTypeWithFavorites(int userId,string ListingType, int currentPageNumber)
		{
			try
			{
				ActionResult actionResult = await GetListingPropertyByType(ListingType, currentPageNumber);
				PaginationModel<PropertyDetailResponse> pagedModel = await
				ActionResultUtility.GetPropertyPageResponse(userId,
				actionResult,
				_context);

				if (pagedModel != null)
					return Ok(pagedModel);

				return NotFound();
			}
			catch (Exception ex)
			{
				StatusCode(StatusCodes.Status500InternalServerError);
			}

			return NotFound();
		}

		/// <summary>
		/// Get property list by user
		/// </summary>
		/// <returns>The list of Properties by its user.</returns>
		// GET: api/Listing/GetListingByUser
		[HttpGet("GetListingPropertyByUser/{User}")]
		public async Task<ActionResult> GetListingPropertyByUser(int User)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			IEnumerable<Listing> properties = await _context.Listings.Where(m => m.Status == true && m.AdminStatus == true && m.UserId == User && m.ListingType != "RE-Professional" && m.DeletedStatus == false).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

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

		/// <summary>
		/// Get favorite property list by user
		/// </summary>
		/// <returns>The list of favorite Properties by its user.</returns>
		// GET: api/Listing/GetListingPropertyByUserWithFavorites/LoginUserId/SearchUserId
		[HttpGet("GetListingPropertyByUserWithFavorites/{LoginUserId}/{SearchUserId}")]
		public async Task<ActionResult> GetListingPropertyByUserWithFavorites(int LoginUserId, int SearchUserId)
		{
			try
			{
				ActionResult actionResult = await GetListingPropertyByUser(SearchUserId);
				PaginationModel<PropertyDetailResponse> response = await ActionResultUtility.GetPropertyDetailResponses(LoginUserId, actionResult,_context);

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

		[Route("GetOperatorByUserId/{User}")]
		[HttpGet]
		public async Task<ActionResult<List<PropertyOperatorResponse>>> GetOperatorByUserId(int User)
		{
			List<PropertyOperatorResponse> listoperators = new List<PropertyOperatorResponse>();

			var userGroup = (from u in _context.Users
							 where u.UserId == User && u.Status == true
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
													.Where(d => d.ListingId == REProfessionalID)
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

		[Route("GetPeopleByListingId/{ListingId}")]
		[HttpGet]
		public async Task<ActionResult> GetPeopleByListingId(int ListingId)
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
									   (listing.ListingType == "Commercial" || listing.ListingType == "Co-Working") && listing.DeletedStatus == false &&
										(((listing.CMCW_ReraId != null && project.PropertyReraId != null) && (listing.CMCW_ReraId == project.PropertyReraId))
								   || ((listing.CMCW_CTSNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_GatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_MilkatNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_PlotNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_SurveyNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == project.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_PropertyTaxBillNumber != null && project.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == project.PropertyAdditionalIdNumber))
									)
										&& (project.LinkingStatus == "Approved") && (project.ListingId == professionalResponse.Listing.ListingId) && project.DeletedStatus == false)
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
				return Ok(professionalsResponse);
			return NotFound();

		}

		[Route("GetPeopleByListingIdWithFavorites/{userId}/{ListingId}")]
		[HttpGet]
		public async Task<ActionResult> GetPeopleByListingIdWithFavorites(int userId, int ListingId)
		{
			try
			{
				ActionResult actionResult = await GetPeopleByListingId(ListingId);
				List<PropertyPeopleResponse> response = await ActionResultUtility.GetPropertyPeopleResponses(userId,actionResult,_context);

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

		/// <summary>
		/// Get property and professionals list by user
		/// </summary>
		/// <returns>The list of favorite Properties and professionals by its user.</returns>
		// GET: api/Listing/GetFavoritesByUser
		[HttpGet("GetFavoritesByUser/{User}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetFavoritesByUser(int User)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			List<Listing> propertiesAndProfessionals = await _context.Listings.Where(m => m.Status == true && m.AdminStatus == true && m.DeletedStatus == false).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

			if (propertiesAndProfessionals.Count() == 0)
				return NotFound();

			List<UserListing> userListings = await (from userListing in _context.UserListings
													where userListing.UserId == User
													select userListing).ToListAsync();

			propertiesAndProfessionals = propertiesAndProfessionals
																	.Join(userListings, p => p.ListingId, u => u.ListingId, (p, u) =>
																	{
																		p.IsFavorite = true;
																		p.FavoriteId = u.Id;
																		return p;
																	}).ToList();

			List<User> users = await _context.Users.AsNoTracking().ToListAsync();
			List<Amenity> amenities = await _context.Amenitys.AsNoTracking().ToListAsync();
			List<Facility> facilities = await _context.Facilitys.AsNoTracking().ToListAsync();
			IEnumerable<REProfessionalMaster> projects = await _context.REProfessionalMasters.AsNoTracking().ToListAsync();
			IEnumerable<ListingImages> images = await _context.ListingImagess.AsNoTracking().ToListAsync();
			IEnumerable<HealthCheck> healthChecks = await _context.HealthChecks.AsNoTracking().ToListAsync();
			IEnumerable<GreenBuildingCheck> greenBldingChecks = await _context.GreenBuildingChecks
																																	.AsNoTracking().ToListAsync();

			PropertyDetailResponse propertyAndProfessional;

			foreach (var item in propertiesAndProfessionals)
			{
				propertyAndProfessional = new PropertyDetailResponse();
				propertyAndProfessional.SpaceListing = item;
				propertyAndProfessional.SpaceUser = users.SingleOrDefault(d => d.UserId == item.UserId);

				propertyAndProfessional.AvailableAmenities = (from amenity in amenities
															  where amenity.ListingId == item.ListingId && amenity.Status == true
															  select amenity)
										.Count();

				propertyAndProfessional.AvailableFacilities = (from facility in facilities
															   where facility.ListingId == item.ListingId && facility.Status == true
															   select facility)
										.Count();

				propertyAndProfessional.AvailableProjects = (from project in projects
															 where project.ListingId == item.ListingId && project.Status == true
															 select project)
									.Count();

				propertyAndProfessional.ListingImagesList = (from image in images
															 where image.ListingId == item.ListingId
															 select image)
											.ToList();

				propertyAndProfessional.AvailableHealthCheck = (from healthCheck in healthChecks
																where healthCheck.ListingId == item.ListingId && healthCheck.Status == true
																select healthCheck).Count();

				propertyAndProfessional.AvailableGreenBuildingCheck = (from GBC in greenBldingChecks
																	   where GBC.ListingId == item.ListingId && GBC.Status == true
																	   select GBC)
													.Count();

				//geting linked re-prof
				propertyAndProfessional.LinkedREProf = null;

				propertyAndProfessional.LinkedREProfCount = 0;
				vModel.Add(propertyAndProfessional);
			}
			if (vModel.Count > 0)
				return Ok(vModel);

			return NotFound();
		}

	}
}