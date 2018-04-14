
using System.Windows;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF.Extensions;
namespace XamarinForms.WpfTest
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            UIElement xfTest;
            InitializeComponent();
            Forms.Init();
            xfTest = new Gabriel.Cat.XamarinForms.PageTest().ToFrameworkElement();
            gMain.Children.Add(xfTest);
        }
    }
}
