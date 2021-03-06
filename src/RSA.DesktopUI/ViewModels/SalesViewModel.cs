﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using RSA.DesktopUI.Library.Api;
using RSA.DesktopUI.Library.Models;
using RSA.DesktopUI.Models;

namespace RSA.DesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private readonly IProductEndpoint _productEndpoint;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public SalesViewModel(IProductEndpoint productEndpoint,
                              ISaleEndpoint saleEndpoint,
                              IMapper mapper,
                              IConfiguration config,
                              StatusInfoViewModel status,
                              IWindowManager window)
        {
            _productEndpoint = productEndpoint;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _config = config;
            _status = status;
            _window = window;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You have no permission to access Sales Form");
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                await TryCloseAsync();
            }
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel>? _products;
        public BindingList<ProductDisplayModel>? Products
        {
            get => _products;
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }
        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            await LoadProducts();
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        private ProductDisplayModel? _selectedProduct;
        public ProductDisplayModel? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private CartItemDisplayModel? _selectedCartItem;
        public CartItemDisplayModel? SelectedCartItem
        {
            get => _selectedCartItem;
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
            get => _cart;
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        } 

        public string SubTotal => CalculateSubTotal().ToString("C");

        private decimal CalculateSubTotal()
        {
            return Cart
                    .Sum(x => x.Product.RetailPrice * x.QuantityInCart);
        }

        public string Tax => CalculateTax().ToString("C");

        private decimal CalculateTax()
        {
            decimal taxRate = _config.GetValue<decimal>("taxRate");
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
            get => _itemQuantity;
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
            //try to find out if item already is in the cart
            CartItemDisplayModel? existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if (existingItem is null)
            {
                var item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);

            }
            else
            {
                existingItem.QuantityInCart += ItemQuantity;
                
            }
            if (SelectedProduct!=null)
            {
                SelectedProduct.QuantityInStock -= ItemQuantity;
                ItemQuantity = 1;
                NotifyOfPropertyChange(() => SubTotal);
                NotifyOfPropertyChange(() => Tax);
                NotifyOfPropertyChange(() => Total);
                NotifyOfPropertyChange(() => CanCheckOut);
            }
            else
            {
                // TODO log it?
            }
        }

        public bool CanRemoveFromCart => SelectedCartItem?.QuantityInCart > 0;

        public void RemoveFromCart()
        {
            if (SelectedCartItem != null)
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
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }
        public bool CanCheckOut => Cart.Count > 0;

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

            await ResetSalesViewModel();
        }
    }
}
