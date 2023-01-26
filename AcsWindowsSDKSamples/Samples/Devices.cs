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

            var deviceManager = await client.GetDeviceManager();
            // Set up event sinks for device manager
            deviceManager.OnMicrophonesUpdated += DeviceManager_OnMicrophonesUpdated;
            deviceManager.OnCamerasUpdated += DeviceManager_OnCamerasUpdated;
            deviceManager.OnSpeakersUpdated += DeviceManager_OnSpeakersUpdated;
            // We can fetch various media devices and query for their states
            var microphone = deviceManager.Microphone;
            var camera = deviceManager.Cameras[3];
            var speaker = deviceManager.Speakers[2];

            // Setup audio preferences
            var audioOptions = new AudioOptions()
            {
                IncomingAudioStream = await GetIncomingAudioStreamAsync(),
                OutgoingAudioStream = await GetOutgoingAudioStreamAsync(),
                Muted = false,
                SpeakerMuted = false
            };
            // Setup video preferences
            var videoOptions = new VideoOptions(new[] { await GetOutgoingVideoStreamAsync() })
            {
                IncomingVideoOptions = new IncomingVideoOptions()
                {
                    AllowVideoFrameTextures = true,
                    ReceiveRawIncomingVideoStreams = true,
                }
            };

            // Start the call and get ready to resposne to device callbacks
            var callAgent = await GetCallAgentAsync();
            var call = await callAgent.StartCallAsync(
                new [] { new CommunicationUserIdentifier("eyxxxxx") },
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
