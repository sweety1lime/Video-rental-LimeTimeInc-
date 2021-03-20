using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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
    public sealed partial class Administration : Page
    {
        string connectionString = @"Data Source=LAPTOP-TAESVC8F\ZXCLOWN;Initial Catalog=Suicide;Persist Security Info=True;User ID=sa;Password=123456789"; SqlConnection connection;
        SqlDataAdapter dataAdapter;
        SqlCommand command;
        public Administration()
        {
            connection = new SqlConnection(connectionString);
            this.InitializeComponent();
        }

        private void Polygon_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            Frame.UpdateLayout();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (Password.Text.Length < 6)
            {
                var message = new MessageDialog("Пароль должен состоять из 6-ти символов").ShowAsync();
            }
            else
            {


                try
                {

                    connection.Open();
                    command = new SqlCommand($"insert into [User] (Login, Password,ID_Role) values ('{Login.Text}','{Password.Text}',{Role.SelectedValue})", connection);
                    command.ExecuteNonQuery();
                    dataAdapter = new SqlDataAdapter($"select ID_User from [User] where Login = '{Login.Text}'", connection);
                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);
                    int id = (int)ds.Tables[0].Rows[0][0];
                    command = new SqlCommand($"insert into Employee (Surname, Name,Patronomic,Series_Number_Passport,Issued_by,Date_Issued,ID_Role,ID_User ) values ('{Last_Name.Text}','{Name_Employee.Text}','{Surname.Text}',{Serial_Number.Text},'{Issued_by.Text}',CONVERT(datetime,'{Date.Date.Value.DateTime}',104),{Role.SelectedValue},{id})", connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    if (exception.Message.Contains("UQ_Login"))
                    {
                        var message = new MessageDialog("Такой Логин уже есть").ShowAsync();
                    }
                    else
                    {
                        var message = new MessageDialog(exception.Message).ShowAsync();
                    }
                }
                finally
                {
                    connection.Close();
                    Load();
                }

            }
        }
        private void Load()
        {
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter("SELECT dbo.Employee.ID_Employee, dbo.Employee.ID_Role, dbo.Employee.ID_User, dbo.Employee.Surname AS Фамилия, dbo.Employee.Name AS Имя, dbo.Employee.Patronomic AS Отчество, dbo.Employee.Series_Number_Passport AS[Серия и номер паспорта], dbo.Employee.Issued_by AS[Кем выдан], dbo.Employee.Date_Issued AS[Дата выдачи], dbo.Role.Name_Role AS Роль, dbo.[User].Login AS Логин, dbo.[User].Password AS Пароль FROM  dbo.Employee INNER JOIN dbo.Role ON dbo.Employee.ID_Role = dbo.Role.ID_role INNER JOIN  dbo.[User] ON dbo.Employee.ID_User = dbo.[User].ID_User AND dbo.Role.ID_role = dbo.[User].ID_Role", connection);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                DataTable table = ds.Tables[0];

                Employee.Columns.Clear();
                Employee.AutoGenerateColumns = false;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Employee.Columns.Add(new DataGridTextColumn()
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

                Employee.ItemsSource = collection;

                Employee.Columns[0].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Employee.Columns[1].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Employee.Columns[2].Visibility = Windows.UI.Xaml.Visibility.Collapsed;


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
            Combo_Load("SELECT ID_Role, Name_Role FROM dbo.Role", Role);
        }
        private void Select_Row()
        {
            if (Employee.SelectedIndex != -1)
            {
                string date = (Employee.SelectedItem as object[])[8].ToString();
                string[] vs = date.Substring(0, 10).Split('.');
                Last_Name.Text = (Employee.SelectedItem as object[])[3].ToString();
                Name_Employee.Text = (Employee.SelectedItem as object[])[4].ToString();
                Surname.Text = (Employee.SelectedItem as object[])[5].ToString();
                Serial_Number.Text = (Employee.SelectedItem as object[])[6].ToString();
                Issued_by.Text = (Employee.SelectedItem as object[])[7].ToString();
                Login.Text = (Employee.SelectedItem as object[])[10].ToString();
                Date.Date = new DateTime(Convert.ToInt32(vs[2]), Convert.ToInt32(vs[1]), Convert.ToInt32(vs[0]));
                Password.Text = (Employee.SelectedItem as object[])[11].ToString();
                Role.SelectedValue = (Employee.SelectedItem as object[])[1].ToString();

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

        private void Employee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Select_Row();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Employee.SelectedIndex != -1)
            {


                try
                {

                    connection.Open();

                    command = new SqlCommand($"delete from Employee where ID_Employee = {(Employee.SelectedItem as object[])[0].ToString()}", connection);
                    command.ExecuteNonQuery();
                    command = new SqlCommand($"Delete from [User] where Login = '{Login.Text}'", connection);
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Employee.SelectedIndex != -1 && Password.Text.Length >= 6)
            {
                try
                {

                    connection.Open();
                    command = new SqlCommand($"delete from Employee where ID_Employee = {(Employee.SelectedItem as object[])[0].ToString()}", connection);
                    command.ExecuteNonQuery();
                    command = new SqlCommand($"update [User] set Login ='{Login.Text}' , Password ='{Password.Text}' ,ID_Role = {Role.SelectedValue} where ID_User = {(Employee.SelectedItem as object[])[1].ToString()}", connection);
                    command.ExecuteNonQuery();
                    dataAdapter = new SqlDataAdapter($"select ID_User from [User] where Login = '{Login.Text}'", connection);
                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);
                    int id = (int)ds.Tables[0].Rows[0][0];
                    command = new SqlCommand($"insert into Employee (Surname, Name,Patronomic,Series_Number_Passport,Issued_by,Date_Issued,ID_Role,ID_User ) values ('{Last_Name.Text}','{Name_Employee.Text}','{Surname.Text}',{Serial_Number.Text},'{Issued_by.Text}',CONVERT(datetime,'{Date.Date.Value.DateTime}',104),{Role.SelectedValue},{id})", connection);
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

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(Users));
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditingCatalog));
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Catalog));
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

        private void Name_Employee_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
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
                if (Char.IsDigit(lastSymb) || lastSymb == ' ')
                {
                    args.Cancel = true;
                }
            }
        }
    }
}
