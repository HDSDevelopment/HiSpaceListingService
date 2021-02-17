using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSpaceListingModels
{
	[Table("UserOperator")]
	public class UserOperator
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public int OperatorId { get; set; }
	}
}
