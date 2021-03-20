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
    public sealed partial class Users : Page
    {
        string connectionString = @"Data Source=LAPTOP-TAESVC8F\ZXCLOWN;Initial Catalog=Suicide;Persist Security Info=True;User ID=sa;Password=123456789"; SqlConnection connection;
        SqlDataAdapter dataAdapter;
        SqlCommand command;
        public Users()
        {
            connection = new SqlConnection(connectionString);
            this.InitializeComponent();
        }
        private void Load()
        {
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter("SELECT dbo.Client.ID_Client, dbo.Rent.ID_Rent, dbo.Client.Surname AS Фамилия, dbo.Client.Name AS Имя, dbo.Client.Patronomic AS Отчество, dbo.Client.Series_Number_Passport AS [Серия номер паспорта],   dbo.Client.Issued_by AS [Кем выдан], dbo.Client.Date_Issued AS [Дата выдачи], dbo.Rent.Date_lease AS [Дата аренды], dbo.Rent.Fine AS Штраф,  dbo.Rent.Date_return AS [Дата возврата] FROM   dbo.Rent INNER JOIN  dbo.Client ON dbo.Rent.ID_Client = dbo.Client.ID_Client", connection);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                DataTable table = ds.Tables[0];

                User.Columns.Clear();
                User.AutoGenerateColumns = false;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    User.Columns.Add(new DataGridTextColumn()
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

                User.ItemsSource = collection;

                User.Columns[0].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                User.Columns[1].Visibility = Windows.UI.Xaml.Visibility.Collapsed;


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
        }
    }
}
