// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Microsoft">
//   2013
// </copyright>
// <summary>
//   The main page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FacebookLoginSimulator
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Windows;
    using System.Windows.Navigation;
    using Windows.System;

    /// <summary>
    /// The main page
    /// </summary>
    public partial class MainPage
    {
        /// <summary>
        /// The callback URI format
        /// </summary>
        private const string CallbackUriFormat = "{0}?{1}";

        /// <summary>
        /// The redirect uri.
        /// </summary>
        private string redirectUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            
            // Enter your access token for development purposes below.
            // You can grab one from the graph api explorer: https://developers.facebook.com/tools/explorer

            this.AccessTokenBox.Text = "CAACEdEose0cBAAaDp3hJMVn5kQmuGcFmTgRgfmByZBVsKtReK7Blso8EQBd30BZAaG1T73GOOoZBZAc2zwdnkoMDlZBAkovB8FwwIdZAlXki9JAAMjtyj6dNrUmqneroHercRr2KFJbdWowMjighg7LZC19qKybaUdFUCgl2mqAOdy1uinlbpiqBQ7okc0KUEYZD";
        }

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.TryGetValue("redirect_uri", out this.redirectUri))
            {
                if (!this.redirectUri.ToLower().StartsWith("msft-"))
                {
                    var loginResponse = new LoginResponse
                                        {
                                            ErrorCode = 2001, 
                                            ErrorReason = "Calling app's URI is invalid", 
                                            Error = "Invalid calling app URI",
                                            ErrorDescription = "Calling app's URI needs to start 'msft-'"
                                        };

                    this.LaunchCallingApp(loginResponse);
                }
            }

            string state;
            if (this.NavigationContext.QueryString.TryGetValue("state", out state))
            {
                this.StateTextBox.Text = state;
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// The access denied button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AccessDeniedButton_OnClick(object sender, RoutedEventArgs e)
        {
            var loginResponse = new LoginResponse
                                {
                                    ErrorCode = 200, 
                                    Error = "access_denied", 
                                    ErrorDescription = "Permissions error", 
                                    ErrorReason = "user_denied"
                                };

            this.LaunchCallingApp(loginResponse);
        }

        /// <summary>
        /// Launches the calling app.
        /// </summary>
        /// <param name="loginResponse">
        /// The login response.
        /// </param>
        private async void LaunchCallingApp(LoginResponse loginResponse)
        {
            if (string.IsNullOrEmpty(this.redirectUri))
            {
                MessageBox.Show("No redirect uri provided");
                return;
            }

            string uri = string.Format(CallbackUriFormat, this.redirectUri, loginResponse);
            Debug.WriteLine("Final Redirect URI: " + uri);
            await Launcher.LaunchUriAsync(new Uri(uri, UriKind.Absolute));
            Application.Current.Terminate();
        }

        /// <summary>
        /// The no network button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NoNetworkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var loginResponse = new LoginResponse
                                {
                                    ErrorCode = 2002, 
                                    ErrorReason = "No connection to facebook", 
                                    Error = "Connection failed", 
                                    ErrorDescription = "Unable to communicate with facebook in order to get the access token"
                                };

            this.LaunchCallingApp(loginResponse);
        }

        /// <summary>
        /// No access token response.
        /// </summary>
        private void NoAccessTokenResponse()
        {
            var loginResponse = new LoginResponse
                           {
                               ErrorCode = 2003,
                               ErrorDescription = "No access token provided in the login simulator",
                               Error = "No access token provided in the login simulator",
                               ErrorReason = "No access token provided in the login simulator"
                           };

            this.LaunchCallingApp(loginResponse);
        }

        /// <summary>
        /// The success button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SuccessButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.AccessTokenBox.Text))
            {
                this.NoAccessTokenResponse();
                return;
            }

            var loginResponse = new LoginResponse
                                {
                                    AccessToken = this.AccessTokenBox.Text,
                                    Expires = DateTime.UtcNow + TimeSpan.FromSeconds(long.Parse(this.AccessTokenExpiresInBox.Text)),
                                    State = this.StateTextBox.Text
                                };

            this.LaunchCallingApp(loginResponse);
        }
    }
}