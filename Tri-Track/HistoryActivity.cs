
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

namespace TriTrack
{
    [Activity(Label = "HistoryActivity")]
    public class HistoryActivity : Activity
    {
        private List<string> hItems;
        private ListView hListView;
        int user_id;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.History);
            user_id = Intent.GetIntExtra("user_id", 0);
            hListView = FindViewById<ListView>(Resource.Id.historyListView);
            hItems = new List<string>();
            MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None; Allow User Variables=True");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = ("SELECT * FROM workouts where user_id=@user_id;");
            command.Parameters.AddWithValue("@user_id", user_id);
            MySqlDataReader username_reader = command.ExecuteReader();
            while (username_reader.Read())
            {
                hItems.Add(username_reader.GetString("distance"));
            }
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, hItems);
            hListView.Adapter = adapter;
            //MyListViewAdapter adapter = new MyListViewAdapter(this, hItems);

            // Create your application here
        }
    }
}
