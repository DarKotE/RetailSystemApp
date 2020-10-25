using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using RSADesktopUI.Library.Api;
using RSADesktopUI.Library.Helpers;
using RSADesktopUI.Library.Models;
using RSADesktopUI.Models;

namespace RSADesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;
        private ISaleEndpoint _saleEndpoint;
        private IMapper _mapper;
        private IConfigHelper _configHelper;
        public SalesViewModel(IProductEndpoint productEndpoint,
                              ISaleEndpoint saleEndpoint,
                              IMapper mapper,
                              IConfigHelper configHelper)
        {
            _productEndpoint = productEndpoint;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
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
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel> _products;
        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductDisplayModel _selectedProduct;
        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);

            }
        }

        private CartItemDisplayModel _selectedCartItem;
        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);

            }
        }


        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
        public BindingList<CartItemDisplayModel> Cart
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
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);


            bool haveSameInCart = existingItem != null;
            if (haveSameInCart)
            {
                existingItem.QuantityInCart += ItemQuantity;
            }
            else
            {
                var item = new CartItemDisplayModel {
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
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanRemoveFromCart => SelectedCartItem?.QuantityInCart > 0;


        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock++;
            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart--;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }
        public bool CanCheckOut
        {
            get
            {
                if (Cart.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        
        public async Task CheckOut()
        {
            var sale = new SaleModel();
            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }
            await _saleEndpoint.PostSale(sale);
        }


    }
}
