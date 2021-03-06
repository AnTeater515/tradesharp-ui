/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* TradeSharp is a C# based data feed and broker neutral Algorithmic 
* Trading Platform that lets trading firms or individuals automate 
* any rules based trading strategies in stocks, forex and ETFs. 
* TradeSharp allows users to connect to providers like Tradier Brokerage, 
* IQFeed, FXCM, Blackwood, Forexware, Integral, HotSpot, Currenex, 
* Interactive Brokers and more. 
* Key features: Place and Manage Orders, Risk Management, 
* Generate Customized Reports etc 
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using MahApps.Metro.Controls;
using MessageBoxUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NHibernate.Util;
using TradeHub.Common.Core.Constants;
using TradeHub.Common.Core.DomainModels;
using TradeHubGui.Common;
using TradeHubGui.Common.Constants;
using TradeHubGui.Common.Models;
using TradeHubGui.Common.ValueObjects;
using TradeHubGui.Views;
using MarketDataProvider = TradeHubGui.Common.Models.MarketDataProvider;

namespace TradeHubGui.ViewModel
{
    public class MarketScannerWindowViewModel : BaseViewModel
    {
        #region Fields

        private string _newSymbol;
        private MetroWindow _scannerWindow;

        private MarketDataProvider _provider;
        private MarketDataDetail _selectedTickDetail;

        /// <summary>
        /// Data Context for SendOrder Window
        /// </summary>
        private SendOrderViewModel _sendOrderViewModel;

        private ObservableCollection<MarketDataDetail> _tickDetailsCollection;
        private ObservableCollection<MarketDataProvider> _providers;

        private RelayCommand _addNewSymbolCommand;
        private RelayCommand _deleteSymbolCommand;
        private RelayCommand _showLimitOrderBookCommand;
        private RelayCommand _showChartCommand;
        private RelayCommand _sendOrderCommand;
        private RelayCommand _unsubscribeCommand;
        private RelayCommand _alertSettingsCommand;
        private RelayCommand _showPositionStatsCommand;

        #endregion

        #region Constructor

