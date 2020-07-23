using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using tangerineExtension;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace tangerine
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            webview.NavigationCompleted += Webview_NavigationCompleted;
            webview.NavigationStarting += Webview_NavigationStarting;
            TangerineClient.Instance.RefreshRequested += RefreshRequested;
            TangerineClient.Instance.NotifyKeyEvent += WebUINotifyKeyEventAsync;
            LoadPage();
        }

        private async void LoadPage()
        {
            ReloadPage(await ConfigAgent.GetAndUpdateHomePage("phoenix"));
        }

        public async void CloseApp()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await ApplicationView.GetForCurrentView().TryConsolidateAsync();
            });
        }

        private void RefreshRequested(object sender, string e)
        {
            ShowSplashAnimation();
            webview.Refresh();
        }

        private async void WebUINotifyKeyEventAsync(object sender, string e)
        {
            try
            {
                await Window.Current.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var keyupData = JsonObject.Parse(e);
                    var isAltKeyDown = keyupData["altKey"].GetBoolean();
                    var key = keyupData["key"].GetString();
                    if (isAltKeyDown)
                    {
                        switch (key)
                        {
                            case "d":
                                ReloadPage(await ConfigAgent.GetAndUpdateHomePage("dragon"));
                                Logger.Info("dragon done.");
                                break;
                            case "e":
                                CloseApp();
                                Logger.Info("app closed.");
                                break;
                            case "p":
                                ReloadPage(await ConfigAgent.GetAndUpdateHomePage("phoenix"));
                                Logger.Info("phoenix done.");
                                break;
                            case "r":
                                ReloadPage(await ConfigAgent.GetHomePageUrlAsync());
                                break;
                            case "u":
                                ReloadPage(await ConfigAgent.GetAndUpdateHomePage("unicorn"));
                                break;
                            case "l":
                                ReloadPage(await ConfigAgent.GetAndUpdateHomePage("localhost"));
                                break;
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private void Webview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // 添加 TangerineClient 为 webview 顶级文档
            // 这样就可以在UI工程(webview加载的网页)直接通过 Window.TangerineClient 获取 TangerineClient 类成员
            webview.AddWebAllowedObject("TangerineClient", TangerineClient.Instance);
        }

        private async void Webview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await Task.Delay(1000);
            HideSplashAnimation();
            RegisterKeyPressEvent();
        }

        private async void RegisterKeyPressEvent()
        {
            try
            {
                var script = @"window.addEventListener('keydown', event => {
                                switch (event.keyCode) {
                                    case 68:  // d key
                                    case 69:  // e key
                                    case 76:  // l key
                                    case 80:  // p key
                                    case 82:  // r key
                                    case 85:  // u key
                                        if (event.altKey === true) {
                                            let keyInfo = {
                                                'key' : event.key,
                                                'keyCode' : event.keyCode,
                                                'location' : event.location,
                                                'ctrlKey' : event.ctrlKey,
                                                'altKey' : event.altKey,
                                                'shiftKey' : event.shiftKey
                                            };
                                            window.TangerineClient.sendKeyEventInfo(JSON.stringify(keyInfo));
                                        }
                                        break;
                                    default:
                                        // ignore
                                }
                            })";

                await webview.InvokeScriptAsync("eval", new[] { script });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (string.IsNullOrEmpty(webview.Source?.ToString()))
            {
                ShowSplashAnimation();
            }
            var parmeter = e.Parameter?.ToString();
            if (!string.IsNullOrEmpty(parmeter))
            {
                webview.Source = new Uri(parmeter);
            }
        }

        private void ReloadPage(string url)
        {
            ShowSplashAnimation();
            webview.Navigate(new Uri(url));
        }

        private void ShowSplashAnimation()
        {
            splashAnimation.Visibility = Visibility.Visible;
            webview.Visibility = Visibility.Collapsed;
        }

        private void HideSplashAnimation()
        {
            splashAnimation.Visibility = Visibility.Collapsed;
            webview.Visibility = Visibility.Visible;
        }
    }
}
