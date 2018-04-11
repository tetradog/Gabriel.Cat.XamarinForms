using AsNum.XFControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Gabriel.Cat.XamarinForms
{
    public class Menu : ContentView
	{
        bool isMenuVisible;
        Layout<View> layoutDefault;
        UniformGrid fields;
        Button btnMenu;
        public event EventHandler<LayoutEventArgs<View>> ToSee;
        public event EventHandler Click;
		public Menu (Layout<View> layoutDefault)
		{
            

            LayoutDefault = layoutDefault;

            btnMenu = new Button();
            btnMenu.Clicked += ClickMenuButton;
            Content = new StackLayout {
				Children = {
					btnMenu
				}
			};
            Fields = new UniformGrid();
            MostrarMenu = false;
            Fields.ColumnDefinitions.Clear();//asi es una lista de un item por fila o deberia
            
		}
        public FileImageSource Image
        {
            get { return btnMenu.Image; }
            set { btnMenu.Image = value; }
        }
        public bool MostrarMenu {
            get => isMenuVisible;
            set {
                isMenuVisible = value;
                //cambio imagen
                PonFondo();
                //muestro o quito menu
                if (ToSee != null)
                {
                    if (isMenuVisible)
                    {
                        ToSee(this, new XamarinForms.LayoutEventArgs<View>(Fields));
                    }
                    else 
                    {
                        ToSee(this, new XamarinForms.LayoutEventArgs<View>(layoutDefault));
                    }
                  
                }
            }
        }

        public UniformGrid Fields { get => fields; set => fields = value==null?new UniformGrid():value; }
        public Layout<View> LayoutDefault {

            get => layoutDefault;
            set {

                if (value==null)
                    throw new NullReferenceException("Se necesita un layout para volver del menú");
                layoutDefault = value;
            }
        }

        private void ClickMenuButton(object sender, EventArgs e)
        {
            if (Click != null)
                Click(this, new EventArgs());
            MostrarMenu=!MostrarMenu;

        }

        public Button AddField(string name,Action action)
        {
            Button btn = new Button();
            btn.Text = name;
            btn.Clicked += (s, e) => action.Invoke();
            Fields.Children.Add(btn);
            return btn;
        }
        public Button AddField(string name, Layout<View> toSee)
        {
            Button btn = new Button();
            btn.Text = name;
            btn.Clicked += (s, e) => { if (ToSee != null) ToSee(this, new LayoutEventArgs<View>(toSee));isMenuVisible = false;PonFondo(); };
            Fields.Children.Add(btn);
            return btn;
        }
        public void RemoveField(Button field)
        {
            Fields.Children.Remove(field);
        }
        private void PonFondo()
        {
            if (isMenuVisible)
            {
                BackgroundColor = Color.LightGray;
            }
            else
            {
                BackgroundColor = Color.Transparent;
            }
        }
    }

}