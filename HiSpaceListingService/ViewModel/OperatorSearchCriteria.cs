using System.Linq;

namespace HiSpaceListingService.ViewModel
{
public class OperatorSearchCriteria
{
		public int Id { get; set; }
		public string OperatorName { get; set; }
		public string CityName { get; set; }
		public int? CurrentPage { get; set; } = 0;
	}
}