using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiSpaceListingService.ViewModel;
using HiSpaceListingModels;
using HiSpaceListingService.Models;

namespace HiSpaceListingService.Utilities
{
    public static class ActionResultUtility
    {
        public static async Task<List<PropertyDetailResponse>> GetPropertyDetailResponses(int userId,                                                                                   ActionResult actionResult, 
                                                                            HiSpaceListingContext _context)
        {
            List<PropertyDetailResponse> response = null;
            List<UserListing> userListings = await (from userListing in _context.UserListings
                                                    where userListing.UserId == userId
                                                    select userListing).ToListAsync();

            Listing spaceListing = null;

            if (actionResult is OkObjectResult objectResult)
            {

                ObjectResult result = (ObjectResult)actionResult;
                response = (List<PropertyDetailResponse>)result.Value;

                foreach (UserListing userListing in userListings)
                {
                    spaceListing = (from property in response
                                    where property.SpaceListing != null &&
                                    property.SpaceListing.ListingId == userListing.ListingId
                                    select property.SpaceListing)
                                                                                .SingleOrDefault();

                    if (spaceListing != null)
                    {
                        spaceListing.IsFavorite = true;
                        spaceListing.FavoriteId = userListing.Id;
                    }
                }
            }
            return response;
        }

        
    }
}
