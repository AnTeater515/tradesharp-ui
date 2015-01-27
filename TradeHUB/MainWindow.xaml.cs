﻿using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TradeHUB.Views;


namespace TradeHUB
{
	public class Instrument
	{
		public string Symbol { get; set; }
		public float BidQty { get; set; }
		public float BidPrice { get; set; }
		public float AskQty { get; set; }
		public float AskPrice { get; set; }
		public float Last { get; set; }
		public float Volume { get; set; }

		public Instrument(string symbol, float bidQty, float bidPrice, float askQty, float askPrice, float last, float volume)
		{
			Symbol = symbol;
			BidQty = bidQty;
			BidPrice = bidPrice;
			AskQty = askQty;
			AskPrice = askPrice;
			Last = last;
			Volume = volume;
		}
	}


	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			MetroWindow mw = new SettingsWindow();
			mw.ShowTitleBar = true;
			mw.ShowIconOnTitleBar = true;
			mw.ShowInTaskbar = false;
			mw.Owner = this;
			mw.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			mw.ShowDialog();
		}

		private void AboutButton_Click(object sender, RoutedEventArgs e)
		{
			this.ShowMessageAsync("About", "Something will be displayed here!", MessageDialogStyle.Affirmative);
		}
	}
}