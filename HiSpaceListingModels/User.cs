using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("User")]
	public class User
	{
		[Key]
		public int UserId { set; get; }
		public string Email { set; get; }
		public string Password { set; get; }

		public string CompanyName { set; get; }

		public string Website { set; get; }

		public string Phone { set; get; }

		public string Doc_CompanyLogo { set; get; }

		public string Address { set; get; }

		public string City { set; get; }

		public string State { set; get; }

		public string Country { set; get; }

		public string Postalcode { set; get; }

		public string Fax { set; get; }

		public string Doc_RCCopy { set; get; }


		public int? UserType { set; get; }

		public int? LoginCount { set; get; }

		public DateTime? LastLoginDateTime { set; get; }

		public bool Status { set; get; }

		public DateTime? CreatedDateTime { set; get; }

		public int? ModifyBy { set; get; }

		public DateTime? ModidyDateTime { get; set; }
		public string UserState { get; set; }
		public bool TermsAndConditions { get; set; }
		public string ProofName { get; set; }
		public string ProofNumber { get; set; }

		[NotMapped]
		public bool IsFavorite { get; set; } = false;
		[NotMapped]
		public int? FavoriteId { get; set; } = null;
		public string ActivationToken { get; set; } = null;
		public DateTime? TokenExpiryAt { get; set; }
	}
}