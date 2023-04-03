using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace AcsWindowsSDKSamples.Samples
{
    internal class Devices : Sample
    {
        async void MonitorDeviceAsync()
        {
            var client = await GetCallClientAsync();

            var deviceManager = await client.GetDeviceManagerAsync();
            // Set up event sinks for device manager
            deviceManager.MicrophonesUpdated += DeviceManager_OnMicrophonesUpdated;
            deviceManager.CamerasUpdated += DeviceManager_OnCamerasUpdated;
            deviceManager.SpeakersUpdated += DeviceManager_OnSpeakersUpdated;
            // We can fetch various media devices and query for their states
            var microphone = deviceManager.Microphone;
            var camera = deviceManager.Cameras[3];
            var speaker = deviceManager.Speakers[2];

            // Configure audio preferences
            var audioOptions = new AudioOptions() {  IsMuted = false };
            // Configure video preference
            var videoOptions = new VideoOptions(new VideoOptions( new[] { await GetOutgoingVideoStreamAsync() }));

            // Start the call and get ready to resposne to device callbacks
            var callAgent = await GetCallAgentAsync();
            var call = await callAgent.StartCallAsync(
                new [] { new UserCallIdentifier("eyxxxxx") },
                new StartCallOptions()
                {
                    AudioOptions = audioOptions,
                    VideoOptions = videoOptions
                });
        }

        private void DeviceManager_OnSpeakersUpdated(object sender, AudioDevicesUpdatedEventArgs args)
        {
            Console.WriteLine($"{args.AddedAudioDevices.Count}-{args.RemovedAudioDevices.Count}");
        }

        private void DeviceManager_OnCamerasUpdated(object sender, VideoDevicesUpdatedEventArgs args)
        {
            Console.WriteLine($"{args.AddedVideoDevices.Count}-{args.RemovedVideoDevices.Count}");
        }

        private void DeviceManager_OnMicrophonesUpdated(object sender, AudioDevicesUpdatedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
