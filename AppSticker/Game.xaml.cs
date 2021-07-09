using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace AppSticker
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Game : Page
    {
        public ObservableCollection<RunProcess> runProcessInfos = new ObservableCollection<RunProcess>();

        public Game()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            GetRunProcesses();
        }

        private async void GetRunProcesses()
        {
            ValueSet valueSet = new ValueSet();

            valueSet.Clear();
            valueSet.Add("verb", "getRunProcesses");

            try
            {
                AppServiceResponse response = await App.Connection.SendMessageAsync(valueSet);

                if (response.Status == AppServiceResponseStatus.Success)
                {
                    if (response.Message["verb"] as string == "success")
                    {
                        // Update UI-bound collections and controls on the UI thread
                        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            var result = response.Message["result"] as string;

                            if (!String.IsNullOrEmpty(result))
                            {
                                runProcessInfos.Clear();

                                List<RunProcess> processes = JsonConvert.DeserializeObject<List<RunProcess>>(result);

                                foreach (var p in processes)
                                {
                                    runProcessInfos.Add(p);
                                }
                            }
                        });
                    }
                }

                //OpenCmdSwitch.Toggled += ToggleSwitch_Toggled;
            }
            catch (Exception ex)
            {

            }
        }

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            RunProcess runProcess = toggleSwitch.DataContext as RunProcess;
            {
                ValueSet valueSet = new ValueSet();

                valueSet.Clear();
                valueSet.Add("verb", "setForegroundWindow");
                valueSet.Add("hWnd", runProcess.HWnd);
                valueSet.Add("on", toggleSwitch.IsOn.ToString());

                try
                {
                    AppServiceResponse response = await App.Connection.SendMessageAsync(valueSet);

                    if (response.Status == AppServiceResponseStatus.Success)
                    {
                        if (response.Message["verb"] as string == "success")
                        {
                            // Update UI-bound collections and controls on the UI thread
                        }
                    }

                    //OpenCmdSwitch.Toggled += ToggleSwitch_Toggled;
                }
                catch (Exception ex)
                {

                }
            }

        }
    }
}
