using System;
using System.Data;
using System.Data.SqlClient;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace LimeInc
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        string connectionString = @"Data Source=LAPTOP-TAESVC8F\ZXCLOWN;Initial Catalog=Suicide;Persist Security Info=True;User ID=sa;Password=123456789"; SqlConnection connection;
        SqlDataAdapter dataAdapter;
        SqlCommand command;
        public MainPage()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Auth.Visibility = Visibility.Collapsed;
            Repeat.Visibility = Visibility.Visible;
            Reg.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Repeat.Visibility = Visibility.Collapsed;
            Reg.Visibility = Visibility.Collapsed;
            Auth.Visibility = Visibility.Visible;
        }

        private void Auth_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter("select * from [User]", connection);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                bool logPassError = true;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i].ItemArray;
                    if (Login.Text == (string)row[1] && Password.Password == (string)row[2])
                    {
                        if (Login.Text.Length == 0)
                        {
                            var message = new MessageDialog("Поле логина не может быть пустым.").ShowAsync();
                            return;
                        }

                        if (Password.Password == "")
                        {
                            var message = new MessageDialog("Поле пароля не может быть пустым.").ShowAsync();
                            return;
                        }
                        connection.Close();
                        if ((int)row[3] == 1)
                        {
                            Frame.Navigate(typeof(Administration));
                            PersonalArea.ID = (int)row[0];
                            logPassError = false;
                        }
                        if ((int)row[3] == 2)
                        {
                            Frame.Navigate(typeof(EditingCatalog));
                            PersonalArea.ID = (int)row[0];
                            logPassError = false;
                        }
                        if ((int)row[3] == 3)
                        {
                            Frame.Navigate(typeof(Catalog));
                            PersonalArea.ID = (int)row[0];
                            Card.Identyti = (int)row[0];
                            logPassError = false;
                        }
                    }
                }
                if (logPassError)
                {
                    var message = new MessageDialog("Неверное имя пользователя или пароль").ShowAsync();
                }
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

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                    if (Password.Password.Length < 6 && Repeat.Password.Length < 6)
                    {
                        var message = new MessageDialog("Пароль должен состоять из 6 символов.").ShowAsync();
                        return;
                    }
                    if (Login.Text.Length == 0)
                    {
                        var message = new MessageDialog("Поле логина не может быть пустым.").ShowAsync();
                        return;
                    }

                    if (Password.Password == "")
                    {
                        var message = new MessageDialog("Поле пароля не может быть пустым.").ShowAsync();
                        return;
                    }
                    if (Repeat.Password == "")
                    {
                        var message = new MessageDialog("Поле повторите пароль не может быть пустым.").ShowAsync();
                        return;
                    }
                    if (Password.Password == Repeat.Password)
                    {

                        connection.Open();
                        command = new SqlCommand($"insert into [User] (Login,Password,ID_Role) values ('{Login.Text}','{Password.Password}',3)", connection);
                        command.ExecuteNonQuery();
                        command = new SqlCommand($"insert into Client (Surname,Name,Patronomic,Series_Number_Passport,Issued_by,Date_Issued,ID_Role,ID_User) values ('','','',0,'',0,3,(select ID_User from [User] where Login = '{Login.Text}'))", connection);
                        command.ExecuteNonQuery();
                    }
                    if(Password.Password != Repeat.Password)
                    {
                        var message = new MessageDialog("Пароли не совпадают").ShowAsync();
                    return;
                    }
                
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("UQ_Login"))
                {
                    var ex = new MessageDialog("Такой Логин уже есть").ShowAsync();
                }
                else
                {

                var message = new MessageDialog(exception.Message).ShowAsync();
                }
            }
            finally
            {
                connection.Close();

            }
        }
    }
}
