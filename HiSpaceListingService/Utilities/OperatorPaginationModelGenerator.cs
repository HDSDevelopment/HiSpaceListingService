using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using HiSpaceListingService.ViewModel;

namespace HiSpaceListingService.Utilities
{
    public static class OperatorPaginationModelGenerator
    {

            public static async Task<PaginationModel<PropertyOperatorResponse>> GetPagedModelFromOperatorIds(
																			HiSpaceListingContext _context,
                                                                                List<int> allOperatorIds, 
                                                                                int currentPageNumber)
        {
    PaginationModel<PropertyOperatorResponse> pagedModel = new PaginationModel<PropertyOperatorResponse>();
            pagedModel.Count = allOperatorIds.Count;
            pagedModel.CurrentPage = currentPageNumber;

            List<int> currentPageOperatorIds = GetCurrentPageOperatorIds(currentPageNumber,
                														allOperatorIds,
                                                                        pagedModel.PageSize);

		    List<PropertyOperatorResponse> vModel =  await GetPropertyOperatorResponseList(_context, currentPageOperatorIds); 
								pagedModel.CurrentPageData = vModel;          
			return pagedModel;
        }

        static List<int> GetCurrentPageOperatorIds(int currentPageNumber, List<int> operatorIds, int pageSize)
		{
				int countToSkip = (currentPageNumber - 1) * pageSize;				
				operatorIds = operatorIds
                                        .Skip(countToSkip)
                                        .Take(pageSize)
                                        .ToList();
				return operatorIds;
		}

     static async Task<List<PropertyOperatorResponse>> GetPropertyOperatorResponseList(HiSpaceListingContext 
																			_context, 
																			List<int> operatorIds)
        {
            List<PropertyOperatorResponse> vModel = new List<PropertyOperatorResponse>();         

         List<User> REOperators = await GetOperatorsFromOperatorIds(_context, operatorIds);
        List<Listing> operatorsListings = await GetListingsOfOperators(_context, REOperators);
        PropertyOperatorResponse operatorResponse;

        List<Listing> listingsOfOperator;
        List<Listing> REProfessionals;
        List<RelatedREProfessional> relatedREProfessionals;
        List<REProfessionalMaster> projects = await _context.REProfessionalMasters
																				.AsNoTracking()
																				.ToListAsync();
        if(operatorsListings.Count() > 0)
        {
            foreach(User REOperator in REOperators)
            {
                operatorResponse = new PropertyOperatorResponse();
                
                operatorResponse.Operator = REOperator;
                listingsOfOperator = GetListingsOfGivenOperator(operatorsListings, REOperator.UserId);
                operatorResponse.TotalCommercial = GetListingsByType(listingsOfOperator, "Commercial").Count;
                operatorResponse.TotalCoWorking = GetListingsByType(listingsOfOperator, "Co-Working").Count;
                REProfessionals = GetListingsByType(listingsOfOperator, "RE-Professional");
                operatorResponse.TotalREProfessional = REProfessionals.Count;
        List<int> REProfessionalIdsForOperators = GetREProfessionalIds(REProfessionals);        
                operatorResponse.roles = await GetRoles(_context, REProfessionalIdsForOperators);
                relatedREProfessionals = RelatedREProfessionalUtility.GetForOperator(_context,                                                                              listingsOfOperator);
                operatorResponse.LinkedREProf = new List<LinkedREPRofessionals>();

                foreach (RelatedREProfessional professional in relatedREProfessionals)
                {
                    LinkedREPRofessionals REProf =RelatedREProfessionalUtility.Set(_context, 
																					projects, 
																					professional);
					operatorResponse.LinkedREProf.Add(REProf);
                }
                operatorResponse.LinkedREProfCount = operatorResponse.LinkedREProf.Count;                
                vModel.Add(operatorResponse);                
            }
        }
         return vModel;
        }

        static async Task<List<User>> GetOperatorsFromOperatorIds(HiSpaceListingContext _context, List<int> operatorIds)
        {
            List<User> operators = await (from REOperator in _context.Users.AsNoTracking()
											select REOperator)
															.Where(n => operatorIds.Contains(n.UserId))
															.ToListAsync();
			return operators;
        }

        static async Task<List<Listing>> GetListingsOfOperators(HiSpaceListingContext _context, 
                                                                    List<User> REOperators)
{
            IEnumerable<int> OperatorsIds = from REOperator in REOperators
                                            select REOperator.UserId;
                                                        

            List<Listing> OperatorsListings = await (from REProperty in _context.Listings
                                                where REProperty.DeletedStatus == false 
                                                select REProperty)
                                                .Where(n => OperatorsIds.Contains(n.UserId))
                                                .ToListAsync();
            return OperatorsListings;
        }

        static List<Listing> GetListingsOfGivenOperator(List<Listing> operatorsListings, int userId)
        {
            List<Listing> listingsOfGivenOperator = (from REProperty in operatorsListings
                                                            where REProperty.UserId == userId
                                                            select REProperty).ToList();
            return listingsOfGivenOperator;
        }

        static List<Listing> GetListingsByType(List<Listing> listings, 
                                                                string listingType)
        {
                return (from listing in listings
                        where listing.Status == true &&
                                listing.AdminStatus == true &&
                                listing.DeletedStatus == false &&
                                listing.ListingType == listingType
                        select listing).ToList();
        }

        async static Task<List<REProfessionalMaster>> GetProjectsByREProfessionals(
                                                                    HiSpaceListingContext _context,
                                                                    List<int> REProfessionalIDs)
        {
        List<REProfessionalMaster> projects = await (from REProject in _context.REProfessionalMasters
                                                                                        .AsNoTracking()
                                                    where REProject.DeletedStatus == false
                                                    select REProject)
                                                        .Where(n => REProfessionalIDs.Contains(n.ListingId))
                                                        .ToListAsync();
        return projects;                                                                    
        }

        static List<int> GetREProfessionalIds(List<Listing> REProfessionals)
        {
            List<int> professionalIds = (from professional in REProfessionals
                                        select professional.ListingId)
                                        .ToList();
            return professionalIds;
        }

        async static Task<List<string>> GetRoles(HiSpaceListingContext _context, List<int> REProfessionalIdsForOperators)
        {
            List<string> rolesByProfessionals = await (from REProject in _context.REProfessionalMasters
                                                                                        .AsNoTracking()
                                                where REProject.DeletedStatus == false
                                                select REProject)
                                                        .Where(n => REProfessionalIdsForOperators.Contains(n.ListingId))
                                                        .Select(n => n.ProjectRole)
                                                        .ToListAsync();
            return rolesByProfessionals;
        }

        

        
    }
}