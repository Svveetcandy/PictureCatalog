using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.CatalogProcessing;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для ChooseCatalog.xaml
    /// </summary>
    public partial class ChooseCatalog : Window
    {
        public string ChoosedCatalog { get; set; }
        Catalogs catalogs;
        public ChooseCatalog(Catalogs catalogs)
        {
            InitializeComponent();
            ShowCatalogs(catalogs);
        }
        void ShowCatalogs(Catalogs catalogs)
        {
            this.catalogs = catalogs;
            foreach(string name in catalogs.CatalogsName)
            {
                lb.Items.Add(new DoCatalogOrPropertie(System.IO.Path.GetFileNameWithoutExtension(name)));
            }
        }
        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            if (item != null && item.IsSelected)
            {
                AcceptCatalog(sender);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AcceptCatalog(sender);
        }
        private void AcceptCatalog(object sender)
        {
            int indexOfChoosedCatalog = lb.SelectedIndex;
            if (indexOfChoosedCatalog >= 0)
            {
                ChoosedCatalog = catalogs.CatalogsName[indexOfChoosedCatalog];
            }
            Close();

        }
    }
}
