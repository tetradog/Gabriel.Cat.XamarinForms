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
        const int MARGEN = 300;
        SortedList<string, List<BindableProperty>> dicPropertiesView;
        SortedList<string, List<View>> dicInterficieView;
        SortedList<string, string> dicDataInterficie;
        SortedList<string, int> dicInterficiesViewsLastIndex;
        SortedList<string, double> dicHeightInterficies;
        List<object> data;
        List<double> heightData;
        bool working;
        int actualIndex;
        public OptimitzedList()
        {

            InitializeComponent();
            dicDataInterficie = new SortedList<string, string>();
            dicInterficieView = new SortedList<string, List<View>>();
            dicPropertiesView = new SortedList<string, List<BindableProperty>>();
            dicInterficiesViewsLastIndex = new SortedList<string, int>();
            dicHeightInterficies = new SortedList<string, double>();
            data = new List<object>();
            heightData = new List<double>();
            svMain.Scrolled += SetPosition;
            SizeChanged += SetSize;
            actualIndex = 0;
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
                if (posItem >=  e.ScrollY)
                    indexFirsItem = i;
            }
            new Action(() => gAux.RowDefinitions[0].Height = e.ScrollY).BeginInvoke();
            if (indexFirsItem >= 0)
                SetFirstItemIndex(indexFirsItem);

        }
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
            Type tipo;
            if (obj != null)
            {
                if (!dicDataInterficie.ContainsKey(obj.GetType().AssemblyQualifiedName))
                    throw new ArgumentException("the object doesn't have a type added");
                tipo = obj.GetType();
                data.Add(obj);
                if (dicHeightInterficies[dicDataInterficie[tipo.AssemblyQualifiedName]] > 0)
                    heightData.Add(dicHeightInterficies[dicDataInterficie[tipo.AssemblyQualifiedName]]);
                else
                {
                    //obtengo la altura del control con la informacion en cuestion
                    heightData.Add(GetHeight(Type.GetType(dicDataInterficie[tipo.AssemblyQualifiedName]), obj));
                }
            }
        }
        public void Add(IList<object> objs)
        {
            if (objs != null)
            {
                for (int i = 0; i < objs.Count; i++)
                    if (!dicDataInterficie.ContainsKey(objs[i].GetType().AssemblyQualifiedName))
                        throw new ArgumentException(string.Format("the object at {0} doesn't have a type added", i));

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
            Action act = () => stkMain.Children.Clear();
            Device.BeginInvokeOnMainThread(act);
        }
        void Add(View view)
        {
            Action act = () => stkMain.Children.Add(view);
            Device.BeginInvokeOnMainThread(act);
        }
        void SetFirstItemIndex(int index)
        {
            string typeInterficie;
            List<View> views;
            SortedList<string, string> dicTypes = new SortedList<string, string>();
            string[] types;
            string type;
            int totalItems = -1;
            double heightActual = 0;
            working = true;
            if (index < 0)
                index = 0;
            else if (index >= data.Count)
                index = data.Count - 1;

            actualIndex = index;
            Clear();
            for (int i = 0; i < data.Count && totalItems < 0; i++)
            {
                heightActual += heightData[i];
                if (heightActual  >= Height)
                    totalItems = i;
            }
            if (totalItems < 0)
                totalItems = data.Count;
            for (int j = index, fJ = j + totalItems; j < fJ; j++)
            {
                type = dicDataInterficie[data[j].GetType().AssemblyQualifiedName];
                if (!dicTypes.ContainsKey(type))
                {
                    dicTypes.Add(type, type);
                    dicInterficiesViewsLastIndex[type] = 1;
                }
                else dicInterficiesViewsLastIndex[type]++;
            }
            types = dicTypes.GetValues();
            for (int j = 0; j < types.Length; j++)
            {
                typeInterficie = types[j];
                views = dicInterficieView[typeInterficie];
                if (dicInterficiesViewsLastIndex[typeInterficie] > views.Count)
                {
                    for (int i = 0; dicInterficiesViewsLastIndex[typeInterficie] > views.Count; i++)
                        views.Add((View)Activator.CreateInstance(Type.GetType(typeInterficie)));
                }
                else
                {
                    for (int i = dicInterficiesViewsLastIndex[typeInterficie], f = views.Count; i < f; i++)
                    {
                        views.RemoveAt(views.Count - 1);
                    }
                }
                dicInterficiesViewsLastIndex[typeInterficie] = 0;
            }

            for (int j = index, fJ = j + totalItems; j < fJ; j++)
            {
                Add(SetData(data[j]));
                dicInterficiesViewsLastIndex[dicDataInterficie[data[j].GetType().AssemblyQualifiedName]]++;
            }
            working = false;
        }



        View SetData(object data)
        {
            string typeInterficie = dicDataInterficie[data.GetType().AssemblyQualifiedName];
            List<View> views = dicInterficieView[typeInterficie];
            View view;

            view = views[dicInterficiesViewsLastIndex[typeInterficie]];

            SetViewData(view, data);
            return view;
        }
        void SetViewData(View view, object data)
        {
            string inteficieType = dicDataInterficie[data.GetType().AssemblyQualifiedName];
            List<BindableProperty> propertiesView = dicPropertiesView[inteficieType];
            for (int i = 0; i < propertiesView.Count; i++)
            {
                view.SetValue(propertiesView[i], data.GetProperty(propertiesView[i].PropertyName));
            }
        }
    }
}