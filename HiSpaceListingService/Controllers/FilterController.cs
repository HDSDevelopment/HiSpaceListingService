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

			PaginationModel<PropertyDetailResponse> pagedModel = await PropertyPaginationModelGenerator.GetPagedModelFromPropertyIds(_context,filteredPropertyIds,currentPageNumber);
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
		[HttpGet("GetListingPropertyByUser/{User}/{currentPageNumber}")]
		public async Task<ActionResult> GetListingPropertyByUser(int User, int currentPageNumber)
		{
			List<int> filteredPropertyIds = await (from property in _context.Listings.AsNoTracking()
												   where property.Status == true &&
												   property.AdminStatus == true &&
												   property.UserId == User &&
												   property.ListingType != "RE-Professional" &&
												   property.DeletedStatus == false
												   orderby property.CreatedDateTime descending
												   select property.ListingId)
			.ToListAsync();
			PaginationModel<PropertyDetailResponse> pagedModel = await PropertyPaginationModelGenerator.GetPagedModelFromPropertyIds(_context,
			filteredPropertyIds,
			currentPageNumber);
			return Ok(pagedModel);
		}

		/// <summary>
		/// Get favorite property list by user
		/// </summary>
		/// <returns>The list of favorite Properties by its user.</returns>
		// GET: api/Listing/GetListingPropertyByUserWithFavorites/LoginUserId/SearchUserId
		[HttpGet("GetListingPropertyByUserWithFavorites/{LoginUserId}/{SearchUserId}/{currentPageNumber}")]
		public async Task<ActionResult> GetListingPropertyByUserWithFavorites(int LoginUserId, int SearchUserId, int currentPageNumber)
		{
			try
			{
				ActionResult actionResult = await GetListingPropertyByUser(SearchUserId, currentPageNumber);
				PaginationModel<PropertyDetailResponse> pagedModel = await
				ActionResultUtility.GetPropertyPageResponse(LoginUserId,
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

		[Route("GetOperatorByUserId/{User}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetOperatorByUserId(int User, int currentPageNumber)
		{
			List<int> allOperatorIds = await (from REOperator in _context.Users
											  where REOperator.UserType == 1 &&
													REOperator.Status == true &&
													REOperator.UserId == User
											  select REOperator.UserId).ToListAsync();
			PaginationModel<PropertyOperatorResponse> model = await OperatorPaginationModelGenerator.GetPagedModelFromOperatorIds(_context, allOperatorIds, currentPageNumber);

			return Ok(model);

		}

		[Route("GetOperatorByUserIdWithFavorites/{LoginUserId}/{SearchUserId}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetOperatorByUserIdWithFavorites(int LoginUserId, int SearchUserId, int currentPageNumber)
		{
			try
			{
				ActionResult actionResult = await GetOperatorByUserId(SearchUserId, currentPageNumber);
				PaginationModel<PropertyOperatorResponse> response = await ActionResultUtility
																		.GetOperatorPageResponse
																								(LoginUserId,
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


		[Route("GetPeopleByListingId/{ListingId}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetPeopleByListingId(int ListingId, int currentPageNumber)
		{
			IEnumerable<User> operators = await _context.Users.ToListAsync();
			IEnumerable<REProfessionalMaster> projects;
			projects = await _context.REProfessionalMasters.ToListAsync();

			List<int> allPeopleIds = await (from listing in _context.Listings.AsNoTracking()
											where listing.ListingType == "RE-Professional" &&
														listing.Status == true &&
														listing.AdminStatus == true &&
														listing.DeletedStatus == false && 
														listing.ListingId == ListingId
											orderby listing.CreatedDateTime descending
											select listing.ListingId).ToListAsync();

			if (allPeopleIds.Count() == 0)
				return NotFound();

			PaginationModel<PropertyPeopleResponse> model = await PeoplePaginationModelGenerator.GetPagedModelFromPropertyIds(_context,allPeopleIds,currentPageNumber);
			return Ok(model);
		}

		[Route("GetPeopleByListingIdWithFavorites/{userId}/{ListingId}/{currentPageNumber}")]
		[HttpGet]
		public async Task<ActionResult> GetPeopleByListingIdWithFavorites(int userId, int ListingId, int currentPageNumber)
		{
			try
			{
				ActionResult actionResult = await GetPeopleByListingId(ListingId, currentPageNumber);
				PaginationModel<PropertyPeopleResponse> response = await ActionResultUtility.GetPeoplePageResponse(userId,actionResult,_context);

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