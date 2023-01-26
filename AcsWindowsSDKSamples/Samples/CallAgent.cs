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

            // Wire up event sinks for call agent
            callAgent.OnCallsUpdated += CallAgent_OnCallsUpdated;
            callAgent.OnIncomingCall += CallAgent_OnIncomingCall;

            // Assemble a list of callees
            var callees = new List<CommunicationIdentifier>() { new MicrosoftTeamsUserIdentifier("<Constants.TEAMS_PUBLIC_CLOUD_MRI_PREFIX.Length>") };

            // Configure audio preferences
            var audioOptions = new AudioOptions()
            {
                IncomingAudioStream = await GetIncomingAudioStreamAsync(),
                OutgoingAudioStream = await GetOutgoingAudioStreamAsync(),
                Muted = false,
                SpeakerMuted = false
            };
            // Configure video preference
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

            // Actually ring the callees
            var call = await callAgent.StartCallAsync(callees, startCallOptions);

            // Give control back to the app, or carry out additional tasks with the call object, such as adding participant
            var phoneNumberOptions = new AddPhoneNumberOptions(new PhoneNumberIdentifier("+14257654321"));
            call.AddParticipant(new PhoneNumberIdentifier("userMRI"), phoneNumberOptions);
        }

        // Accept incoming calls
        private async void CallAgent_OnIncomingCall(object sender, IncomingCall incomingCall) // Wrap this with a EventArgs
        {
            Console.WriteLine($"{incomingCall.CallerInfo.DisplayName}");

            // Configure how we want to accept the incomnig call
            var acceptCallOptions = new AcceptCallOptions() {
                AudioOptions = new AudioOptions() {
                    IncomingAudioStream = await GetIncomingAudioStreamAsync(),
                    OutgoingAudioStream = await GetOutgoingAudioStreamAsync(),
                    Muted = false,
                    SpeakerMuted = false
                },
                VideoOptions = new VideoOptions( new[] { await GetOutgoingVideoStreamAsync() })
                {
                    IncomingVideoOptions = new IncomingVideoOptions()
                    {
                        AllowVideoFrameTextures = true,
                        ReceiveRawIncomingVideoStreams = true,
                    }
                }
            };
            // Actually accept the incoming call
            var call = await incomingCall.AcceptAsync(acceptCallOptions);

            var client = await GetCallClientAsync(); // Developer should be able to grab the client either from sender or incomingCall
            var cameras = (await client.GetDeviceManager()).Cameras;
            await call.StartVideo(new LocalVideoStream(cameras.First())); // We are going to feed the video with LocalVideoStream off a local camera

            // Have fun with the call

            await call.HangUpAsync(new HangUpOptions()
            {
                ForEveryone = true
            });

            // Access misc properties on a call object
            Console.WriteLine($"{call.CallerInfo.Identifier}/{call.CallEndReason}");
        }

        private void CallAgent_OnCallsUpdated(object sender, CallsUpdatedEventArgs args)
        {
        }
    }
}
