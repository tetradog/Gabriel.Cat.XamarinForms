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
            for (int i = 0; i < 10; i++)
                stkMain.Children.Add(new RadioButton() { Text="RadioButton"+i});
        }
	}
}