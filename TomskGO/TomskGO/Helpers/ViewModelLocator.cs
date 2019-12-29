using System;
using Xamarin.Forms;

namespace TomskGO.Core.Helpers
{
    public static class ViewModelLocator
    {
        public static readonly BindableProperty WireType =
            BindableProperty.CreateAttached("WireType", typeof(Type), typeof(ViewModelLocator),
                default(bool), propertyChanged: OnWireTypeChanged);

        public static Type GetWireType(BindableObject bindable) =>
            (Type)bindable.GetValue(WireType);

        public static void SetWireType(BindableObject bindable, Type value) =>
            bindable.SetValue(WireType, value);

        private static void OnWireTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view is null) return;

            if (newValue is null) return;

            var newType = (Type)newValue;
            var viewModel = App.Services.GetService(newType);
            view.BindingContext = viewModel;
        }
    }
}