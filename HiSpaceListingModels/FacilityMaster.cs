using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSpaceListingModels
{
	[Table("FacilityMaster")]
	public class FacilityMaster
	{
		[Key]
		public int FacilityMasterId { set; get; }
		public string Name { set; get; }
		public bool Status { set; get; }
	}
}
