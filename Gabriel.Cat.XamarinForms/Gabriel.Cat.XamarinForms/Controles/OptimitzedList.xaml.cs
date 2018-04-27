using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gabriel.Cat.S.Extension;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gabriel.Cat.XamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptimitzedList : ContentView
    {
        const int MARGEN = 30;
        public static readonly OptimitzedList StaticTypeDefinition = new OptimitzedList();
        public static Color SelectedColor = Color.LightBlue;
        public static Color UnSelectedColor = Color.Transparent;

        SortedList<string, List<BindableProperty>> dicPropertiesView;
        SortedList<string, List<View>> dicInterficieView;
        SortedList<string, string> dicDataInterficie;
        SortedList<string, int> dicInterficiesViewsLastIndex;
        SortedList<string, double> dicHeightInterficies;
        SortedList<string, bool> dicImplementsISlectedItem;
        List<object> data;
        List<double> heightData;
        List<bool> dataSelected;
        bool working;
        int actualIndex;
        TapGestureRecognizer tapView;
        public event EventHandler<SelectedItemEventArgs> SelectedItem;
        public event EventHandler<SelectedItemEventArgs> UnSelectedItem;

        public OptimitzedList()
        {

            InitializeComponent();
            dicDataInterficie = new SortedList<string, string>();
            dicInterficieView = new SortedList<string, List<View>>();
            dicPropertiesView = new SortedList<string, List<BindableProperty>>();
            dicInterficiesViewsLastIndex = new SortedList<string, int>();
            dicHeightInterficies = new SortedList<string, double>();
            dicImplementsISlectedItem = new SortedList<string, bool>();
            dataSelected = new List<bool>();
            data = new List<object>();
            heightData = new List<double>();
            svMain.Scrolled += SetPosition;

            actualIndex = 0;
            tapView = new TapGestureRecognizer();
            tapView.Tapped += (s, e) => {
                ISelectedItem selectedItem = s as ISelectedItem;
                View view = s as View;
                int indexObj = actualIndex + stkMain.Children.IndexOf(view);
                if (selectedItem != null)
                    selectedItem.IsSelected = !selectedItem.IsSelected;
                else
                {
                    if (!dataSelected[indexObj])
                    {
                        view.BackgroundColor = SelectedColor;
                    }
                    else
                    {
                        view.BackgroundColor = UnSelectedColor;
                    }
                }

                if (!dataSelected[indexObj])
                {
                    SelectedItemEvent(data[indexObj]);
                }
                else
                {
                    UnSelectedItemEvent(data[indexObj]);
                }
            };
        }

        private void SetSize(object sender, EventArgs e)
        {
            try
            {
                SetFirstItemIndex(actualIndex);
            }
            catch { }
        }

        private void SetPosition(object sender, ScrolledEventArgs e)
        {
            double posItem = 0;
            int indexFirsItem = -1;
            while (working)
                System.Threading.Thread.Sleep(100);
            for (int i = 0; i < heightData.Count && indexFirsItem < 0; i++)
            {
                posItem += heightData[i];
                if (posItem >= e.ScrollY)
                    indexFirsItem = i;
            }
            new Action(() => gAux.RowDefinitions[0].Height = e.ScrollY).BeginInvoke();
            if (indexFirsItem >= 0)
                SetFirstItemIndex(indexFirsItem);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObjectType"></param>
        /// <param name="interficieDataType">si implementa ISelectedItem cuando hagan Tap  IsSelected=!IsSelected</param>
        /// <param name="heightIsVariable"></param>
        /// <param name="propertiesInterficie"></param>
        public void AddType(Type dataObjectType, Type interficieDataType, bool heightIsVariable, params BindableProperty[] propertiesInterficie)
        {
            if (!interficieDataType.IsSubclassOf(typeof(View)))
                throw new ArgumentException(string.Format("the {0} most inhert from {1}", interficieDataType.AssemblyQualifiedName, nameof(View)));
            for (int i = 0; i < propertiesInterficie.Length; i++)
            {
                if (dataObjectType.GetProperty(propertiesInterficie[i].PropertyName) == null)
                    throw new ArgumentException(String.Format("property {0} at position {1} doesn't exist on {3}", propertiesInterficie[i].PropertyName, i, dataObjectType.AssemblyQualifiedName));
                if (interficieDataType.GetProperty(propertiesInterficie[i].PropertyName) == null)
                    throw new ArgumentException(String.Format("property {0} at position {1} doesn't exist on {3}", propertiesInterficie[i].PropertyName, i, interficieDataType.AssemblyQualifiedName));

            }
            if (!dicDataInterficie.ContainsKey(dataObjectType.AssemblyQualifiedName))
            {
                dicDataInterficie.Add(dataObjectType.AssemblyQualifiedName, interficieDataType.AssemblyQualifiedName);
                dicInterficiesViewsLastIndex.Add(interficieDataType.AssemblyQualifiedName, 0);
                dicInterficieView.Add(interficieDataType.AssemblyQualifiedName, new List<View>());
                dicPropertiesView.Add(interficieDataType.AssemblyQualifiedName, new List<BindableProperty>());
                dicImplementsISlectedItem.Add(interficieDataType.AssemblyQualifiedName, interficieDataType.ImplementInterficie(typeof(ISelectedItem)));
                if (heightIsVariable)
                    dicHeightInterficies.Add(interficieDataType.AssemblyQualifiedName, -1);
                else
                {
                    dicHeightInterficies.Add(interficieDataType.AssemblyQualifiedName, GetHeight(interficieDataType, dataObjectType));
                }
            }
            dicPropertiesView[interficieDataType.AssemblyQualifiedName].AddRange(propertiesInterficie.Except(dicPropertiesView[interficieDataType.AssemblyQualifiedName]));
        }
        double GetHeight(Type interficieType, Type dataType)
        {
            return GetHeight(interficieType, Activator.CreateInstance(dataType));
        }
        double GetHeight(Type interficieType, object objDataTest)
        {
            double height;
            View viewTest = (View)Activator.CreateInstance(interficieType);
            SetViewData(viewTest, objDataTest);

            height = viewTest.GetSizeRequest(double.PositiveInfinity, 1000).Request.Height;//mirar que sea la real

            return height;

        }
        /// <summary>
        ///  Add into data list, must Refresh to apply on UI
        /// </summary>
        /// <param name="obj"></param>
        public void Add(object obj)
        {
            OptimitzedList list;
            Type tipo;
            if (obj != null)
            {
                tipo = obj.GetType();
                if (!dicDataInterficie.ContainsKey(tipo.AssemblyQualifiedName)&&!StaticTypeDefinition.dicDataInterficie.ContainsKey(tipo.AssemblyQualifiedName))
                    throw new ArgumentException("the object doesn't have a type added");
                
                data.Add(obj);
                if (dicDataInterficie.ContainsKey(tipo.AssemblyQualifiedName))
                {
                    list = this;
                }
                else list = StaticTypeDefinition;

                    if (list.dicHeightInterficies[list.dicDataInterficie[tipo.AssemblyQualifiedName]] > 0)
                        heightData.Add(list.dicHeightInterficies[list.dicDataInterficie[tipo.AssemblyQualifiedName]]);
                    else
                    {
                        //obtengo la altura del control con la informacion en cuestion
                        heightData.Add(list.GetHeight(Type.GetType(list.dicDataInterficie[tipo.AssemblyQualifiedName]), obj));
                    }
            
                dataSelected.Add(false);
            }
        }
        public void Add(IList<object> objs)
        {
            Type type;
            if (objs != null)
            {
                for (int i = 0; i < objs.Count; i++)
                {
                    type = objs[i].GetType();
                    if (!dicDataInterficie.ContainsKey(type.AssemblyQualifiedName) && !StaticTypeDefinition.dicDataInterficie.ContainsKey(type.AssemblyQualifiedName))
                        throw new ArgumentException(string.Format("the object at {0} doesn't have a type added", i));
                }

                for (int i = 0; i < objs.Count; i++)
                    Add(objs[i]);
            }
        }
        /// <summary>
        ///  Remove from data list, must Refresh to apply on UI
        /// </summary>
        /// <param name="objs"></param>
        public void Remove(IList<object> objs)
        {
            if (objs != null)
                for (int i = 0; i < objs.Count; i++)
                    Remove(objs[i]);
        }
        /// <summary>
        /// Remove from data list, must Refresh to apply on UI
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(object obj)
        {
            int index = data.IndexOf(obj);
            if (index >= 0)
            {
                heightData.RemoveAt(index);
                data.Remove(index);
                dataSelected.RemoveAt(index);
            }
        }
        public void Refresh()
        {
            double heightTotal = 1;
            Action act;
            for (int i = 0; i < heightData.Count; i++)
                heightTotal += heightData[i];
            act = () => gAux.HeightRequest = heightTotal;
            act.BeginInvoke();
            SetFirstItemIndex(actualIndex);
        }
        void Clear()
        {
            Action act = () => { stkMain.Children.Clear(); };
            Device.BeginInvokeOnMainThread(act);
        }
        void Add(View view)
        {
            Action act = () => stkMain.Children.Add(view);
            Device.BeginInvokeOnMainThread(act);
        }
        void SetFirstItemIndex(int index)
        {
            try
            {
                working = true;
                SetChangesOnIUToData();
                ISetFirstItemIndex(index);
            }
            finally
            {
                working = false;
            }
        }
        /// <summary>
        /// Set changes on UI to objects data
        /// </summary>
        public void SetChangesOnIUToData()
        {
            for (int i = 0, j = actualIndex; i < stkMain.Children.Count; i++, j++)
                SetDataView(data[j], stkMain.Children[i]);
        }

        private void SetDataView(object data, View view)
        {
            Type type = view.GetType();
            List<BindableProperty> properties = dicPropertiesView.ContainsKey(type.AssemblyQualifiedName)? dicPropertiesView[type.AssemblyQualifiedName]: StaticTypeDefinition.dicPropertiesView[type.AssemblyQualifiedName];
            for (int i = 0; i < properties.Count; i++)
                data.SetProperty(properties[i].PropertyName, view.GetProperty(properties[i].PropertyName));
        }

        void ISetFirstItemIndex(int index)
        {
            string typeInterficie;
            List<View> views;
            SortedList<string, string> dicTypes = new SortedList<string, string>();
            string[] types;
            string type;
            int totalItems = -1;
            double heightActual = 0;
            bool implementedAux;
            if (index < 0)
                index = 0;
            else if (index >= data.Count)
                index = data.Count - 1;

            actualIndex = index;
            Clear();
            for (int i = 0; i < data.Count && totalItems < 0; i++)
            {
                heightActual += heightData[i];
                if (heightActual > Height + MARGEN)
                    totalItems = i;
            }
            if (totalItems < 0)
                totalItems = data.Count;
            for (int j = index, fJ = j + totalItems; j < fJ; j++)
            {
                type = dicDataInterficie.ContainsKey(data[j].GetType().AssemblyQualifiedName) ? dicDataInterficie[data[j].GetType().AssemblyQualifiedName]:StaticTypeDefinition.dicDataInterficie[data[j].GetType().AssemblyQualifiedName];
                if (!dicTypes.ContainsKey(type))
                {
                    dicTypes.Add(type, type);
                    if (!dicInterficiesViewsLastIndex.ContainsKey(type))
                        dicInterficiesViewsLastIndex.Add(type, 0);
                    if (!dicInterficieView.ContainsKey(type))
                        dicInterficieView.Add(type, new List<View>());
                    dicInterficiesViewsLastIndex[type] = 1;
                }
                else dicInterficiesViewsLastIndex[type]++;
            }
            types = dicTypes.GetValues();
            for (int j = 0; j < types.Length; j++)
            {
                typeInterficie = types[j];
              
                views = dicInterficieView[typeInterficie];
                implementedAux = dicImplementsISlectedItem.ContainsKey(typeInterficie) ? dicImplementsISlectedItem[typeInterficie] : StaticTypeDefinition.dicImplementsISlectedItem[typeInterficie];

                if (dicInterficiesViewsLastIndex[typeInterficie] > views.Count)
                {
                     for (int i = 0; dicInterficiesViewsLastIndex[typeInterficie] > views.Count; i++)
                    {
                        views.Add((View)Activator.CreateInstance(Type.GetType(typeInterficie)));
                        views[views.Count - 1].GestureRecognizers.Add(tapView);
                       
                    }
                }
                else
                {
                    for (int i = dicInterficiesViewsLastIndex[typeInterficie], f = views.Count; i < f; i++)
                    {
                        
                        views[views.Count - 1].GestureRecognizers.Remove(tapView);
                        views.RemoveAt(views.Count - 1);
                    }
                }
                dicInterficiesViewsLastIndex[typeInterficie] = 0;
            }

            for (int j = index, fJ = j + totalItems; j < fJ; j++)
            {
                Add(SetData(data[j]));
                dicInterficiesViewsLastIndex[dicDataInterficie.ContainsKey(data[j].GetType().AssemblyQualifiedName) ? dicDataInterficie[data[j].GetType().AssemblyQualifiedName]:StaticTypeDefinition.dicDataInterficie[data[j].GetType().AssemblyQualifiedName]]++;
            }
          
        }

        private void UnSelectedItemEvent(object item)
        {
            Selection(UnSelectedItem, false, item);
        }
        private void SelectedItemEvent(object  item)
        {
            Selection(SelectedItem, true,item);
        }
        private void Selection(EventHandler<SelectedItemEventArgs> evento, bool value, object item)
        {
            if (evento != null)
                evento(this,new SelectedItemEventArgs( item));
            dataSelected[data.IndexOf(item)] = value;
        }
        View SetData(object data)
        {
            Type type=data.GetType();
            string typeInterficie = dicDataInterficie.ContainsKey(type.AssemblyQualifiedName)? dicDataInterficie[type.AssemblyQualifiedName]: StaticTypeDefinition.dicDataInterficie[type.AssemblyQualifiedName];
            List<View> views = dicInterficieView[typeInterficie];
            View view;

            view = views[dicInterficiesViewsLastIndex[typeInterficie]];

            SetViewData(view, data);
            return view;
        }
        void SetViewData(View view, object objData)
        {
            Type typeObj = objData.GetType();
            Type typeView = view.GetType();
            string typeInterficie = dicDataInterficie.ContainsKey(typeObj.AssemblyQualifiedName) ? dicDataInterficie[typeObj.AssemblyQualifiedName] : StaticTypeDefinition.dicDataInterficie[typeObj.AssemblyQualifiedName];
            List<BindableProperty> propertiesView = dicPropertiesView.ContainsKey(typeObj.AssemblyQualifiedName) ? dicPropertiesView[typeInterficie]: StaticTypeDefinition.dicPropertiesView[typeInterficie];
            ISelectedItem selectedItem;
            for (int i = 0; i < propertiesView.Count; i++)
            {
                view.SetValue(propertiesView[i], objData.GetProperty(propertiesView[i].PropertyName));
            }
            if ((dicImplementsISlectedItem.ContainsKey(typeView.AssemblyQualifiedName) ? dicImplementsISlectedItem[typeView.AssemblyQualifiedName]:StaticTypeDefinition.dicImplementsISlectedItem[typeView.AssemblyQualifiedName]) &&data.Count>0)
            {
                selectedItem = ((ISelectedItem)view);
                selectedItem.Item = objData;//lo pongo por si el programador quiere saber que item tiene el control
                selectedItem.IsSelected = dataSelected[this.data.IndexOf(objData)];
            }

        }
    }

}