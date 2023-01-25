using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcsWindowsSDKSamples.Samples
{
    internal class CallAgentSample : Sample
    {
        async void CallAgentAsync()
        {
            var callAgent = await GetCallAgentAsync();

            callAgent.OnCallsUpdated += CallAgent_OnCallsUpdated;
            callAgent.OnIncomingCall += CallAgent_OnIncomingCall;

            var callees = new List<CommunicationIdentifier>() { new MicrosoftTeamsUserIdentifier("Constants.TEAMS_PUBLIC_CLOUD_MRI_PREFIX.Length") };

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


            // Add participant
            var phoneNumberOptions = new AddPhoneNumberOptions(new PhoneNumberIdentifier("+14257654321"));
            call.AddParticipant(new PhoneNumberIdentifier("userMRI"), phoneNumberOptions);
        }

        private async void CallAgent_OnIncomingCall(object sender, IncomingCall incomingCall) // Wrap this with a EventArgs
        {
            Console.WriteLine($"{incomingCall.CallerInfo.DisplayName}");

            var client = await GetCallClientAsync();
            var cameras = (await client.GetDeviceManager()).Cameras;

            var acceptCallOptions = new AcceptCallOptions() {
                AudioOptions = new AudioOptions() {
                    IncomingAudioStream = await GetIncomingAudioStreamAsync(),
                    OutgoingAudioStream = await GetOutgoingAudioStreamAsync(),
                    Muted = false,
                    SpeakerMuted = false
                },
                VideoOptions = new VideoOptions( new[] { new OutgoingVideoStream() })
                {
                    IncomingVideoOptions = new IncomingVideoOptions()
                    {
                        AllowVideoFrameTextures = true,
                        ReceiveRawIncomingVideoStreams = true,
                    }
                }
            };
            var call = await incomingCall.AcceptAsync(acceptCallOptions);

            await call.StartVideo(new LocalVideoStream(cameras.First()));

            await call.HangUpAsync(new HangUpOptions()
            {
                ForEveryone = true
            });

            var callInfo = call.CallerInfo;
            var endReason = call.CallEndReason;
        }

        private void CallAgent_OnCallsUpdated(object sender, CallsUpdatedEventArgs args)
        {
        }
    }
}
