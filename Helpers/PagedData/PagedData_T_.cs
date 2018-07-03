using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Helpers.PagedData
{
    public class PagedData<T>
    where T : class
    {
        public int CurrentPage
        {
            get;
            set;
        }

        public IEnumerable<T> Data
        {
            get;
            set;
        }

        public int NumberOfPages
        {
            get;
            set;
        }

        public int TotalNumberOfRecords
        {
            get;
            set;
        }

        public PagedData()
        {
        }
    }
}