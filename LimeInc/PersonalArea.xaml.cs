using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class PersonalArea : Page
    {
        string connectionString = @"Data Source=LAPTOP-TAESVC8F\ZXCLOWN;Initial Catalog=Suicide;Persist Security Info=True;User ID=sa;Password=123456789"; SqlConnection connection;
        SqlDataAdapter dataAdapter;
        SqlCommand command;
        public static int ID;
        public PersonalArea()
        {
            connection = new SqlConnection(connectionString);
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                connection.Open();
                command = new SqlCommand($"update Client set Surname = '{Last_Name.Text}',Name = '{Name_Client.Text}',Patronomic = '{Surname.Text}',Series_Number_Passport = {Serial_Number.Text},Issued_by = '{Issued_by.Text}',Date_Issued = Convert(datetime,'{Date.Date.Value.DateTime}',104),ID_Role = 3  where ID_User = {ID}", connection);
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
        private void Load()
        {
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter($"SELECT ID_Rent,  Date_lease AS [Дата аренды], Date_return AS [Дата возврата], Fine AS Штраф  FROM     dbo.Rent where ID_Client = (select ID_Client from Client where ID_User = {PersonalArea.ID})", connection);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                DataTable table = ds.Tables[0];

                Rent.Columns.Clear();
                Rent.AutoGenerateColumns = false;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Rent.Columns.Add(new DataGridTextColumn()
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

                Rent.ItemsSource = collection;



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
        private void Load_String()
        {
           


                try
                {
                    connection.Open();
                    dataAdapter = new SqlDataAdapter($"SELECT   dbo.Client.ID_Client, dbo.Client.Surname, dbo.Client.Name, dbo.Client.Patronomic, dbo.Client.Series_Number_Passport, dbo.Client.Issued_by, dbo.Client.Date_Issued, dbo.[User].Login, dbo.[User].Password FROM            dbo.Client INNER JOIN  dbo.[User] ON dbo.Client.ID_User = dbo.[User].ID_User where dbo.Client.ID_User = {ID}", connection);
                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);

                    string date = ds.Tables[0].Rows[0][6].ToString();
                    string[] vs = date.Substring(0, 10).Split('.');
                    Last_Name.Text = (string)ds.Tables[0].Rows[0][1];
                    Name_Client.Text = (string)ds.Tables[0].Rows[0][2];
                    Surname.Text = (string)ds.Tables[0].Rows[0][3];
                    Serial_Number.Text = Convert.ToInt64(ds.Tables[0].Rows[0][4]).ToString();
                    Issued_by.Text = (string)ds.Tables[0].Rows[0][5];
                    Date.Date = new DateTime(Convert.ToInt32(vs[2]), Convert.ToInt32(vs[1]), Convert.ToInt32(vs[0]));
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
            Load_String();

        }

        private void Serial_Number_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Length > 0)
            {
                char lastSymb = Convert.ToChar(args.NewText.Substring(args.NewText.Length - 1));
                if (!Char.IsDigit(lastSymb) || lastSymb == ' ' || Serial_Number.Text.Length >= 10)
                {
                    args.Cancel = true;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Catalog));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Rent.SelectedIndex != -1) {
                try
                {
                    connection.Open();
                    DateTime zaglushka = new DateTime(2021, 03, 7);
                    TimeSpan date = zaglushka - (DateTime)(Rent.SelectedItem as object[])[1];
                    //TimeSpan date = DateTime.Today - (DateTime)(Rent.SelectedItem as object[])[0];
                    int shtraf = 1;
                    if (date.Days >= 8)
                    {
                        shtraf = (date.Days-7) * 330;
                    }
                    command = new SqlCommand($"update Rent set Date_return = GETDATE(), Fine = {shtraf}  where ID_Rent = {(Rent.SelectedItem as object[])[0]}", connection);
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
        }

        private void Last_Name_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Length > 0)
            {
                char lastSymb = Convert.ToChar(args.NewText.Substring(args.NewText.Length - 1));
                if (Char.IsDigit(lastSymb) || lastSymb == ' ')
                {
                    args.Cancel = true;
                }
            }
        }

        private void Name_Client_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Length > 0)
            {
                char lastSymb = Convert.ToChar(args.NewText.Substring(args.NewText.Length - 1));
                if (Char.IsDigit(lastSymb) || lastSymb == ' ')
                {
                    args.Cancel = true;
                }
            }
        }

        private void Surname_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Length > 0)
            {
                char lastSymb = Convert.ToChar(args.NewText.Substring(args.NewText.Length - 1));
                if (Char.IsDigit(lastSymb) || lastSymb == ' ' )
                {
                    args.Cancel = true;
                }
            }
        }
    }
}
