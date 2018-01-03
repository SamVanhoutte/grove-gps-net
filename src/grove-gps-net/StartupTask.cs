using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using NmeaParser.Nmea.Gps;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace grove_gps_net
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static BackgroundTaskDeferral _deferral = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                _deferral = taskInstance.GetDeferral();


                string aqsFilter = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqsFilter);

                foreach (DeviceInformation t in dis)
                    System.Diagnostics.Debug.WriteLine($"UART Port: {t.Name}, {t.Id}, {t.Kind}");

                var serialPort = await SerialDevice.FromIdAsync(dis[0].Id);
                serialPort.DataBits = 8;
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(0);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(0);
                serialPort.BaudRate = 9600;

                var device = new NmeaParser.SerialPortDevice(serialPort);
                device.OpenAsync().Wait();
                device.MessageReceived += Device_MessageReceived;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void Device_MessageReceived(object sender, NmeaParser.NmeaMessageReceivedEventArgs e)
        {
            try
            {
                switch (e.Message.MessageType)
                {
                    case "GPGSA": // Overall satellite data
                        var gsaMessage = (Gpgsa)e.Message;
                        Debug.WriteLine($"Sat data.  Mode: {gsaMessage.GpsMode}. Count: {gsaMessage.SVs.Count}");
                        break;
                    case "GPGGA": // Fix information
                        var ggaMessage = (Gpgga)e.Message;
                        Debug.WriteLine($"Fix info.  Lat: {ggaMessage.Latitude}. Lon: {ggaMessage.Longitude}. Height: {ggaMessage.Altitude} {ggaMessage.AltitudeUnits}");
                        break;
                    case "GPRMC": // Recommended minimum data for gps
                        var rmcMessage = (Gprmc)e.Message;
                        Debug.WriteLine($"Fix info.  Lat: {rmcMessage.Latitude}. Lon: {rmcMessage.Longitude}. Course: {rmcMessage.Course}. Speed: {rmcMessage.Speed}");
                        break;
                    case "GPGSV": // Detailed Satellite data
                        var gsvMessage = (Gpgsv)e.Message;
                        Debug.WriteLine($"Fix info.  Sats in view: {gsvMessage.SVsInView }. Msgcount: {gsvMessage.TotalMessages}.");
                        break;
                    default:
                        Debug.WriteLine($"Other message received: {e.Message.MessageType} - {e.Message}");
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

        }
    }
}
