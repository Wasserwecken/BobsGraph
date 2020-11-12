using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace BobsGraphPlugin
{
    public static class OverlayWindowHelper
    {
        public const string OVERLAY_CLICKABLEELEMENTS = "_clickableElements";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool TryMarkAsClickable(FrameworkElement element)
        {
            var overlayType = Core.OverlayWindow.GetType();

            var clickableElementsField = overlayType.GetField(OVERLAY_CLICKABLEELEMENTS, BindingFlags.NonPublic | BindingFlags.Instance);
            if (clickableElementsField == null)
            {
                Log.Warn($"Cannot get the clickable elements field information of the overlay window");
                return false;
            }
            var clickableElements = clickableElementsField.GetValue(Core.OverlayWindow) as IList<FrameworkElement>;
            if (clickableElements == null)
            {
                Log.Warn($"Cannot get the value of clickable elements field of the overlay window");
                return true;
            }

            clickableElements.Add(element);
            return true;
        }
    }
}
