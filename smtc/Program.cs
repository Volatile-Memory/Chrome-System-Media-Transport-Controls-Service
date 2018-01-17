using System;
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

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "chrome-extension://pcgabebhohhgkkahkpklfdfblgilicec/":
                        Debug.WriteLine("[SMTC] Called from Sway.fm Media Controls Chrome Extension.");
                        break;
                    default:
                        Debug.WriteLine("[SMTC] Called without recognized commandline args.");
                        break;
                }
            }
            
#if DEBUG
            new SMTC_API.SMTC_api_example();
#endif
            try
            { 
                _smtcWrapper.StartJsonStdIOMessagePump();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("---------");
                Debug.WriteLine(ex);
                Debug.WriteLine("=========");
                //Debugger.Launch();
            }
        }
    }
}