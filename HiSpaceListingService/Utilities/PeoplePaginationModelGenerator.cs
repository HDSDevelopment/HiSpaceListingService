using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using HiSpaceListingService.ViewModel;

namespace HiSpaceListingService.Utilities
{
    public static class PeoplePaginationModelGenerator
    {
        public static async Task<PaginationModel<PropertyPeopleResponse>> GetPagedModelFromPropertyIds(
																			HiSpaceListingContext _context,
                                                                                List<int> allPeopleIds, 
                                                                                int currentPageNumber)
        {
        PaginationModel<PropertyPeopleResponse> pagedModel = new PaginationModel<PropertyPeopleResponse>();
            pagedModel.Count = allPeopleIds.Count;
            pagedModel.CurrentPage = currentPageNumber;

            List<int> currentPagePeopleIds = GetCurrentPagePeopleIds(currentPageNumber,
                														allPeopleIds,
                                                                        pagedModel.PageSize);				
			List<PropertyPeopleResponse> vModel = await GetPropertyPeopleResponseList(_context, 
                                                                            currentPagePeopleIds); 
				pagedModel.CurrentPageData = vModel;          
		
		return pagedModel;
        }

        static List<int> GetCurrentPagePeopleIds(int currentPageNumber, List<int> peopleIds, int pageSize)
		{
				int countToSkip = (currentPageNumber - 1) * pageSize;				
				peopleIds = peopleIds
                                        .Skip(countToSkip)
                                        .Take(pageSize)
                                        .ToList();
				return peopleIds;
		}

        static async Task<List<PropertyPeopleResponse>> GetPropertyPeopleResponseList(
                                                                HiSpaceListingContext _context, 
																			List<int> peopleIds)
		{
         	List<PropertyPeopleResponse> vModel = new List<PropertyPeopleResponse>();
			List<Listing> people = await GetPeopleFromPeopleIds(_context, peopleIds);
            List<int> operatorIdsOfPeople = GetOperatorIdsFromPeople(people);
            List<User> operatorsOfPeople = await GetOperatorsFromOperatorIds(_context, operatorIdsOfPeople);
            List<REProfessionalMaster> peopleProjects = await GetProjectsOfPeople(_context, peopleIds);

            PropertyPeopleResponse peopleResponse;

            if(people.Count > 0)
            foreach(Listing REProfessional in people)
            {
                peopleResponse = new PropertyPeopleResponse();
                peopleResponse.Listing = REProfessional;
                peopleResponse.Operator = GetOperatorOfREProfessional(REProfessional, operatorsOfPeople);
                peopleResponse.Projects = GetProjectsForGivenProfessional(REProfessional.ListingId, 
                                                                            peopleProjects);
                peopleResponse.Roles = GetRolesInProjects(peopleResponse.Projects);
                peopleResponse.TotalProjects = peopleResponse.Projects.Count();
					List<RelatedREOperator> relatedREOperators = await RelatedREOperatorUtility.Get(_context, peopleResponse.Listing.ListingId);
					peopleResponse.LinkedOpr = new List<LinkedOperators>();
                LinkedOperators linkedOperator;
					foreach (RelatedREOperator relatedREOperator in relatedREOperators)
					{
						linkedOperator = await RelatedREOperatorUtility.Set(_context, relatedREOperator);
						peopleResponse.LinkedOpr.Add(linkedOperator);
					}
					vModel.Add(peopleResponse);
            }
            return vModel;
        }

        static async Task<List<Listing>> GetPeopleFromPeopleIds(HiSpaceListingContext _context, List<int> peopleIds)
		{
			List<Listing> people = await (from REProfessional in _context.Listings.AsNoTracking()
											select REProfessional)
															.Where(n => peopleIds.Contains(n.ListingId))
															.ToListAsync();
			return people;
		}

        static List<int> GetOperatorIdsFromPeople(List<Listing> people)
        {
            List<int> operatorIds = (from professional in people
                                    select professional.UserId)
                                                                .ToList();
            return operatorIds;
        }

        async static Task<List<User>> GetOperatorsFromOperatorIds(HiSpaceListingContext _context, 
                                                                List<int> operatorIds)
        {
            List<User> REOperators = await (from REOperator in _context.Users.AsNoTracking()
                                    select REOperator)
                                    .Where(n => operatorIds.Contains(n.UserId))
                                    .ToListAsync();
            return REOperators;
        }

        static User GetOperatorOfREProfessional(Listing REProfessional, List<User> REOperators)
        {
            User REOperator = (from user in REOperators
                            where user.UserId == REProfessional.UserId
                            select user)
                            .SingleOrDefault();
            return REOperator;
        }

        static async Task<List<REProfessionalMaster>> GetProjectsOfPeople(HiSpaceListingContext _context, 
                                                                                List<int> peopleIds)
        {
        List<REProfessionalMaster> projects = await (from REProject in _context.REProfessionalMasters.AsNoTracking()
                                                    select REProject)
                                                                .Where(n => peopleIds.Contains(n.ListingId))
                                                                .ToListAsync();
        return projects;
        }

        static List<REProfessionalMaster> GetProjectsForGivenProfessional(int professionalId, 
                                                            List<REProfessionalMaster> peopleProjects)
        {
            List<REProfessionalMaster> projects = (from REProject in peopleProjects
                                                    where REProject.ListingId == professionalId
                                                    select REProject)
                                                    .ToList();
            return projects;
        }

        static List<string> GetRolesInProjects(List<REProfessionalMaster> professionalProjects)
        {
            List<string> roles = (from REProject in professionalProjects
                                    select REProject.ProjectRole)
                                    .Distinct()
                                    .ToList();
            return roles;
        }
}
}
