using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TomskGO.Models;

namespace TomskGO.Providers
{
    abstract class AbstractFeedProvider : Handlers.WebHandler
    {
        protected abstract FeedProviderData ProviderData { get; }

        protected override Uri BaseAddress => new Uri(ProviderData?.BaseUrl);

        public abstract Task<IEnumerable<FeedModel>> ProvideData();

        #region Auto-implemented
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}