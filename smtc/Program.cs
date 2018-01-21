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



#if DEBUG
            new SMTC_API.SMTC_api_example();
#endif

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "chrome-extension://pcgabebhohhgkkahkpklfdfblgilicec/":
                        Debug.WriteLine("[SMTC] Called from Sway.fm Media Controls Chrome Extension.");
                        break;

                    case "chrome-extension://knldjmfmopnpolahpmmgbagdohdnhkik/":
                        Debug.WriteLine("[SMTC] Called from Native Messaging Example Chrome Extension.");
                        break;

                    default:
                        Debug.WriteLine("[SMTC] Called without recognized commandline args.");
                        break;
                }
            }

            try
            {
                _smtcWrapper.StartJsonStdIOMessagePump();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("-----ProcessMessage----");
                Debug.WriteLine(ex);
                Debug.WriteLine("=========");
            }

        }
    }
}