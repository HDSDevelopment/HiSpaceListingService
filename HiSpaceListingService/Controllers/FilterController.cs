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
			var properties = await _context.Listings.Where(m => m.Status == true && m.locality == Location && m.ListingType != "RE-Professional").OrderByDescending(d => d.CreatedDateTime).ToListAsync();

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

		/// <summary>
		/// Get property list by type
		/// </summary>
		/// <returns>The list of Properties by its type.</returns>
		// GET: api/Listing/GetListingByType
		[HttpGet("GetListingPropertyByType/{Type}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingPropertyByType(string Type)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			var properties = await _context.Listings.Where(m => m.Status == true && m.ListingType == Type && m.ListingType != "RE-Professional").OrderByDescending(d => d.CreatedDateTime).ToListAsync();

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

		/// <summary>
		/// Get property list by user
		/// </summary>
		/// <returns>The list of Properties by its user.</returns>
		// GET: api/Listing/GetListingByUser
		[HttpGet("GetListingPropertyByUser/{User}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingPropertyByUser(int User)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			var properties = await _context.Listings.Where(m => m.Status == true && m.UserId == User && m.ListingType != "RE-Professional").OrderByDescending(d => d.CreatedDateTime).ToListAsync();

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

		/// <summary>
		/// Get property list by location and type
		/// </summary>
		/// <returns>The list of Properties by its location and type.</returns>
		// GET: api/Listing/GetListingByTypeAndLocation
		[HttpGet("GetListingByTypeAndLocation/{Type}/{Location}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingByTypeAndLocation(string Type, string Location)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			var properties = await _context.Listings.Where(m => m.Status == true && m.ListingType == Type && m.locality == Location).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

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

		[Route("GetOperatorByUserId/{User}")]
		[HttpGet]
		public ActionResult<List<PropertyOperatorResponse>> GetOperatorByUserId(int User)
		{
			List<PropertyOperatorResponse> listoperators = new List<PropertyOperatorResponse>();

			var users = (from u in _context.Users
						 where u.UserId == User && u.Status == true
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

		[Route("GetPeopleByListingId/{ListingId}")]
		[HttpGet]
		public ActionResult<List<PropertyPeopleResponse>> GetPeopleByListingId(int ListingId)
		{
			List<PropertyPeopleResponse> ppl = new List<PropertyPeopleResponse>();
			var users = (from r in _context.Listings
						 join a in _context.Users on r.UserId equals a.UserId
						 where r.ListingId == ListingId && r.ListingType == "RE-Professional"
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
									   || l.CMCW_PropertyTaxBillNumber == r.PropertyAdditionalIdNumber) && r.ListingId == p.Listing.ListingId)
									  select new
									  {
										  l.ListingId,
										  r.REProfessionalMasterId,
										  l.UserId,
										  r.ProjectRole,
										  r.ProjectName,
										  r.ImageUrl
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

	}
}