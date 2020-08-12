using System;
using System.IO;
using System.Threading;
using Tweetinvi;


namespace TwitterBotBtcHelper
{
    class Program
    {

        static void Main(string[] args)
        {
            DefaultLogin();
            while (true)
            {
                int timesPosted = 0;

                Console.WriteLine($"Number of times the price of btc has posted in this instance is {timesPosted}");
                Console.WriteLine($"\n");
                Console.WriteLine("Program will run again in 10 minutes");

                string json;
                decimal targetPrice;

                //start web client
                using (var web = new System.Net.WebClient())
                {
                    var url = @"https://api.coindesk.com/v1/bpi/currentprice.json";
                    json = web.DownloadString(url);
                }
                //parse into usable data
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                var currentPrice = Convert.ToDecimal(obj.bpi.USD.rate.Value);
                Console.WriteLine($"BTC: ${currentPrice}");



                //some math for additional information
                decimal remainder = currentPrice % 1000;
                decimal distanceToMilestone = 1000 - remainder;

                //find the next whole thousand dollar for a targetprice point
                targetPrice = (currentPrice - remainder) + 1000;

                Console.WriteLine($"${distanceToMilestone} away from {targetPrice}");
                Console.ForegroundColor = ConsoleColor.Blue;
                string textToTweet = $"The current price of BTC is: ${Convert.ToDouble(currentPrice)} \n #BTC #CryptoCurrency #Coindesk \n \n \n Powered by CoinDesk \n https://www.coindesk.com/price/bitcoin ";
                Console.ResetColor();
                //post special string when milestones are hit
                //but what if we start the program and the price is already 14000

                if (timesPosted == 0)
                {
                    Tweet.PublishTweet(textToTweet);
                    timesPosted++;
                }

                if (currentPrice > targetPrice)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    textToTweet = $"Bitcoin is above ${targetPrice} again! \n #BTC #CryptoCurrency #Coindesk \n Powered by CoinDesk \n https://www.coindesk.com/price/bitcoin ";
                    Console.ResetColor();
                    Tweet.PublishTweet(textToTweet);
                    timesPosted++;
                    targetPrice += 1000;
                }

                Console.WriteLine("It is not time to post yet, the program will try again in 60 minutes...");
                Thread.Sleep(600000);   //wait for 10 minutes
            }//end of while loop
        }

        public static void DefaultLogin()
        {
            //read passwords from file here using your own file path

            string pathOfApiKeys = $@".{Path.DirectorySeparatorChar}api_keys.txt";
            //read file and put contents into array
            string[] allKeys = File.ReadAllLines(pathOfApiKeys);

            //default keys for test bot
            string ApiKey = allKeys[0];
            string ApiKeySecret = allKeys[1];
            string AccessToken = allKeys[2];
            string AccessTokenSecret = allKeys[3];

            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials(ApiKey, ApiKeySecret, AccessToken, AccessTokenSecret);

            //Login
            var user = User.GetAuthenticatedUser();
            if (user != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Login Successful");
                //validate which user is loged in
                Console.WriteLine($"{user} is signed in.");

                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could Not Login, Check Credentials or Internet Connection");
                Console.ResetColor();
            }
            Console.WriteLine("\n");
        }
    }
}


