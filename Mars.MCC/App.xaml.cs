using System;
using System.Windows;
using Mars.MCC.InternalServices;
using Mars.MCC.InternalServices.Impl;

namespace Mars.MCC
{
    public partial class App
    {
        public static IRetranslatorClient RetranslatorClient { get; } = new RetranslatorClient(new HttpClientFactory(Settings.Default.RetranslatorUrl));

        public static void ShowError(Exception error)
        {
            var e = error.GetBaseException();
            MessageBox.Show(e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
        }
    }
}
