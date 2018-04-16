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
         (nameof(IsSelected), typeof(bool), typeof(RadioButton), defaultValue: false, propertyChanged: IsSelectedChangeProperty);
        public static readonly BindableProperty TextProperty = BindableProperty.Create
         (nameof(Text), typeof(object), typeof(RadioButton), defaultValue: DEFAULTCONTENT, propertyChanged: TextChangeProperty);
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
        object objText;
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
            objText = DEFAULTCONTENT;
            id = genId.Siguiente();
            InitializeComponent();
            btnOnOff.BackgroundColor = UnselectedColor;


        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set
            {
                SetValue(IsSelectedProperty, value);

            }
        }
        public object Text
        {
            get => GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value != null ? value : DEFAULTCONTENT);

            }
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }
        public Color UnselectedColor
        {
            get => (Color)GetValue(UnselectedColorProperty);
            set
            {
                SetValue(UnselectedColorProperty, value);

            }
        }
        public string IdGroup
        {
            get => (string)GetValue(IdGroupProperty);
            set
            {

                SetValue(IdGroupProperty, value == null ? DEFAULTGROUP : value);
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
            return objText.ToString();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as RadioButton);
        }
        public bool Equals(RadioButton obj)
        {
            bool equals = obj != null;
            if (equals)
                equals = IdGroup.Equals(obj.IdGroup) && Text.Equals(obj.Text);
            return equals;
        }
        private static void IsSelectedChangeProperty(BindableObject bindable, object oldValue, object newValue)
        {
            RadioButton rb;
            RadioButton aux;
            bool value;
            try
            {
                rb = (RadioButton)bindable;
                if (oldValue != newValue)
                {
                    value = (bool)newValue;

                    if (value)
                    {
                        if (rb.Selected != null)
                            rb.Selected(rb, new EventArgs());
                        if (rb.SelectionMode == Selection.Singel && selectionDic[rb.IdGroup].Count == 1)
                        {
                            aux = selectionDic[rb.IdGroup].GetValueAt(0);
                            aux.SetValue(IsSelectedProperty, false);
                            aux.SelectionChanged();
                            selectionDic[rb.IdGroup].Remove(aux.id);
                        }
                        selectionDic[rb.IdGroup].Add(rb.id, rb);
                    }
                    else if (selectionDic[rb.IdGroup].ContainsKey(rb.id))
                    {
                        if (selectionDic[rb.IdGroup].Count > 1)
                            selectionDic[rb.IdGroup].Remove(rb.id);
                    }


                    rb.SelectionChanged();
                }
            }
            catch { }
        }
        private static void TextChangeProperty(BindableObject bindable, object oldValue, object newValue)
        {

            RadioButton rb;
            try
            {
                rb = (RadioButton)bindable;
                if (!ReferenceEquals(oldValue, newValue))
                {
                    rb.objText = newValue;
                    rb.lblRbtn.Text = rb.ToString();

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
                    if (!rb.IsSelected)
                        rb.btnOnOff.BackgroundColor = (Color)newValue;

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
                    if (rb.IsSelected)
                        rb.btnOnOff.BackgroundColor = (Color)newValue;
                }
            }
            catch { }
        }
        private static void IdGroupChangeProperty(BindableObject bindable, object oldValue, object newValue)
        {
            RadioButton rb;
            string strOld = oldValue.ToString();
            string strNew = newValue.ToString();
            try
            {
                rb = (RadioButton)bindable;
                if (oldValue != newValue)
                {

                    if (!selectionDic.ContainsKey(strNew))
                        selectionDic.Add(strNew, new LlistaOrdenada<int, RadioButton>());
                    if (!selectionModeDic.ContainsKey(strNew))
                    {
                        selectionModeDic.Add(strNew, default(Selection));
                    }
                    if (selectionDic[strOld].ContainsKey(rb.id))
                        selectionDic[strOld].Remove(rb.id);


                }
            }
            catch { }
        }
    }
}