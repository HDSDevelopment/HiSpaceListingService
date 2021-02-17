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
using System.Diagnostics;

namespace HiSpaceListingService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommonController : ControllerBase
	{
		private readonly HiSpaceListingContext _context;

		public CommonController(HiSpaceListingContext context)
		{
			_context = context;
		}

		// GET: api/Common/GetAllPropertyLocationSearch/
		[HttpGet]
		[Route("GetAllPropertyLocationSearch")]
		public async Task<ActionResult<List<PropertyLocationSearchResponse>>> GetAllPropertyLocationSearch()
		{
			List<PropertyLocationSearchResponse> response = new List<PropertyLocationSearchResponse>();
			var locations = await (from r in _context.Listings.AsNoTracking()
								   where r.ListingType != "RE-Professional" && r.AdminStatus == true && r.Status == true && r.DeletedStatus == false
								   group r by r.locality into g
								   select new
								   {
									   locality = g.Key,
									   localCount = g.Count()
								   })
								   .OrderBy(d => d.locality)
							.ToListAsync();

			foreach (var item in locations)
			{
				if (item.locality != null)
				{
					response.Add(new PropertyLocationSearchResponse()
					{
						locality = item.locality,
						localityInUseCount = item.localCount
					});
				}
			}
			return response;
		}

		// GET: api/Common/GetPropertyLocationWithMinimumCountSearch/
		[HttpGet]
		[Route("GetPropertyLocationWithMinimumCountSearch")]
		public async Task<ActionResult<List<PropertyLocationSearchResponse>>> GetPropertyLocationWithMinimumCountSearch()
		{

			const int minimumNumberOfProperties = 20;

			List<PropertyLocationSearchResponse> response = new List<PropertyLocationSearchResponse>();
			var locations = await (from r in _context.Listings.AsNoTracking()
								   where r.ListingType != "RE-Professional" && r.AdminStatus == true && r.Status == true && r.DeletedStatus == false
								   group r by r.locality into g
								   where g.Count() > minimumNumberOfProperties
								   select new
								   {
									   locality = g.Key,
									   localCount = g.Count()
								   })
			.ToListAsync();

			foreach (var item in locations)
			{
				if (item.locality != null)
				{
					response.Add(new PropertyLocationSearchResponse()
					{
						locality = item.locality,
						localityInUseCount = item.localCount
					});
				}
			}
			return response;
		}

		// GET: api/Common/GetAllPropertySearchByUserID/UserId
		//[HttpGet]
		[Route("GetAllPropertySearchByUserID/{UserID}")]
		[HttpGet]
		public async Task<ActionResult<List<PropertyAndPeopleDetailWithLinkedSearchResponse>>> GetAllPropertySearchByUserID(int UserID)
		{
			List<PropertyAndPeopleDetailWithLinkedSearchResponse> response = new List<PropertyAndPeopleDetailWithLinkedSearchResponse>();
			var properties = await (from r in _context.Listings.AsNoTracking()
									where r.UserId == UserID
									&& r.AdminStatus == true
									&& r.Status == true
									&& r.DeletedStatus == false
									select new { r.ListingId, r.Name, r.RE_FirstName, r.RE_LastName, r.ListingType })
									.ToListAsync();

			foreach (var item in properties)
			{
				if (item != null)
				{
					response.Add(new PropertyAndPeopleDetailWithLinkedSearchResponse()
					{
						ListingId = item.ListingId,
						Name = item.Name,
						RE_FirstName = item.RE_FirstName,
						RE_LastName = item.RE_LastName,
						ListingType = item.ListingType
					});
				}
			}
			return response;
		}

		// GET: api/Common/GetAllReProfessionalSearchByUserID/UserId
		//[HttpGet]
		[Route("GetAllReProfessionalSearchByUserID/{UserID}")]
		[HttpGet]
		public async Task<ActionResult<List<LinkedREPRofessionals>>> GetAllReProfessionalSearchByUserID(int UserID)
		{
			//geting linked re-prof
			var linkedREProf = await (from l in _context.Listings
									  from r in _context.REProfessionalMasters
									  where (l.UserId == UserID && l.DeletedStatus == false &&
									 (l.ListingType == "Commercial" || l.ListingType == "Co-Working") &&
									(((l.CMCW_ReraId != null && r.PropertyReraId != null) && (l.CMCW_ReraId == r.PropertyReraId))
									|| ((l.CMCW_CTSNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_CTSNumber == r.PropertyAdditionalIdNumber))
									|| ((l.CMCW_GatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_GatNumber == r.PropertyAdditionalIdNumber))
									|| ((l.CMCW_MilkatNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_MilkatNumber == r.PropertyAdditionalIdNumber))
									|| ((l.CMCW_PlotNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PlotNumber == r.PropertyAdditionalIdNumber))
									|| ((l.CMCW_SurveyNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_SurveyNumber == r.PropertyAdditionalIdNumber))
									|| ((l.CMCW_PropertyTaxBillNumber != null && r.PropertyAdditionalIdNumber != null) && (l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber))
									 ) && r.DeletedStatus == false)
									  select new
									  {
										  l.ListingId,
										  r.REProfessionalMasterId,
										  l.UserId,
										  r.ProjectRole,
										  r.ProjectName,
										  r.ImageUrl
									  }).ToListAsync();

			List<LinkedREPRofessionals> response = new List<LinkedREPRofessionals>();

			var projects = await (from project in _context.REProfessionalMasters.AsNoTracking()
								  select new { project.REProfessionalMasterId, project.ListingId })
							.ToListAsync();

			var RENames = await (from listing in _context.Listings.AsNoTracking()
						 		select new { listing.ListingId, listing.RE_FirstName, listing.RE_LastName })
								 .ToListAsync();

			foreach (var item in linkedREProf)
			{
				if (item != null)
				{
					int GetListingIdOnReProfessional = (from project in projects
														where project.REProfessionalMasterId == item.REProfessionalMasterId
														select project.ListingId)
														.First();

					var REName = (from reName in RENames
								  where reName.ListingId == GetListingIdOnReProfessional
								  select reName)
									.First();

					response.Add(new LinkedREPRofessionals()
					{
						Property_ListingId = item.ListingId,
						ReProfessional_ListingId = GetListingIdOnReProfessional,
						REProfessionalMasterId = item.REProfessionalMasterId,
						UserId = item.UserId,
						ProjectRole = item.ProjectRole,
						ProjectName = item.ProjectName,
						ImageUrl = item.ImageUrl,
						REFirstName = REName.RE_FirstName,
						RELastName = REName.RE_LastName
					});
				}

			}
			return response;
		}

		// GET: api/Common/GetAllPropertyTypeSearch/
		[HttpGet]
		[Route("GetAllPropertyTypeSearch")]
		public async Task<ActionResult<List<PropertyTypeSearchResponse>>> GetAllPropertyTypeSearch()
		{
			List<PropertyTypeSearchResponse> response = new List<PropertyTypeSearchResponse>();
			var Types = await (from r in _context.Listings.AsNoTracking()
							   where r.AdminStatus == true && r.Status == true && r.DeletedStatus == false
							   group r by r.ListingType into g
							   select new
							   {
								   PropType = g.Key,
								   PropCount = g.Count()
							   })
							.ToListAsync();

			foreach (var item in Types)
			{
				if (item.PropType != null)
				{
					response.Add(new PropertyTypeSearchResponse()
					{
						PropertyType = item.PropType,
						PropertyTypeInUseCount = item.PropCount
					});
				}
			}
			return response;
		}

		// GET: api/Common/GetAllPropertyLevelSearch/
		[HttpGet]
		[Route("GetAllPropertyLevelSearch")]
		public async Task<ActionResult<List<PropertyLevelSearchResponse>>> GetAllPropertyLevelSearch()
		{
			List<PropertyLevelSearchResponse> response = new List<PropertyLevelSearchResponse>();
			//CommercialLevels
			var CommercialLevels = await (from r in _context.Listings.AsNoTracking()
										  group r by r.CommercialType into g
										  select new
										  {
											  Level = g.Key,
											  LevelCount = g.Count()
										  })
											.ToListAsync();

			foreach (var item in CommercialLevels)
			{
				if (item.Level != null)
				{
					response.Add(new PropertyLevelSearchResponse()
					{
						PropertyLevel = item.Level,
						PropertyLevelInUseCount = item.LevelCount
					});
				}
			}
			//CoworkingLevels
			var CoworkingLevels = await (from r in _context.Listings.AsNoTracking()
										 group r by r.CoworkingType into g
										 select new
										 {
											 Level = g.Key,
											 LevelCount = g.Count()
										 })
								   .ToListAsync();

			foreach (var item in CoworkingLevels)
			{
				if (item.Level != null)
				{
					response.Add(new PropertyLevelSearchResponse()
					{
						PropertyLevel = item.Level,
						PropertyLevelInUseCount = item.LevelCount
					});
				}
			}
			//REprofessionalLevels
			var REprofessionalLevels = await (from r in _context.Listings.AsNoTracking()
											  group r by r.REprofessionalsType into g
											  select new
											  {
												  Level = g.Key,
												  LevelCount = g.Count()
											  })
												.ToListAsync();

			foreach (var item in REprofessionalLevels)
			{
				if (item.Level != null)
				{
					response.Add(new PropertyLevelSearchResponse()
					{
						PropertyLevel = item.Level,
						PropertyLevelInUseCount = item.LevelCount
					});
				}
			}
			return response;
		}

		// GET: api/Common/GetAllPropertyListerSearch/
		[HttpGet]
		[Route("GetAllPropertyListerSearch")]
		public async Task<ActionResult<IEnumerable<PropertyListerSearchResponse>>> GetAllPropertyListerSearch()
		{
			List<PropertyListerSearchResponse> response = new List<PropertyListerSearchResponse>();
			var users = await (from user in _context.Users.AsNoTracking()
							   where user.Status == true
							   select new { user.UserId, user.CompanyName }
								)
								.OrderBy(d => d.CompanyName)
								.ToListAsync();

			var properties = await (from property in _context.Listings.AsNoTracking()
									where property.ListingType != "RE-Professional"
									&& property.Status == true
									&& property.AdminStatus == true
									&& property.DeletedStatus == false
									select property.UserId).ToListAsync();

			foreach (var item in users)
			{
				response.Add(new PropertyListerSearchResponse()
				{
					UserId = item.UserId,
					CompanyName = item.CompanyName,
					PropertyListerInUseCount = properties.Count(d => d == item.UserId)
				});
			}
			return response;
		}

		// GET: api/Common/GetAllOperatorSearch/
		[HttpGet]
		[Route("GetAllOperatorSearch")]
		public async Task<ActionResult<IEnumerable<PropertyListerSearchResponse>>> GetAllOperatorSearch()
		{
			List<PropertyListerSearchResponse> response = new List<PropertyListerSearchResponse>();
			var users = await (from user in _context.Users.AsNoTracking()
							   where user.Status == true
							   select new { user.UserId, user.CompanyName })
						.OrderBy(d => d.CompanyName)
						.ToListAsync();

			var listingUserIDs = await (from listing in _context.Listings.AsNoTracking()
										where listing.Status == true && listing.AdminStatus == true && listing.DeletedStatus == false
										select listing.UserId)
								.ToListAsync();

			foreach (var item in users)
			{
				response.Add(new PropertyListerSearchResponse()
				{
					UserId = item.UserId,
					CompanyName = item.CompanyName,
					PropertyListerInUseCount = listingUserIDs.Count(d => d == item.UserId)
				});
			}
			return response;
		}
		//Operator filter dropdown start
		// GET: api/Common/GetOperatorListForOperatorFilter/Location
		[HttpGet]
		[Route("GetOperatorListForOperatorFilter/{Location}")]
		public async Task<ActionResult<IEnumerable<OperatorFilterOperatorList>>> GetOperatorListForOperatorFilter(string Location)
		{
			List<OperatorFilterOperatorList> response = new List<OperatorFilterOperatorList>();

			var users = _context.Users
									.Where(d => d.Status == true && d.UserType == 1)
									.OrderBy(d => d.CompanyName)
									.Select(b => new { b.UserId, b.City, b.CompanyName });

			if (Location != "All")
				users = users.Where(d => d.City == Location);

			var usersInCity = await users.AsNoTracking().ToListAsync();

			List<int> listings = await _context.Listings.AsNoTracking()
									.Where(d => d.Status == true && d.AdminStatus == true && d.DeletedStatus == false)
									.Select(b => b.UserId)
									.ToListAsync();

			foreach (var item in usersInCity)
			{
				response.Add(new OperatorFilterOperatorList()
				{
					UserId = item.UserId,
					CompanyName = item.CompanyName,
					PropertyCount = listings.Count(d => d == item.UserId)
				});

			}
			return response.OrderBy(d => d.CompanyName).ToList();
		}
		//People filter dropdown start
		// GET: api/Common/GetPeopleListForPeopleFilter/Location
		[HttpGet]
		[Route("GetPeopleListForPeopleFilter/{Location}")]
		public async Task<ActionResult<IEnumerable<PeopleFilterPeopleList>>> GetPeopleListForPeopleFilter(string Location)
		{
			List<PeopleFilterPeopleList> response = new List<PeopleFilterPeopleList>();
			var listers = (from listing in _context.Listings
						   where listing.Status == true
						   && listing.AdminStatus == true
						   && listing.ListingType == "RE-Professional"
						   && listing.DeletedStatus == false
						   select new { listing.ListingId, listing.RE_FirstName, listing.RE_LastName, listing.locality });

			if (Location != "All")
				listers = (from listing in listers
						   where listing.locality == Location
						   select listing);

			var listersByLocation = await listers.AsNoTracking().ToListAsync();

			IEnumerable<int> projectIDs = await (from project in _context.REProfessionalMasters
												.AsNoTracking()
												 select project.ListingId)
												.ToListAsync();

			foreach (var item in listersByLocation)
			{
				response.Add(new PeopleFilterPeopleList()
				{
					ListingId = item.ListingId,
					RE_FirstName = item.RE_FirstName,
					RE_LastName = item.RE_LastName,
					RE_FullName = item.RE_FirstName + "-" + item.RE_LastName,
					ProjectCount = projectIDs.Count(projectID => projectID == item.ListingId)
				});
			}
			return response;
		}
		// GET: api/Common/GetLocationListForOperatorFilter/
		[HttpGet]
		[Route("GetLocationListForOperatorFilter")]
		public async Task<ActionResult<IEnumerable<LocationFilterOperatorList>>> GetLocationListForOperatorFilter()
		{
			List<LocationFilterOperatorList> response = new List<LocationFilterOperatorList>();
			IEnumerable<string> cities = await (from user in _context.Users.AsNoTracking()
												where user.Status == true && user.UserType == 1
												select user.City)
											.Distinct()
											.ToListAsync();

			foreach (var item in cities)
			{
				if (item != null)
					response.Add(new LocationFilterOperatorList()
					{
						OperatorLocation = item
					});
			}
			return response;
		}
		//Operator filter dropdown end

		//property filter start
		// GET: api/Common/GetLocationListForPropertyFilter/
		[HttpGet]
		[Route("GetLocationListForPropertyFilter")]
		public async Task<ActionResult<IEnumerable<LocationFilterPropertyList>>> GetLocationListForPropertyFilter()
		{
			List<LocationFilterPropertyList> response = new List<LocationFilterPropertyList>();
			var Listers = await _context.Listings.AsNoTracking()
								.Where(d => d.Status == true && d.AdminStatus == true && d.DeletedStatus == false && (d.ListingType == "Commercial" || d.ListingType == "Co-Working"))
								.Select(d => d.locality)
								.Distinct()
								.ToListAsync();

			foreach (var item in Listers)
			{
				if (item != null)
					response.Add(new LocationFilterPropertyList()
					{
						PropertyLocation = item
					});
			}
			return response;
		}

		//People filter start
		// GET: api/Common/GetLocationListForPeopleFilter/
		[HttpGet]
		[Route("GetLocationListForPeopleFilter")]
		public async Task<ActionResult<IEnumerable<LocationFilterPropertyList>>> GetLocationListForPeopleFilter()
		{
			List<LocationFilterPropertyList> response = new List<LocationFilterPropertyList>();
			var Listers = await _context.Listings.AsNoTracking()
							.Where(d => d.Status == true && d.AdminStatus == true && d.DeletedStatus == false && d.ListingType == "RE-Professional")
							.Select(d => d.locality)
							.Distinct()
							.ToListAsync();

			foreach (var item in Listers)
			{
				if (item != null)
					response.Add(new LocationFilterPropertyList()
					{
						PropertyLocation = item
					});
			}
			return response;
		}
		//property filter end
		// GET: api/Common/GetAllPeopleSearch/
		[HttpGet]
		[Route("GetAllPeopleSearch")]
		public async Task<ActionResult<IEnumerable<PeopleNameSearchResponse>>> GetAllPeopleSearch()
		{
			List<PeopleNameSearchResponse> response = new List<PeopleNameSearchResponse>();
			var professionals = await (from listing in _context.Listings.AsNoTracking()
									   where listing.ListingType == "RE-Professional" && listing.DeletedStatus == false
									   select new { listing.ListingId, listing.RE_FirstName, listing.RE_LastName })
									.ToListAsync();

			List<int> projects = await (from project in _context.REProfessionalMasters.AsNoTracking()
										select project.ListingId)
									.ToListAsync();

			foreach (var item in professionals)
			{
				if ((item.RE_FirstName != null) || (item.RE_LastName != null))
				{
					response.Add(new PeopleNameSearchResponse()
				{
					ListingId = item.ListingId,
					RE_FirstName = item.RE_FirstName,
					RE_LastName = item.RE_LastName,
					ProjectCount = projects.Count(d => d == item.ListingId)
				});
			}
		}
			return response;
		}

		// GET: api/Common/GetAmenityMasterList/
		[HttpGet]
		[Route("GetAmenityMasterList")]
		public async Task<ActionResult<IEnumerable<AmenityMaster>>> GetAmenityMasterList()
		{

			return await _context.AmenityMasters.AsNoTracking().OrderBy(d => d.Name).ToListAsync();
		}

		// GET: api/Common/GetFacilityMasterList/
		[HttpGet]
		[Route("GetFacilityMasterList")]
		public async Task<ActionResult<IEnumerable<FacilityMaster>>> GetFacilityMasterList()
		{

			return await _context.FacilityMasters.AsNoTracking().OrderBy(d => d.Name).ToListAsync();
		}
	}
}