﻿using System.ComponentModel;

namespace RSA.DesktopUI.Models
{
    public class ProductDisplayModel : INotifyPropertyChanged
    {
        private int _quantityInStock;
#nullable disable //Doesn't work with Automapper 
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }
        public int QuantityInStock
        {
            get
            {
                return _quantityInStock;
            }
            set
            {
                _quantityInStock = value;
                CallPropertyChanged(nameof(QuantityInStock));
            }
        }
        public bool IsTaxable { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
