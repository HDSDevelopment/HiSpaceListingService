using System;
using System.Collections.Generic;

namespace HiSpaceListingService.ViewModel
{
public class PaginationModel<T>
{
    public int CurrentPage { get; set; } = 1;
    public int Count { get; set; }
    public int PageSize { get; set; } = 5;

    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

    public List<T> CurrentPageData { get; set; }
}
}