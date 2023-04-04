using Azure.Communication.Calling.WindowsClient;
using System;
using System.Threading.Tasks;

namespace AcsWindowsSDKSamples.Samples
{
    internal class CallFeatures : Sample
    {
        internal async Task<DominantSpeakersCallFeature> GetDominantSpeakersCallFeatureAsync() { throw new NotImplementedException(); }

        async void UseFeatureAsync()
        {
            var call = await GetCallAsync();

            // Configure DominantSpeakers feature
            var dominantSpeakers = call.Features.DominantSpeakers;
            Console.WriteLine($"{dominantSpeakers.Name}, {dominantSpeakers.DominantSpeakersDetails.LastUpdatedAt}");
            dominantSpeakers.DominantSpeakersChanged += OnDominantSpeakersChanged;

            // Configure recording feature
            var recordingFeature = call.Features.Recording;
            Console.WriteLine($"{recordingFeature.Name}, {recordingFeature.IsRecordingActive}");
            recordingFeature.IsRecordingActiveChanged += OnIsRecordingActiveChanged;

            // Configure Transcription feature
            var transcriptionFeature = call.Features.Transcription;
            Console.WriteLine($"{transcriptionFeature.Name}, {transcriptionFeature.IsTranscriptionActive}");
            transcriptionFeature.IsTranscriptionActiveChanged += OnIsTranscriptionActiveChanged;

            // DataChannel feature
            // var dataChannelFeature = call.Features.DataChannel;
        }

        private async void OnIsTranscriptionActiveChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnIsRecordingActiveChanged(object sender, PropertyChangedEventArgs e)
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
