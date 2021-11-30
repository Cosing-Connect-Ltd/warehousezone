using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Warehouse.PropertiesSync.SyncLibrary
{
    public static class LinqHelpers
    {
        public static IEnumerable<IEnumerable<T>> Batches<T>(this IEnumerable<T> items, int maxItems)
        {
            return items.Select((item, inx) => new { item, inx })
                .GroupBy(x => x.inx / maxItems)
                .Select(g => g.Select(x => x.item));
        }
    }
}