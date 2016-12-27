using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LCMS.Common
{
    public class PagedCollection<T> : IEnumerable<T>
    {
        public PagedCollection(IEnumerable<T> result, int total)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (total < 0)
            {
                throw new ArgumentOutOfRangeException("total");
            }

            Result = new ReadOnlyCollection<T>(new List<T>(result));
            Total = total;
        }

        public ICollection<T> Result { get; private set; }

        public int Total { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return Result.Count == 0;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Result.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Result.GetEnumerator();
        }
    }
}
