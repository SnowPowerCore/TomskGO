﻿using System.ComponentModel;

namespace TomskGO.Models
{
    class Tag : INotifyPropertyChanged
    {
        private bool _selected = false;
        public string Name { get; set; }

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        #region Auto-implemented
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
