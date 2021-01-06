using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSpaceListingModels
{	
    [Table("UserListing")]
	public class UserListing
	{
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ListingId { get; set; }
    }
}