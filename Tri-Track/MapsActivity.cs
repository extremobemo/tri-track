
using System;
using System.Collections.Generic;
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
        PolylineOptions polyline = new PolylineOptions();
        MarkerOptions start = new MarkerOptions();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);
            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.the_fucking_map);
            mapFragment.GetMapAsync(this);
            Button switchB = FindViewById<Button>(Resource.Id.switch_button);
            latlonglist = FindViewById<TextView>(Resource.Id.LATLONG);
            getPos();

            switchB.Click += delegate
            {
                start.SetPosition(new LatLng(position.Latitude, position.Longitude));
                start.SetTitle("Start");
                daMap.AddMarker(start);
                polyline.Add(new LatLng(position.Latitude, position.Longitude));
                StartListening();
                LatLng latlng = new LatLng(position.Latitude, position.Longitude);  
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);  
                daMap.MoveCamera(camera);  
                //daMap.MoveCamera(CameraUpdateFactory.NewLatLng(new LatLng(lat, _long)));

            };

        }

        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            position = e.Position;
            string latstring = position.Latitude.ToString();
            string longstring = position.Latitude.ToString();
            latlonglist.Text += (latstring + "," + longstring + "\n");
            DrawMarker();
            FindViewById<Button>(Resource.Id.switch_button).Text = position.Latitude.ToString();
        }


        public void DrawMarker(){
            lat = position.Latitude;
            _long = position.Longitude;
            polyline.Add(new LatLng(lat, _long));
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
            await locator.StartListeningAsync(new TimeSpan(0, 0, 0, 3), 0.0, true);
            locator.PositionChanged += Locator_PositionChanged;
        }
     
        }
    }
