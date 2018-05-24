
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
using Android.Content;

namespace TriTrack
{
    [Activity(Label = "Account Creation")]
    public class UserCreationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            SetContentView(Resource.Layout.UserCreation);
            Button create_account = FindViewById<Button>(Resource.Id.confirm_account);
           // Coming back to this later... TextView first = FindViewById<Button>(Resource.Id.first);
            create_account.Click += delegate
            {
                MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None");
                connection.Open();
                create_account.Text = "HELLOO";
            };
        }
    }
}
