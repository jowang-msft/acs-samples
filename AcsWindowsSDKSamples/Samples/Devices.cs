using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcsWindowsSDKSamples.Samples
{
    internal class Devices : Sample
    {
        async void MonitorDeviceAsync()
        {
            var client = await GetCallClientAsync();

            var deviceManager = await client.GetDeviceManagerAsync();

            // Get a list of speakers
            IReadOnlyList<AudioDeviceDetails> speakers = deviceManager.Speakers;
            // Get a list of cameras
            IReadOnlyList<VideoDeviceDetails> cameras = deviceManager.Cameras;
            // Get a list of microphones
            IReadOnlyList<AudioDeviceDetails> microphonse = deviceManager.Microphones;

            // Set up event sinks for device manager
            deviceManager.MicrophonesUpdated += OnMicrophonesUpdated;
            deviceManager.SpeakersUpdated += OnSpeakersUpdated;
            deviceManager.CamerasUpdated += OnCamerasUpdated;

            // Get active devices
            var microphone = deviceManager.Microphone;
            var camera = cameras.First();
            Console.WriteLine($"{camera.Name}, {camera.CameraFacing.HasFlag(CameraFacing.Front)}");

            // Switch active mic and speaker
            deviceManager.SetMicrophone(microphonse.Last());
            deviceManager.SetSpeaker(speakers.First());
        }

        private async void OnMicrophonesUpdated(object sender, AudioDevicesUpdatedEventArgs args)
        {
            Console.WriteLine($"{args.AddedAudioDevices.Count}-{args.AddedAudioDevices.Count}");
        }

        private async void OnSpeakersUpdated(object sender, AudioDevicesUpdatedEventArgs args)
        {
            Console.WriteLine($"{args.AddedAudioDevices.Count}-{args.RemovedAudioDevices.Count}");
        }

        private async void OnCamerasUpdated(object sender, VideoDevicesUpdatedEventArgs args)
        {
            Console.WriteLine($"{args.AddedVideoDevices.Count}-{args.RemovedVideoDevices.Count}");
        }
    }
}
