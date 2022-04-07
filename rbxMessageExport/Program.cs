using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace rbxMessageExport
{
    class Program
    {
        static WebClient Client;

        static string User = "";
        static string API = "https://privatemessages.roblox.com/v1/messages?messageTab={0}&pageNumber={1}&pageSize=20";

        static bool SaveRobloxMessage = false;
        static bool SaveNewerMessages = false;

        static int TotalMessagesSaved = 0;

        private static void SaveMessages(string Type)
        {
            int CurrentPage = 0;
            int TotalPages = 1;

            List<Message> Messages = new List<Message>();
            FileStream MessageFile = new FileStream($"{User}\\{Type.Substring(0, 1).ToUpper() + Type.Substring(1)} Messages.json", FileMode.Create);

            while (CurrentPage < TotalPages)
            {
                Console.WriteLine($"Gathering messages page {CurrentPage + 1}/{TotalPages}");

                try
                {
                    string Response = Client.DownloadString(string.Format(API, Type, CurrentPage));

                    JObject RJson = JObject.Parse(Response);

                    if (RJson["totalPages"].Value<int>() > TotalPages)
                    TotalPages = RJson["totalPages"].Value<int>();

                    foreach (Message message in RJson["collection"].Value<JArray>().ToObject<List<Message>>())
                    {
                        if (!SaveRobloxMessage && message.sender.id == 1) continue;
                        if (!SaveNewerMessages && message.created.Year > 2018) continue;

                        Messages.Add(message);
                        TotalMessagesSaved++;
                    }

                    CurrentPage++;
                }
                catch (Exception x)
                {
                    Console.WriteLine(x);
                    Console.WriteLine("\nFailed to get messages, waiting 5 seconds before trying again...");
                    Thread.Sleep(5000);
                }

                Console.Clear();
            }

            byte[] Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Messages));

            MessageFile.Write(Data, 0, Data.Length);
            MessageFile.Close();
        }
        static void Main(string[] args)
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput(), Console.InputEncoding, false, 1024));

            Client = new WebClient();

            Console.Write("ROBLOSECURITY: ");

            string Token = Console.ReadLine();

            Client.Headers["Cookie"] = $".ROBLOSECURITY={Token};";

            try {
                string UserData = Client.DownloadString("https://users.roblox.com/v1/users/authenticated");
                
                JObject JS = JObject.Parse(UserData);

                User = JS["name"].Value<string>();
            }
            catch { Console.Clear(); Console.WriteLine("Error: Cookie is invalid or internet is unavailable!"); Console.ReadKey(); Environment.Exit(1); }

            Console.Clear();
            Console.Write("Would you like to save messages sent from ROBLOX? (Y/N): ");

            SaveRobloxMessage = Console.ReadKey().Key == ConsoleKey.Y;

            Console.Clear();
            Console.Write("Would you like to save messages after January 1st 2019 (Messages after this day will be deleted off ROBLOX)? (Y/N): ");

            SaveNewerMessages = Console.ReadKey().Key == ConsoleKey.Y;

            Console.Clear();

            if (!Directory.Exists(User))
                Directory.CreateDirectory(User);

            SaveMessages("inbox");
            SaveMessages("sent");
            SaveMessages("archive");

            Console.WriteLine($"Saved a total of {TotalMessagesSaved} messages!");

            Console.ReadLine();
        }
    }
}