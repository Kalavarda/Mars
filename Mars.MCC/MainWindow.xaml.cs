using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.RuntimeBroker.Models;

namespace Mars.MCC
{
    public partial class MainWindow
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public Instance SelectedInstance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var cancellationTokenSource = new CancellationTokenSource(Timeout);

            var client = App.RetranslatorClient;
            try
            {
                Cursor = Cursors.Wait;
                var getInstancesResult = await client.GetInstancesAsync(cancellationTokenSource.Token);
                if (getInstancesResult.Error != null)
                    throw new Exception(getInstancesResult.Error.Message);
                foreach (var instance in getInstancesResult.Result)
                {
                    var menuItem = new MenuItem
                    {
                        Header = instance.MachineName,
                        IsCheckable = true,
                        Tag = instance
                    };
                    menuItem.Click += MenuItem_Instance_Click;
                    _miInstance.Items.Add(menuItem);
                }
            }
            catch (Exception error)
            {
                App.ShowError(error);
            }
            finally
            {
                Cursor = null;
            }
        }

        private void MenuItem_Instance_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            SelectedInstance = (Instance)menuItem.Tag;
            menuItem.IsChecked = true;
        }
    }
}