        public MarketScannerWindowViewModel(MetroWindow scannerWindow, MarketDataProvider provider, ObservableCollection<MarketDataProvider> providers)
        {
            _sendOrderViewModel = new SendOrderViewModel();
            _tickDetailsCollection = provider.MarketDetailCollection;

            _scannerWindow = scannerWindow;
            _provider = provider;
            _providers = providers;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Collection of Tick Details for watching related to certain market data provider
        /// </summary>
        public ObservableCollection<MarketDataDetail> TickDetailsCollection
        {
            get { return _tickDetailsCollection; }
            set
            {
                _tickDetailsCollection = value;
                OnPropertyChanged("TickDetailsCollection");
            }
        }

        /// <summary>
        /// Selected Tick Detail
        /// </summary>
        public MarketDataDetail SelectedTickDetail
        {
            get { return _selectedTickDetail; }
            set
            {
                if (_selectedTickDetail != value)
                {
                    _selectedTickDetail = value;
                    OnPropertyChanged("SelectedTickDetail");
                }
            }
        }

        /// <summary>
        /// New symbol for adding to the scanner
        /// </summary>
        public string NewSymbol
        {
            get { return _newSymbol; }
            set
            {
                if (_newSymbol != value)
                {
                    _newSymbol = value.Trim();
                    OnPropertyChanged("NewSymbol");
                }
            }
        }

        /// <summary>
        /// Contains Market Data Provider Details
        /// </summary>
        public MarketDataProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != value)
                {
                    _provider = value;
                    OnPropertyChanged("Provider");
                }
            }
        }

        /// <summary>
        /// Collection of market data providers used for 'Send Order' window
        /// </summary>
        public ObservableCollection<MarketDataProvider> Providers
        {
            get { return _providers; }
            set
            {
                if (_providers != value)
                {
                    _providers = value;
                    OnPropertyChanged("Providers");
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command used for adding new symbol to the scanner
        /// </summary>
        public ICommand AddNewSymbolCommand
        {
            get
            {
                return _addNewSymbolCommand ?? (_addNewSymbolCommand = new RelayCommand(param => AddNewSymbolExecute(), param => AddNewSymbolCanExecute()));
            }
        }

        /// <summary>
        /// Command used for deleting symbol from scanner
        /// </summary>
        public ICommand DeleteSymbolCommand
        {
            get
            {
                return _deleteSymbolCommand ?? (_deleteSymbolCommand = new RelayCommand(param => DeleteSymbolCommandExecute(param)));
            }
        }

        /// <summary>
        /// Command used for showing LOB
        /// </summary>
        public ICommand ShowLimitOrderBookCommand
        {
            get
            {
                return _showLimitOrderBookCommand ?? (_showLimitOrderBookCommand = new RelayCommand(param => ShowLimitOrderBookExecute(param)));
            }
        }

        /// <summary>
        /// Command used for showing chart
        /// </summary>
        public ICommand ShowChartCommand
        {
            get
            {
                return _showChartCommand ?? (_showChartCommand = new RelayCommand(param => ShowChartExecute(param)));
            }
        }

        /// <summary>
        /// Command used for sending order
        /// </summary>
        public ICommand SendOrderCommand
        {
            get
            {
                return _sendOrderCommand ?? (_sendOrderCommand = new RelayCommand(param => SendOrderExecute(param)));
            }
        }

        /// <summary>
        /// Command used for showing position stats
        /// </summary>
        public ICommand ShowPositionStatsCommand
        {
            get
            {
                return _showPositionStatsCommand ?? (_showPositionStatsCommand = new RelayCommand(param => ShowPositionStatsExecute(param)));
            }
        }

        /// <summary>
        /// Command used for setting price alert conditions
        /// </summary>
        public ICommand AlertSettingsCommand
        {
            get
            {
                return _alertSettingsCommand ?? (_alertSettingsCommand = new RelayCommand(param => ShowAlertSettingsExecute(param)));
            }
        }

        #endregion

        #region Trigger Methods for Commands

        private void AddNewSymbolExecute()
        {
            // Check if symbol already exists in TickDetailsMap
            if (_provider.IsSymbolLoaded(NewSymbol))
            {
                // Select existing TickDetail
                SelectedTickDetail = TickDetailsCollection.First(x => x.Security.Symbol == NewSymbol);
            }
            else
            {
                // Create new tick detail's object
                MarketDataDetail tickDetail = new MarketDataDetail(new Security() { Symbol = NewSymbol });

                // Add Tick Detail object to Provider's maps
                _provider.AddMarketDetail(tickDetail);

                // Select new tick detail in DataGrid
                SelectedTickDetail = tickDetail;

                // Create a new subscription request for requesting market data
                var subscriptionRequest = new SubscriptionRequest(tickDetail.Security, _provider, MarketDataType.Tick, SubscriptionType.Subscribe);

                // Raise Event to notify listeners
                EventSystem.Publish<SubscriptionRequest>(subscriptionRequest);
            }

            // Clear NewSymbol string
            NewSymbol = string.Empty;
        }

        /// <summary>
        /// If new symbol is not entered or if market provider is disconneted, return false, otherwise return true
        /// </summary>
        private bool AddNewSymbolCanExecute()
        {
            if (string.IsNullOrEmpty(NewSymbol) || _provider.ConnectionStatus.Equals(ConnectionStatus.Disconnected))
                return false;

            return true;
        }

        /// <summary>
        /// Delete symbol from scanner
        /// </summary>
        private void DeleteSymbolCommandExecute(object param)
        {
            if (WPFMessageBox.Show(_scannerWindow,
                string.Format("Delete symbol {0}?", (param as MarketDataDetail).Security.Symbol),
                string.Format("{0} Scanner", _provider.ProviderName),
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var tickDetail = (MarketDataDetail)param;

                // Remove from Providers collection
                _provider.RemoveMarketInformation(tickDetail.Security.Symbol);

                // Create a new un-subscription request for requesting market data
                var unsubscriptionRequest = new SubscriptionRequest(tickDetail.Security, _provider, MarketDataType.Tick, SubscriptionType.Unsubscribe);

                // Raise Event to notify listeners
                EventSystem.Publish<SubscriptionRequest>(unsubscriptionRequest);

                // Try to find and to close LOB for removed tick detail
                string title = string.Format("LOB - {0} ({1})", tickDetail.Security.Symbol, _provider.ProviderName);
                LimitOrderBookWindow lobWindow = (LimitOrderBookWindow)FindWindowByTitle(title);
                if (lobWindow != null)
                {
                    lobWindow.DataContext = null;
                    lobWindow.Close();
                }
            }
        }

        /// <summary>
        /// Show LOB for current TickDetail
        /// </summary>
        /// <param name="param">current TickDetail</param>
        private void ShowLimitOrderBookExecute(object param)
        {
            SelectedTickDetail = (MarketDataDetail)param;
            string title = string.Format("LOB - {0} ({1})", SelectedTickDetail.Security.Symbol, Provider.ProviderName);
            
            // if LOB window is already shown, just activate it
            LimitOrderBookWindow lobWindow = (LimitOrderBookWindow)FindWindowByTitle(title);
            if (lobWindow != null)
            {
                lobWindow.Activate();
                return;
            }

            // if LOB window is not already shown, create the new one and show it
            lobWindow = new LimitOrderBookWindow();
            lobWindow.Title = title;
            lobWindow.DataContext = new LimitOrderBookViewModel(SelectedTickDetail);
            lobWindow.Show();
        }

        /// <summary>
        /// Show Chart for current TickDetail
        /// </summary>
        /// <param name="param">current TickDetail</param>
        private void ShowChartExecute(object param)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send Order for current TickDetail
        /// </summary>
        /// <param name="param">current TickDetail</param>
        private void SendOrderExecute(object param)
        {
            SelectedTickDetail = (MarketDataDetail)param;

            _sendOrderViewModel.SetOrderExecutionProvider(Provider.ProviderName);
            _sendOrderViewModel.SetSecurityInformation(SelectedTickDetail.Security, SelectedTickDetail.BidPrice, SelectedTickDetail.AskPrice);

            SendOrderWindow orderWindow = new SendOrderWindow();
            orderWindow.DataContext = _sendOrderViewModel;
            orderWindow.Owner = _scannerWindow;

            orderWindow.ShowDialog();
        }

        /// <summary>
        /// Show Position Stats for current TickDetail
        /// </summary>
        /// <param name="param">current TickDetail</param>
        private void ShowPositionStatsExecute(object param)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Displays Price Alert Conditions window
        /// </summary>
        /// <param name="param"></param>
        private void ShowAlertSettingsExecute(object param)
        {
            PriceAlertSettings priceAlertSettingsWindow = new PriceAlertSettings();
            var viewModel = new PriceAlertSettingsViewModel();

            // Set existing conditions
            viewModel.PopulateExistingBidCondition(SelectedTickDetail.GetBidAlertConditions().FirstOrDefault());
            viewModel.PopulateExistingAskCondition(SelectedTickDetail.GetAskAlertConditions().FirstOrDefault());
            viewModel.PopulateExistingTradeCondition(SelectedTickDetail.GetTradeAlertConditions().FirstOrDefault());

            priceAlertSettingsWindow.DataContext = viewModel;

            priceAlertSettingsWindow.Owner = _scannerWindow;
            priceAlertSettingsWindow.ShowDialog();

            // Save new BID Condition
            if (viewModel.BidAlertCondition != null)
                SelectedTickDetail.AddBidAlertConditions(new List<PriceAlertCondition>
                {
                    new PriceAlertCondition(viewModel.BidAlertCondition.ConditionOperator,
                        viewModel.BidAlertCondition.ConditionPrice)
                });
            else
            {
                SelectedTickDetail.AddBidAlertConditions(new List<PriceAlertCondition>());
            }

            // Save new ASK Condition
            if (viewModel.AskAlertCondition != null)
                SelectedTickDetail.AddAskAlertConditions(new List<PriceAlertCondition>
                {
                    new PriceAlertCondition(viewModel.AskAlertCondition.ConditionOperator,
                        viewModel.AskAlertCondition.ConditionPrice)
                });
            else
            {
                SelectedTickDetail.AddAskAlertConditions(new List<PriceAlertCondition>());
            }

            // Save new TRADE Condition
            if (viewModel.TradeAlertCondition != null)
                SelectedTickDetail.AddTradeAlertConditions(new List<PriceAlertCondition>
                {
                    new PriceAlertCondition(viewModel.TradeAlertCondition.ConditionOperator,
                        viewModel.TradeAlertCondition.ConditionPrice)
                });
            else
            {
                SelectedTickDetail.AddTradeAlertConditions(new List<PriceAlertCondition>());
            }
        }

        #endregion

        /// <summary>
        /// Remove all symbols from scanner
        /// </summary>
        public void RemoveAllSymbols()
        {
            foreach (MarketDataDetail marketDataDetail in TickDetailsCollection.ToList())
            {
                // Create a new un-subscription request for requesting market data
                var unsubscriptionRequest = new SubscriptionRequest(marketDataDetail.Security, _provider, MarketDataType.Tick, SubscriptionType.Unsubscribe);

                // Raise Event to notify listeners
                EventSystem.Publish<SubscriptionRequest>(unsubscriptionRequest);

                _provider.RemoveMarketInformation(marketDataDetail.Security.Symbol);
            }
        }
    }
}
