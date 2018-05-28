
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);
            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.the_fucking_map);
            mapFragment.GetMapAsync(this);
            Button switchB = FindViewById<Button>(Resource.Id.switch_button);
            switchB.Click += delegate
            {
                

                double lat = position.Latitude;
                double _long = position.Longitude;
                MarkerOptions markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(new LatLng(lat, _long));
                markerOpt1.SetTitle("me rn");
                daMap.AddMarker(markerOpt1);
                LatLng latlng = new LatLng(lat, _long);  
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);  
                daMap.MoveCamera(camera);  
                //daMap.MoveCamera(CameraUpdateFactory.NewLatLng(new LatLng(lat, _long)));
                FindViewById<Button>(Resource.Id.switch_button).Text = position.Longitude.ToString();
            };
        }


        public void OnMapReady(GoogleMap googleMap)
        {
            
            daMap = googleMap;
            daMap.MapType = GoogleMap.MapTypeHybrid;
            getPos();



        }
        public async void getPos(){
            var locator = CrossGeolocator.Current;
            position = await locator.GetPositionAsync();
        }

        public void OnLocationChange(Location location)
        {


        }

        private void UpdateLocation(Location location)
        {
            //
            if (location != null)
            {
                //
                var lat = location.Latitude;
                var _long = location.Longitude;
            }
        }
    }
}