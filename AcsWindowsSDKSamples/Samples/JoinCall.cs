using Azure.Communication.Calling.WindowsClient;
using System;

namespace AcsWindowsSDKSamples.Samples
{
    internal class JoinCall : Sample
    {
        private async void JoinCallAsync()
        {
            // Configure audio preferences
            var audioOptions = new AudioOptions() {  IsMuted = false };
            // Configure video preference
            var videoOptions = new VideoOptions(new VideoOptions( new[] { await GetOutgoingVideoStreamAsync() }));

            var locator = await GetJoinMeetingLocatorAsync();
            var joinCallOptions = new JoinCallOptions()
            {
                AudioOptions = audioOptions,
                VideoOptions = videoOptions,
            };

            var callAgent = await GetCallAgentAsync();
            var call = await callAgent.JoinAsync(locator, joinCallOptions);

            // Join group call using group id (GUID)
            var groupCallLocator = new GroupCallLocator(Guid.Parse("<groupId>"));
            await callAgent.JoinAsync(groupCallLocator, joinCallOptions);
            // Join group chat call using thread id (ALPHA only)
            //var groupChatCallLocator = new GroupChatCallLocator("<threadid>"); //TFL
            //await callAgent.JoinAsync(groupChatCallLocator, joinCallOptions);
        }
    }
}
