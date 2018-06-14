using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using MySql.Data.MySqlClient;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace TriTrack
{
    [Activity(Label = "Tri-Track", Theme = "@style/Theme")]
    public class MapsActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private SupportToolbar daToolbar;
        private MyActionBarDrawerToggle daDrawerToggle;
        private DrawerLayout daDrawerLayout;
        private ListView daLeftDrawer;
        private string workoutMode;
        GoogleMap daMap;
        Position position;
        IGeolocator locator = CrossGeolocator.Current;
        double lat;
        double _long;
        Position last_position;
        Timer timer;
        //TextView latlonglist;
        TextView distanceText;
        public PolylineOptions polyline; 
        Intent startServiceIntent;
        int sec;
        int min;
        int hour;
        double distance = 0;
        TextView TimerText;
        MarkerOptions start = new MarkerOptions();
        MarkerOptions finish = new MarkerOptions();
        bool WorkoutInProgress = false;
        int user_id;
        Button switchB;
        Switch WorkOutMode;
        private List<string> drawerOptions;
        private ArrayAdapter drawerOptionsAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            startServiceIntent = new Intent(this, typeof(TriTrackService));
            user_id = Intent.GetIntExtra("user_id", 0);
            SetContentView(Resource.Layout.Map);
            daToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(daToolbar);
            daLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);


            daDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            drawerOptions = new List<string>();
            drawerOptions.Add("Home");
            drawerOptions.Add("History");
            drawerOptions.Add("Log Out");
            drawerOptionsAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, drawerOptions);
            daLeftDrawer.Adapter = drawerOptionsAdapter;
            daLeftDrawer.ItemClick += DaLeftDrawer_ItemClick;

            daDrawerToggle = new MyActionBarDrawerToggle(
                this,
                daDrawerLayout,
                Resource.String.openDrawer,
                Resource.String.closeDrawer);
            SetSupportActionBar(daToolbar);
            daDrawerLayout.AddDrawerListener(daDrawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            daDrawerToggle.SyncState();
            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.the_fucking_map);
            mapFragment.GetMapAsync(this);
            switchB = FindViewById<Button>(Resource.Id.switch_button);
            switchB.Enabled = false;
            distanceText = FindViewById<TextView>(Resource.Id.distance);
            WorkOutMode = FindViewById<Switch>(Resource.Id.type);
            //latlonglist = FindViewById<TextView>(Resource.Id.LATLONG);
            getPos();
            switchB.Click += delegate
            {
                if(WorkoutInProgress == false){
                    if(WorkOutMode.Checked == false){
                        workoutMode = "BIKE";
                    }
                    else{
                        workoutMode = "RUN";
                    }
                    getPos();
                    switchB.SetBackgroundColor(Android.Graphics.Color.Red);
                    daMap.Clear();
                    polyline = new PolylineOptions().InvokeWidth(20).InvokeColor(Color.Red.ToArgb());
                    sec = 0;
                    min = 0;
                    hour = 0;
                    distance = 0;
                    timer = new Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += Timer_Elapsed;
                    timer.Start();
                    TimerText = FindViewById<TextView>(Resource.Id.timer_text);
                    TimerText.Text = ("0:00:00");
                    WorkoutInProgress = true;
                    start.SetPosition(new LatLng(position.Latitude, position.Longitude));
                    start.SetTitle("Start");
                    daMap.AddMarker(start);
                    polyline.Add(new LatLng(position.Latitude, position.Longitude));
                    StartListening();
                    //LatLng latlng = new LatLng(position.Latitude, position.Longitude);  
                    //CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);  
                    //daMap.MoveCamera(camera);
                    switchB.Text = "FINISH WORKOUT";
                }
                else if(WorkoutInProgress == true){
                    // THIS GETS A STRING OF THE POLYLINE
                    //MAYBE STORE THIS A BLOB IN THE SQL DATABASE String.Join(":", polyline.Points);
                    switchB.SetBackgroundColor(Android.Graphics.Color.ParseColor("#219653"));
                    switchB.Enabled = false;
                    switchB.Text = "START NEW WORKOUT";
                    WorkoutInProgress = false;
                    timer.Stop();
                    finish.SetPosition(new LatLng(position.Latitude, position.Longitude));
                    finish.SetTitle("Finish");
                    daMap.AddMarker(finish);
                    StopListening();
                    Android.App.AlertDialog.Builder diaglog = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = diaglog.Create();
                    alert.SetCanceledOnTouchOutside(false);
                    alert.SetCancelable(false);
                    alert.SetTitle("Good Work");
                    alert.SetMessage("Your workout is complete, would you like to record it?");
                    alert.SetButton("Yes", (c, ev) => {
                        switchB.Enabled = true;
                        SubmitWorkoutToDatabase();
                        switchB.Text = "START NEW WORKOUT"; //TODO: SEND WORKOUT INFO TO THE DATABASE!
                        alert.Dismiss(); //TODO: save polyine data to new table in the database.
                    });
                    alert.SetButton2("No", (c, ev) =>{
                        switchB.Enabled = true;
                        daMap.Clear();
                        alert.Dismiss();
                        sec = 0;
                        min = 0;
                        hour = 0;
                        distance = 0;
                        TimerText.Text = ("0:00:00");
                        switchB.Text = "START NEW WORKOUT";
                    });
                    alert.Show();
                }
            };
        }

        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            last_position = position;
            position = e.Position;
            DrawMarker();
            //FindViewById<Button>(Resource.Id.switch_button).Text = position.Latitude.ToString();
        }
        public void DrawMarker(){
            lat = position.Latitude;
            _long = position.Longitude;
            distance += Calculate_Distance();
            distanceText.Text = string.Format("DISTANCE: {0}", Math.Round(distance,2));
            polyline.Add(new LatLng(lat, _long));
            //polyline.Add(new LatLng(40.739487, -96.65715119999999)); //THIS IS FOR DEBUGGING
            daMap.AddPolyline(polyline);
        }

        private double Calculate_Distance()
        {
            return last_position.CalculateDistance(position);

        }

        public void OnMapReady(GoogleMap googleMap)
        { 
            daMap = googleMap;
            daMap.MapType = GoogleMap.MapTypeHybrid;
            getPos();
        }
        public async void getPos(){
            position = await locator.GetPositionAsync();
            LatLng latlng = new LatLng(position.Latitude, position.Longitude);  
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 17);  
            daMap.MoveCamera(camera);
            if(switchB.Text != "FINISH WORKOUT"){
                switchB.Text = "START NEW WORKOUT";
                switchB.Enabled = true;
            }



        }
        public async void StartListening(){
            await locator.StartListeningAsync(new TimeSpan(0, 0, 0, 3), 0.0, true, new Plugin.Geolocator.Abstractions.ListenerSettings
            {
                ActivityType = Plugin.Geolocator.Abstractions.ActivityType.Fitness,
                AllowBackgroundUpdates = true
            });
            locator.PositionChanged += Locator_PositionChanged;
        }

        public async void StopListening(){
            await locator.StopListeningAsync();
        }
        protected override void OnPause()
        {
            base.OnPause();
            if(WorkoutInProgress){
                StopListening();
                StartService(startServiceIntent);
            }

        }
        protected override void OnResume()
        {
            base.OnResume();
            if(WorkoutInProgress){
                StopService(startServiceIntent);
            }
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            sec++;
            if(sec > 59){
                min++;
                sec=0;
            }
            if(min > 59){
                hour++;
                min = 0;
            }

            RunOnUiThread(() =>
            {
                TimerText.Text = string.Format("{0}:{1:00}:{2:00}",hour, min, sec);
            });
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            daDrawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            return;
        }

        void DaLeftDrawer_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if(drawerOptions[e.Position].ToString() == "Log Out"){
                //WorkoutInProgress = false;
                Intent intent = new Intent(this, typeof(MainActivity));
                this.StartActivity(intent);
            }

            if(drawerOptions[e.Position].ToString() == "History"){
                daDrawerLayout.CloseDrawers();
                OpenHistory();
            }
        }
        public Task<bool> SubmitWorkout()
        {
            return Task.Run(() =>
            {
                MySqlConnection connection = new MySqlConnection("server=extremobemotestserver.mysql.database.azure.com;port=3306;database=test;user id=extremobemo@extremobemotestserver;password=Morris98;SslMode=None");
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = ("INSERT INTO workouts (polyline, user_id, distance, time, type) VALUES (@polyline, @user_id, @distance, @time, @type);");
                command.Parameters.AddWithValue("@polyline", String.Join(";", polyline.Points));
                command.Parameters.AddWithValue("@user_id", user_id);
                command.Parameters.AddWithValue("@distance", string.Format("DISTANCE: {0}", Math.Round(distance, 2)));
                command.Parameters.AddWithValue("@time", string.Format("{0}:{1:00}:{2:00}", hour, min, sec));
                command.Parameters.AddWithValue("type", workoutMode);
                command.ExecuteNonQuery();
                connection.Close();
                return true;
            });
                                }
       

        public async void SubmitWorkoutToDatabase()
        {
            bool x = await SubmitWorkout();
        }

        public Task<bool> OpenHistoryTask()
        {
            return Task.Run(() =>
            {
                Intent intent = new Intent(this, typeof(HistoryActivity));
                intent.PutExtra("user_id", user_id);
                this.StartActivity(intent);
                return true;
            });
        }
         
        public async void OpenHistory(){
            bool x = await OpenHistoryTask();
        }

    }
}
