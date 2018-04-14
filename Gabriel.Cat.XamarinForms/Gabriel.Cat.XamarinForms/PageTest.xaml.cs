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
            RadioButton rb1, rb2;
			InitializeComponent();
            rb1 = new RadioButton();
            rb2 = new RadioButton();
            rb1.Content = "RadioButton1";
            rb2.Content = "RadioButton2";
            rb.Content = "RadioButton0";

        }
	}
}