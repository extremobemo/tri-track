
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MySql.Data.MySqlClient;
//using Android.Content;

namespace TriTrack
{
    [Activity(Label = "Account Creation")]
    public class UserCreationActivity : Activity
    {
        Button create_account;
        string first;
        string last;
        string username;
        string password;
        ProgressBar loading;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.UserCreation);
            create_account = FindViewById<Button>(Resource.Id.confirm_account);

            loading = FindViewById<ProgressBar>(Resource.Id.creation_progress_bar);
            create_account.Click += delegate
            {   
                loading.Visibility = ViewStates.Visible;
                CreateAccountButtonClick();
                //FindViewById<ProgressBar>(Resource.Id.creation_progress_bar).Visibility = ViewStates.Visible;
            };
        }


        public async void CreateAccountButtonClick()
        {
            bool x = await SubmitNewUser();
            if (x == true)
            {
                DisplaySuccessDialog();
            }
            else{
                DisplayMissingInfoError();
            }
        }

        private void DisplayMissingInfoError()
        {
            Android.App.AlertDialog.Builder diaglog = new AlertDialog.Builder(this);
            AlertDialog alert = diaglog.Create();
            alert.SetTitle("Error");
            alert.SetMessage("Please be sure to fill out all required information.");
            alert.SetButton("Okay", (c, ev) =>
            {
                alert.Dismiss();
            });
            alert.Show();

        }

        private void DisplaySuccessDialog()
        {
            Android.App.AlertDialog.Builder diaglog = new AlertDialog.Builder(this);
            AlertDialog alert = diaglog.Create();
            alert.SetTitle("Success!");
            alert.SetMessage("Your account has been created.");
            alert.SetButton("Okay", (c, ev) =>
            {
                alert.Dismiss();
                Intent intent = new Intent(this, typeof(MainActivity));
                this.StartActivity(intent);
            });
            alert.Show();
        }


        public Task<bool> SubmitNewUser()
        {
            return Task.Run(() =>
            {
                first = FindViewById<EditText>(Resource.Id.first_name_field).Text;
                last = FindViewById<EditText>(Resource.Id.last_name_field).Text;
                username = FindViewById<EditText>(Resource.Id.username_create).Text;
                password = FindViewById<EditText>(Resource.Id.password_create).Text;
                if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last)
                   || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return false;
                }
                //create_account.Enabled = false;
                MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None; Allow User Variables=True");
                connection.Open();
                List<string> names = new List<string>();
                var command = connection.CreateCommand();
                command.CommandText = ("SELECT username FROM users where username=@username;");
                command.Parameters.AddWithValue("@username", username);
                MySqlDataReader username_reader = command.ExecuteReader();
                while (username_reader.Read())
                {
                    names.Add(username_reader.GetString("username"));
                }
                username_reader.Close();
                if(names.Count() > 0){
                    return false;
                }
                MySqlCommand insert_new_user = connection.CreateCommand();
                insert_new_user.CommandText = ("INSERT into users(first_name, last_name, username, password)values(@first_text, @last_text, @username_text, @password_text);");
                insert_new_user.Parameters.AddWithValue("@first_text", first);
                insert_new_user.Parameters.AddWithValue("@last_text", last);
                insert_new_user.Parameters.AddWithValue("@username_text", username);
                insert_new_user.Parameters.AddWithValue("@password_text", password);
                insert_new_user.ExecuteNonQuery();
                connection.Close();
                return true;
            });

        }



    }
}

