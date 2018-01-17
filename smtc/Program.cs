using System.Diagnostics;

namespace smtc
{
    internal class Program
    {
        private static SystemMediaTransportControlsWrapper _smtcWrapper = new SystemMediaTransportControlsWrapper();

        private static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener("SMTCDebug.log"));
            Debug.AutoFlush = true;

            _smtcWrapper.StartJsonStdIOMessagePump();
        }
    }
}