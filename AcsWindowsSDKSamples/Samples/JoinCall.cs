using Azure.Communication.Calling.WindowsClient;
using System;

namespace AcsWindowsSDKSamples.Samples
{
    internal class JoinCall : Sample
    {
        private async void JoinCallAsync()
        {
            // Setup audio preference when joining the call
            var audioOptions = new AudioOptions()
            {
                IncomingAudioStream = await GetIncomingAudioStreamAsync(),
                OutgoingAudioStream = await GetOutgoingAudioStreamAsync(),
                Muted = false,
                SpeakerMuted = false
            };
            // Setup video preference when joining the call
            var videoOptions = new VideoOptions(new[] { await GetOutgoingVideoStreamAsync() })
            {
                IncomingVideoOptions = new IncomingVideoOptions()
                {
                    AllowVideoFrameTextures = true,
                    ReceiveRawIncomingVideoStreams = true,
                }
            };

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
            // Join group chat call using thread id
            var groupChatCallLocator = new GroupChatCallLocator("<threadid>"); //TFL
            await callAgent.JoinAsync(groupChatCallLocator, joinCallOptions);
        }
    }
}
