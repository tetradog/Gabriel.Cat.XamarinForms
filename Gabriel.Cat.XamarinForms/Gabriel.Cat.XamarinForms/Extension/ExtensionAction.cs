using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gabriel.Cat.S.Extension
{
   public static class ExtensionAction
    {
        public static void BeginInvoke(this Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }
    }
}
