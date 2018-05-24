using Android.App;
using Android.Widget;
using Android.OS;
using MySqlConnector;
using MySql.Data.MySqlClient;
using Android.Content;

namespace TriTrack
{
    [Activity(Label = "Tri-Track", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None");
            //connection.Open();
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button sign_in_button = FindViewById<Button>(Resource.Id.sign_in_button);
            Button create_account_button = FindViewById<Button>(Resource.Id.create_account_button);
            sign_in_button.Click += delegate { sign_in_button.Text = $"{count++} clicks!"; };
            create_account_button.Click += delegate
            {
                Intent intent = new Intent(this, typeof(UserCreationActivity));
                this.StartActivity(intent);
            };



        }
    }
}


