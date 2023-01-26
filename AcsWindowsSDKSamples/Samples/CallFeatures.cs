using Azure.Communication.Calling.WindowsClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AcsWindowsSDKSamples.Samples
{
    internal class CallFeatures : Sample
    {
        internal async Task<DominantSpeakersCallFeature> GetDominantSpeakersCallFeatureAsync() { throw new NotImplementedException(); }

        async void AddFeatureAsync()
        {
            // Setup audio preference
            var audioOptions = new AudioOptions()
            {
                IncomingAudioStream = await GetIncomingAudioStreamAsync(),
                OutgoingAudioStream = await GetOutgoingAudioStreamAsync(),
                Muted = false,
                SpeakerMuted = false,
            };
            // Setup video preference
            var videoOptions = new VideoOptions(new[] { await GetOutgoingVideoStreamAsync() })
            {
                IncomingVideoOptions = new IncomingVideoOptions()
                {
                    AllowVideoFrameTextures = true,
                    ReceiveRawIncomingVideoStreams = true,
                }
            };

            // Start the call
            var callAgent = await GetCallAgentAsync();
            var call = await callAgent.StartCallAsync(
                new [] { new CommunicationUserIdentifier("eyxxxxx") },
                new StartCallOptions()
                {
                    AudioOptions = audioOptions,
                    VideoOptions = videoOptions
                });

            // Enable on caption feature
            var captionsFeature = (CaptionsCallFeature)call.GetCallFeatureExtension(HandleType.CaptionsCallFeature); // The factory is being reworked
            captionsFeature.OnCaptionsReceived += CaptionsFeature_OnCaptionsReceived;
            await captionsFeature.StartCaptionsAsync(new StartCaptionsOptions() { Language = "en-us" });

            // Enable background blur feature
            var client = await GetCallClientAsync();
            var cameras = (await client.GetDeviceManager()).Cameras;
            var localVideoStream = new LocalVideoStream(cameras.First());
            var videoEffectsLocalVideoStreamFeature = (VideoEffectsLocalVideoStreamFeature)localVideoStream.GetLocalVideoStreamFeatureExtension(HandleType.VideoEffectsLocalVideoStreamFeature);
            var backgroundBlurVideoEffect = new BackgroundBlurEffect();
            videoEffectsLocalVideoStreamFeature.OnVideoEffectEnabled += VideoEffectsLocalVideoStreamFeature_OnVideoEffectEnabled;
            videoEffectsLocalVideoStreamFeature.OnVideoEffectDisabled += VideoEffectsLocalVideoStreamFeature_OnVideoEffectDisabled;
            videoEffectsLocalVideoStreamFeature.OnVideoEffectError += VideoEffectsLocalVideoStreamFeature_OnVideoEffectError;
            if (videoEffectsLocalVideoStreamFeature.IsEffectSupported(backgroundBlurVideoEffect))
            {
                videoEffectsLocalVideoStreamFeature.EnableEffect(backgroundBlurVideoEffect);
                videoEffectsLocalVideoStreamFeature.DisableEffect(backgroundBlurVideoEffect);
            }

            // Diagnostic feature
            var diagnosticsCallFeature = (DiagnosticsCallFeature)call.GetCallFeatureExtension(HandleType.DiagnosticsCallFeature);
            var networkDiagnostics = diagnosticsCallFeature.Network;
            networkDiagnostics.OnNetworkQualityDiagnosticChanged += NetworkDiagnostics_OnNetworkQualityDiagnosticChanged;
            networkDiagnostics.OnNetworkFlagDiagnosticChanged += NetworkDiagnostics_OnNetworkFlagDiagnosticChanged;
            var mediaDiagnostics = diagnosticsCallFeature.Media;
            mediaDiagnostics.OnMediaFlagDiagnosticChanged += MediaDiagnostics_OnMediaFlagDiagnosticChanged;

            // Dominant speaker feature
            var dominantSpeakersFeature = (DominantSpeakersCallFeature)call.GetCallFeatureExtension(HandleType.DominantSpeakersCallFeature);
            dominantSpeakersFeature.OnDominantSpeakersChanged += DominantSpeakersFeature_OnDominantSpeakersChanged;

            // TranscriptionCallFeature
            var transcriptionFeature = (TranscriptionCallFeature)call.GetCallFeatureExtension(HandleType.TranscriptionCallFeature);
            transcriptionFeature.OnIsTranscriptionActiveChanged += TranscriptionFeature_OnIsTranscriptionActiveChanged;
        }

        private void TranscriptionFeature_OnIsTranscriptionActiveChanged(object sender, PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private async void DominantSpeakersFeature_OnDominantSpeakersChanged(object sender, PropertyChangedEventArgs args) // Is sender the dominant speaker?
        {
            var dominantSpeakersFeature = await GetDominantSpeakersCallFeatureAsync();
            var dominantSpeakers = dominantSpeakersFeature.DominantSpeakersInfo;
            foreach (CommunicationIdentifier communicationIdentifier in dominantSpeakers.Speakers)
            {
                //string mri = IdentifierHelpers.CommunicationIdentifierToMri(communicationIdentifier);
                Console.WriteLine(communicationIdentifier.RawId);
            }
        }

        private void MediaDiagnostics_OnMediaFlagDiagnosticChanged(object sender, MediaFlagDiagnosticChangedEventArgs args)
        {
            Console.WriteLine($"{args.Value}");
            throw new NotImplementedException();
        }

        private void NetworkDiagnostics_OnNetworkFlagDiagnosticChanged(object sender, NetworkFlagDiagnosticChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void NetworkDiagnostics_OnNetworkQualityDiagnosticChanged(object sender, NetworkQualityDiagnosticChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void CaptionsFeature_OnCaptionsReceived(object sender, CaptionsInfo captionsInfo)
        {
            Console.WriteLine(captionsInfo.Text);
        }

        private void VideoEffectsLocalVideoStreamFeature_OnVideoEffectError(object sender, VideoEffectErrorEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void VideoEffectsLocalVideoStreamFeature_OnVideoEffectDisabled(object sender, VideoEffectDisabledEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void VideoEffectsLocalVideoStreamFeature_OnVideoEffectEnabled(object sender, VideoEffectEnabledEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
