using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Dynamic;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Drawing;

// Nuget
using Newtonsoft.Json;

// Custom
using Veylib.CLIUI;
using Veylib;

namespace SendItSpammer
{
    internal class Program
    {
        static Core core = Core.GetInstance();
        static Random rand = new Random();
        static void sendReq(string recId, string content)
        {
            dynamic reqJson = new ExpandoObject();

            reqJson.type = "sendit.post-type:question-and-answer-v1";
            reqJson.timer = 0;

            reqJson.recipient_identity = new ExpandoObject();
            reqJson.recipient_identity.type = "id";
            reqJson.recipient_identity.value = recId;

            reqJson.data = new ExpandoObject();
            reqJson.data.question = content;

            reqJson.ext_data = new ExpandoObject();
            reqJson.ext_data.sticker_id = "8748ac40-6301-4ed7-a0e3-b2d196fbdcb1";
            reqJson.ext_data.author_shadow_token = $"5eb5b415-{rand.Next(1000, 9999)}-{rand.Next(1000, 9999)}-{rand.Next(1000, 9999)}-623c68893654";

            var req = WebRequest.Create("https://api.getsendit.com/v1/posts");
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add("App-Id", "c2ad997f-1bf2-4f2c-b5fd-83926e8f3c65");
            req.Headers.Add("App-Version", "1.0");

            // Write request
            var str = req.GetRequestStream();
            byte[] serialized = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reqJson));
            str.Write(serialized, 0, serialized.Length);
            str.Close();

            string rawRes = "{}";
            try
            {
                rawRes = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
            } catch (WebException ex)
            {
                if (ex.Response == null)
                    return;

                rawRes = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }

            dynamic json = JsonConvert.DeserializeObject(rawRes);

            Debug.WriteLine(rawRes);
            if ((string)json.status != "success")
                core.WriteLine(Color.Red, $"Failed to send: {json.error.code}");
            else
                core.WriteLine(Color.Lime, $"Sent request with data {content}");
        }

        static dynamic getInfo(string id)
        {
            var req = WebRequest.Create($"https://api.getsendit.com/v1/stickers/{id}");
            req.Headers.Add("App-Id", "c2ad997f-1bf2-4f2c-b5fd-83926e8f3c65");
            req.Headers.Add("App-Version", "1.0");

            string rawRes = "{}";
            try
            {
                rawRes = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
            }
            catch (WebException ex)
            {
                rawRes = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }

            dynamic json = JsonConvert.DeserializeObject(rawRes);

            Debug.WriteLine(rawRes);
            if ((string)json.status != "success")
                core.WriteLine(Color.Red, $"Failed to get information for ID: {json.error.code}");

            return json;
        }

        static void Main(string[] args)
        {
            var props = new Core.StartupProperties {
                LogoString = @"                    _ _ _                                                   
 ___  ___ _ __   __| (_) |_   ___ _ __   __ _ _ __ ___  _ __ ___   ___ _ __ 
/ __|/ _ \ '_ \ / _` | | __| / __| '_ \ / _` | '_ ` _ \| '_ ` _ \ / _ \ '__|
\__ \  __/ | | | (_| | | |_  \__ \ |_) | (_| | | | | | | | | | | |  __/ |   
|___/\___|_| |_|\__,_|_|\__| |___/ .__/ \__,_|_| |_| |_|_| |_| |_|\___|_|   
                                 |_|",
                Author = new Core.StartupAuthorProperties
                {
                    Name = "verlox",
                    Url = "verlox.cc"
                },
                Title = new Core.StartupConsoleTitleProperties
                {
                    Text = "Sendit spammer by verlox"
                },
                MOTD = "running endpoints since 2020",
                SilentStart = true,
            };
            core.Start(props);

            string link = core.ReadLine("Link : ");
            string content = core.ReadLine("Content : ");

            ServicePointManager.DefaultConnectionLimit = 20;

            var author = getInfo(link.Split('/')[4].Split('?')[0]).payload.sticker.author;

            var table = new AsciiTable(new AsciiTable.Properties { Colors = new AsciiTable.ColorProperties { RainbowDividers = true } });
            table.AddColumn("Name");
            table.AddColumn((string)author.display_name);

            table.WriteTable();
            var conf = core.ReadLine("Is this correct? [Y/n] ");

            if (conf != "" && conf.ToLower() != "y")
                Environment.Exit(0);

            for (var x =0;x < 20; x++)
            {
                new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            sendReq((string)author.id, content);
                        }
                        catch { }
                    }
                }).Start();
            }
        }
    }
}
