using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;
using Windows.Networking.PushNotifications;

namespace AcsWindowsSDKSamples.Samples
{
    internal class PushNotificationAgentSample : Sample
    {
        private CallAgent callAgent;

        async void WNSAsync()
        {
            // Register to WNS
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channel.PushNotificationReceived += OnPushNotificationReceived;
            var hub = new NotificationHub("{CHANNEL_NAME}", "{SECRET_FROM_PNHUB_RESOURCE}");
            var result = await hub.RegisterNativeAsync(channel.Uri);


            using(var callAgent = await GetCallAgentAsync())
            {
                await callAgent.RegisterForPushNotificationAsync(result.RegistrationId);
                callAgent.IncomingCallReceived += OnIncomingCall;
                // Starts a call
                var call = await callAgent.StartCallAsync(
                    new List<CallIdentifier>() { new UserCallIdentifier("<ACS public MRI>") },
                    new StartCallOptions() {
                        AudioOptions = new AudioOptions() {  IsMuted = false }
                    });

                // Start the bump and have fun with the call

                await callAgent.UnregisterPushNotificationAsync();
                await call.HangUpAsync(new HangUpOptions() { ForEveryone = false });
            }
        }

        private async void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            switch (args.NotificationType) {
                case PushNotificationType.Toast:
                case PushNotificationType.Tile:
                case PushNotificationType.TileFlyout:
                case PushNotificationType.Badge: 
                    break;
                case PushNotificationType.Raw:
                    RawNotification rawNotification = args.RawNotification;
                    string channelId = rawNotification.ChannelId;
                    string content = rawNotification.Content;

                    var pushNotificationDetails = PushNotificationDetails.Parse(content);
                    Console.Write($"{pushNotificationDetails.CallId}, {pushNotificationDetails.FromDisplayName}");
                    await callAgent.HandlePushNotificationAsync(pushNotificationDetails);
                    break;
                default: break;
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
    }
}
