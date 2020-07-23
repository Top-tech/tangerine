using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace tangerineExtension
{
    [AllowForWeb]
    public sealed class TangerineClient
    {
        private TangerineClient()
        {
        }
        public event EventHandler<string> NotifyKeyEvent;
        public event EventHandler<string> RefreshRequested;
        public static TangerineClient Instance { get; } = new TangerineClient();

        public void SendKeyEventInfo(string keyEventInfo)
        {
            Task.Run(async () =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    NotifyKeyEvent?.Invoke(this, keyEventInfo);
                });
            });
        }

        // =======================================================
        // ============== 下面部分用来与 UI 工程交互 ===============
        // =======================================================

        public void Exit()
        {
            string keyInfo = "{\"key\":\"e\",\"keyCode\":69,\"location\":0,\"ctrlKey\":false,\"altKey\":true,\"shiftKey\":false}";
            SendKeyEventInfo(keyInfo);
            //直接关闭
            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            //{
            //    await ApplicationView.GetForCurrentView().TryConsolidateAsync();
            //});
        }
    }
}
