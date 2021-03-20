using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace LimeInc
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class EditingCatalog : Page
    {

        string connectionString = @"Data Source=LAPTOP-TAESVC8F\ZXCLOWN;Initial Catalog=Suicide;Persist Security Info=True;User ID=sa;Password=123456789"; SqlConnection connection;
        SqlDataAdapter dataAdapter;
        SqlCommand command;
        byte[] buffer;
        public EditingCatalog()
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
            try
            {
                connection.Open();
                command = new SqlCommand($"insert into Publisher (Name, Date_Issued,Price,Cost_project,ID_Type,[ID_Type material] ) values ('{Name_Material.Text}',CONVERT(datetime,'{Date.Date.Value.DateTime}',104),{Cost_Material.Text},{Cost_Rent.Text},{Type_Media.SelectedValue},{Type_Material.SelectedValue})", connection) ;
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                var message = new MessageDialog(exception.Message).ShowAsync();
            }
            finally
            {
                connection.Close();
                Load();
            }
        }

        private void Combo_Load(string query, ComboBox comboBox)
        {
            try
            {
                connection.Open();
                DataSet ds = new DataSet();

                dataAdapter = new SqlDataAdapter(query, connection);
                dataAdapter.Fill(ds);

                Dictionary<String, string> tableDictionary = new Dictionary<string, string>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    tableDictionary.Add(row[1].ToString(), row[0].ToString());
                }

                comboBox.ItemsSource = tableDictionary;
                comboBox.SelectedValuePath = "Value";
                comboBox.DisplayMemberPath = "Key";

                if (comboBox.Items.Count != 0)
                    comboBox.SelectedIndex = 0;
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
            Load_Material();
            Combo_Load("SELECT ID_type_media, Name_type FROM dbo.Type_Media", Type_Material);
            Combo_Load("SELECT ID_type_media_file, Name FROM dbo.Type_Media_File", Type_Media);
            Combo_Load("SELECT ID_type_media, Name_type FROM dbo.Type_Media", material);
            Combo_Load("SELECT ID_Publisher, Name FROM Publisher", ZXC);

            if(PersonalArea.ID == 1)
            {
                Save_Material.Visibility = Visibility.Collapsed;
                Save_Publisher.Visibility = Visibility.Collapsed;
                Update_Material.Visibility = Visibility.Collapsed;
                Update_Publisher.Visibility = Visibility.Collapsed;
                Delete_Material.Visibility = Visibility.Collapsed;
                Delete_Publisher.Visibility = Visibility.Collapsed;
                Choose.Visibility = Visibility.Collapsed;
            }
        }

        private void Select_Row()
        {
            if (Publisher.SelectedIndex != -1)
            {
                string date = (Publisher.SelectedItem as object[])[4].ToString();
                string [] vs = date.Substring(0, 10).Split('.');
                Name_Material.Text = (Publisher.SelectedItem as object[])[3].ToString();
                Date.Date = new DateTime(Convert.ToInt32(vs[2]), Convert.ToInt32(vs[1]), Convert.ToInt32(vs[0]));
                Cost_Material.Text = (Publisher.SelectedItem as object[])[5].ToString();
                Cost_Rent.Text = (Publisher.SelectedItem as object[])[6].ToString();
                Type_Material.SelectedValue = (Publisher.SelectedItem as object[])[2].ToString();
                Type_Media.SelectedValue = (Publisher.SelectedItem as object[])[1].ToString();
                
            }
            if (Materials.SelectedIndex != -1)
            {
                string date = (Materials.SelectedItem as object[])[3].ToString();
                string[] vs = date.Substring(0, 10).Split('.');
                Name_material.Text = (Materials.SelectedItem as object[])[2].ToString();
                Date_iss.Date = new DateTime(Convert.ToInt32(vs[2]), Convert.ToInt32(vs[1]), Convert.ToInt32(vs[0]));
                Duration.Text = (Materials.SelectedItem as object[])[4].ToString();
                Name_Studio.Text = (Materials.SelectedItem as object[])[5].ToString();
                Producer.Text = (Materials.SelectedItem as object[])[6].ToString();
                Actors.Text = (Materials.SelectedItem as object[])[7].ToString();
                Performer.Text = (Materials.SelectedItem as object[])[8].ToString();
                Author.Text = (Materials.SelectedItem as object[])[9].ToString();
                Number.Text = (Materials.SelectedItem as object[])[10].ToString();
                material.SelectedValue = (Materials.SelectedItem as object[])[1].ToString();
            }
        }
        private void Load()
        {
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter("SELECT dbo.Publisher.ID_Publisher, dbo.Publisher.ID_Type, dbo.Publisher.[ID_Type material], dbo.Publisher.Name AS Название, dbo.Publisher.Date_Issued AS [Дата создания], dbo.Publisher.Price AS [Цена за материал],  dbo.Publisher.Cost_project AS [Стоимость аренды], dbo.Type_Media_File.Name AS [Тип носителя], dbo.Type_Media.Name_type AS [Тип файла] FROM            dbo.Publisher INNER JOIN dbo.Type_Media ON dbo.Publisher.[ID_Type material] = dbo.Type_Media.ID_type_media INNER JOIN  dbo.Type_Media_File ON dbo.Publisher.ID_Type = dbo.Type_Media_File.ID_type_media_file", connection);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                DataTable table = ds.Tables[0];

                Publisher.Columns.Clear();
                Publisher.AutoGenerateColumns = false;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Publisher.Columns.Add(new DataGridTextColumn()
                    {
                        Header = table.Columns[i].ColumnName,
                        Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                    });
                }

                var collection = new ObservableCollection<object>();
                foreach (DataRow row in table.Rows)
                {
                    collection.Add(row.ItemArray);
                }

                Publisher.ItemsSource = collection;

                Publisher.Columns[0].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Publisher.Columns[1].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Publisher.Columns[2].Visibility = Windows.UI.Xaml.Visibility.Collapsed;


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

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                FileOpenPicker dialog = new FileOpenPicker();
                dialog.FileTypeFilter.Add(".jpg") ;
                dialog.FileTypeFilter.Add(".png") ;
                StorageFile file = await dialog.PickSingleFileAsync() ;
                if (file != null)
                {
                    Picture.Text = file.Path;
                    using (var inputStream = await file.OpenSequentialReadAsync())
                    {
                        var readStream = inputStream.AsStreamForRead();
                        buffer = new byte[readStream.Length];
                        await readStream.ReadAsync(buffer, 0, buffer.Length);
                       
                    }
                }
            }
            catch (Exception exception)
            {
                var message = new MessageDialog(exception.Message).ShowAsync();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Publisher.SelectedIndex != -1)
                {

                connection.Open();
                command = new SqlCommand($"update Publisher set Name = '{Name_Material.Text}' , Date_Issued = CONVERT(datetime,'{Date.Date.Value.DateTime}',104),Price = {Cost_Material.Text},Cost_project = {Cost_Rent.Text},ID_Type = {Type_Media.SelectedValue},[ID_Type material] = {Type_Material.SelectedValue} where ID_Publisher = {(Publisher.SelectedItem as object[])[0].ToString()}", connection);
                command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                var message = new MessageDialog(exception.Message).ShowAsync();
            }
            finally
            {
                connection.Close();
                Load();
            }
        }

        private void Publisher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Select_Row();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Publisher.SelectedIndex != -1)
                {

                    connection.Open();
                    command = new SqlCommand($"delete from Publisher where ID_Publisher = {(Publisher.SelectedItem as object[])[0].ToString()}", connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                var message = new MessageDialog(exception.Message).ShowAsync();
            }
            finally
            {
                connection.Close();
                Load();
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                command = new SqlCommand($"insert into Video_and_Audio_Material ( Name,Date_Issued,Duration,Name_film_studio,Producer,Actors_main_role,Performer,Author,Number_Issued,Cover_Image,ID_Type,ID_Publisher) values ('{Name_material.Text}',CONVERT(datetime,'{Date_iss.Date.Value.DateTime}',104),'{Duration.Text}','{Name_Studio.Text}','{Producer.Text}','{Actors.Text}','{Performer.Text}','{Author.Text}','{Number.Text}',@img,{material.SelectedValue},{ZXC.SelectedValue})", connection);
                command.Parameters.Add(new SqlParameter("@img",buffer));
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                var message = new MessageDialog(exception.Message).ShowAsync();
            }
            finally
            {
                connection.Close();
                Load_Material();
            }
        }

        private void Load_Material()
        {
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter("SELECT dbo.Video_and_Audio_Material.ID_Material, dbo.Video_and_Audio_Material.ID_Type, dbo.Video_and_Audio_Material.Name AS Наименование," +
                    " dbo.Video_and_Audio_Material.Date_Issued AS [Дата выпуска], dbo.Video_and_Audio_Material.Duration AS Длительность, dbo.Video_and_Audio_Material.Name_film_studio AS [Название студии], dbo.Video_and_Audio_Material.Producer AS Режиссер," +
                    " dbo.Video_and_Audio_Material.Actors_main_role AS [Актеры главных ролей], dbo.Video_and_Audio_Material.Performer AS Исполнитель, dbo.Video_and_Audio_Material.Author AS Автор, dbo.Video_and_Audio_Material.Number_Issued AS [Номер выпуска]," +
                    " dbo.Video_and_Audio_Material.Cover_Image AS Обложка, dbo.Type_Media.Name_type AS [Тип материала] FROM  dbo.Video_and_Audio_Material INNER JOIN dbo.Type_Media ON dbo.Video_and_Audio_Material.ID_Type = dbo.Type_Media.ID_type_media", connection);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                DataTable table = ds.Tables[0];

                Materials.Columns.Clear();
                Materials.AutoGenerateColumns = false;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Materials.Columns.Add(new DataGridTextColumn()
                    {
                        Header = table.Columns[i].ColumnName,
                        Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                    });
                }

                var collection = new ObservableCollection<object>();
                foreach (DataRow row in table.Rows)
                {
                    var a = row.ItemArray;
                    byte[] b = (byte[])a[11];
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < 10; i++)
                    {
                        stringBuilder.Insert(i, b[i]);
                    }
                    a[11] = stringBuilder.ToString();
                    collection.Add(a);
                }

                Materials.ItemsSource = collection;
                Materials.Columns[0].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Materials.Columns[1].Visibility = Windows.UI.Xaml.Visibility.Collapsed;



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

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                command = new SqlCommand($"update Video_and_Audio_Material set Name = '{Name_material.Text}',Date_Issued = CONVERT(datetime,'{Date_iss.Date.Value.DateTime}',104),Duration = '{Duration.Text}',Name_film_studio = '{Name_Studio.Text}',Producer = '{Producer.Text}',Actors_main_role = '{Actors.Text}',Performer = '{Performer.Text}',Author = '{Author.Text}',Number_Issued ={Number.Text},ID_Type = {material.SelectedValue} where ID_Material = {(Materials.SelectedItem as object[])[0].ToString()}", connection);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                var message = new MessageDialog(exception.Message).ShowAsync();
            }
            finally
            {
                connection.Close();
                Load_Material();
            }
        }

        private void Materials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Select_Row();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                command = new SqlCommand($"delete from Video_and_Audio_Material where ID_Material = {(Materials.SelectedItem as object[])[0].ToString()}", connection);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                var message = new MessageDialog(exception.Message).ShowAsync();
            }
            finally
            {
                connection.Close();
                Load_Material();
            }
        }

        private void Cost_Material_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Length > 0)
            {
                char lastSymb = Convert.ToChar(args.NewText.Substring(args.NewText.Length - 1));
                if (!Char.IsDigit(lastSymb) || lastSymb == ' ')
                {
                    args.Cancel = true;
                }
            }
        }

        private void Cost_Rent_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Length > 0)
            {
                char lastSymb = Convert.ToChar(args.NewText.Substring(args.NewText.Length - 1));
                if (!Char.IsDigit(lastSymb) || lastSymb == ' ')
                {
                    args.Cancel = true;
                }
            }
        }

        private void Number_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Length > 0)
            {
                char lastSymb = Convert.ToChar(args.NewText.Substring(args.NewText.Length - 1));
                if (!Char.IsDigit(lastSymb) || lastSymb == ' ')
                {
                    args.Cancel = true;
                }
            }
        }
    }
}
