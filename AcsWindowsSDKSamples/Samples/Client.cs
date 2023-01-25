using Azure.Communication.Calling.WindowsClient;
using System;

namespace AcsWindowsSDKSamples.Samples
{
    internal class ClientSample : Sample
    {
        async void SetupAsync()
        {
            var callClient = new CallClient(new CallClientOptions()
            {
                Diagnostics = new CallDiagnosticsOptions()
                {
                    AppName = "appName",
                    AppVersion = "1.0",
                    Tags = new string[] { "tag1", "tag2", "tag3" }
                }
            });

            var tokenRefreshOptions = new CommunicationTokenRefreshOptions(true);
            tokenRefreshOptions.OnTokenRefreshRequested += OnTokenRefreshRequested;

            var creds = new CommunicationTokenCredential("initial-token", tokenRefreshOptions);

            var callAgentOptions = new CallAgentOptions()
            {
                 DisplayName = "Agent 007",
                 EmergencyCallOptions = new EmergencyCallOptions() { CountryCode = "911" }
            };

            var callAgent = await callClient.CreateCallAgent(creds, callAgentOptions);
        }

        private async void OnTokenRefreshRequested(object sender, TokenRefreshRequestedEventArgs args)
        {
            var accessToken = args.AccessToken;
            Console.WriteLine($"{accessToken.Token}-{accessToken.ExpiresOn}");
            // Do something about the token, such as update it
        }
    }
}
