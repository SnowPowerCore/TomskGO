using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace TomskGO.Models
{
    class NavigationGroup : ObservableRangeCollection<NavigationItem>
    {
        public string GroupName { get; set; }

        public NavigationGroup(List<NavigationItem> items = null)
        {
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Add(items[i]);
                }
            }
        }
    }

    class NavigationItem
    {
        public string Name { get; set; }
        public string ResourceString { get; set; }
        public Color Color { get; set; } = Color.WhiteSmoke;
        public ICommand Command { get; set; }
    }
}
