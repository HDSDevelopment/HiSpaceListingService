using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using HiSpaceListingService.ViewModel;


namespace HiSpaceListingService.Utilities
{
    public static class RelatedREOperatorUtility
{
 		public async static Task<List<RelatedREOperator>> Get(HiSpaceListingContext _context,
		 													int relatedPropertyId)
		{
            List<RelatedREOperator> relatedOperators = new List<RelatedREOperator>();
            			relatedOperators = await (from listing in _context.Listings.AsNoTracking()
									   		from REProject in _context.REProfessionalMasters.AsNoTracking()
									   where (
									   (listing.ListingType == "Commercial" || listing.ListingType == "Co-Working") && listing.DeletedStatus == false &&
										(((listing.CMCW_ReraId != null && REProject.PropertyReraId != null) && (listing.CMCW_ReraId == REProject.PropertyReraId))
								   || ((listing.CMCW_CTSNumber != null && REProject.PropertyAdditionalIdNumber != null) && (listing.CMCW_CTSNumber == REProject.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_GatNumber != null && REProject.PropertyAdditionalIdNumber != null) && (listing.CMCW_GatNumber == REProject.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_MilkatNumber != null && REProject.PropertyAdditionalIdNumber != null) && (listing.CMCW_MilkatNumber == REProject.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_PlotNumber != null && REProject.PropertyAdditionalIdNumber != null) && (listing.CMCW_PlotNumber == REProject.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_SurveyNumber != null && REProject.PropertyAdditionalIdNumber != null) && (listing.CMCW_SurveyNumber == REProject.PropertyAdditionalIdNumber))
								   || ((listing.CMCW_PropertyTaxBillNumber != null && REProject.PropertyAdditionalIdNumber != null) && (listing.CMCW_PropertyTaxBillNumber == REProject.PropertyAdditionalIdNumber))
									)
										&& (REProject.LinkingStatus == "Approved") && (REProject.ListingId == relatedPropertyId) && (REProject.DeletedStatus == false))
									   select new RelatedREOperator
									   {
										   ListingId = listing.ListingId,
										   ProjectId = REProject.REProfessionalMasterId,
										   UserId = listing.UserId,
										   ProjectRole = REProject.ProjectRole,
										   ProjectName = REProject.ProjectName,
										   ImageUrl = REProject.ImageUrl,
										   OperatorName = REProject.OperatorName,
										   LinkingStatus = REProject.LinkingStatus
									   }).ToListAsync();
        return relatedOperators;
        }

		public static async Task<LinkedOperators> Set(HiSpaceListingContext _context, 
													RelatedREOperator relatedREOperator)
		{
					LinkedOperators Oper = new LinkedOperators();
				int listingIdOnReProfessional = await (from project in _context.REProfessionalMasters.AsNoTracking()
													 where project.REProfessionalMasterId == relatedREOperator.ProjectId
													 select project.ListingId)
													.FirstAsync();
					//var GetListingIdOnpro = _context.Listings.Where(d => d.ListingId == GetListingIdOnReProfessional).Select(d => d.UserId).First();
					Oper.Property_ListingId = relatedREOperator.ListingId;
					Oper.ReProfessional_ListingId = listingIdOnReProfessional;
					Oper.REProfessionalMasterId = relatedREOperator.ProjectId;
					Oper.UserId = relatedREOperator.UserId;
					Oper.ProjectRole = relatedREOperator.ProjectRole;
					Oper.ProjectName = relatedREOperator.ProjectName;					

					Oper.PropertyName = await (from listing in _context.Listings.AsNoTracking()
										 where listing.ListingId == relatedREOperator.ListingId
										 select listing.Name)
										.FirstAsync();

					var companyNameAndLogo = await (from operatr in _context.Users.AsNoTracking()
											where operatr.UserId == relatedREOperator.UserId
											select new {operatr.CompanyName, operatr.Doc_CompanyLogo})
												.FirstAsync();

					Oper.CompanyName = companyNameAndLogo.CompanyName;
					Oper.Doc_CompanyLogo = companyNameAndLogo.Doc_CompanyLogo;
			return Oper;
		}
}
}