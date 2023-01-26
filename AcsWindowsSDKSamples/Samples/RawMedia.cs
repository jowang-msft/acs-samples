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
            // Setup raw audio (incoming/outgoing) streams
            var rawIncomingAudioOptions = new RawIncomingAudioOptions(AudioSampleRate.SampleRate_48000, AudioChannelMode.ChannelMode_Stereo, AudioFormat.Pcm_16_Bit);
            var rawIncomingAudioStream = new RawIncomingAudioStream(rawIncomingAudioOptions);
            rawIncomingAudioStream.OnNewAudioBufferAvailable += RawIncomingAudioStream_OnNewAudioBufferAvailable;
            rawIncomingAudioStream.OnAudioStreamStopped += RawIncomingAudioStream_OnAudioStreamStopped;

            var rawOutgoingAudioOptions = new RawOutgoingAudioOptions(AudioSampleRate.SampleRate_48000, AudioChannelMode.ChannelMode_Stereo, AudioFormat.Pcm_16_Bit, OutgoingAudioMsOfDataPerBlock.Ms_20);
            var rawOutgoingAudioStream = new RawOutgoingAudioStream(rawOutgoingAudioOptions);
            rawOutgoingAudioStream.OnAudioStreamReady += OutgoingAudioStream_OnAudioStreamReady;
            rawOutgoingAudioStream.OnAudioStreamStopped += OutgoingAudioStream_OnAudioStreamStopped;

            var audioOptions = new AudioOptions()
            {
                IncomingAudioStream = rawIncomingAudioStream,
                OutgoingAudioStream = rawOutgoingAudioStream,
                Muted = false,
                SpeakerMuted = false
            };

            // Setup raw outgoing video streams
            var rawVideoOptions = new RawOutgoingVideoStreamOptions();
            rawVideoOptions.SetVideoFormats(new VideoFormat[] { new VideoFormat()
            {
                Width = 1280,
                Height = 720,
                PixelFormat = PixelFormat.Rgba,
                VideoFrameKind = VideoFrameKind.VideoSoftware,
                FramesPerSecond = 30,
                Stride1 = 1280 * 4,
            } });
            rawVideoOptions.OnOutgoingVideoStreamStateChanged += Options_OnOutgoingVideoStreamStateChanged;
            rawVideoOptions.OnVideoFrameSenderChanged += Options_OnVideoFrameSenderChanged;
            // Setup VirtualRawOutgoingVideoStream
            var virtualRawOutgoingVideoStream = new VirtualRawOutgoingVideoStream(rawVideoOptions);
            // Setup ScreenShareRawOutgoingVideoStream - high resolution
            var screenShareRawOutgoingVideoStream = new ScreenShareRawOutgoingVideoStream(rawVideoOptions);

            // See RemoteParticipant_OnVideoStreamStateChanged for example of accessing IncomingVideoStream

            // Start the call
            var videoOptions = new VideoOptions(new List<OutgoingVideoStream>() { virtualRawOutgoingVideoStream, screenShareRawOutgoingVideoStream }.ToArray())
            {
                IncomingVideoOptions = new IncomingVideoOptions() /// Why can't we setup IncomingVideoStream here like we do with AudioOption?
                {
                    AllowVideoFrameTextures = true,
                    ReceiveRawIncomingVideoStreams = true,
                }
            };

            var callAgent = await GetCallAgentAsync();
            var call = await callAgent.StartCallAsync(
                new [] { new CommunicationUserIdentifier("8:acs:xxxxx") },
                new StartCallOptions()
                {
                    AudioOptions = audioOptions,
                    VideoOptions = videoOptions,
                });

            // Start VirtualRawOutgoingVideoStream
            await call.StartVideo(virtualRawOutgoingVideoStream);
            // Start ScreenShareRawOutgoingVideoStream
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
                            case IncomingVideoStreamKind.Raw:
                                var rawIncomingVideoStream = incomingVideoStream as RawIncomingVideoStream; // Is there another way to get RawIncomingVideoStream?
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
