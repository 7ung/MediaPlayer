using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Model
{
    public static class ObserList
    {
        // ref: http://stackoverflow.com/questions/1945461/how-do-i-sort-an-observable-collection
        // ref template: Enumerable.Orderby
        public static void SortBy<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> selector)
        {
            // parameter this sẽ có tác động trực tiếp lên thằng nào gọi .SortBy
            List<TSource> sorted = source.OrderBy(selector).ToList();
            for (int i = 0; i < sorted.Count; )
            {
                // so sánh mãng được sắp xếp
                if (source[i].Equals(sorted[i]))
                {
                    // nếu khác thì sắp lại cho giống mãng sắp xếp
                    TSource t = source[i];
                    source.RemoveAt(i);
                    source.Insert(sorted.IndexOf(t), t);
                }
                else
                {
                    i++;
                }
            }
        }

    }
}
