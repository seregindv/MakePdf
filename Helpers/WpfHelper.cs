using Fonet;
using Fonet.Render.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MakePdf.Helpers
{
    public static class WpfHelper
    {

        public static bool IsTablet
        {
            get
            {
                return Tablet.TabletDevices.Count > 0;
            }
        }

        public static void DisableWPFTabletSupport()
        {
            // Get a collection of the tablet devices for this window.  
            var devices = Tablet.TabletDevices;
            if (devices.Count > 0)
            {
                // Get the Type of InputManager.
                var inputManagerType = typeof(InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.
                var stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.
                    var stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.
                    while (devices.Count > 0)
                        // Remove the first tablet device in the devices collection.
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, stylusLogic, new object[] { (uint)0 });
                }
            }
        }

        public static void EnableFocusTracking()
        {
            var cp = new InputPanelConfiguration();
            var icp = cp as IInputPanelConfiguration;
            if (icp != null)
                icp.EnableFocusTracking();
        }

        public static FonetDriver GetFonetDriver()
        {
            return new FonetDriver { Options = new PdfRendererOptions { FontType = FontType.Subset } };
        }
    }
}
