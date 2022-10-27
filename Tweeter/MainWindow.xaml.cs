using System;
using System.Collections.Generic;
using System.Linq;
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
using Tweetinvi;
using Tweetinvi.Models;
using System.IO;
using System.Text.Json;

namespace Tweeter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        TwitterClient twitter;
        bool sending = false;
        bool isCtrlPressed = false;
        public MainWindow()
        {
            InitializeComponent();
            var data = getAuthDataFromFile();
            if (data == null) { Close(); return; }
            twitter = new TwitterClient(data.ConsumerKey, data.ConsumerSecret, data.AccessToken, data.AccessTokenSecret);
        }

        private TwitterAPIAuthData getAuthDataFromFile()
        {
            string fileName = Directory.GetCurrentDirectory() + "\\AuthData.json";
            if (!File.Exists(fileName))
            {
                MessageBox.Show("APIのアクセストークンが存在しません。\nAuthData.jsonに必要事項を記入してください",
                    "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                var kari = new TwitterAPIAuthData();
                kari.ConsumerKey = "Consumer_Key_here";
                kari.ConsumerSecret = "Consumer_Secret_here";
                kari.AccessToken = "Access_Token_here";
                kari.AccessTokenSecret = "Access_Secret_here";
                var op = new JsonSerializerOptions { WriteIndented = true };
                string kariJson = JsonSerializer.Serialize(kari, op);
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.Write(kariJson);
                }
                return null;
            }
            string jsonText;
            try
            {
                using (StreamReader r = new StreamReader(fileName))
                {
                    jsonText = r.ReadToEnd();
                }
                TwitterAPIAuthData authData = JsonSerializer.Deserialize<TwitterAPIAuthData>(jsonText);
                return authData;
            } catch
            {
                Console.WriteLine("Failed To Load Auth Data.");
            }
            return null;
        }

        private async Task doTweet()
        {
            bool failed = false;
            if (TweetText.Text.Length == 0) return;
            if (TweetText.Text.Length > 280)
            {
                MessageBox.Show("テキストの最大数を超えています", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            sending = true;
            TweetText.IsEnabled = false;
            TweetButton.IsEnabled = false;
            try
            {
                await twitter.Tweets.PublishTweetAsync(TweetText.Text);
            } catch(Exception ex)
            {
                failed = true;
                MessageBox.Show($"ツイートの送信に失敗しました。Cause : {ex.Message}");
            }

            if (!failed)
                TweetText.Text = string.Empty;
            TweetText.IsEnabled = true;
            TweetButton.IsEnabled = true;
            sending = false;
        }

        private async void TweetButton_Click(object sender, RoutedEventArgs e)
        {
            await doTweet();
        }

        private async void TweetText_KeyDown(object sender, KeyEventArgs e)
        {
            if (sending) return;
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) isCtrlPressed = true;
            if (e.Key == Key.Enter)
            {
                if (isCtrlPressed)
                {
                    e.Handled = true;
                    await doTweet();
                }
            }
        }

        private void TweetText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) isCtrlPressed = false;
        }
    }
}
