using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gabriel.Cat.XamarinForms.Controles;
namespace Gabriel.Cat.XamarinForms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageTest : ContentPage
	{
		public PageTest ()
		{
			InitializeComponent();
            opMain.AddType(typeof(string), typeof(RadioButton), false,RadioButton.TextProperty);
            for (int i = 0; i < 100; i++)
                opMain.Add("RadioButton"+i);
            opMain.Refresh();
        }
	}
}