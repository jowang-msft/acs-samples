using Azure.Communication.Calling.WindowsClient;
using System;
using System.Threading.Tasks;

namespace AcsWindowsSDKSamples.Samples
{
    /// <summary>
    /// Fake helpers
    /// </summary>
    internal class Sample
    {
        internal async Task<CallClient> GetCallClientAsync() { throw new NotImplementedException(); }
        internal async Task<CallAgent> GetCallAgentAsync() { throw new NotImplementedException(); }
        internal async Task<Call> GetCallAsync() { throw new NotImplementedException(); }

        /*
        internal async Task<IncomingAudioStream> GetIncomingAudioStreamAsync() { throw new NotImplementedException(); }
        internal async Task<IncomingVideoStream> GetIncomingVideoStreamAsync() { throw new NotImplementedException(); }
        */
        internal async Task<OutgoingAudioStream> GetOutgoingAudioStreamAsync() { throw new NotImplementedException(); }
        internal async Task<OutgoingVideoStream> GetOutgoingVideoStreamAsync() { throw new NotImplementedException(); }
    }
}
