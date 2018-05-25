
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        EditText first;
        EditText last;
        EditText username;
        EditText password;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            SetContentView(Resource.Layout.UserCreation);
            create_account = FindViewById<Button>(Resource.Id.confirm_account);
            first = FindViewById<EditText>(Resource.Id.first_name_field);
            last = FindViewById<EditText>(Resource.Id.last_name_field);
            username = FindViewById<EditText>(Resource.Id.username_create);
            password = FindViewById<EditText>(Resource.Id.password_create);


            create_account.Click += delegate
            {
                SubmitNewUser();
            };
        }

        private void DisplayMissingInfoError(){
            Android.App.AlertDialog.Builder diaglog = new AlertDialog.Builder(this);
            AlertDialog alert = diaglog.Create();
            alert.SetTitle("Error");
            alert.SetMessage("Please be sure to fill out all required information.");
            alert.SetButton("Okay", (c, ev) =>{
                alert.Dismiss();
            });
            alert.Show();

        }

        private int CheckUsername(string new_username, MySqlConnection c){
            List<string> names = new List<string>();
            var command = c.CreateCommand();
            command.CommandText = ("SELECT username FROM users where username=@username;");
            command.Parameters.AddWithValue("@username", new_username);
            MySqlDataReader username_reader = command.ExecuteReader();
            while (username_reader.Read())
            {
                names.Add(username_reader.GetString("username"));
            }
            username_reader.Close();
            return names.Count();
        }

        private void SubmitNewUser(){
            if (string.IsNullOrWhiteSpace(first.Text) || string.IsNullOrWhiteSpace(last.Text)
                   || string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text))
            {
                DisplayMissingInfoError();
            }

            else
            {
                create_account.Enabled = false;
                MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None; Allow User Variables=True");
                connection.Open();
                var matches = CheckUsername(username.Text, connection);
                if (matches > 0)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Invalid username");
                    alert.SetMessage("That username is already in use, please select a different one.");
                    alert.SetButton("Okay", (c, ev) =>
                    {
                        alert.Dismiss();
                        create_account.Enabled = true;
                    });
                    alert.Show();
                }
                else{
                    MySqlCommand insert_new_user = connection.CreateCommand();
                    insert_new_user.CommandText = ("INSERT into users(first_name, last_name, username, password)values(@first_text, @last_text, @username_text, @password_text);");
                    insert_new_user.Parameters.AddWithValue("@first_text", first.Text);
                    insert_new_user.Parameters.AddWithValue("@last_text", last.Text);
                    insert_new_user.Parameters.AddWithValue("@username_text", username.Text);
                    insert_new_user.Parameters.AddWithValue("@password_text", password.Text);
                    insert_new_user.ExecuteNonQuery();
                    connection.Close();
                    Android.App.AlertDialog.Builder diaglog = new AlertDialog.Builder(this);
                    AlertDialog alert = diaglog.Create();
                    alert.SetTitle("Success");
                    alert.SetMessage("Your account has been created!");
                    alert.SetButton("Okay", (c, ev) =>
                    {
                        Intent intent = new Intent(this, typeof(MainActivity));
                        this.StartActivity(intent);
                        alert.Dismiss();

                    });
                    alert.Show();
                }
            }
        }
    }
}
