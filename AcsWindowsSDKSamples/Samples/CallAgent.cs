using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;

namespace AcsWindowsSDKSamples.Samples
{
    internal class CallAgentSample : Sample
    {
        async void CallAgentAsync()
        {
            using(var callAgent = await GetCallAgentAsync())
            {
                // Wire up event sinks for call agent
                callAgent.CallsUpdated += OnCallsUpdated;
                callAgent.IncomingCallReceived += OnIncomingCall;

                // Assemble a list of callees
                var callees = new List<CallIdentifier>() {
                    new PhoneNumberCallIdentifier("+1 425 76543221"),
                    new UserCallIdentifier("<ACS public MRI>"),
                    new MicrosoftTeamsUserCallIdentifier("<Constants.TEAMS_PUBLIC_CLOUD_MRI_PREFIX.Length>") { CloudEnvironment = CallCloudEnvironment.Public },
                    new PhoneNumberCallIdentifier("+1 4257654321"),
                    new UnknownCallIdentifier("<user id>")
                };

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

                await callAgent.RegisterForPushNotificationAsync("<Device token>");
                callAgent.HandlePushNotificationAsync(new PushNotificationDetails() { });
                callAgent.UnregisterPushNotificationAsync();

                // Start the bump and have fun with the call
            }
        }

        // Accept incoming calls
        private async void OnIncomingCall(object sender, IncomingCallReceivedEventArgs args)
        {
            var callAgent = sender as CallAgent;
            var incomingCall = args.IncomingCall;

            Console.WriteLine($"{callAgent.Calls.Count}");

            Console.WriteLine($"{incomingCall.CallerDetails.DisplayName}");

            // Configure how we want to accept the incomnig call
            var acceptCallOptions = new AcceptCallOptions() {
                VideoOptions = new VideoOptions( new[] { await GetOutgoingVideoStreamAsync() })
            };

            // Accept the incoming call
            var call = await incomingCall.AcceptAsync(acceptCallOptions);

            // Interact with the call object
        }

        private async void OnCallsUpdated(object sender, CallsUpdatedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
