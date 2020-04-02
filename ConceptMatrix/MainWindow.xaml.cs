﻿// Concept Matrix 3.
// Licensed under the MIT license.

namespace ConceptMatrix.GUI
{
	using System;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using ConceptMatrix;
	using ConceptMatrix.GUI.Services;
	using ConceptMatrix.GUI.Views;
	using ConceptMatrix.Services;

	/// <summary>
	/// Interaction logic for MainWindow.xaml.
	/// </summary>
	public partial class MainWindow : Window
	{
		private UserControl currentView;
		private ViewService viewService;

		public MainWindow()
		{
			this.InitializeComponent();

			this.viewService = App.Services.Get<ViewService>();
			this.viewService.ShowingDrawer += this.OnShowDrawer;
			this.viewService.ShowingView += this.OnShowView;

			this.AlwaysOnTopToggle.IsChecked = App.Settings.AlwaysOnTop;

			this.OnShowView("Home", typeof(HomeView));
		}

		private void OnShowView(string path, Type type)
		{
			try
			{
				this.currentView = (UserControl)Activator.CreateInstance(type);
				this.ViewArea.Content = this.currentView;
			}
			catch (TargetInvocationException ex)
			{
				Log.Write(new Exception($"Failed to create view: {type}", ex.InnerException));
			}
			catch (Exception ex)
			{
				Log.Write(new Exception($"Failed to create view: {type}", ex));
			}
		}

		private void OnShowDrawer(string title, Type viewType, DrawerDirection direction)
		{
			UserControl view = null;
			try
			{
				view = (UserControl)Activator.CreateInstance(viewType);
			}
			catch (TargetInvocationException ex)
			{
				Log.Write(new Exception($"Failed to create view: {viewType}", ex.InnerException));
				return;
			}
			catch (Exception ex)
			{
				Log.Write(new Exception($"Failed to create view: {viewType}", ex));
				return;
			}

			switch (direction)
			{
				case DrawerDirection.Left:
				{
					this.DrawerLeft.Content = view;
					this.DrawerHost.IsLeftDrawerOpen = true;
					this.LeftTitle.Content = title;
					break;
				}

				case DrawerDirection.Top:
				{
					this.DrawerTop.Content = view;
					this.DrawerHost.IsTopDrawerOpen = true;
					break;
				}

				case DrawerDirection.Right:
				{
					this.DrawerRight.Content = view;
					this.DrawerHost.IsRightDrawerOpen = true;
					this.RightTitle.Content = title;
					break;
				}

				case DrawerDirection.Bottom:
				{
					this.DrawerBottom.Content = view;
					this.DrawerHost.IsBottomDrawerOpen = true;
					break;
				}
			}
		}

		private void OnTitleBarMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			this.ActiveBorder.Visibility = Visibility.Visible;
			this.InActiveBorder.Visibility = Visibility.Collapsed;
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			this.ActiveBorder.Visibility = Visibility.Collapsed;
			this.InActiveBorder.Visibility = Visibility.Visible;
		}

		private void OnCloseClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void OnMinimiseClick(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void OnThemeClick(object sender, RoutedEventArgs e)
		{
			App.Services.Get<IViewService>().ShowDrawer<ThemeSettingsView>("Theme");
		}

		private void OnAlwaysOnTopChecked(object sender, RoutedEventArgs e)
		{
			this.Topmost = true;
			App.Settings.AlwaysOnTop = true;
			App.Settings.Save();
		}

		private void OnAlwaysOnTopUnchecked(object sender, RoutedEventArgs e)
		{
			this.Topmost = false;
			App.Settings.AlwaysOnTop = false;
			App.Settings.Save();
		}
	}
}