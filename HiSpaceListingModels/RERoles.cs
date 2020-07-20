﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("RERoles")]
	public class RERoles
	{
		[Key]
		public int RERols { set; get; }
		public int ListingId { set; get; }
		public int REProfessionalMasterId { set; get; }
		public string RERole { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
	}
}
