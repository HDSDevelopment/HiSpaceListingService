using HiSpaceListingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.ViewModel
{
	public class InvestorViewModel : Investor
	{
		Investor _investor;

		public bool IsSuccessMessageSent { set; get; }

		public InvestorViewModel(Investor investor) 
		{
			_investor = investor;
			
		}
		public void Assign()
		{
			this.InvestorId = _investor.InvestorId;
			this.FirstName = _investor.FirstName;
			this.LastName = _investor.LastName;
			this.Email = _investor.Email;
			this.Phone = _investor.Phone;
			this.InvestmentType = _investor.InvestmentType;
			this.PropertyType = _investor.PropertyType;
			this.Currency = _investor.Currency;
			this.MinRange= _investor.MinRange;
			this.MaxRange = _investor.MaxRange;
			this.During = _investor.During;
			this.Country = _investor.Country;
			this.State = _investor.State;
			this.District = _investor.District;
			this.Neighborhood = _investor.Neighborhood;
			this.Comment = _investor.Comment;
			this.CreatedDateTime = _investor.CreatedDateTime;
		}
		public Investor GetInvestor()
		{
			_investor.InvestorId = InvestorId;
			_investor.FirstName = FirstName;
			_investor.LastName = LastName;
			_investor.Email = Email;
			_investor.Phone = Phone;
			_investor.InvestmentType = InvestmentType;
			_investor.PropertyType = PropertyType;
			_investor.Currency = Currency;
			_investor.MinRange = MinRange;
			_investor.MaxRange = MaxRange;
			_investor.During = During;
			_investor.Country = Country;
			_investor.State = State;
			_investor.District = District;
			_investor.Neighborhood = Neighborhood;
			_investor.Comment = Comment;
			_investor.CreatedDateTime = CreatedDateTime;
			return _investor;
		}
	}
}
