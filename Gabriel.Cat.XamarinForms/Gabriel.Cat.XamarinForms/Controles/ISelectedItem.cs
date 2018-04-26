using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.XamarinForms
{
    public interface ISelectedItem
    {
        event EventHandler<SelectedItemEventArgs> SelectedItem;
        event EventHandler<SelectedItemEventArgs> UnSelectedItem;
        object Item { get; set; }
        bool IsSelected { get; set; }
    }
    public class SelectedItemEventArgs : EventArgs
    {
        object item;

        public SelectedItemEventArgs(object item)
        {
            Item = item;
        }

        public object Item { get => item; private set => item = value; }
    }
}
