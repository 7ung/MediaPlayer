using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Model
{
    public class AlphaKeyGroup<T>
    {
        const string GlobalGroupKey = "\uD83C\uDF10";
        // use to get key info
        public delegate string GetKeyDelegate(T item);

        // khoá thuộc tính để chọn group
        public string Key { get; private set; }
        public List<T> InternalList { get; private set; }
        public AlphaKeyGroup(string key)
        {
            Key = key;
            InternalList = new List<T>();
        }

        public static List<AlphaKeyGroup<T>> CreatGroups(IEnumerable<T> items, Func<T, string> keyselector, bool sort )
        {
            Windows.Globalization.Collation.CharacterGroupings slg = new Windows.Globalization.Collation.CharacterGroupings();
            List<AlphaKeyGroup<T>> list = CreateDefaultGroups(slg);

            foreach (T item in items)
            {
                int index = 0;
                string label = slg.Lookup(keyselector(item));
                index = list.FindIndex(alphakeygroup => alphakeygroup.Key.Equals(label, StringComparison.CurrentCulture));
                if (index >= 0 && index < list.Count)
                {
                    list[index].InternalList.Add(item);
                }
            }
            if (sort)
            {
                foreach (AlphaKeyGroup<T> group in list)
                {
                   group.InternalList.Sort((c0, c1) => { return keyselector(c0).CompareTo(keyselector(c1)); });
                }
            }
            return list;
        }

        private static List<AlphaKeyGroup<T>> CreateDefaultGroups(Windows.Globalization.Collation.CharacterGroupings slg)
        {
            List<AlphaKeyGroup<T>> list = new List<AlphaKeyGroup<T>>();
            foreach (var item in slg)
            {
                if (item.Label == "") continue;
                if (item.Label == "...")
                    list.Add(new AlphaKeyGroup<T>(GlobalGroupKey));
                else
                    list.Add(new AlphaKeyGroup<T>(item.Label));
            }
            return list;
        }
    }
}
