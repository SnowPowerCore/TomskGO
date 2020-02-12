using System;
using Xamarin.Forms.Xaml;

namespace TomskGO.Core.Helpers
{
    public class ViewModelLocatorExtension : IMarkupExtension
    {
        public Type ViewModelType { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider) =>
            null != ViewModelType ? App.Services.GetService(ViewModelType) : default;
    }
}