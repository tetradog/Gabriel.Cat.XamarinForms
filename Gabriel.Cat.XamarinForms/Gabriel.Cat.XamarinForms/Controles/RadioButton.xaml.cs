using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gabriel.Cat.S.Utilitats;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gabriel.Cat.XamarinForms.Controles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RadioButton : ContentView,IClauUnicaPerObjecte
    {
        public enum Selection
        {
            Singel, Multiple
        }

        public const string DEFAULTGROUP = "default";
        const string DEFAULTCONTENT = "";

        static LlistaOrdenada<string, LlistaOrdenada<int,RadioButton>> selectionDic;
        static LlistaOrdenada<string, Selection> selectionModeDic;
        static GenIdInt genId;
       
        
        int id;
        string idGroup;
        bool isSelected;
        Color colorSelected;
        Color colorUnSelected;
        object content;
        Selection selectionMode;

        public event EventHandler Selected;

        static RadioButton()
        {
            selectionDic = new LlistaOrdenada<string, LlistaOrdenada<int,RadioButton>>();
            selectionModeDic = new LlistaOrdenada<string, Selection>();

            selectionDic.Add(DEFAULTGROUP, new LlistaOrdenada<int,RadioButton>());
            selectionModeDic.Add(DEFAULTCONTENT, Selection.Singel);
            genId = new GenIdInt();

        }
        public RadioButton()
        {
            id =genId.Siguiente();
            content = DEFAULTCONTENT;
            idGroup = DEFAULTGROUP;
            btnOnOff = new Button();
            IdGroup = DEFAULTGROUP;
            InitializeComponent();

           

            IsSelected = false;

            SelectedColor = Color.AliceBlue;
            UnSelectedColor = Color.White;

            
            
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {


                if (value)
                {
                    if (Selected != null)
                        Selected(this, new EventArgs());
                    if (SelectionMode == Selection.Singel&&selectionDic[IdGroup].Count==1)
                    {
                       
                        selectionDic[IdGroup].GetValueAt(0).IsSelected = false;
                        
                    }
                    selectionDic[IdGroup].Add(id,this);
                }
                else if (value != isSelected)
                {
                    selectionDic[idGroup].Remove(id);
                }

                isSelected = value;
                SelectionChanged();
            }
        }
        public object Content
        {
            get => content;
            set
            {
                content = value != null ? value : DEFAULTCONTENT;
                lblRbtn.Text = ToString();
            }
        }

        public Color SelectedColor
        {
            get => colorSelected;
            set
            {
                colorSelected = value;
                if (!isSelected)
                    btnOnOff.BackgroundColor = colorSelected;
            }
        }
        public Color UnSelectedColor
        {
            get => colorUnSelected;
            set
            {
                colorUnSelected = value;
                if (!isSelected)
                    btnOnOff.BackgroundColor = colorSelected;
            }
        }
        public string IdGroup
        {
            get => idGroup;
            set
            {
                Selection selectionMode;
                bool aux;
                if (value == null)
                    value = DEFAULTGROUP;

                if (!selectionDic.ContainsKey(value))
                    selectionDic.Add(value, new LlistaOrdenada<int, RadioButton>());

                if (selectionDic[IdGroup].ContainsKey(id))
                    selectionDic[IdGroup].Remove(id);
              
                selectionMode = SelectionMode;
                idGroup = value;
               
                if (!selectionModeDic.ContainsKey(idGroup))
                {
                    selectionModeDic.Add(idGroup, selectionMode);
                }

                aux = IsSelected;
                isSelected = !aux;
                IsSelected = aux;



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
                    for (int i = 0; i < selectionDic[idGroup].Count; i++)
                        selectionDic[idGroup].GetValueAt(i).IsSelected = false;
                    this.isSelected = isSelected;
                    SelectionChanged();

                }
            }
        }

        IComparable IClauUnicaPerObjecte.Clau => id;

        private void SelectionChanged()
        {
            
            if (isSelected)
            {
                btnOnOff.BackgroundColor = SelectedColor;
            }
            else
            {
                btnOnOff.BackgroundColor = UnSelectedColor;
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
    }
}