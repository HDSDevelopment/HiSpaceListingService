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
		[HttpGet]
		public ActionResult<List<PropertyLocationSearchResponse>> GetAllPropertyLocationSearch()
		{
			List<PropertyLocationSearchResponse> response = new List<PropertyLocationSearchResponse>();
			var locations = from r in _context.Listings
							where r.ListingType != "RE-Professional"
							group r by r.locality into g
							select new
							{
								locality = g.Key,
								localCount = g.Count()
							};
			foreach(var item in locations)
			{
				if(item.locality != null)
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
		public ActionResult<List<PropertyAndPeopleDetailWithLinkedSearchResponse>> GetAllPropertySearchByUserID(int UserID)
		{
			List<PropertyAndPeopleDetailWithLinkedSearchResponse> response = new List<PropertyAndPeopleDetailWithLinkedSearchResponse>();
			var properties = from r in _context.Listings
							 where r.UserId == UserID
							 select new { r };
			foreach (var item in properties)
			{
				if (item.r != null)
				{
					response.Add(new PropertyAndPeopleDetailWithLinkedSearchResponse()
					{
						ListingId = item.r.ListingId,
						Name = item.r.Name,
						RE_FirstName = item.r.RE_FirstName,
						RE_LastName = item.r.RE_LastName,
						ListingType = item.r.ListingType
					});
				}
			}
			return response;
		}

		// GET: api/Common/GetAllReProfessionalSearchByUserID/UserId
		//[HttpGet]
		[Route("GetAllReProfessionalSearchByUserID/{UserID}")]
		[HttpGet]
		public ActionResult<List<LinkedREPRofessionals>> GetAllReProfessionalSearchByUserID(int UserID)
		{
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

			List<LinkedREPRofessionals> response = new List<LinkedREPRofessionals>();
			foreach (var item in linkedREProf)
			{
				if(item != null)
				{
					var GetListingIdOnReProfessional = _context.REProfessionalMasters.Where(d => d.REProfessionalMasterId == item.REProfessionalMasterId).Select(d => d.ListingId).First();
					response.Add(new LinkedREPRofessionals()
					{
					Property_ListingId = item.ListingId,
					ReProfessional_ListingId = GetListingIdOnReProfessional,
					REProfessionalMasterId = item.REProfessionalMasterId,
					UserId = item.UserId,
					ProjectRole = item.ProjectRole,
					ProjectName = item.ProjectName,
					ImageUrl = item.ImageUrl,
					REFirstName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_FirstName).First(),
					RELastName = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.RE_LastName).First()
					});	
				}
				
			}
			return response;
		}

		// GET: api/Common/GetAllPropertyTypeSearch/
		[HttpGet]
		[Route("GetAllPropertyTypeSearch")]
		[HttpGet]
		public ActionResult<List<PropertyTypeSearchResponse>> GetAllPropertyTypeSearch()
		{
			List<PropertyTypeSearchResponse> response = new List<PropertyTypeSearchResponse>();
			var Types = from r in _context.Listings
							group r by r.ListingType into g
							select new
							{
								PropType = g.Key,
								PropCount = g.Count()
							};
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
		[HttpGet]
		public ActionResult<List<PropertyLevelSearchResponse>> GetAllPropertyLevelSearch()
		{
			List<PropertyLevelSearchResponse> response = new List<PropertyLevelSearchResponse>();
			//CommercialLevels
			var CommercialLevels = from r in _context.Listings
						group r by r.CommercialType into g
						select new
						{
							Level = g.Key,
							LevelCount = g.Count()
						};
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
			var CoworkingLevels = from r in _context.Listings
								   group r by r.CoworkingType into g
								   select new
								   {
									   Level = g.Key,
									   LevelCount = g.Count()
								   };
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
			var REprofessionalLevels = from r in _context.Listings
								  group r by r.REprofessionalsType into g
								  select new
								  {
									  Level = g.Key,
									  LevelCount = g.Count()
								  };
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
		[HttpGet]
		public async Task<ActionResult<IEnumerable<PropertyListerSearchResponse>>> GetAllPropertyListerSearch()
		{
			List<PropertyListerSearchResponse> response = new List<PropertyListerSearchResponse>();
			var Listers = await _context.Users.OrderBy(d => d.UserId).ToListAsync();
			foreach (var item in Listers)
			{
				response.Add(new PropertyListerSearchResponse()
				{
					UserId = item.UserId,
					CompanyName = item.CompanyName,
					PropertyListerInUseCount = _context.Listings.Count(d => d.UserId == item.UserId && d.ListingType != "RE-Professional")
				});
			}
			return response;
		}

		// GET: api/Common/GetAllOperatorSearch/
		[HttpGet]
		[Route("GetAllOperatorSearch")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<PropertyListerSearchResponse>>> GetAllOperatorSearch()
		{
			List<PropertyListerSearchResponse> response = new List<PropertyListerSearchResponse>();
			var Listers = await _context.Users.Where(d => d.Status == true).OrderBy(d => d.UserId).ToListAsync();
			foreach (var item in Listers)
			{
				response.Add(new PropertyListerSearchResponse()
				{
					UserId = item.UserId,
					CompanyName = item.CompanyName,
					PropertyListerInUseCount = _context.Listings.Count(d => d.UserId == item.UserId)
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
			var Listers = await _context.Users.ToListAsync();
			if(Location == "All")
			{

				Listers = await _context.Users.Where(d => d.Status == true && d.UserType == 1).OrderBy(d => d.UserId).ToListAsync();
			}
			else
			{
				Listers = await _context.Users.Where(d => d.Status == true && d.UserType == 1 && d.City == Location).OrderBy(d => d.UserId).ToListAsync();
			}
			foreach (var item in Listers)
			{
				response.Add(new OperatorFilterOperatorList()
				{
					UserId = item.UserId,
					CompanyName = item.CompanyName,
					PropertyCount = _context.Listings.Count(d => d.UserId == item.UserId)
				});
			}
			return response;
		}
		//People filter dropdown start
		// GET: api/Common/GetPeopleListForPeopleFilter/Location
		[HttpGet]
		[Route("GetPeopleListForPeopleFilter/{Location}")]
		public async Task<ActionResult<IEnumerable<PeopleFilterPeopleList>>> GetPeopleListForPeopleFilter(string Location)
		{
			List<PeopleFilterPeopleList> response = new List<PeopleFilterPeopleList>();
			var Listers = await _context.Listings.ToListAsync();
			if (Location == "All")
			{
				Listers = await _context.Listings.Where(d => d.Status == true && d.AdminStatus == true && d.ListingType == "RE-Professional").OrderBy(d => d.ListingId).ToListAsync();
			}
			else
			{
				Listers = await _context.Listings.Where(d => d.Status == true && d.AdminStatus == true && d.ListingType == "RE-Professional" && d.locality == Location).OrderBy(d => d.ListingId).ToListAsync();
			}
			foreach (var item in Listers)
			{
				response.Add(new PeopleFilterPeopleList()
				{
					ListingId = item.ListingId,
					RE_FirstName = item.RE_FirstName,
					RE_LastName = item.RE_LastName,
					RE_FullName = item.RE_FirstName+" - "+item.RE_LastName,
					ProjectCount = _context.REProfessionalMasters.Count(d => d.ListingId == item.ListingId)
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
			var Listers = await _context.Users.Where(d => d.Status == true && d.UserType == 1).Select(d => d.City).Distinct().ToListAsync();
			foreach (var item in Listers)
			{
				if(item != null)
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
			var Listers = await _context.Listings.Where(d => d.Status == true && d.AdminStatus == true && (d.ListingType == "Commercial" || d.ListingType == "Co-Working")).Select(d => d.locality).Distinct().ToListAsync();
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
			var Listers = await _context.Listings.Where(d => d.Status == true && d.AdminStatus == true && d.ListingType == "RE-Professional" ).Select(d => d.locality).Distinct().ToListAsync();
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
		[HttpGet]
		public async Task<ActionResult<IEnumerable<PeopleNameSearchResponse>>> GetAllPeopleSearch()
		{
			List<PeopleNameSearchResponse> response = new List<PeopleNameSearchResponse>();
			var People = await _context.Listings.Where(d => d.ListingType == "RE-Professional").ToListAsync();
			foreach (var item in People)
			{
				response.Add(new PeopleNameSearchResponse()
				{
					ListingId = item.ListingId,
					RE_FirstName = item.RE_FirstName,
					RE_LastName = item.RE_LastName,
					ProjectCount = _context.REProfessionalMasters.Count(d => d.ListingId == item.ListingId)
				});
			}
			return response;
		}

		// GET: api/Common/GetAmenityMasterList/
		[HttpGet]
		[Route("GetAmenityMasterList")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<AmenityMaster>>> GetAmenityMasterList()
		{

			return await _context.AmenityMasters.OrderBy(d => d.Name).ToListAsync();
		}

		// GET: api/Common/GetFacilityMasterList/
		[HttpGet]
		[Route("GetFacilityMasterList")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<FacilityMaster>>> GetFacilityMasterList()
		{

			return await _context.FacilityMasters.OrderBy(d => d.Name).ToListAsync();
		}
	}
}