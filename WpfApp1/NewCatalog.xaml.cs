using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для NewCatalog.xaml
    /// </summary>
    public partial class NewCatalog : Window
    {
        public string Path { get; private set; }
        public bool Accept { get; private set; } = false;
        string catalogsLocation;
        public NewCatalog(Catalogs catalogs)
        {
            InitializeComponent();
            catalogsLocation = catalogs.CatalogsLoction;
        }

        private void CancelNewCatalog(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateNewCatalog(object sender, RoutedEventArgs e)
        {
            string newCatalogName = TextBoxCatalogName.Text.Trim();
            Path = catalogsLocation + "\\" + newCatalogName + ".txt";
            Accept = true;
            Close();


        }

        private void KeyEvents(object sender, KeyEventArgs e)
        {
            string ke = e.Key.ToString();
            if (e.Key.ToString() == "Return")
            {
                string newCatalogName = TextBoxCatalogName.Text.Trim();
                Path = catalogsLocation + "\\" + newCatalogName + ".txt";
                Accept = true;
                Close();
            }
        }

        public string CreateNewName()
        {
            return TextBoxCatalogName.Text.Trim();
        }
    }
}
