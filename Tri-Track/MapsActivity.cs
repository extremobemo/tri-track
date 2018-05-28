
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TriTrack
{
    [Activity(Label = "MapsActivity")]
    public class MapsActivity : Activity, IOnMapReadyCallback
    {
        GoogleMap daMap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);
            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.the_fucking_map);
            mapFragment.GetMapAsync(this);
            Button switchB = FindViewById<Button>(Resource.Id.switch_button);
            switchB.Click += delegate {

                daMap.MapType = GoogleMap.MapTypeNormal;
            };
        }

      
        public void OnMapReady(GoogleMap googleMap)
        {
            daMap = googleMap;
            daMap.MapType = GoogleMap.MapTypeHybrid;

        }
    }
}
