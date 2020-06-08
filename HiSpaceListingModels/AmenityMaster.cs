using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("AmenityMaster")]
	public class AmenityMaster
	{
		[Key]
		public int AmenityMasterId { set; get; }
		public string Name { set; get; }
		public bool Status { set; get; }
	}
}
