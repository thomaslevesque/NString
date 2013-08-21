using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NString.Internal
{
    static class CollectionExtensions
    {
        internal static IList<T> AsReadOnly<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
    }
}
