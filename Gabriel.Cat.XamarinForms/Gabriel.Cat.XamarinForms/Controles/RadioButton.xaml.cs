using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gabriel.Cat.S.Utilitats;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gabriel.Cat.XamarinForms.Controles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RadioButton : ContentView, IClauUnicaPerObjecte
    {
        public enum Selection
        {
            Singel, Multiple
        }

        public const string DEFAULTGROUP = "default";
        const string DEFAULTCONTENT = "";

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create
         (nameof(IsSelected), typeof(bool), typeof(RadioButton),defaultValue:false,propertyChanged:IsSelectedChangeProperty);
        public static readonly BindableProperty ContentProperty = BindableProperty.Create
         (nameof(Content), typeof(object), typeof(RadioButton), defaultValue: DEFAULTCONTENT, propertyChanged: ContentChangeProperty);
        public static readonly BindableProperty IdGroupProperty = BindableProperty.Create
        (nameof(IdGroup), typeof(string), typeof(RadioButton), defaultValue: DEFAULTGROUP, propertyChanged: IdGroupChangeProperty);
        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create
        (nameof(SelectedColor), typeof(Color), typeof(RadioButton), defaultValue: Color.SkyBlue, propertyChanged: SelectedColorChangeProperty);
        public static readonly BindableProperty UnselectedColorProperty = BindableProperty.Create
        (nameof(UnselectedColor), typeof(Color), typeof(RadioButton), defaultValue: Color.White, propertyChanged: UnselectedColorChangeProperty);

        static LlistaOrdenada<string, LlistaOrdenada<int, RadioButton>> selectionDic;
        static LlistaOrdenada<string, Selection> selectionModeDic;
        static GenIdInt genId;


        int id;
        public event EventHandler Selected;

        static RadioButton()
        {
            selectionDic = new LlistaOrdenada<string, LlistaOrdenada<int, RadioButton>>();
            selectionModeDic = new LlistaOrdenada<string, Selection>();

            selectionDic.Add(DEFAULTGROUP, new LlistaOrdenada<int, RadioButton>());
            selectionModeDic.Add(DEFAULTGROUP, Selection.Singel);
            genId = new GenIdInt();

        }
        public RadioButton()
        {
            id = genId.Siguiente();
            InitializeComponent();
            btnOnOff.BackgroundColor = UnselectedColor;


        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set
            {

                RadioButton aux;
                if (value)
                {
                    if (Selected != null)
                        Selected(this, new EventArgs());
                    if (SelectionMode == Selection.Singel && selectionDic[IdGroup].Count == 1)
                    {
                        aux = selectionDic[IdGroup].GetValueAt(0);
                        aux.SetValue(IsSelectedProperty, false);
                        aux.SelectionChanged();
                        selectionDic[IdGroup].Remove(aux.id);
                    }
                    selectionDic[IdGroup].Add(id, this);
                }
                else if (selectionDic[IdGroup].ContainsKey(id))
                {
                    if (selectionDic[IdGroup].Count > 1)
                        selectionDic[IdGroup].Remove(id);
                }

                SetValue(IsSelectedProperty, selectionDic[IdGroup].ContainsKey(id));
                SelectionChanged();
            }
        }
        public new object Content
        {
            get => GetValue(ContentProperty);
            set
            {
                SetValue(ContentProperty, value != null ? value : DEFAULTCONTENT);
                lblRbtn.Text = ToString();
            }
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set
            {
                SetValue(SelectedColorProperty, value);
                if (!IsSelected)
                    btnOnOff.BackgroundColor = SelectedColor;
            }
        }
        public Color UnselectedColor
        {
            get => (Color)GetValue(UnselectedColorProperty);
            set
            {
                SetValue(UnselectedColorProperty, value);
                if (!IsSelected)
                    btnOnOff.BackgroundColor = UnselectedColor;
            }
        }
        public string IdGroup
        {
            get => (string)GetValue(IdGroupProperty);
            set
            {


                if (value == null)
                    value = DEFAULTGROUP;

                if (!selectionDic.ContainsKey(value))
                    selectionDic.Add(value, new LlistaOrdenada<int, RadioButton>());

                if (selectionDic[IdGroup].ContainsKey(id))
                    selectionDic[IdGroup].Remove(id);
                SetValue(IdGroupProperty, value);
                if (!selectionModeDic.ContainsKey(IdGroup))
                {
                    selectionModeDic.Add(IdGroup, default(Selection));
                }

            }
        }

        public Selection SelectionMode
        {
            get => selectionModeDic[IdGroup];
            set
            {
                bool isSelected;
                selectionModeDic[IdGroup] = value;
                if (value == Selection.Singel)
                {
                    isSelected = IsSelected;
                    for (int i = 0; i < selectionDic[IdGroup].Count; i++)
                        selectionDic[IdGroup].GetValueAt(i).IsSelected = false;
                    SetValue(IsSelectedProperty, isSelected);
                    SelectionChanged();

                }
            }
        }

        IComparable IClauUnicaPerObjecte.Clau => id;

        private void SelectionChanged()
        {
            if (btnOnOff != null)
            {
                if (IsSelected)
                {
                    btnOnOff.BackgroundColor = SelectedColor;
                }
                else
                {
                    btnOnOff.BackgroundColor = UnselectedColor;
                }
            }
        }
        private void SeleccionClick(object sender, EventArgs e)
        {
            IsSelected = !IsSelected;
        }

        public override string ToString()
        {
            return Content.ToString();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as RadioButton);
        }
        public bool Equals(RadioButton obj)
        {
            bool equals = obj != null;
            if (equals)
                equals = IdGroup.Equals(obj.IdGroup) && Content.Equals(obj.Content);
            return equals;
        }
        private static void IsSelectedChangeProperty(BindableObject bindable, object oldValue, object newValue)
        {
            RadioButton rb;
            try
            {
                rb = (RadioButton)bindable;
                if (oldValue != newValue)
                {
                    rb.IsSelected = (bool)newValue;
                }
            }
            catch { }
        }
        private static void ContentChangeProperty(BindableObject bindable, object oldValue, object newValue)
        {

            RadioButton rb;
            try
            {
                rb = (RadioButton)bindable;
                if (oldValue != newValue)
                {
                    rb.Content = newValue;
                }
            }
            catch { }
        }
        private static void UnselectedColorChangeProperty(BindableObject bindable, object oldValue, object newValue)
        {
            RadioButton rb;
            try
            {
                rb = (RadioButton)bindable;
                if (oldValue != newValue)
                {
                    rb.UnselectedColor = (Color)newValue;
                }
            }
            catch { }
        }
        private static void SelectedColorChangeProperty(BindableObject bindable, object oldValue, object newValue)
        {
                RadioButton rb;
                try
                {
                    rb = (RadioButton)bindable;
                    if (oldValue != newValue)
                    {
                        rb.SelectedColor = (Color)newValue;
                    }
               }
                catch { }
        }
        private static void IdGroupChangeProperty(BindableObject bindable, object oldValue, object newValue)
        {
                RadioButton rb;
                try
                {
                    rb = (RadioButton)bindable;
                    if (oldValue != newValue)
                    {
                        rb.IdGroup = (string)newValue;
                }
            }
            catch { }
        }
    }
}