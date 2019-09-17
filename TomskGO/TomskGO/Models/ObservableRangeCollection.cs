using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace TomskGO.Models
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        private readonly object _lockObj = new object();

        protected enum ProcessRangeAction
        {
            Add,
            Replace,
            Remove
        };

        public ObservableRangeCollection() : base() { }

        public ObservableRangeCollection(IEnumerable<T> collection) : base(collection) { }

        public ObservableRangeCollection(List<T> list) : base(list) { }

        protected virtual void ProcessRange(IEnumerable<T> collection, ProcessRangeAction action)
        {
            lock (_lockObj)
            {
                try
                {
                    if (collection == null) throw new ArgumentNullException("collection");

                    var items = collection as IList<T> ?? collection.ToList();
                    if (!items.Any()) return;

                    CheckReentrancy();

                    if (action == ProcessRangeAction.Replace) Items.Clear();

                    foreach (var item in items)
                    {
                        if (action == ProcessRangeAction.Remove) Items.Remove(item);
                        else Items.Add(item);
                    }

                    OnPropertyChanged(new PropertyChangedEventArgs(CountString));
                    OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
                catch { }
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            ProcessRange(collection, ProcessRangeAction.Add);
        }

        public void ReplaceRange(IEnumerable<T> collection)
        {
            ProcessRange(collection, ProcessRangeAction.Replace);
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            ProcessRange(collection, ProcessRangeAction.Remove);
        }
    }
}