using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("ListingImages")]
	public class ListingImages
	{
		[Key]
		public int ListingImagesId { set; get; }
		public int ListingId { set; get; }
		public string Name { set; get; }
		public string ImageUrl { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
	}
}
