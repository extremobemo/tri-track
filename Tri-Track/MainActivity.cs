using Android.App;
using Android.Widget;
using Android.OS;
using MySqlConnector;
using MySql.Data.MySqlClient;

namespace TriTrack
{
    [Activity(Label = "Tri-Track", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        //int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None");
            connection.Open();
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.button1);

            //button.Click += delegate { button.Text = $"{count++} clicks!"; };
        }
    }
}

