using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using HiSpaceListingService.ViewModel;

namespace HiSpaceListingService.Utilities
{
public static class PropertyPaginationModelGenerator
{
        public static async Task<PaginationModel<PropertyDetailResponse>> GetPagedModelFromPropertyIds(
																			HiSpaceListingContext _context,
                                                                                List<int> allPropertyIds, 
                                                                                int currentPageNumber)
        {
        PaginationModel<PropertyDetailResponse> pagedModel = new PaginationModel<PropertyDetailResponse>();
            pagedModel.Count = allPropertyIds.Count;
            pagedModel.CurrentPage = currentPageNumber;

            List<int> currentPagePropertyIds = GetCurrentPagePropertyIds(currentPageNumber,
                														allPropertyIds,
                                                                        pagedModel.PageSize);

								List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();
								vModel = await GetPropertyDetailResponseList(_context, currentPagePropertyIds);            
								pagedModel.CurrentPageData = vModel;
          
					return pagedModel;
        }

        		static List<int> GetCurrentPagePropertyIds(int currentPageNumber, List<int> propertyIds, int pageSize)
		{
				int countToSkip = (currentPageNumber - 1) * pageSize;				
				propertyIds = propertyIds
                                        .Skip(countToSkip)
                                        .Take(pageSize)
                                        .ToList();
				return propertyIds;
		}

        	public static async Task<List<PropertyDetailResponse>> GetPropertyDetailResponseList(HiSpaceListingContext _context, List<int> PropertyIds)
		{
			List<PropertyDetailResponse> vModel = new List<PropertyDetailResponse>();

			List<Listing> properties = await GetPropertiesFromPropertyIds(_context, PropertyIds);

			List<User> propertiesUsers = await GetUsersForProperties(_context, properties);
				
			List<int> amenitiesPropertyIds = await GetPropertyIdsOfAmenties(_context, PropertyIds);

			List<int> facilitiesPropertyIds = await GetPropertyIdsOfFacilities(_context, PropertyIds);
			
			List<ListingImages> images = await GetImagesOfProperties(_context, PropertyIds);

			List<int> healthChecksPropertyIds = await GetPropertyIdsOfHealthChecks(_context, PropertyIds);
			
			List<int> gbcsPropertyIds = await GetPropertyIdsOfGreenBuildingChecks(_context, PropertyIds);

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
				List<RelatedREProfessional> RelatedREProf = RelatedREProfessionalUtility.Get(projects, item);

				property.LinkedREProf = new List<LinkedREPRofessionals>();

				foreach (RelatedREProfessional professional in RelatedREProf)
				{
					LinkedREPRofessionals REProf = RelatedREProfessionalUtility.Set(_context,
																					projects,
																					professional);
					property.LinkedREProf.Add(REProf);
				}

				property.LinkedREProfCount = property.LinkedREProf.Count;
				vModel.Add(property);
			}			
			return vModel;
		}

        static async Task<List<Listing>> GetPropertiesFromPropertyIds(HiSpaceListingContext _context, IEnumerable<int> propertyIds)
		{
			List<Listing> properties = await (from property in _context.Listings.AsNoTracking()
																				select property)
																							.Where(n => propertyIds.Contains(n.ListingId))
																							.ToListAsync();
			return properties;
		}

        static User GetUserForProperty(List<User> propertiesUsers, int userId)
		{
            User propertyUser = (from user in propertiesUsers
                         where user.UserId == userId
                         select user)
																		.SingleOrDefault();
						return propertyUser;
		}

        static int GetAmenitiesCountForProperty(List<int> amenitiesPropertyIds, int propertyId)
		{
			int amenitiesCount = (from amenityPropertyId in amenitiesPropertyIds
								    where amenityPropertyId == propertyId
									select amenityPropertyId)
																.Count();
			return amenitiesCount;
		}

        static int GetFacilitiesCountForProperty(List<int> facilitiesPropertyIds, int propertyId)
		{
			int facilitiesCount = (from facilitypropertyId in facilitiesPropertyIds
																				where facilitypropertyId == propertyId
																				select facilitypropertyId)
																											.Count();
			return facilitiesCount;
		}

		static int GetProjectsCountForProperty(IEnumerable<REProfessionalMaster> projects, int propertyId)
		{			
			int projectsCount = (from project in projects
														where project.ListingId == propertyId && project.Status == true
														select project.ListingId)
																									.Count();
			return projectsCount;
		}

		static List<ListingImages> GetImagesForProperty(List<ListingImages> propertiesImages, int propertyId)
		{
			List<ListingImages> propertyImages = (from propertyImage in propertiesImages
																						where propertyImage.ListingId == propertyId
																						select propertyImage)
																									.ToList();
			return propertyImages;
		}

		static int GetHealthChecksCountForProperty(List<int> healthChecksPropertyIds, int propertyId)
		{			
			int healthChecksCount = (from propertyHealthCheckId in healthChecksPropertyIds
																where propertyHealthCheckId == propertyId
																select propertyHealthCheckId)
																									.Count();
			return healthChecksCount;
		}

		static int GetGreenBuildingChecksCountForProperty(List<int> gbcsPropertyIds, int propertyId)
		{			
			int gbcChecksCount = (from propertyGBCId in gbcsPropertyIds
																where propertyGBCId == propertyId
																select propertyGBCId)
																									.Count();
			return gbcChecksCount;
		}

