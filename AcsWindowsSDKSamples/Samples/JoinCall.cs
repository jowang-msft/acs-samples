using Azure.Communication.Calling.WindowsClient;
using System;

namespace AcsWindowsSDKSamples.Samples
{
    internal class JoinCall : Sample
    {
        private async void JoinCallAsync()
        {
            var joinCallOptions = new JoinCallOptions()
            {
                AudioOptions = new AudioOptions() {  IsMuted = false },
                VideoOptions = new VideoOptions(new VideoOptions( new[] { await GetOutgoingVideoStreamAsync() }))
            };

            using(var callAgent = await GetCallAgentAsync())
            {
                // Join group call using group id (GUID)
                var groupCallLocator = new GroupCallLocator(Guid.Parse("<groupId>"));
                await callAgent.JoinAsync(groupCallLocator, joinCallOptions);

                var teamMeetingLocator = new TeamsMeetingCoordinatesLocator("<threadId>", Guid.Parse("<organizatoinId>"), Guid.Parse("<tenantId>"), "<Message>");
                await callAgent.JoinAsync(teamMeetingLocator, joinCallOptions);

                var teamMettingLink = new TeamsMeetingLinkLocator("<TeamLink>");
                await callAgent.JoinAsync(teamMettingLink, joinCallOptions);

                // Join group chat call using thread id (ALPHA only)
                //var groupChatCallLocator = new GroupChatCallLocator("<threadid>"); //TFL
                //await callAgent.JoinAsync(groupChatCallLocator, joinCallOptions);
            }
        }
    }
}
