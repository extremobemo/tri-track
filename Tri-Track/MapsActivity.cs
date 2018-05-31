using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace TriTrack
{
    [Activity(Label = "MapsActivity")]
    public class MapsActivity : Activity, IOnMapReadyCallback
    {
        GoogleMap daMap;
        Position position;
        IGeolocator locator = CrossGeolocator.Current;
        double lat;
        double _long;
        TextView latlonglist;
        public PolylineOptions polyline = new PolylineOptions().InvokeWidth(20).InvokeColor(Color.Red.ToArgb());
        Intent startServiceIntent;

        MarkerOptions start = new MarkerOptions();
        MarkerOptions finish = new MarkerOptions();
        bool WorkoutInProgress = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            startServiceIntent = new Intent(this, typeof(TriTrackService));
            SetContentView(Resource.Layout.Map);
            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.the_fucking_map);
            mapFragment.GetMapAsync(this);
            Button switchB = FindViewById<Button>(Resource.Id.switch_button);
            latlonglist = FindViewById<TextView>(Resource.Id.LATLONG);
            getPos();

            switchB.Click += delegate
            {
                if(WorkoutInProgress == false){
                    //StartService(startServiceIntent);
                    WorkoutInProgress = true;
                    start.SetPosition(new LatLng(position.Latitude, position.Longitude));
                    start.SetTitle("Start");
                    daMap.AddMarker(start);
                    polyline.Add(new LatLng(position.Latitude, position.Longitude));
                    StartListening();
                    LatLng latlng = new LatLng(position.Latitude, position.Longitude);  
                    CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);  
                    daMap.MoveCamera(camera);
                    switchB.Text = "WORKOUT IN PROGRESS";
                }
                else{
                    WorkoutInProgress = false;
                    finish.SetPosition(new LatLng(position.Latitude, position.Longitude));
                    finish.SetTitle("Finish");
                    daMap.AddMarker(finish);
                    StopListening();
                    Android.App.AlertDialog.Builder diaglog = new AlertDialog.Builder(this);
                    AlertDialog alert = diaglog.Create();
                    alert.SetTitle("Good Work");
                    alert.SetMessage("Your workout is complete, would you like to record it?");
                    alert.SetButton("Yes", (c, ev) => {
                        alert.Dismiss(); //TODO: save polyine data to new table in the database.
                    });
                    alert.SetButton2("No", (c, ev) =>{
                        alert.Dismiss(); 
                    });
                    alert.Show();

                }
                 


            };

        }

        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            position = e.Position;
            DrawMarker();
            //FindViewById<Button>(Resource.Id.switch_button).Text = position.Latitude.ToString();
        }


        public void DrawMarker(){
            lat = position.Latitude;
            _long = position.Longitude;
            polyline.Add(new LatLng(lat, _long));
            //polyline.Add(new LatLng(40.739487, -96.65715119999999)); //THIS IS FOR DEBUGGING
            daMap.AddPolyline(polyline);
        }


        public void OnMapReady(GoogleMap googleMap)
        { 
            daMap = googleMap;
            daMap.MapType = GoogleMap.MapTypeHybrid;
            getPos();
        }
        public async void getPos(){
            position = await locator.GetPositionAsync();

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


    }
}
