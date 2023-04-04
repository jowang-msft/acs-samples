using Azure.Communication.Calling.WindowsClient;
using System;
using System.Collections.Generic;

namespace AcsWindowsSDKSamples.Samples
{
    internal class ClientSample : Sample
    {
        async void CallClientAsync()
        {
            // Create client
            using(var callClient = new CallClient(new CallClientOptions() {
                    Diagnostics = new CallDiagnosticsOptions() // With diagnostic options
                    {
                        AppName = "appName",
                        AppVersion = "1.0",
                        Tags = new List<string> { "tag1", "tag2", "tag3" }
                    }
                }))
            {
                // Prepare token auto-refresher for call agent
                var tokenRefreshOptions = new CallTokenRefreshOptions(true); // true == proactively refresh on a fixed timer
                tokenRefreshOptions.TokenRefreshRequested += OnTokenRefreshRequested;

                var creds = new CallTokenCredential("<initial-token>", tokenRefreshOptions); // Credential is specific to each call agent

                // Prepare calll agent option
                var callAgentOptions = new CallAgentOptions()
                {
                     DisplayName = "Agent 007",
                     EmergencyCallOptions = new EmergencyCallOptions() { CountryCode = "911" } // with 911 call option
                };

                // Actually create the call agent with credential and options
                using (var callAgent = await callClient.CreateCallAgentAsync(creds, callAgentOptions))
                {
                    // Start pumping the messages and have fun calling.
                }
            }
        }

        private async void OnTokenRefreshRequested(object sender, CallTokenRefreshRequestedEventArgs args)
        {
            var accessToken = args.CallToken;
            Console.WriteLine($"{accessToken.Token}-{accessToken.ExpiresOn}");
            // Do something about the token, or update it
        }
    }
}
