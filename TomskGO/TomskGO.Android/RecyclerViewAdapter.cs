using System;
using System.Collections;
using System.Collections.Concurrent;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;
using XRecyclerView = TomskGO.Controls.RecyclerView;

namespace TomskGO.Droid
{
    class RecyclerViewAdapter : RecyclerView.Adapter, View.IOnClickListener
    {
        ConcurrentDictionary<int, int> _rowHeights = new ConcurrentDictionary<int, int>();

        private readonly XRecyclerView _fastListView;
        private readonly IList _dataSource;

        public RecyclerViewAdapter(XRecyclerView listView)
        {
            _fastListView = listView;
            _dataSource = (IList)listView.ItemsSource;
        }

        public override int ItemCount => _dataSource.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var itemType = GetItemViewType(position);
            var _h = (RecyclerViewHolder)holder;
            _h.ItemView.SetOnClickListener(this);
            var item = _dataSource[position];
            if (item != null)
            {
                var _viewCell = _fastListView.CreateDefault(item);
                _viewCell.BindingContext = item;
                _viewCell.Parent = _fastListView;

                int specifiedRowHeight;

                _rowHeights.TryGetValue(position, out specifiedRowHeight);
                if (specifiedRowHeight <= 0)
                {
                    specifiedRowHeight = 44;
                    specifiedRowHeight = (int)this.CalculateHeightForCell(_fastListView, _viewCell);
                }
                _h.UpdateUi(_viewCell, item, _fastListView, specifiedRowHeight);
            }
        }

        public void OnClick(View v)
        {
            //TO-DO
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            int specifiedRowHeight;

            _rowHeights.TryGetValue(viewType, out specifiedRowHeight);

            // -- Gestion du Cache
            if (specifiedRowHeight <= 0)
            {
                specifiedRowHeight = 44;
                var item = _dataSource[viewType];
                if (item != null)
                {
                    var _viewCell = _fastListView.CreateDefault(item);
                    _viewCell.BindingContext = item;
                    _viewCell.Parent = _fastListView;

                    specifiedRowHeight = (int)this.CalculateHeightForCell(_fastListView, _viewCell);
                    _rowHeights[viewType] = specifiedRowHeight;
                }
            }

            var contentFrame = new FrameLayout(parent.Context)
            {
                LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
                {
                    Height = (int)(specifiedRowHeight * Forms.Context.Resources.DisplayMetrics.Density),
                    Width = (int)(_fastListView.Width * Forms.Context.Resources.DisplayMetrics.Density)
                }
            };
            contentFrame.DescendantFocusability = DescendantFocusability.AfterDescendants;

            var viewHolder = new RecyclerViewHolder(contentFrame);
            Console.WriteLine("Position : {0}, Height : {1}", viewType, specifiedRowHeight);
            return viewHolder;
        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Forms.Context.Resources.DisplayMetrics.Density);
            return dp;
        }

        public override int GetItemViewType(int position)
        {
            return (position);
        }

        public override long GetItemId(int position)
        {
            var item = _dataSource[position];
            return item.GetHashCode();
        }

        IVisualElementRenderer _renderer = null;

        public float CalculateHeightForCell(XRecyclerView fastListView, ViewCell cell)
        {
            var target = cell.View;

            if (_renderer == null)
                _renderer = Platform.CreateRendererWithContext(target, Forms.Context);
            else
                _renderer.SetElement(target);

            Platform.SetRenderer(target, _renderer);

            var req = target.Measure(fastListView.Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
            return (float)req.Request.Height;
        }

        public void ClearRowsHeight()
        {
            _rowHeights.Clear();
        }

        public void ClearRowHeight(int position)
        {
            _rowHeights[position] = 0;
        }
    }
}