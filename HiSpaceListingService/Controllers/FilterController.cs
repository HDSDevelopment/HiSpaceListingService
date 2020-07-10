﻿using System;
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
		[HttpGet("GetListingByLocation/{Location}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingByLocation(string Location)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			var properties = await _context.Listings.Where(m => m.Status == true && m.locality == Location).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

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
		[HttpGet("GetListingByType/{Type}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingByType(string Type)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			var properties = await _context.Listings.Where(m => m.Status == true && m.ListingType == Type).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

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
		[HttpGet("GetListingByUser/{User}")]
		public async Task<ActionResult<IEnumerable<PropertyDetailResponse>>> GetListingByUser(int User)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
			var properties = await _context.Listings.Where(m => m.Status == true && m.UserId == User).OrderByDescending(d => d.CreatedDateTime).ToListAsync();

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

	}
}