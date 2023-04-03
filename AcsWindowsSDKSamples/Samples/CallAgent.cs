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
            callAgent.CallsUpdated += CallAgent_OnCallsUpdated;
            callAgent.IncomingCallReceived += CallAgent_OnIncomingCall;

            // Assemble a list of callees
            var callees = new List<CallIdentifier>() { new MicrosoftTeamsUserCallIdentifier("<Constants.TEAMS_PUBLIC_CLOUD_MRI_PREFIX.Length>") };

            // Configure audio preferences
            var audioOptions = new AudioOptions() {  IsMuted = false };
            // Configure video preference
            var videoOptions = new VideoOptions(new VideoOptions( new[] { await GetOutgoingVideoStreamAsync() }));

            var startCallOptions = new StartCallOptions()
            {
                AudioOptions = audioOptions,
                VideoOptions = videoOptions
            };

            // Actually ring the callees
            var call = await callAgent.StartCallAsync(callees, startCallOptions);

            // Give control back to the app, or carry out additional tasks with the call object, such as adding participant
            var phoneNumberOptions = new AddPhoneNumberOptions(new PhoneNumberCallIdentifier("+14257654321"));
            call.AddParticipant(new PhoneNumberCallIdentifier("userMRI"), phoneNumberOptions);
        }

        // Accept incoming calls
        private async void CallAgent_OnIncomingCall(object sender, IncomingCallReceivedEventArgs args) // Wrap this with a EventArgs
        {
            var incomingCall = args.IncomingCall;
            
            Console.WriteLine($"{incomingCall.CallerDetails.DisplayName}");

            // Configure how we want to accept the incomnig call
            var acceptCallOptions = new AcceptCallOptions() {
                VideoOptions = new VideoOptions( new[] { await GetOutgoingVideoStreamAsync() })
            };

            // Actually accept the incoming call
            var call = await incomingCall.AcceptAsync(acceptCallOptions);

            var client = await GetCallClientAsync(); // Developer should be able to grab the client either from sender or incomingCall
            var cameras = (await client.GetDeviceManagerAsync()).Cameras;
            await call.StartVideoAsync(new LocalVideoStream(cameras.First())); // We are going to feed the video with LocalVideoStream off a local camera

            // Have fun with the call

            await call.HangUpAsync(new HangUpOptions()
            {
                ForEveryone = true
            });

            // Access misc properties on a call object
            Console.WriteLine($"{call.CallerDetails.Identifier}/{call.CallEndReason}");
        }

        private void CallAgent_OnCallsUpdated(object sender, CallsUpdatedEventArgs args)
        {
        }
    }
}
