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
        Handler handler;
        double lat;
        double _long;
        Position position;
        IGeolocator locator = CrossGeolocator.Current;
        public PolylineOptions polyline = new PolylineOptions().InvokeWidth(20).InvokeColor(Color.Red.ToArgb());

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
            return StartCommandResult.Sticky;
        }

    void RegisterForegroundService()
        {
            var notification = new Notification.Builder(this).SetColor(Color.Green.ToArgb()).Build();
            StartForeground(10000, notification);
        }


        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            position = e.Position;
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
