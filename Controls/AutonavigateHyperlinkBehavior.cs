using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows.Documents;
using System.Diagnostics;

namespace MakePdf.Controls
{
    public class AutonavigateHyperlinkBehavior : Behavior<Hyperlink>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.RequestNavigate += AssociatedObject_RequestNavigate;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RequestNavigate -= AssociatedObject_RequestNavigate;
            base.OnDetaching();
        }

        void AssociatedObject_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var linkContol = sender as Hyperlink;
            if (linkContol == null)
                return;
            var address = linkContol.NavigateUri.ToString();
            if (address.Length == 0)
                return;
            Process.Start(address);
        }

    }
}