using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AcsWindowsSDKSamples.Samples
{
    internal class CallFeatures : Sample
    {
        internal async Task<DominantSpeakersCallFeature> GetDominantSpeakersCallFeatureAsync() { throw new NotImplementedException(); }

        async void AddFeatureAsync()
        {
            var client = await GetCallClientAsync();

            var callAgent = await GetCallAgentAsync();

            var callees = new List<CommunicationIdentifier>() { new CommunicationUserIdentifier("eyxxxxx") };

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

            var call = await callAgent.StartCallAsync(callees, startCallOptions);

            // Turn on caption feature
            var options = new StartCaptionsOptions()
            {
                Language = "en-us"
            };
            var captionsFeature = (CaptionsCallFeature)call.GetCallFeatureExtension(HandleType.CaptionsCallFeature);
            captionsFeature.OnCaptionsReceived += CaptionsFeature_OnCaptionsReceived;
            await captionsFeature.StartCaptionsAsync(options);

            // Turn on background blur feature
            var cameras = (await client.GetDeviceManager()).Cameras;
            var localVideoStream = new LocalVideoStream(cameras.First());
            var videoEffectsLocalVideoStreamFeature = (VideoEffectsLocalVideoStreamFeature)localVideoStream.GetLocalVideoStreamFeatureExtension(HandleType.VideoEffectsLocalVideoStreamFeature);

            var backgroundBlurVideoEffect = new BackgroundBlurEffect(); // How do we event set Name?
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
