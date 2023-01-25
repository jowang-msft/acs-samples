using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace AcsWindowsSDKSamples.Samples
{
    internal class RawMedia : Sample
    {
        private async void RawAsync()
        {
            // Raw audio
            var rawIncomingAudioOptions = new RawIncomingAudioOptions(AudioSampleRate.SampleRate_48000, AudioChannelMode.ChannelMode_Stereo, AudioFormat.Pcm_16_Bit);
            var rawIncomingAudioStream = new RawIncomingAudioStream(rawIncomingAudioOptions);
            rawIncomingAudioStream.OnNewAudioBufferAvailable += RawIncomingAudioStream_OnNewAudioBufferAvailable;
            rawIncomingAudioStream.OnAudioStreamStopped += RawIncomingAudioStream_OnAudioStreamStopped;

            var outgoingAudioOptions = new RawOutgoingAudioOptions(AudioSampleRate.SampleRate_48000, AudioChannelMode.ChannelMode_Stereo, AudioFormat.Pcm_16_Bit, OutgoingAudioMsOfDataPerBlock.Ms_20);
            var outgoingAudioStream = new RawOutgoingAudioStream(outgoingAudioOptions);
            outgoingAudioStream.OnAudioStreamReady += OutgoingAudioStream_OnAudioStreamReady;
            outgoingAudioStream.OnAudioStreamStopped += OutgoingAudioStream_OnAudioStreamStopped;

            var audioOptions = new AudioOptions()
            {
                IncomingAudioStream = rawIncomingAudioStream,
                OutgoingAudioStream = outgoingAudioStream,
                Muted = false,
                SpeakerMuted = false
            };

            // Raw video
            var videoFormat = new VideoFormat()
            {
                Width = 1280,
                Height = 720,
                PixelFormat = PixelFormat.Rgba,
                VideoFrameKind = VideoFrameKind.VideoSoftware,
                FramesPerSecond = 30,
                Stride1 = 1280 * 4,
            };

            // Virtual outgoing video
            var rawVideoOptions = new RawOutgoingVideoStreamOptions();
            rawVideoOptions.SetVideoFormats(new VideoFormat[] { videoFormat }); // Why not through ctor?
            rawVideoOptions.OnOutgoingVideoStreamStateChanged += Options_OnOutgoingVideoStreamStateChanged;
            rawVideoOptions.OnVideoFrameSenderChanged += Options_OnVideoFrameSenderChanged;
            var virtualRawOutgoingVideoStream = new VirtualRawOutgoingVideoStream(rawVideoOptions);
            // Share screen outgoing video - high resolution
            var screenShareRawOutgoingVideoStream = new ScreenShareRawOutgoingVideoStream(rawVideoOptions);

            //var rowIncomingVideoStream = new RawIncomingVideoStream(RawIncomingVideoStream);
            //rowIncomingVideoStream.OnVideoFrameAvailable += RowIncomingVideoStream_OnVideoFrameAvailable;

            // Start the call
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
                VideoOptions = videoOptions,
            };

            var callAgent = await GetCallAgentAsync();

            var callees = new List<CommunicationIdentifier>() { new CommunicationUserIdentifier("8:acs:xxxxx") };
            var call = await callAgent.StartCallAsync(callees, startCallOptions);

            await call.StartVideo(virtualRawOutgoingVideoStream);
            await call.StartVideo(screenShareRawOutgoingVideoStream);

            call.OnRemoteParticipantsUpdated += Call_OnRemoteParticipantsUpdated;
        }

        private void Call_OnRemoteParticipantsUpdated(object sender, ParticipantsUpdatedEventArgs args)
        {
            foreach (var remoteParticipant in args.AddedParticipants)
            {
                remoteParticipant.OnVideoStreamStateChanged += RemoteParticipant_OnVideoStreamStateChanged;
                remoteParticipant.OnVideoStreamsUpdated += RemoteParticipant_OnVideoStreamsUpdated;
            }
        }

        private void RemoteParticipant_OnVideoStreamsUpdated(object sender, RemoteVideoStreamsEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void RemoteParticipant_OnVideoStreamStateChanged(object sender, VideoStreamStateChangedEventArgs args)
        {
            var incomingVideoStream = args.VideoStream as IncomingVideoStream;

            switch (incomingVideoStream.VideoStreamState)
            {
                case VideoStreamState.Available:
                    {
                        switch (incomingVideoStream.IncomingVideoStreamKind)
                        {
                            case IncomingVideoStreamKind.Remote:
                                break;
                            case IncomingVideoStreamKind.Raw:
                                var rawIncomingVideoStream = incomingVideoStream as RawIncomingVideoStream;
                                rawIncomingVideoStream.OnVideoFrameAvailable += RawIncomingVideoStream_OnVideoFrameAvailable;
                                rawIncomingVideoStream.Start();
                                break;
                        }
                        break;
                    }
            }
        }

        private void RawIncomingVideoStream_OnVideoFrameAvailable(object sender, VideoFrameAvailableEventArgs args)
        {
            Console.Write($"{args.VideoStreamId}");
        }

        private void RowIncomingVideoStream_OnVideoFrameAvailable(object sender, VideoFrameAvailableEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void RawIncomingAudioStream_OnAudioStreamStopped(object sender, PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void RawIncomingAudioStream_OnNewAudioBufferAvailable(object sender, IncomingAudioEventArgs args)
        {
            using (IMemoryBufferReference reference = args.IncomingAudioBuffer.Buffer.CreateReference())
            {
                // Do something with the buffer
            }
        }

        private void OutgoingAudioStream_OnAudioStreamStopped(object sender, PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OutgoingAudioStream_OnAudioStreamReady(object sender, PropertyChangedEventArgs args)
        {
            // PropertyChangedEventArgs should carry a OutgoingStream object
            //outgoingAudioStream_.SendOutgoingAudioBuffer(buffer);
            throw new NotImplementedException();
        }

        private async void Options_OnVideoFrameSenderChanged(object sender, VideoFrameSenderChangedEventArgs args)
        {
            var softwareBasedVideoFrameSender = args.VideoFrameSender as SoftwareBasedVideoFrameSender;

            VideoFormat videoFormat = softwareBasedVideoFrameSender.VideoFormat;
            uint bufferSize = (uint)(videoFormat.Width * videoFormat.Height) * 4;

            var memoryBuffer = new MemoryBuffer(bufferSize);

            // Prepare memorybufer

            var confirm = await softwareBasedVideoFrameSender.SendFrameAsync(memoryBuffer, args.VideoFrameSender.TimestampInTicks);

            Console.WriteLine($"{confirm.Status}");
        }


        private void Options_OnOutgoingVideoStreamStateChanged(object sender, OutgoingVideoStreamStateChangedEventArgs args)
        {
            //args.OutgoingVideoStreamState
        }
    }
}
