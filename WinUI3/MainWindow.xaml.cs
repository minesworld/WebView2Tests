using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            adressTextBox.Text = "https://www.microsoft.com";

            Debug.WriteLine(DateTime.Now.ToString() + " WinUI3 MainWindow");
        }

        private void OnClickNoDTEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnClickNoDTEvent ******");
                ((Button)sender).IsEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnClickNoDTEvent catch: " + ex.Message);
            }
        }

        private string js = "setInterval(function() { window.chrome.webview.postMessage('{}'); }, 10000);";

        private async void OnClickStart(object sender, RoutedEventArgs e)
        {
            try
            {
                var testAwait = testAwaitCheckBox.IsChecked == true;
                var testWebMessage = testWebMessageCheckBox.IsChecked == true;
                var testDTOverlay = testDToverlayCheckBox.IsChecked == true;
                var testDTevents = testDTeventsCheckBox.IsChecked == true;
                var testNavigation = testNavigationCheckBox.IsChecked == true;

                var optionsMsg = ((testAwait) ? " Await" : "") + ((testWebMessage) ? " WebMessage" : "") + ((testDTOverlay) ? " DT-Overlay" : "") + ((testDTevents) ? " DT-Events" : "") + ((testNavigation) ? " Navigation" : "");
                Debug.WriteLine(DateTime.Now.ToString() + " OnClickStart:" + optionsMsg + " @ " + adressTextBox.Text);

                if (testAwait == false)
                {
                    await webView.EnsureCoreWebView2Async();
                    webView.CoreWebView2.ProcessFailed += OnWebViewProcessFailed;

                    if (testWebMessage)
                    {
                        webView.WebMessageReceived += OnWebMessageReceived;
                        webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                        await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(js);
                    }

                    if (testDTevents)
                    {
                        webView.CoreWebView2.GetDevToolsProtocolEventReceiver("Overlay.nodeHighlightRequested").DevToolsProtocolEventReceived += OnOverlayNodeHighlightRequested;
                        webView.CoreWebView2.GetDevToolsProtocolEventReceiver("Overlay.inspectNodeRequested").DevToolsProtocolEventReceived += OnInspectNodeRequested;
                    }

                    if (testDTOverlay)
                    {
                        await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("DOM.enable", "{}");
                        await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Overlay.enable", "{}");
                        await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Overlay.setInspectMode", "{\"mode\":\"searchForNode\",\"highlightConfig\":{\"showInfo\":true,\"contentColor\":{\"r\": 155, \"g\": 11, \"b\": 239, \"a\": 0.7}}}");
                    }

                    if (testNavigation)
                    {
                        webView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
                    }
                }
                else
                {
                    await Start(testWebMessage, testDTOverlay, testDTevents, testNavigation);
                }

                webView.Source = new Uri(adressTextBox.Text);

                commandPanel.IsHitTestVisible = false;
                adressTextBox.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnClickStart catch: " + ex.Message);
            }
        }

        private async Task Start(bool testWebMessage, bool testDTOverlay, bool testDTevents, bool testNavigation)
        {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.ProcessFailed += OnWebViewProcessFailed;

            if (testWebMessage)
            {
                webView.WebMessageReceived += OnWebMessageReceived;
                webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(js);
            }

            if (testDTevents)
            {
                webView.CoreWebView2.GetDevToolsProtocolEventReceiver("Overlay.nodeHighlightRequested").DevToolsProtocolEventReceived += OnOverlayNodeHighlightRequested;
                webView.CoreWebView2.GetDevToolsProtocolEventReceiver("Overlay.inspectNodeRequested").DevToolsProtocolEventReceived += OnInspectNodeRequested;
            }

            if (testDTOverlay)
            {
                await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("DOM.enable", "{}");
                await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Overlay.enable", "{}");
                await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Overlay.setInspectMode", "{\"mode\":\"searchForNode\",\"highlightConfig\":{\"showInfo\":true,\"contentColor\":{\"r\": 155, \"g\": 11, \"b\": 239, \"a\": 0.7}}}");
            }

            if (testNavigation)
            {
                webView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
            }
        }

        private void OnWebViewProcessFailed(CoreWebView2 sender, CoreWebView2ProcessFailedEventArgs args)
        {
            try
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnWebViewProcessFailed " + args.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnWebViewProcessFailed catch: " + ex.Message);
            }
        }

        private void OnWebMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            try
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnWebMessageReceived " + args.WebMessageAsJson);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnWebMessageReceived catch: " + ex.Message);
            }
        }

        private void OnOverlayNodeHighlightRequested(CoreWebView2 sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs args)
        {
            try
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnOverlayNodeHighlightRequested " + args.ParameterObjectAsJson);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnOverlayNodeHighlightRequested catch: " + ex.Message);
            }
        }

        private void OnInspectNodeRequested(CoreWebView2 sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs args)
        {
            try
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnInspectNodeRequested " + args.ParameterObjectAsJson);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnInspectNodeRequested catch: " + ex.Message);
            }
        }

        private void OnNavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            try
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnNavigationCompleted " + args.WebErrorStatus.ToString() + " @ " + webView.Source);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(DateTime.Now.ToString() + " OnNavigationCompleted catch: " + ex.Message);
            }
        }
    }
}
