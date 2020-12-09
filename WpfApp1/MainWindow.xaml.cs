using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.Preview;
using WpfApp1.MetaData;
using System.Windows.Forms;
using MaterialDesignThemes.Wpf;

namespace WpfApp1
{

    public partial class MainWindow : Window
    {
        List<Metadata> allMetadata = new List<Metadata>();
        string pathToMetadata;
        Catalogs catalogs = new Catalogs();
        ChooseCatalog chooseCatalog;
        public MainWindow()
        {
            InitializeComponent();


        }

        private void FindOnePicture(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();

            openDialog.Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf";
            if (openDialog.ShowDialog() == true)
            {
                string path = openDialog.FileName;

                PictureWindow pictureWindow = new PictureWindow(catalogs, true, path);
                pictureWindow.ShowDialog();

                chooseCatalog = null;

            }
        }

        private void FindPictures(object sender, RoutedEventArgs e)
        {


            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();

            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                lstw.ItemsSource = null;
                lstw.Items.Clear();
                allMetadata.Clear();

                List<string> fullFilesPath = new List<string>(System.IO.Directory.GetFiles(folderBrowser.SelectedPath, "*.jpg", SearchOption.AllDirectories));
                for (int i = 0; i < fullFilesPath.Count; i++)
                {
                    MetadataInteraction pictureInfo = new MetadataInteraction();
                    allMetadata.Add(pictureInfo.ExtractMetadata(fullFilesPath[i]));
                }
                OutputPreview outputPreview = new OutputPreview();
                lstw.ItemsSource = outputPreview.ViewPictures(allMetadata);
                CatalogName.Text = folderBrowser.SelectedPath;

                chooseCatalog = null;
            }

        }
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListViewItem item = sender as System.Windows.Controls.ListViewItem;
            if (item != null && item.IsSelected)
            {
                PictureWindow pictureWindow = new PictureWindow(catalogs, true, allMetadata, lstw.SelectedIndex);
                pictureWindow.Show();
            }
        }

        void lvUsersColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {

                    string header = headerClicked.Column.Header.ToString();
                    header = header.Substring(header.LastIndexOf(' ') + 1);
                    ListViewColumnComparer sortedList = new ListViewColumnComparer();
                    sortedList.Sort(header, allMetadata);
                    lstw.ItemsSource = null;
                    lstw.Items.Clear();
                    OutputPreview outputPreview = new OutputPreview();
                    lstw.ItemsSource = outputPreview.ViewPictures(allMetadata);
                }
            }
        }

        private void CreateCatalog(object sender, RoutedEventArgs e)
        {
            NewCatalog newCatalog = new NewCatalog(catalogs);
            newCatalog.ShowDialog();
            if (newCatalog.Accept)
            {
                allMetadata.Clear();
                File.Create(newCatalog.Path);
                pathToMetadata = newCatalog.Path;
                catalogs.CatalogsName.Add(newCatalog.Path);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            chooseCatalog = new ChooseCatalog(catalogs);
            chooseCatalog.ShowDialog();
            if (chooseCatalog.ChoosedCatalog != null)
            {
                lstw.ItemsSource = null;
                lstw.Items.Clear();
                AllMetadata bufMetadata = new AllMetadata();
                allMetadata = bufMetadata.ReadAllMetadata(chooseCatalog.ChoosedCatalog);
                var list = allMetadata.GroupBy(x => x.Path).Select(y => y.First());
                allMetadata = new List<Metadata>(list);
                lstw.ItemsSource = new OutputPreview().ViewPictures(allMetadata);
                CatalogName.Text = System.IO.Path.GetFileNameWithoutExtension(chooseCatalog.ChoosedCatalog);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ChooseCatalog chooseCatalog = new ChooseCatalog(catalogs);
            chooseCatalog.ShowDialog();
            if (chooseCatalog.ChoosedCatalog != null)
            {
                File.Delete(chooseCatalog.ChoosedCatalog);
                catalogs.CatalogsName.RemoveAt(catalogs.CatalogsName.IndexOf(chooseCatalog.ChoosedCatalog));
            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            ChooseCatalog chooseCatalog = new ChooseCatalog(catalogs);
            chooseCatalog.ShowDialog();
            if (chooseCatalog != null && chooseCatalog.ChoosedCatalog != null)
            {
                NewCatalog newCatalog = new NewCatalog(catalogs);
                newCatalog.ShowDialog();
                if (newCatalog.Accept)
                {
                    File.Move(chooseCatalog.ChoosedCatalog, newCatalog.Path);
                    catalogs.CatalogsName[catalogs.CatalogsName.IndexOf(chooseCatalog.ChoosedCatalog)] = newCatalog.Path;
                }
            }
        }

        void DeleteContextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (chooseCatalog != null)
            {
                foreach (PictureData item in lstw.SelectedItems)
                {
                    PictureWindow pictureWindow = new PictureWindow(catalogs, false, allMetadata, allMetadata.FindIndex(x => x.Path == item.FullName));
                    pictureWindow.DeletePictureWithoutClick(chooseCatalog);

                }

                lstw.ItemsSource = null;
                lstw.Items.Clear();
                AllMetadata bufMetadata = new AllMetadata();
                allMetadata = bufMetadata.ReadAllMetadata(chooseCatalog.ChoosedCatalog);
                var list = allMetadata.GroupBy(x => x.Path).Select(y => y.First());
                allMetadata = new List<Metadata>(list);
                lstw.ItemsSource = new OutputPreview().ViewPictures(allMetadata);
            }
            else
            {
                PictureData picture = lstw.SelectedItem as PictureData;
                File.Delete(picture.FullName);
            }

        }

        void AddContextMenu_Click(object sender, RoutedEventArgs e)
        {
            chooseCatalog = new ChooseCatalog(catalogs);
            chooseCatalog.ShowDialog();
            foreach (PictureData item in lstw.SelectedItems)
            {
                PictureWindow pictureWindow = new PictureWindow(catalogs, false, allMetadata, allMetadata.FindIndex(x => x.Path == item.FullName));
                pictureWindow.AddPictureWithoutClick(chooseCatalog);
            }

        }
        private void RenameContextMenu_Click(object sender, RoutedEventArgs e)
        {

            if (chooseCatalog != null)
            {
                int count = 0;
                NewCatalog newCatalog = new NewCatalog(catalogs);
                newCatalog.ShowDialog();
                foreach (PictureData item in lstw.SelectedItems)
                {
                    PictureWindow pictureWindow = new PictureWindow(catalogs, false, allMetadata, allMetadata.FindIndex(x => x.Path == item.FullName));

                    if (lstw.SelectedItems.Count > 1)
                    {
                        pictureWindow.RenamePictureWithoutClick(chooseCatalog, newCatalog.CreateNewName() + count++);
                    }
                    else
                    {
                        pictureWindow.RenamePictureWithoutClick(chooseCatalog, newCatalog.CreateNewName());
                    }


                }
                lstw.ItemsSource = null;
                lstw.Items.Clear();
                AllMetadata bufMetadata = new AllMetadata();
                allMetadata = bufMetadata.ReadAllMetadata(chooseCatalog.ChoosedCatalog);
                var list = allMetadata.GroupBy(x => x.Path).Select(y => y.First());
                allMetadata = new List<Metadata>(list);
                lstw.ItemsSource = new OutputPreview().ViewPictures(allMetadata);


            }


        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            OutputPreview outputPreview = new OutputPreview();

            if (SearchTextBox.Text == "" || SearchTextBox.Text == null)
            {
                lstw.ItemsSource = outputPreview.ViewPictures(allMetadata);
            }
            else
            {
                lstw.ItemsSource = null;
                lstw.Items.Clear();

                lstw.ItemsSource = outputPreview.SearchOfPictureIndex(outputPreview.ViewPictures(allMetadata), comboBox.Text, SearchTextBox.Text);
            }





        }
    }
}
