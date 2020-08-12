using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("REProfessionalMaster")]
	public class REProfessionalMaster
	{
		[Key]
		public int REProfessionalMasterId { set; get; }
		public int ListingId { set; get; }
		public string ProjectName { set; get; }
		public string ImageUrl { set; get; }
		public string Description { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
		public string DocumentUrl { set; get; }
		public string PropertyReraId { set; get; }
		public string PropertyAdditionalIdName { set; get; }
		public string PropertyAdditionalIdNumber { set; get; }
		public string ProjectRole { set; get; }
		public string OperatorName { set; get; }
		public string LinkingStatus { set; get; }

	}
}
