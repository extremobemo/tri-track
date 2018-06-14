
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
using Android.Support.V4.Widget;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;

namespace TriTrack
{
    [Activity(Label = "Tri-Track", Theme = "@style/Theme")]
    public class HistoryActivity : AppCompatActivity
    {

        private SupportToolbar daToolbar;
        private List<string> hItems;
        private ListView hListView;
        int user_id;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.History);
            daToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(daToolbar);
            user_id = Intent.GetIntExtra("user_id", 0);
            hListView = FindViewById<ListView>(Resource.Id.historyListView);
            hItems = new List<string>();
            LoadHistory();

            //MyListViewAdapter adapter = new MyListViewAdapter(this, hItems);

            // Create your application here
        }


        public Task<bool> LoadHistoryTask()
        {
            return Task.Run(() =>
            {
                MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None; Allow User Variables=True");
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = ("SELECT * FROM workouts where user_id=@user_id;");
                command.Parameters.AddWithValue("@user_id", user_id);
                MySqlDataReader username_reader = command.ExecuteReader();
                RunOnUiThread(() =>
                { 
                    while (username_reader.Read())
                    {
                        hItems.Add(username_reader.GetString("type") + ", " + username_reader.GetString("distance"));
                    }
                    ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, hItems);
                    hListView.Adapter = adapter;
                });

                return true;
            });
        }

        public async void LoadHistory(){
            bool x = await LoadHistoryTask();
        }
    }
}
