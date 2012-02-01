using System;
using System.Collections.Generic;

namespace uNhAddIns.Pagination
{
	/// <summary>
	/// Results paginator.
	/// </summary>
	/// <typeparam name="T">The type of Entity.</typeparam>
	/// <seealso cref="IPaginator"/>
	/// <seealso cref="BasePaginator"/>
	/// <seealso cref="IPageProvider{T}"/>
	public class Paginator<T> : BasePaginator, IPageProvider<T>
	{
		private int pageSize;
		private long? rowsCount;
		private IRowsCounter counter;
		private readonly IPaginable<T> source;

		/// <summary>
		/// Create a new instance of <see cref="Paginator{T}"/>.
		/// </summary>
		/// <param name="pageSize">The page's elements size.</param>
		/// <param name="paginable">The paginable.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="paginable"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="pageSize"/> equal or less than zero.</exception>
		/// <remarks>
		/// If the <paramref name="paginable"/> implements <see cref="IRowsCounter"/> the <see cref="Counter"/> 
		/// property is set and the paginator work in "AutoCalcPages mode".
		/// </remarks>
		public Paginator(int pageSize, IPaginable<T> paginable)
		{
			if (paginable == null)
				throw new ArgumentNullException("paginable");
			if (pageSize <= 0)
				throw new ArgumentOutOfRangeException("pageSize",
				                                      string.Format("Page size expected greater than zero ; was {0}.", pageSize));
			this.pageSize = pageSize;
			source = paginable;
			// Check if the paginable implements IRowsCounter too
			Counter = paginable as IRowsCounter;
		}

		/// <summary>
		/// Create a new instance of <see cref="Paginator{T}"/>.
		/// </summary>
		/// <param name="pageSize">The page's elements size.</param>
		/// <param name="paginable">The paginable.</param>
		/// <param name="autoCalcPages">Enable or disable the "AutoCalcPages mode"; for more detail <see cref="AutoCalcPages"/>.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="paginable"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="pageSize"/> equal or less than zero.</exception>
		public Paginator(int pageSize, IPaginable<T> paginable, bool autoCalcPages)
			: this(pageSize, paginable)
		{
			AutoCalcPages = autoCalcPages;
		}

		/// <summary>
		/// Create a new instance of <see cref="Paginator{T}"/>.
		/// </summary>
		/// <param name="pageSize">The page's elements quantity.</param>
		/// <param name="paginable">The paginable.</param>
		/// <param name="counter">The rows counter.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="paginable"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="pageSize"/> equal or less than zero.</exception>
		/// <remarks> 
		/// If <paramref name="counter"/> is null it is simply ignored.
		/// If <paramref name="counter"/> is available the paginator work with "AutoCalcPages mode" enabled.
		/// <para>
		/// The Paginator don't make any check about queries. 
		/// This mean, for example, that the resposablity to check if the <paramref name="counter"/> query 
		/// work according the <paramref name="paginable"/> query is by the paginator user.
		/// Write your tests to be sure.
		/// </para>
		/// </remarks>
		public Paginator(int pageSize, IPaginable<T> paginable, IRowsCounter counter)
			: this(pageSize, paginable)
		{
			Counter = counter;
		}

		/// <summary>
		/// State of "AutoCalcPages mode".
		/// </summary>
		/// <remarks>
		/// Default = false.
		/// "AutoCalcPages mode" is enabled automatically when a <see cref="Counter"/> is available.
		/// If <see cref="Counter"/> is null and "AutoCalcPages mode" is enabled the paginator use 
		/// the <see cref="IPaginable{T}.ListAll()"/> to know the amount of availables pages.
		/// <para>
		/// Be carefully enabling "AutoCalcPages mode", without a specific <see cref="IRowsCounter"/>,
		/// because the first time you try to get <see cref="LastPageNumber"/> or <see cref="RowsCount"/> 
		/// the paginator automatically fecth all entities only to know the value to return.
		/// </para>
		/// The best practice is: use the constructor with <see cref="IRowsCounter"/>.
		/// </remarks>
		public bool AutoCalcPages { get; private set; }

