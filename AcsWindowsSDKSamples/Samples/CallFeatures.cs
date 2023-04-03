using Azure.Communication.Calling.WindowsClient;
using System;
using System.Threading.Tasks;

namespace AcsWindowsSDKSamples.Samples
{
    internal class CallFeatures : Sample
    {
        internal async Task<DominantSpeakersCallFeature> GetDominantSpeakersCallFeatureAsync() { throw new NotImplementedException(); }

        async void AddFeatureAsync()
        {
            // Configure audio preferences
            var audioOptions = new AudioOptions() {  IsMuted = false };
            // Configure video preference
            var videoOptions = new VideoOptions(new VideoOptions( new[] { await GetOutgoingVideoStreamAsync() }));

            // Start the call
            var callAgent = await GetCallAgentAsync();
            var call = await callAgent.StartCallAsync(
                new [] { new UserCallIdentifier("eyxxxxx") },
                new StartCallOptions()
                {
                    AudioOptions = audioOptions,
                    VideoOptions = videoOptions
                });

            // Enable on caption feature
            var dominantSpeakers = call.Features.DominantSpeakers;
            dominantSpeakers.DominantSpeakersChanged += OnDominantSpeakersChanged;

            var recordingFeature = call.Features.Recording;
            recordingFeature.IsRecordingActiveChanged += OnIsRecordingActiveChanged;

            var transcriptionFeature = call.Features.Transcription;
            transcriptionFeature.IsTranscriptionActiveChanged += OnIsTranscriptionActiveChanged;
        }

        private void OnIsTranscriptionActiveChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnIsRecordingActiveChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnDominantSpeakersChanged(object sender, PropertyChangedEventArgs e)
        {
            var dominantSpeakersFeature = sender as DominantSpeakersCallFeature;
            var dominantSpeakers = dominantSpeakersFeature.DominantSpeakersDetails;
            foreach (var callIdentifier in dominantSpeakers.Speakers)
            {
                Console.WriteLine(callIdentifier.RawId);
            }
        }

        private void TranscriptionFeature_OnIsTranscriptionActiveChanged(object sender, PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
