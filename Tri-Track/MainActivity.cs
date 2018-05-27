using Android.App;
using Android.Widget;
using Android.OS;
using MySqlConnector;
using MySql.Data.MySqlClient;
using Android.Content;
using System.Collections.Generic;

namespace TriTrack
{
    [Activity(Label = "Tri-Track", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        string username;
        string password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None");
            //connection.Open();

            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            FindViewById<TextView>(Resource.Id.username_field).Text = TriTrack.Utils.Settings.LastUsername;
            FindViewById<TextView>(Resource.Id.password_field).Text = TriTrack.Utils.Settings.LastPassword;
            ImageView logo = FindViewById<ImageView>(Resource.Id.tri_track_logo);
            logo.SetImageResource(Resource.Drawable.tri_track);
            // Get our button from the layout resource,
            // and attach an event to it
            Button sign_in_button = FindViewById<Button>(Resource.Id.sign_in_button);
            Button create_account_button = FindViewById<Button>(Resource.Id.create_account_button);

            sign_in_button.Click += delegate { 
                
                sign_in_button.Text = CheckUsernameLogin().ToString(); };
                TriTrack.Utils.Settings.LastUsername = username;
                TriTrack.Utils.Settings.LastPassword = password;

            create_account_button.Click += delegate
            {
                Intent intent = new Intent(this, typeof(UserCreationActivity));
                this.StartActivity(intent);
            };

        }

        public bool CheckUsernameLogin(){
            MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None");
            connection.Open();
            username = FindViewById <TextView>(Resource.Id.username_field).Text;
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
            if (names.Count != 1){
                Android.App.AlertDialog.Builder diaglog = new AlertDialog.Builder(this);
                AlertDialog alert = diaglog.Create();
                alert.SetTitle("Error");
                alert.SetMessage("Invalid username or password.");
                alert.SetButton("Okay", (c, ev) => {
                    alert.Dismiss();
                });
                alert.Show();
                return false;
            }
            else{
                command.CommandText = ("SELECT password FROM users where username=@username;");
                command.Parameters.AddWithValue("@username", username);
                MySqlDataReader password_reader = command.ExecuteReader();
                password_reader.Read();
                user_password = password_reader.GetString(0);
                if(user_password == password){
                    Intent intent = new Intent(this, typeof(MapsActivity));
                    this.StartActivity(intent);
                    return true;
                }
                else{
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
            return false;
        }

        public string CheckPasswordLogin(string username){
            
            return "secret";
        }

    }
}


