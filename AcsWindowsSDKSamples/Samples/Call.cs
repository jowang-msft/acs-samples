using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation.Provider;

namespace AcsWindowsSDKSamples.Samples
{
    internal class CallSample : Sample
    {
        async void CallAsync()
        {
            var call = await GetCallAsync();

            // Give control back to the app, or carry out additional tasks with the call object, such as adding participant
            var phoneNumberOptions = new AddPhoneNumberOptions(new PhoneNumberCallIdentifier("+1 4257654320"));
            call.AddParticipant(new PhoneNumberCallIdentifier("<userMRI>"), phoneNumberOptions);

            var client = await GetCallClientAsync();
            var cameras = (await client.GetDeviceManagerAsync()).Cameras;
            var localVideoStream = new LocalVideoStream(cameras.First());
            await call.StartVideoAsync(localVideoStream);

            // Set up event sinks for call object
            call.IdChanged += OnIdChanged;
            call.StateChanged += OnStateChanged;
            call.IsMutedChanged += OnIsMutedChanged;
            call.StateChanged += OnStateChanged1;
            call.LocalVideoStreamsUpdated += OnLocalVideoStreamsUpdated;
            call.RemoteParticipantsUpdated += OnRemoteParticipantsUpdated;

            // Have fun with the call
            await call.HoldAsync();
            await call.ResumeAsync();
            await call.MuteAsync();
            await call.UnmuteAsync();
            await call.SendDtmfAsync(DtmfTone.D);
            await call.StopVideoAsync(localVideoStream);
            localVideoStream.Stop();

            await call.HangUpAsync(new HangUpOptions()
            {
                ForEveryone = true
            });
        }

        private async void OnRemoteParticipantsUpdated(object sender, ParticipantsUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnLocalVideoStreamsUpdated(object sender, LocalVideoStreamsUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnStateChanged1(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnIsMutedChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnStateChanged(object sender, PropertyChangedEventArgs args)
        {
            var call = sender as Call;

            switch (call.State)
            {
                case CallState.Connected:
                    Console.WriteLine($"{call.CallerDetails.DisplayName}, {call.Direction}, {call.RemoteParticipants.Count}");
                    break;
                case CallState.Disconnected:
                    Console.WriteLine($"{call.CallEndReason.Subcode}");
                    break;
                default: break;
            }
        }

        private async void OnIdChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnCallsUpdated(object sender, CallsUpdatedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
