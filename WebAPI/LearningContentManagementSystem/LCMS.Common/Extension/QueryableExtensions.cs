using System;
using System.Linq;
using System.Linq.Expressions;

namespace LCMS.Common.Extension
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string propertyName)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var sorts = propertyName.Split(",".ToArray());

            for (var i = 0; i < sorts.Length; i++)
            {
                int descIndex = sorts[i].IndexOf(" DESC");
                if (descIndex >= 0)
                {
                    propertyName = sorts[i].Substring(0, descIndex).Trim();
                }

                int ascIndex = sorts[i].IndexOf(" ASC");
                if (ascIndex >= 0)
                {
                    propertyName = sorts[i].Substring(0, ascIndex).Trim();
                }

                if (descIndex == -1 && ascIndex == -1)
                {
                    propertyName = sorts[i].Trim();
                }

                if (string.IsNullOrEmpty(propertyName))
                {
                    return source;
                }

                ParameterExpression parameter = Expression.Parameter(source.ElementType, String.Empty);
                MemberExpression property = Expression.Property(parameter, propertyName);
                LambdaExpression lambda = Expression.Lambda(property, parameter);

                string methodName = (descIndex < 0) ? (i == 0) ? "OrderBy" : "ThenBy" : (i == 0) ? "OrderByDescending" : "ThenByDescending";

                Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                    new Type[] { source.ElementType, property.Type },
                                                    source.Expression, Expression.Quote(lambda));

                source = source.Provider.CreateQuery<T>(methodCallExpression);
            }

            return source;
        }
    }
}
