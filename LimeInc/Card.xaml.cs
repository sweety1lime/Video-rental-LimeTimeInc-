using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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
    public sealed partial class Card : Page
    {
        string connectionString = @"Data Source=LAPTOP-TAESVC8F\ZXCLOWN;Initial Catalog=Suicide;Persist Security Info=True;User ID=sa;Password=123456789";
        SqlConnection connection;
        SqlDataAdapter dataAdapter;
        SqlCommand command;
        public int ID;
        public static int Identyti;
        public Card()
        {
            this.InitializeComponent();
            connection = new SqlConnection(connectionString);
        }

        public async void Load_Card()
        {

            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter("SELECT        dbo.Video_and_Audio_Material.ID_Material, dbo.Publisher.[ID_Type material], dbo.Video_and_Audio_Material.ID_Publisher, dbo.Publisher.ID_Type," +
                    " dbo.Video_and_Audio_Material.Name AS [Наименование материала],   dbo.Video_and_Audio_Material.Date_Issued AS [Дата выска], dbo.Video_and_Audio_Material.Duration AS Длительность, dbo.Video_and_Audio_Material.Name_film_studio AS Студия, " +
                    "   dbo.Video_and_Audio_Material.Producer AS Режиссер, dbo.Video_and_Audio_Material.Actors_main_role AS [Актеры главных роллей], dbo.Video_and_Audio_Material.Performer AS Исполнитель,   dbo.Video_and_Audio_Material.Number_Issued AS [Номер выпуска], dbo.Video_and_Audio_Material.Author AS Автор, dbo.Video_and_Audio_Material.Cover_Image AS Обложка, dbo.Type_Media_File.Name," +
                    "   dbo.Type_Media.Name_type, dbo.Publisher.Name AS Издание, dbo.Publisher.Price AS Цена, dbo.Publisher.Cost_project AS [Стоимость аренды] FROM      dbo.Publisher INNER JOIN  dbo.Video_and_Audio_Material ON dbo.Publisher.ID_Publisher = dbo.Video_and_Audio_Material.ID_Publisher INNER JOIN  dbo.Type_Media ON dbo.Publisher.[ID_Type material] = dbo.Type_Media.ID_type_media INNER JOIN  " +
                    $" dbo.Type_Media_File ON dbo.Publisher.ID_Type = dbo.Type_Media_File.ID_type_media_file where ID_Material = {ID}", connection);
                DataSet Ds = new DataSet();
                dataAdapter.Fill(Ds);

                var bitmapImage = new BitmapImage();
                var stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(((byte[])Ds.Tables[0].Rows[0][13]).AsBuffer());
                stream.Seek(0);
                bitmapImage.SetSource(stream);
                Icon.Source = bitmapImage;
                NameFile.Text = Ds.Tables[0].Rows[0][4].ToString();
                if (Ds.Tables[0].Rows[0][5].ToString() != "")
                {
                    Description.Text += "Дата выпуска: " + Ds.Tables[0].Rows[0][5].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][6].ToString() != "")
                {
                    Description.Text += "Длительность: " + Ds.Tables[0].Rows[0][6].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][7].ToString() != "")
                {
                    Description.Text += "Студия: " + Ds.Tables[0].Rows[0][7].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][8].ToString() != "")
                {
                    Description.Text += "Режиссер: " + Ds.Tables[0].Rows[0][8].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][9].ToString() != "")
                {
                    Description.Text += "Актеры главных ролей: " + Ds.Tables[0].Rows[0][9].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][10].ToString() != "")
                {
                    Description.Text += "Исполнитель: " + Ds.Tables[0].Rows[0][10].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][11].ToString() != "")
                {
                    Description.Text += "Номер выпуска: " + Ds.Tables[0].Rows[0][11].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][12].ToString() != "")
                {
                    Description.Text += "Автор: " + Ds.Tables[0].Rows[0][12].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][14].ToString() != "0")
                {
                    Description.Text += "Тип носителя: " + Ds.Tables[0].Rows[0][14].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][15].ToString() != "0")
                {
                    Description.Text += "Тип материала: " + Ds.Tables[0].Rows[0][15].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][16].ToString() != "0")
                {
                    Description.Text += "Издание: " + Ds.Tables[0].Rows[0][16].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][17].ToString() != "0")
                {
                    Description.Text += "Ценв: " + Ds.Tables[0].Rows[0][17].ToString() + "\n";

                }
                if (Ds.Tables[0].Rows[0][18].ToString() != "0")
                {
                    Description.Text += "Стоимость аренды: " + Ds.Tables[0].Rows[0][18].ToString() + "\n";

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

            if (PersonalArea.ID == 1)
            {
                Rent.Visibility = Visibility.Collapsed;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ID = (int)e.Parameter;
        }

        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            Load_Card();
        }

        private void Rent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter($"select ID_Client from Client where ID_User = '{Identyti}'", connection);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                int id = (int)ds.Tables[0].Rows[0][0];
                command = new SqlCommand($"insert into Rent (Date_lease,Date_return,Fine,ID_Client) values (GETDATE(),NULL,NULL,{id})", connection);
                command.ExecuteNonQuery();

                var message = new MessageDialog("Материал арендован").ShowAsync();
            }
            catch (Exception exception)
            {
                var message = new MessageDialog(exception.Message).ShowAsync();
            }
            finally
            {
                connection.Close();

            }
        }
    }
}
