using Ecam.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Models {
	public class PaginatedList<T>:List<T> {

		public int PageIndex { get; private set; }
		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }
		public int TotalPages { get; private set; }

		public PaginatedList(IQueryable<T> source,int pageIndex,int pageSize) {
			PageIndex = pageIndex - 1;
			PageSize = pageSize;
			TotalCount = source.Count();
			TotalPages = (int)Math.Ceiling(DataTypeHelper.SafeDivision(TotalCount,(decimal)PageSize));
			this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize).Execute());
		}
         

		public PaginatedList(IQueryable<T> source,int pageIndex,int pageSize,ref int totalCount) {
			PageIndex = pageIndex - 1;
			PageSize = pageSize;
			TotalCount = source.Count();
			TotalPages = (int)Math.Ceiling(DataTypeHelper.SafeDivision(TotalCount,(double)PageSize));
			totalCount = TotalCount;
			this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize).Execute());
		}

	}

	public class PaginatedListResult {
		public int total;
		public IEnumerable<object> rows;
	}

	public class PaginatedListResult<T> {
		public int total;
		public IEnumerable<T> rows;
	}

	public class Paging {

		public Paging() {
			this.PageSize = 10;
			this.PageIndex = 1;
			this.SortName = "id";
			this.SortOrder = "asc";
			this.Total = 0;
		}

		public int PageIndex { get; set; }

		public int PageSize { get; set; }

		public string SortName { get; set; }

		public string SortOrder { get; set; }

		public int Total { get; set; }
	}
}
