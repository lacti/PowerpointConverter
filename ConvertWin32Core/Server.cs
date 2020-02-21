using System;
using System.IO;
using EmbedIO;
using EmbedIO.WebApi;
using Swan.Logging;

namespace ConvertWin32Core
{
    public class Server : IDisposable
    {
        private const string DefaultUrl = "http://+:19292";

        static Server()
        {
            LoggingUtils.InitLogger();
        }

        public string Url { get; set; } = DefaultUrl;
        public event Action<ServerState> OnStateChanged;

        public ServerState State { get; private set; }
        private WebServer Server_ { get; set; }

        public Server()
        {
            ConversionWorker.Instance.OnTaskStart += Instance_OnTaskStart;
            ConversionWorker.Instance.OnTaskEnd += Instance_OnTaskEnd;
        }

        public void Start()
        {
            if (Server_ != null)
            {
                Stop();
            }
            FireNewServerState(ServerState.Running);
            Server_ = CreateWebServer();
            Server_.RunAsync();
            ConversionWorker.Instance.Start();
        }

        public void Stop()
        {
            if (Server_ == null)
            {
                return;
            }
            FireNewServerState(ServerState.Stopped);
            ConversionWorker.Instance.Stop();
            Server_.Dispose();
            Server_ = null;
        }

        public void Dispose()
        {
            Stop();
        }

        public void OpenBrowser()
        {
            var browser = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(Url.Replace("+", "localhost")) { UseShellExecute = true }
            };
            browser.Start();
        }

        private WebServer CreateWebServer()
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix(Url)
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithWebApi("/", m => m
                    .WithController<ConverterController>());

            server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();
            return server;
        }

        private void Instance_OnTaskEnd()
        {
            FireNewServerState(Server_ != null ? ServerState.Running : ServerState.Stopped);
        }

        private void Instance_OnTaskStart()
        {
            FireNewServerState(ServerState.Working);
        }

        private void FireNewServerState(ServerState newState)
        {
            State = newState;
            OnStateChanged.Invoke(newState);
        }
    }
}
