using System;

using Xamarin.Forms;

namespace Gabriel.Cat.XamarinForms
{
    public class LayoutEventArgs<T>:EventArgs where T:View
    {
        Layout<T> layout;
        public LayoutEventArgs(Layout<T> layout)
        {
            Layout = layout;
        }
        public Layout<T> Layout { get => layout; private set => layout = value; }
    }
}