using System;

namespace GourmeJunk.Models.ViewModels
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages => (int) Math.Ceiling((double) TotalItems / ItemsPerPage);

        public string UrlParam { get; set; }
    }
}
