using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LimeInc
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Catalog : Page
    {

        string connectionString = @"Data Source=LAPTOP-TAESVC8F\ZXCLOWN;Initial Catalog=Suicide;Persist Security Info=True;User ID=sa;Password=123456789";
        SqlConnection connection;
        SqlDataAdapter dataAdapter;
        SqlCommand command;
        public Catalog()
        {
            this.InitializeComponent();
            connection = new SqlConnection(connectionString);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PersonalArea));
        }
        public async void Load_Card()// получает информацию и выгружает объекты на экран
        {
           
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter("SELECT dbo.Video_and_Audio_Material.ID_Material, dbo.Video_and_Audio_Material.ID_Type, dbo.Video_and_Audio_Material.Name, dbo.Video_and_Audio_Material.Cover_Image, dbo.Type_Media.Name_type FROM  dbo.Type_Media INNER JOIN   dbo.Video_and_Audio_Material ON dbo.Video_and_Audio_Material.ID_Type = dbo.Type_Media.ID_type_media", connection);
                DataSet Ds = new DataSet();
                dataAdapter.Fill(Ds);


                DataTable Table = new DataTable();
                 for (int i = 0; i < Ds.Tables[0].Rows.Count;i++)
                {
                    Card_Materialxaml card = new Card_Materialxaml();
                    var bitmapImage = new BitmapImage();
                    var stream = new InMemoryRandomAccessStream();
                    await stream.WriteAsync(((byte[])Ds.Tables[0].Rows[i][3]).AsBuffer());
                    stream.Seek(0);
                    bitmapImage.SetSource(stream);
                    card.Title = Ds.Tables[0].Rows[i][2].ToString();
                    card.Message = Ds.Tables[0].Rows[i][4].ToString();
                    card.Icon = bitmapImage;
                    card.Margin = new Thickness(10, 10, 10, 10);
                    card.VerticalAlignment = VerticalAlignment.Top;
                    var iternal = (int)Ds.Tables[0].Rows[i][0];
                    card.Tapped += async (sender, e) =>
                     {
                         CoreApplicationView newView = CoreApplication.CreateNewView();
                         int newViewId = 0;
                         await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {
                             Frame frame = new Frame();
                             frame.Navigate(typeof(Card),iternal);
                             Window.Current.Content = frame;
                             Window.Current.Activate();

                             newViewId = ApplicationView.GetForCurrentView().Id;
                         });
                         bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
                     };
                    Massive.Children.Add(card);

                }
            }
            catch (Exception ex)
            {

                var message = new MessageDialog(ex.Message).ShowAsync();

            }
            finally
            {
                connection.Close();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Card();
            if (PersonalArea.ID == 1)
            {
                Personal_Area.Visibility = Visibility.Collapsed;
            }
        }
    }
}
