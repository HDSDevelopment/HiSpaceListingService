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
					PropertyListerInUseCount = _context.Listings.Count(d => d.UserId == item.UserId)
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