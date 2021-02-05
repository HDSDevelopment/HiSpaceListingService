using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using HiSpaceListingService.ViewModel;


namespace HiSpaceListingService.Utilities
{
    public static class RelatedREProfessionalUtility
{
 		public static List<RelatedREProfessional> Get(List<REProfessionalMaster> projects, 																Listing property)
		{
			List<RelatedREProfessional> RelatedREProf = new List<RelatedREProfessional>();
			RelatedREProf = (from r in projects
									where (
									(((r.PropertyReraId != null && property.CMCW_ReraId != null) && (r.PropertyReraId == property.CMCW_ReraId))
									|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_CTSNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_CTSNumber))
									|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_MilkatNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_MilkatNumber))
									|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_PlotNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_PlotNumber))
									|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_SurveyNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_SurveyNumber))
									|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_PropertyTaxBillNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_PropertyTaxBillNumber))
									|| ((r.PropertyAdditionalIdNumber != null && property.CMCW_GatNumber != null) && (r.PropertyAdditionalIdNumber == property.CMCW_GatNumber)))
									&& (r.LinkingStatus == "Approved") && (r.DeletedStatus == false))
									select new RelatedREProfessional
									{
										ListingId = property.ListingId,
										ProjectId = r.REProfessionalMasterId,
										UserId = property.UserId,
										ProjectRole = r.ProjectRole,
										ProjectName = r.ProjectName,
										ImageUrl = r.ImageUrl,
										OperatorName = r.OperatorName,
										LinkingStatus = r.LinkingStatus
									}).ToList();
				return RelatedREProf;
		}

		public static List<RelatedREProfessional> GetForOperator(HiSpaceListingContext _context, 
                                                                        List<Listing> listingsOfOperator)
        {
		List<RelatedREProfessional> linkedREProfessionals = (from listing in listingsOfOperator
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
                                            select new RelatedREProfessional
                                            {
                                                ListingId = listing.ListingId,
                                                ProjectId = ReProf.REProfessionalMasterId,
                                                UserId = listing.UserId,
                                                ProjectRole = ReProf.ProjectRole,
                                                ProjectName = ReProf.ProjectName,
                                                ImageUrl = ReProf.ImageUrl,
                                                OperatorName = ReProf.OperatorName,
                                                LinkingStatus = ReProf.LinkingStatus
                                            }).ToList();
        return linkedREProfessionals;
        }

        public static LinkedREPRofessionals Set(HiSpaceListingContext _context, 
															IEnumerable<REProfessionalMaster> projects, 
															RelatedREProfessional professional)
		{
					LinkedREPRofessionals REProf = new LinkedREPRofessionals();
		var GetListingIdOnReProfessional = projects.Where(d => d.REProfessionalMasterId == 					
																professional.ProjectId)
													.Select(d => d.ListingId)
													.First();

					REProf.Property_ListingId = professional.ListingId;
					REProf.ReProfessional_ListingId = GetListingIdOnReProfessional;
					REProf.REProfessionalMasterId = professional.ProjectId;
					REProf.UserId = professional.UserId;
					REProf.ProjectRole = professional.ProjectRole;
					REProf.OperatorName = professional.OperatorName;
					REProf.LinkingStatus = professional.LinkingStatus;
					REProf.ProjectName = professional.ProjectName;
					REProf.ImageUrl = professional.ImageUrl;

					var REName = (from listing in _context.Listings.AsNoTracking()
									where listing.ListingId == GetListingIdOnReProfessional
									select new { listing.RE_FirstName, listing.RE_LastName })
																.First();

					REProf.REFirstName = REName.RE_FirstName;
					REProf.RELastName = REName.RE_LastName;
					return REProf;
        }
    }
}
