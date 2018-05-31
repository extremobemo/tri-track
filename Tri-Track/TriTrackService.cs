using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading;
using System.Drawing;
using Android.Gms.Maps.Model;
using Plugin.Geolocator.Abstractions;
using Plugin.Geolocator;
using Android.Support.V4.Content;
using System.IO;

namespace TriTrack
{

    [Service]
    public class TriTrackService : Service
    {
        //Action runnable;
        Handler handler;
        double lat;
        double _long;
        Position position;
        IGeolocator locator = CrossGeolocator.Current;
        public PolylineOptions polyline = new PolylineOptions().InvokeWidth(20).InvokeColor(Color.Red.ToArgb());
        //Notification notification;

        public override void OnCreate()
        {
            base.OnCreate();
            handler = new Handler();
            getPos();
            StartListening();

        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
                   
            RegisterForegroundService();   
            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }

    void RegisterForegroundService()
        {
            //check if file exists, delete it if it does. then recreate it to be freshly written to. 
            //var pathToNewFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/TriTrack";
            //Directory.CreateDirectory(pathToNewFolder);
            var notification = new Notification.Builder(this).SetColor(Color.Green.ToArgb()).Build();
            // Enlist this instance of the service as a foreground service
            StartForeground(10000, notification);
        }


        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            position = e.Position;
            //var pathToNewFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/TriTrack";
            //using (var writer = new StreamWriter(Path.Combine(pathToNewFile, "cached.txt"), true))
            //{
                //writer.WriteLine(position.Latitude.ToString() + ',' + position.Longitude.ToString());
            //}
        }


        public void UpdatePolyline()
        {
            lat = position.Latitude;
            _long = position.Longitude;
            polyline.Add(new LatLng(lat, _long));
        }


        public async void getPos()
        {
            position = await locator.GetPositionAsync();

        }
        public async void StartListening()
        {
            await locator.StartListeningAsync(new TimeSpan(0, 0, 0, 3), 0.0, true, new Plugin.Geolocator.Abstractions.ListenerSettings
            {
                ActivityType = Plugin.Geolocator.Abstractions.ActivityType.Fitness,
                AllowBackgroundUpdates = true
            });
            locator.PositionChanged += Locator_PositionChanged;
        }

        public async void StopListening()
        {
            await locator.StopListeningAsync();
        }
    }
}
