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
            deviceManager.OnMicrophonesUpdated += DeviceManager_OnMicrophonesUpdated;
            deviceManager.OnCamerasUpdated += DeviceManager_OnCamerasUpdated;
            deviceManager.OnSpeakersUpdated += DeviceManager_OnSpeakersUpdated;

            var microphone = deviceManager.Microphone;
            var camera = deviceManager.Cameras[3];
            var speaker = deviceManager.Speakers[2];

            var callAgent = await GetCallAgentAsync();

            var callees = new List<CommunicationIdentifier>() { new CommunicationUserIdentifier("eyxxxxx") };

            var audioOptions = new AudioOptions()
            {
                IncomingAudioStream = await GetIncomingAudioStreamAsync(),
                OutgoingAudioStream = await GetOutgoingAudioStreamAsync(),
                Muted = false,
                SpeakerMuted = false
            };

            var videoOptions = new VideoOptions(new[] { new OutgoingVideoStream() })
            {
                IncomingVideoOptions = new IncomingVideoOptions()
                {
                    AllowVideoFrameTextures = true,
                    ReceiveRawIncomingVideoStreams = true,
                }
            };

            var startCallOptions = new StartCallOptions()
            {
                AudioOptions = audioOptions,
                VideoOptions = videoOptions
            };

            var call = await callAgent.StartCallAsync(callees, startCallOptions);
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
