using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColinChang.IdentityServer.ResourceOwnerPasswordCredentialWpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IdentityServerOptions _options;
        private readonly DiscoveryDocumentResponse _disco;

        public MainWindow()
        {
            InitializeComponent();

            _options = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
                .GetSection(nameof(IdentityServerOptions)).Get<IdentityServerOptions>();

            using var client = new HttpClient();
            _disco = client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _options.Address,
                // Policy = new DiscoveryPolicy
                // {
                //     RequireHttps = false,
                //     ValidateEndpoints = false,
                //     ValidateIssuerName = false
                // }
            }).Result;
            if (_disco.IsError)
                MessageBox.Show(_disco.Error);
        }

        private async void Signin_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Username and password are required!");
                return;
            }

            //获取Token
            using var client = new HttpClient();
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = _disco.TokenEndpoint,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Scope = _options.Scope,

                UserName = txtUsername.Text,
                Password = txtPassword.Password
            });
            if (tokenResponse.IsError)
            {
                MessageBox.Show(tokenResponse.Error);
                return;
            }

            txtToken.Text = tokenResponse.AccessToken;
        }

        private async void RequestIdentityData_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtToken.Text))
            {
                MessageBox.Show("Please signin first.");
                return;
            }

            using var client = new HttpClient();
            client.SetBearerToken(txtToken.Text);

            var response = await client.GetAsync(_disco.UserInfoEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }
            txtIdentityData.Text = await response.Content.ReadAsStringAsync();
        }

        private async void RequestApi_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtToken.Text))
            {
                MessageBox.Show("Please signin first.");
                return;
            }

            using var client = new HttpClient();
            client.SetBearerToken(txtToken.Text);

            var response = await client.GetAsync(_options["WeatherApi"]);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }

            txtApiResult.Text = await response.Content.ReadAsStringAsync();
        }
    }
}