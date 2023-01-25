using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AcsWindowsSDKSamples.Samples
{
    internal class JoinCall : Sample
    {
        private async void JoinCallAsync()
        {
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

            var callAgent = await GetCallAgentAsync();
            JoinCallOptions joinCallOptions = new JoinCallOptions()
            {
                AudioOptions = audioOptions,
                VideoOptions = videoOptions
            };

            var locator = await GetJoinMeetingLocatorAsync();
            var call = await callAgent.JoinAsync(locator, joinCallOptions);

            //
            var groupCallLocator = new GroupCallLocator(Guid.Parse("groupId"));
            await callAgent.JoinAsync(groupCallLocator, joinCallOptions);

            var groupChatCallLocator = new GroupChatCallLocator("threadid"); //TFL
            await callAgent.JoinAsync(groupChatCallLocator, joinCallOptions);
        }
    }
}