		static async Task<List<User>> GetUsersForProperties(HiSpaceListingContext _context, 
															List<Listing> properties)
		{
			IEnumerable<int> userIdsForProperties = from REProperty in properties
																							select REProperty.UserId;
																	
			List<User> users = await (from user in _context.Users.AsNoTracking()
																select user)
																			.Where(n => userIdsForProperties.Contains(n.UserId))
																			.ToListAsync();
			return users;
		}


		static async Task<List<int>> GetPropertyIdsOfAmenties(HiSpaceListingContext _context, 
																IEnumerable<int> propertyIds)
		{
						List<int> amenitiesPropertyIds = await (from amenity in _context.Amenitys.AsNoTracking() 
																							where amenity.Status == true
																							select amenity.ListingId)
																												.Where(n => propertyIds.Contains(n))
																												.ToListAsync();
						return amenitiesPropertyIds;
		}

		static async Task<List<int>> GetPropertyIdsOfFacilities(HiSpaceListingContext _context, 
																IEnumerable<int> propertyIds)
		{
				List<int> facilitiesPropertyIds = await (from facility in _context.Facilitys.AsNoTracking()
													where facility.Status == true
													select facility.ListingId)								.Where(n => propertyIds.Contains(n))
													.ToListAsync();
						return facilitiesPropertyIds;
		}

		static async Task<List<int>> GetPropertyIdsOfHealthChecks(HiSpaceListingContext _context, 
																	IEnumerable<int> propertyIds)
		{
			List<int> healthChecks = await (from healthCheck in _context.HealthChecks.AsNoTracking()
											where healthCheck.Status == true
											select healthCheck.ListingId)
														.Where(n => propertyIds.Contains(n))
														.ToListAsync();
			return healthChecks;
		}

		static async Task<List<ListingImages>> GetImagesOfProperties(HiSpaceListingContext _context, 
																	IEnumerable<int> propertyIds)
		{
				List<ListingImages> images = await _context.ListingImagess.AsNoTracking()
														.Where(n => propertyIds.Contains(n.ListingId))
														.ToListAsync();
				return images;
		}

		static async Task<List<int>> GetPropertyIdsOfGreenBuildingChecks(HiSpaceListingContext _context, 
																		IEnumerable<int> propertyIds)
		{
						List<int> gbcsPropIds = await (from gbc in _context.GreenBuildingChecks.AsNoTracking()
														where gbc.Status == true
														select gbc.ListingId)
																.Where(n => propertyIds.Contains(n))
																.ToListAsync();
						return gbcsPropIds;
		}

		//static List<RelatedREProfessional> GetRelatedREProfessionals(List<REProfessionalMaster> projects, 																Listing property)
		//{
		//	List<RelatedREProfessional> RelatedREProf = new List<RelatedREProfessional>();
		//	RelatedREProf = (from r in projects
		//							where (
		//							(((r.PropertyReraId != null && property.CMCW_ReraId != null) && (r.PropertyReraId == property.CMCW_ReraId))
		//							|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_CTSNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_CTSNumber))
		//							|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_MilkatNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_MilkatNumber))
		//							|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_PlotNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_PlotNumber))
		//							|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_SurveyNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_SurveyNumber))
		//							|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_PropertyTaxBillNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_PropertyTaxBillNumber))
		//							|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_GatNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_GatNumber)))
		//							&& (r.LinkingStatus == "Approved") && (r.DeletedStatus == false))
		//							select new RelatedREProfessional
		//							{
		//								ListingId = property.ListingId,
		//								ProjectId = r.REProfessionalMasterId,
		//								UserId = property.UserId,
		//								ProjectRole = r.ProjectRole,
		//								ProjectName = r.ProjectName,
		//								ImageUrl = r.ImageUrl,
		//								OperatorName = r.OperatorName,
		//								LinkingStatus = r.LinkingStatus
		//							}).ToList();
		//		return RelatedREProf;
		//}

		//static LinkedREPRofessionals SetRelatedREProfessional(HiSpaceListingContext _context, 
		//													IEnumerable<REProfessionalMaster> projects, 
		//													RelatedREProfessional professional)
		//{
		//			LinkedREPRofessionals REProf = new LinkedREPRofessionals();
		//					var GetListingIdOnReProfessional = projects.Where(d => d.REProfessionalMasterId == 
		//																										professional.ProjectId)
		//																													.Select(d => d.ListingId)
		//																													.First();
		//			REProf.Property_ListingId = professional.ListingId;
		//			REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
		//			REProf.REProfessionalMasterId = professional.ProjectId;
		//			REProf.UserId = professional.UserId;
		//			REProf.ProjectRole = professional.ProjectRole;
		//			REProf.OperatorName = professional.OperatorName;
		//			REProf.LinkingStatus = professional.LinkingStatus;
		//			REProf.ProjectName = professional.ProjectName;
		//			REProf.ImageUrl = professional.ImageUrl;

		//			var REName = (from listing in _context.Listings.AsNoTracking()
		//										where listing.ListingId == GetListingIdOnReProfessional
		//										select new { listing.RE_FirstName, listing.RE_LastName })
		//														.First();

		//			REProf.REFirstName = REName.RE_FirstName;
		//			REProf.RELastName = REName.RE_LastName;
		//			return REProf;				
		//}
	}
}