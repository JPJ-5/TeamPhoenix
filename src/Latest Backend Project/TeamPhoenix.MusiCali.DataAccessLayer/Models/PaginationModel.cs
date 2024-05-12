//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
//{
//    public class PageList<T> : List<T>
//    {
//        public int CurrentPage { get; set; }
//        public int TotalPages { get; set; }
//        public int PageSize { get; set; }
//        public int TotalCount { get; set; }

//        public PageList(IEnumerable<T> items, int count, int pageNumber, int pageSize) 
//        {
//            CurrentPage = pageNumber;
//            TotalPages = (int) Math.Ceiling(count/(double) pageSize);
//            PageSize = pageSize;
//            TotalCount = count;
//            AddRange(items);
//        }

//        public static Task<PageList<T>> CreatePagination(IQueryable<T> source, int pageNumber, int pageSize)
//        {
//            int count = source.Count();  // Synchronous count
//            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();  // Synchronous to list
//            var pageList = new PageList<T>(items, count, pageNumber, pageSize);
//            return Task.FromResult(pageList);
//        }

//    }




    

//}

