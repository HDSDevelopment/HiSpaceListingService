using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("Facility")]
	public class Facility
	{
		[Key]
		public int FacilityId { set; get; }
		public int ListingId { set; get; }
		public int? FacilityMasterId { set; get; }
		public string Name { set; get; }
		public string FacilityDistance { set; get; }
		public string Description { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
	}
}
