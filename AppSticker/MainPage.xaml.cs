using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace AppSticker
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public MainPage()
        {
            Current = this;
            this.InitializeComponent();

            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            //SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            //{
            //    // Handle the Back pressed  
            //};
        }

        public void CallBack()
        {
            Game_Click(null, null);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await LoadBgService();
        }

        public async Task LoadBgService()
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                App.AppServiceConnected += MainPage_AppServiceConnected;
                App.AppServiceDisconnected += MainPage_AppServiceDisconnected;
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        private async void MainPage_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            App.Connection.RequestReceived += AppServiceConnection_RequestReceived;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // enable UI to access  the connection
                //btnRegKey.IsEnabled = true;
            });
        }

        private async void MainPage_AppServiceDisconnected(object sender, EventArgs e)
        {
            //await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // disable UI to access the connection

                // ask user if they want to reconnect
                Reconnect();
            });
        }

        /// <summary>
        /// Ask user if they want to reconnect to the desktop process
        /// </summary>
        private async void Reconnect()
        {
            MessageDialog dlg = new MessageDialog("Sticker service lost. Reconnect?");
            UICommand yesCommand = new UICommand("Yes", async (r) =>
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            });
            dlg.Commands.Add(yesCommand);
            UICommand noCommand = new UICommand("No", (r) => { });
            dlg.Commands.Add(noCommand);
            await dlg.ShowAsync();
        }

        /// <summary>
        /// Handle calculation request from desktop process
        /// (dummy scenario to show that connection is bi-directional)
        /// </summary>
        private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            //double d1 = (double)args.Request.Message["D1"];
            //double d2 = (double)args.Request.Message["D2"];
            double result = 2;

            ValueSet response = new ValueSet();
            response.Add("RESULT", result);
            await args.Request.SendResponseAsync(response);

            // log the request in the UI for demo purposes
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //tbRequests.Text += string.Format("Request: {0} + {1} --> Response = {2}\r\n", d1, d2, result);
            });
        }


        //public void RegisterConnection()
        //{
        //    if (App.Connection != null)
        //    {
        //        App.Connection.RequestReceived += Connection_RequestReceived;
        //    }
        //}

        //private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        //{
        //    var deferral = args.GetDeferral();

        //    ValueSet message = args.Request.Message;
        //    ValueSet returnData = new ValueSet();
        //    returnData.Add("response", "success");

        //    // get the verb or "command" for this request
        //    string verb = message["verb"] as String;

        //    switch (verb)
        //    {

        //    }

        //    try
        //    {
        //        // Return the data to the caller.
        //        await args.Request.SendResponseAsync(returnData);
        //    }
        //    catch (Exception e)
        //    {
        //        // Your exception handling code here.
        //    }
        //    finally
        //    {
        //        // Complete the deferral so that the platform knows that we're done responding to the app service call.
        //        // Note for error handling: this must be called even if SendResponseAsync() throws an exception.
        //        deferral.Complete();
        //    }
        //}


        private void Game_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.Navigate(typeof(Game));
        }

        private void Work_Click(object sender, RoutedEventArgs e)
        {
            MyFrame.Navigate(typeof(Work));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
        }

    }
}
