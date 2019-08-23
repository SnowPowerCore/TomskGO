using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TomskGO.Models;

namespace TomskGO.Interfaces
{
    interface IFeedProvider : INotifyPropertyChanged
    {
        ICommand ProvideDataCommand { get; }

        ObservableCollection<FeedModel> FeedData { get; set; }
    }
}