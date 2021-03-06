﻿using Android.App;
using Android.Widget;
using Android.OS;
using MySqlConnector;
using MySql.Data.MySqlClient;
using Android.Content;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TriTrack
{
    [Activity(Label = "Tri-Track", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        string username;
        string password;
        int user_id;
        Button sign_in_button;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None");
            //connection.Open();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            FindViewById<TextView>(Resource.Id.username_field).Text = TriTrack.Utils.Settings.LastUsername;
            FindViewById<TextView>(Resource.Id.password_field).Text = TriTrack.Utils.Settings.LastPassword;
            ImageView logo = FindViewById<ImageView>(Resource.Id.tri_track_logo);
            logo.SetImageResource(Resource.Drawable.tri_track);
            sign_in_button = FindViewById<Button>(Resource.Id.sign_in_button);
            Button create_account_button = FindViewById<Button>(Resource.Id.create_account_button);
            sign_in_button.Click += delegate
            {
                SignInButtonClick();
                FindViewById<ProgressBar>(Resource.Id.login_loading).Visibility = Android.Views.ViewStates.Visible;

            };

                create_account_button.Click += delegate
                {
                    Intent intent = new Intent(this, typeof(UserCreationActivity));
                    this.StartActivity(intent);
                };

         }
 
        public async void SignInButtonClick(){
            bool x = await CheckUsernameLogin();
            if(x == false){
                ShowAlert();
                FindViewById<ProgressBar>(Resource.Id.login_loading).Visibility = Android.Views.ViewStates.Invisible;
            }
        }

        public Task<bool> CheckUsernameLogin()
        {
            return Task.Run(() =>
            {
                MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None");
                connection.Open();
                username = FindViewById<TextView>(Resource.Id.username_field).Text;
                TriTrack.Utils.Settings.LastUsername = FindViewById<TextView>(Resource.Id.username_field).Text;
                password = FindViewById<TextView>(Resource.Id.password_field).Text;
                TriTrack.Utils.Settings.LastPassword = FindViewById<TextView>(Resource.Id.password_field).Text;
                string user_password;
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
                if (names.Count != 1)
                {
                    return false;
                }
                else
                {
                    command.CommandText = ("SELECT password FROM users where username=@username;");
                    command.Parameters.AddWithValue("@username", username);
                    MySqlDataReader password_reader = command.ExecuteReader();
                    password_reader.Read();
                    user_password = password_reader.GetString(0);
                    if (user_password == password)
                    {
                        password_reader.Close();
                        command.CommandText = ("SELECT user_id FROM users where username=@username;");
                        command.Parameters.AddWithValue("@username", username);
                        MySqlDataReader user_id_reader = command.ExecuteReader();
                        user_id_reader.Read();
                        user_id = user_id_reader.GetInt32(0);
                        Intent intent = new Intent(this, typeof(MapsActivity));
                        intent.PutExtra("user_id", user_id);
                        this.StartActivity(intent);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            });
     }

        public void ShowAlert(){
            Android.App.AlertDialog.Builder diaglog = new AlertDialog.Builder(this);
            AlertDialog alert = diaglog.Create();
            alert.SetTitle("Error");
            alert.SetMessage("Incorrect username or password.");
            alert.SetButton("Okay", (c, ev) => {
                alert.Dismiss();
            });
            alert.Show();
        }

    }
}


