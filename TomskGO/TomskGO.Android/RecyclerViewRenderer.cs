using Android.Content;
using Android.Support.V7.Widget;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using TomskGO.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ARecyclerView = Android.Support.V7.Widget.RecyclerView;
using RecyclerView = TomskGO.Controls.RecyclerView;

[assembly: ExportRenderer(typeof(RecyclerView), typeof(RecyclerViewRenderer))]
namespace TomskGO.Droid
{
    class RecyclerViewRenderer : ViewRenderer<RecyclerView, ARecyclerView>
    {
        public RecyclerViewRenderer(Context c) : base(c) { }

        public static void Init()
        {
            #pragma warning disable 0219
            var ignore = typeof(RecyclerViewRenderer);
            #pragma warning restore 0219
        }

        protected override void OnElementChanged(ElementChangedEventArgs<RecyclerView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var itemsSource = e.OldElement.ItemsSource as INotifyCollectionChanged;
                if (itemsSource != null)
                {
                    itemsSource.CollectionChanged -= ItemsSourceOnCollectionChanged;
                }
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var recyclerView = new ARecyclerView(Context);
                    recyclerView.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
                    SetNativeControl(recyclerView);

                    var linearLayout = new LinearLayoutManager(Context, OrientationHelper.Vertical, false);

                    recyclerView.SetLayoutManager(linearLayout);

                    UpdateAdapter();
                }

                var itemsSource = e.NewElement.ItemsSource as INotifyCollectionChanged;
                if (itemsSource != null)
                {
                    itemsSource.CollectionChanged += ItemsSourceOnCollectionChanged;
                }
                Control.GetAdapter().NotifyDataSetChanged();

                Control.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                Control.HasFixedSize = true;

                Control.SetClipToPadding(false);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Element.ItemsSource))
            {
                UpdateAdapter();
            }
        }

        private void UpdateAdapter()
        {
            var newItemsSource = Element.ItemsSource as INotifyCollectionChanged;
            if (newItemsSource != null)
            {
                newItemsSource.CollectionChanged += ItemsSourceOnCollectionChanged;
            }
            var adapter = new RecyclerViewAdapter(Element) { HasStableIds = true };
            Control.SetAdapter(adapter);
        }

        private void ItemsSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var adapter = Control.GetAdapter();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    adapter.NotifyItemRangeInserted
                        (
                        positionStart: e.NewStartingIndex,
                        itemCount: e.NewItems.Count
                        );
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (((IList)Element.ItemsSource).Count == 0)
                    {
                        adapter.NotifyDataSetChanged();
                        return;
                    }
                    ((RecyclerViewAdapter)adapter).ClearRowHeight(e.OldStartingIndex);
                    adapter.NotifyItemRangeRemoved
                        (
                        positionStart: e.OldStartingIndex,
                        itemCount: e.OldItems.Count
                        );
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ((RecyclerViewAdapter)adapter).ClearRowHeight(e.OldStartingIndex);
                    adapter.NotifyItemRangeChanged
                        (
                        positionStart: e.OldStartingIndex,
                        itemCount: e.OldItems.Count
                        );
                    break;
                case NotifyCollectionChangedAction.Move:
                    for (var i = 0; i < e.NewItems.Count; i++)
                        adapter.NotifyItemMoved
                            (
                            fromPosition: e.OldStartingIndex + i,
                            toPosition: e.NewStartingIndex + i
                            );
                    break;
                case NotifyCollectionChangedAction.Reset:
                    adapter.NotifyDataSetChanged();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}