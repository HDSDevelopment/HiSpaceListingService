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
        public static async Task<PaginationModel<PropertyDetailResponse>> GetPropertyDetailResponses(int userId, ActionResult actionResult,
                                                                            HiSpaceListingContext _context)
        {
            PaginationModel<PropertyDetailResponse> response = null;
            List<UserListing> userListings = await (from userListing in _context.UserListings
                                                    where userListing.UserId == userId
                                                    select userListing).ToListAsync();

            Listing spaceListing = null;

            if (actionResult is OkObjectResult objectResult)
            {

                ObjectResult result = (ObjectResult)actionResult;
                response = (PaginationModel<PropertyDetailResponse>)result.Value;

                foreach (UserListing userListing in userListings)
                {
                    spaceListing = (from property in response.CurrentPageData
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

    public static async Task<PaginationModel<PropertyDetailResponse>> GetPropertyPageResponse(int userId,                                                                                 ActionResult actionResult,
                                                                            HiSpaceListingContext _context)
        {
            PaginationModel<PropertyDetailResponse> response = null;
            List<UserListing> userListings = await (from userListing in _context.UserListings
                                                    where userListing.UserId == userId
                                                    select userListing).ToListAsync();

            Listing spaceListing = null;

            if (actionResult is OkObjectResult objectResult)
            {

                ObjectResult result = (ObjectResult)actionResult;
                response = (PaginationModel<PropertyDetailResponse>)result.Value;                

                foreach (UserListing userListing in userListings)
                {
                    spaceListing = (from property in response.CurrentPageData
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

        public static async Task<List<PropertyPeopleResponse>> GetPropertyPeopleResponses(int userId, ActionResult actionResult,
                                                                        HiSpaceListingContext _context)
        {
            List<PropertyPeopleResponse> response = null;
            List<UserListing> userListings = await (from userListing in _context.UserListings
                                                    where userListing.UserId == userId
                                                    select userListing).ToListAsync();

            Listing listing = null;

            if (actionResult is OkObjectResult objectResult)
            {

                ObjectResult result = (ObjectResult)actionResult;
                response = (List<PropertyPeopleResponse>)result.Value;

                foreach (UserListing userListing in userListings)
                {
                    listing = (from property in response
                               where property.Listing != null &&
                               property.Listing.ListingId == userListing.ListingId
                               select property.Listing)
                                                                                .SingleOrDefault();

                    if (listing != null)
                    {
                        listing.IsFavorite = true;
                        listing.FavoriteId = userListing.Id;
                    }
                }
            }
            return response;
        }

        public static async Task<PaginationModel<PropertyPeopleResponse>> GetPeoplePageResponse(
                                                                                int userId,
                                                                                ActionResult actionResult,
                                                                                HiSpaceListingContext _context)
        {
            List<UserListing> userListings = await (from userListing in _context.UserListings
                                                    where userListing.UserId == userId
                                                    select userListing).ToListAsync();
            Listing listing = null;
            PaginationModel<PropertyPeopleResponse> response = null;

            if (actionResult is OkObjectResult objectResult)
            {
                ObjectResult result = (ObjectResult)actionResult;
                response = (PaginationModel<PropertyPeopleResponse>)result.Value;

                foreach (UserListing userListing in userListings)
                {
                    listing = (from property in response.CurrentPageData
                               where property.Listing != null &&
                               property.Listing.ListingId == userListing.ListingId
                               select property.Listing)
                                                                                .SingleOrDefault();
                    if (listing != null)
                    {
                        listing.IsFavorite = true;
                        listing.FavoriteId = userListing.Id;
                    }
                }
            }
            return response;
        }

        public static async Task<List<PropertyOperatorResponse>> GetPropertyOperatorResponses(int userId,
                                                                            ActionResult actionResult,
                                                                HiSpaceListingContext _context)
        {
            List<PropertyOperatorResponse> response = null;
            List<UserOperator> userOperators = await (from userOperator in _context.UserOperators
                                                      where userOperator.UserId == userId
                                                      select userOperator)
                                                                    .ToListAsync();

            User REOperator = null;

            if (actionResult is OkObjectResult objectResult)
            {
                ObjectResult result = (ObjectResult)actionResult;
                response = (List<PropertyOperatorResponse>)result.Value;

                foreach (UserOperator userOperator in userOperators)
                {
                    REOperator = (from property in response
                                  where property.Operator != null &&
                                  property.Operator.UserId == userOperator.OperatorId
                                  select property.Operator)
                                                        .SingleOrDefault();

                    if (REOperator != null)
                    {
                        REOperator.IsFavorite = true;
                        REOperator.FavoriteId = userOperator.Id;
                    }
                }
            }
            return response;
        }

        public static async Task<PaginationModel<PropertyOperatorResponse>> GetOperatorPageResponse(
                                                                                int userId,
                                                                                ActionResult actionResult,
                                                                                HiSpaceListingContext _context)
        {
            List<UserOperator> userOperators = await (from userOperator in _context.UserOperators
                                                      where userOperator.UserId == userId
                                                      select userOperator).ToListAsync();
            User REOperator = null;
            PaginationModel<PropertyOperatorResponse> response = null;

            if (actionResult is OkObjectResult objectResult)
            {
                ObjectResult result = (ObjectResult)actionResult;
                response = (PaginationModel<PropertyOperatorResponse>)result.Value;

                foreach (UserOperator userOperator in userOperators)
                {
                    REOperator = (from property in response.CurrentPageData
                                  where property.Operator != null &&
                                  property.Operator.UserId == userOperator.OperatorId
                                  select property.Operator)
                                                                                .SingleOrDefault();
                    if (REOperator != null)
                    {
                        REOperator.IsFavorite = true;
                        REOperator.FavoriteId = userOperator.Id;
                    }
                }
            }
            return response;
        }
    }
}
