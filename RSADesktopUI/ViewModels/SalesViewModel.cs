using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RSADesktopUI.Library.Api;
using RSADesktopUI.Library.Helpers;
using RSADesktopUI.Library.Models;

namespace RSADesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;
        private IConfigHelper _configHelper;
        public SalesViewModel(IProductEndpoint productEndpoint,
                              IConfigHelper configHelper)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;


        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }

        private BindingList<ProductModel> _products;
        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);

            }
        }


        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();
        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public string SubTotal
        {
            get
            {
                return CalculateSubTotal().ToString("C");
            }
        }

        private decimal CalculateSubTotal()
        {
            return Cart
                    .Sum(x => x.Product.RetailPrice * x.QuantityInCart);
        }

        public string Tax
        {
            get
            {
                return CalculateTax().ToString("C");
            }
        }

        private decimal CalculateTax()
        {
            decimal taxRate = _configHelper.GetTaxRate() / 100;
            return Cart
                    .Where(x => x.Product.IsTaxable)
                    .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                return total.ToString("C");
            }
        }


        private int _itemQuantity = 1;
        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public bool CanAddToCart =>
            //condition
            ItemQuantity > 0
            && SelectedProduct?.QuantityInStock >= ItemQuantity;

        public void AddToCart()
        {
            //try to findout if item already is in the cart
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);


            bool haveSameInCart = existingItem != null;
            if (haveSameInCart)
            {
                existingItem.QuantityInCart += ItemQuantity;
                //TODO replace this
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                var item = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => Cart);
        }

        public bool CanRemoveFromCart => true;
            //UserName?.Length > 0 && Password?.Length > 0;
        public void RemoveFromCart()
        {
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
        }
        public bool CanCheckOut => true;
    //UserName?.Length > 0 && Password?.Length > 0;
        public void CheckOut()
        {

        }


    }
}
