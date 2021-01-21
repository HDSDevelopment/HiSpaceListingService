using HiSpaceListingModels;
using HiSpaceListingService.Models;
using HiSpaceListingService.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.Utilities
{
	public static class ActionResultUtility
	{
		public static async Task<List<PropertyDetailResponse>> GetPropertyDetailResponses(int userId, ActionResult actionResult,
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
	}
}