		/// <summary>
		/// The <see cref="IRowsCounter"/> settled by constructor.
		/// </summary>
		public IRowsCounter Counter
		{
			get { return counter; }
			protected set
			{
				counter = value;
				AutoCalcPages = counter != null;
			}
		}

		#region IPageProvider<T> Members

		/// <summary>
		/// Number of visible objects of each page.
		/// </summary>
		/// <remarks>Change PageSize mean reset the <see cref="RowsCount"/>.</remarks>
		public int PageSize
		{
			get { return pageSize; }
			set
			{
				if (!pageSize.Equals(value))
				{
					pageSize = value;
					ResetLastPageNumber();
				}
			}
		}

		/// <summary>
		/// The total rows count.
		/// </summary>
		/// <remarks>Return null if rows count is not available.</remarks>
		public long? RowsCount
		{
			get
			{
				CheckRowCount();
				return rowsCount;
			}
		}
		
		/// <summary>
		/// Get True if the paginator has query results. False in other case.
		/// </summary>
		public bool HasPages
		{
			get { return FirstPageNumber != 0; }
		}

		/// <summary>
		/// Get the list of objects for a given page number and move the current page.
		/// </summary>
		/// <param name="pageNumber">The page number.</param>
		/// <returns>The list of objects.</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public IList<T> GetPage(int pageNumber)
		{
			GotoPageNumber(pageNumber);
			return source.GetPage(PageSize, CurrentPageNumber.Value);
		}

		/// <summary>
		/// Get the list of objects of the first page and move the current page.
		/// </summary>
		/// <returns>The list of objects.</returns>
		public IList<T> GetFirstPage()
		{
			return GetPage(FirstPageNumber);
		}

		/// <summary>
		/// Get the list of objects of the last page and move the current page.
		/// </summary>
		/// <returns>The list of objects.</returns>
		/// <exception cref="NotSupportedException">When <see cref="AutoCalcPages"/> is false</exception>
		public IList<T> GetLastPage()
		{
			if (!LastPageNumber.HasValue)
				throw new NotSupportedException("GetLastPage() is not supported when AutoCalcPages is false.");
			return GetPage(LastPageNumber.Value);
		}

		/// <summary>
		/// Get the list of objects of the next page and move the current page.
		/// </summary>
		/// <returns>The list of objects.</returns>
		public IList<T> GetNextPage()
		{
			return GetPage(NextPageNumber);
		}

		/// <summary>
		/// Get the list of objects of the previous page and move the current page.
		/// </summary>
		/// <returns>The list of objects.</returns>
		public IList<T> GetPreviousPage()
		{
			return GetPage(PreviousPageNumber);
		}

		/// <summary>
		/// Get the list of objects of the current page.
		/// </summary>
		/// <returns>The list of objects.</returns>
		/// <exception cref="NotSupportedException">When the current page is not available.</exception>
		public IList<T> GetCurrentPage()
		{
			if (!CurrentPageNumber.HasValue)
				throw new NotSupportedException("Current page not available.");
			return source.GetPage(PageSize, CurrentPageNumber.Value);
		}

		#endregion

		/// <summary>
		/// Move the current page to a given page number.
		/// </summary>
		/// <param name="pageNumber">The page number.</param>
		public new void GotoPageNumber(int pageNumber)
		{
			base.GotoPageNumber(pageNumber);
		}

		/// <summary>
		/// The number of the last page if available; otherwise null.
		/// </summary>
		public override int? LastPageNumber
		{
			get
			{
				CheckRowCount();
				return base.LastPageNumber;
			}
		}

		private void CheckRowCount()
		{
			if (!base.LastPageNumber.HasValue && AutoCalcPages)
			{
				CalcLastPageAndRowCount();
			}
		}

		private void CalcLastPageAndRowCount()
		{
			rowsCount = counter != null ? counter.GetRowsCount(source.GetSession()) : source.ListAll().Count;
			ResetLastPageNumber();
		}

		private void ResetLastPageNumber() 
		{
			base.LastPageNumber = Convert.ToInt32(rowsCount.GetValueOrDefault() / PageSize) + ((rowsCount.GetValueOrDefault() % PageSize) == 0 ? 0 : 1);
		}
	}
}
