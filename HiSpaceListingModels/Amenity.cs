using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("Amenity")]
	public class Amenity
	{
		[Key]
		public int AmenityId { set; get; }
		public int ListingId { set; get; }
		public int? AmenityMasterId { set; get; }
		public string Name { set; get; }
		public string AmenitiesPayment { set; get; }
		public int? PartialCount { set; get; }
		public decimal? Price { set; get; }
		public string Description { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
	}
}
