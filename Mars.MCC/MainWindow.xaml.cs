using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mars.MCC.InternalServices;
using Microsoft.RuntimeBroker.Models;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Mars.MCC
{
    public partial class MainWindow
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public Instance SelectedInstance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            foreach (var type in typeof(CommandBase).Assembly.GetExportedTypes().Where(t => !t.IsAbstract).Where(t => t.IsAssignableTo(typeof(CommandBase))))
            {
                var menuItem = new MenuItem
                {
                    Header = type.Name,
                    Tag = type
                };
                menuItem.Click += MenuItem_CreateCommand_Click;
                _miCommandCreate.Items.Add(menuItem);
            }

            Loaded += MainWindow_Loaded;
        }

        private async void MenuItem_CreateCommand_Click(object sender, RoutedEventArgs e)
        {
            var type = (Type)((FrameworkElement)sender).Tag;
            var command = (CommandBase)Activator.CreateInstance(type);

            if (command is GetSystemInfoCommand getSystemInfoCommand)
                getSystemInfoCommand.CommandParameters = new SystemInfoCommandParameters{ MachineName = true };

            var cancellationTokenSource = new CancellationTokenSource(Timeout);

            var client = App.RetranslatorClient;
            try
            {
                Cursor = Cursors.Wait;

                var addResult = await client.AddCommandsAsync(SelectedInstance.Id, command, cancellationTokenSource.Token);
                if (addResult.Error != null)
                    throw new Exception(addResult.Error.Message);

                await ReloadCommandsAsync(client, cancellationTokenSource);
            }
            catch (Exception error)
            {
                _dataGrid.ItemsSource = null;
                App.ShowError(error);
            }
            finally
            {
                Cursor = null;
            }
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

        private async void MenuItem_Instance_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            SelectedInstance = (Instance)menuItem.Tag;
            menuItem.IsChecked = true;

            await ReloadCommandsAsync();
        }

        private async Task ReloadCommandsAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource(Timeout);

            var client = App.RetranslatorClient;
            try
            {
                Cursor = Cursors.Wait;
                await ReloadCommandsAsync(client, cancellationTokenSource);
            }
            catch (Exception error)
            {
                _dataGrid.ItemsSource = null;
                App.ShowError(error);
            }
            finally
            {
                Cursor = null;
            }
        }

        private async Task ReloadCommandsAsync(IRetranslatorClient client, CancellationTokenSource cancellationTokenSource)
        {
            var getCommandsResult = await client.GetCommandsAsync(SelectedInstance.Id, DateTimeOffset.Now.AddDays(-7),
                DateTimeOffset.Now, cancellationTokenSource.Token);
            if (getCommandsResult.Error != null)
                throw new Exception(getCommandsResult.Error.Message);

            _dataGrid.ItemsSource = getCommandsResult.Result.OrderByDescending(r => r.Id);
        }
    }
}
