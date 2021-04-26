using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Model
{

    public enum SortDirection
    {
        ASC, DESC
    }
    public class RestaurantQuery
    {
       public string SearchPhase { get; set; }
       public  int ItemsPerPage { get; set; }
       public  int PageNumber { get; set; }
       public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
