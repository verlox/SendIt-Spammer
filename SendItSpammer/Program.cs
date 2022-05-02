﻿using System;
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

        static int sentReqs = 0;
        static async void sendReq(string recId, string content)
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

            try
            {
                dynamic json = JsonConvert.DeserializeObject(rawRes);
                Debug.WriteLine(rawRes);
                if ((string)json.status != "success")
                    core.WriteLine(Color.Red, $"Failed to send: {json.error.code}");
                else
                {
                    core.WriteLine(Color.Lime, $"Sent request with data {content}");
                    sentReqs++;
                    core.UpdateTitleStatus($"Sent {sentReqs} requests");
                }
            } catch (Exception ex)
            {
                core.WriteLine(Color.Red, "We do a little trolling?");
                Debug.WriteLine(ex);
            }
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
            Console.WindowHeight = 50;
            var props = new Core.StartupProperties {
                LogoString = @"                    _ _ _                                                   
 ___  ___ _ __   __| (_) |_   ___ _ __   __ _ _ __ ___  _ __ ___   ___ _ __ 
/ __|/ _ \ '_ \ / _` | | __| / __| '_ \ / _` | '_ ` _ \| '_ ` _ \ / _ \ '__|
\__ \  __/ | | | (_| | | |_  \__ \ |_) | (_| | | | | | | | | | | |  __/ |   
|___/\___|_| |_|\__,_|_|\__| |___/ .__/ \__,_|_| |_| |_|_| |_| |_|\___|_|   
                                 |_|",
                SplashScreen = new Core.StartupSpashScreenProperties
                {
                    AutoGenerate = true,
                    DisplayProgressBar = true,
                    AutoCenter = true,
                },
                Author = new Core.StartupAuthorProperties
                {
                    Name = "verlox",
                    Url = "verlox.cc",
                },
                Title = new Core.StartupConsoleTitleProperties
                {
                    Text = "Sendit spammer by verlox",
                },
                MOTD = new Core.StartupMOTDProperties
                {
                    Text = "running endpoints since 2020",
                },
            };

#if DEBUG
            props.SplashScreen = null;
#endif

            core.Start(props);

            string link = core.ReadLine("Link : ");
            string content = core.ReadLine("Content : ");

            ServicePointManager.DefaultConnectionLimit = 25;

            var info = getInfo(link.Split('/')[4].Split('?')[0]);
            var author = info.payload.sticker.author;

            var table = new AsciiTable(new AsciiTable.Properties { Colors = new AsciiTable.ColorProperties { RainbowDividers = true } });
            table.AddColumn("Name");
            table.AddColumn((string)author.display_name);
            table.AddRow("ID", (string)author.id);
            table.AddRow("Gender", (string)author.gender ?? "Unknown");

            table.WriteTable();

            core.WriteLine("Is this correct?");
            var resp = new SelectionMenu("Yes", "No").Activate();

            if (resp != "Yes")
                Environment.Exit(0);

            string trolling = @"[0;34;40m                                                                                                    [0m
[0;34;40m                           .[0;32;40m.[0;31;40m:[0;34;40m:[0;32;40m:[0;31;40m:[0;34;40m;[0;32;40m;[0;31;40m;[0;34;40m;[0;32;40m;[0;31;40m;[0;34;40m;[0;32;40m;[0;31;40m;[0;34;40m;[0;32;40m;[0;31;40m;[0;34;40m;[0;32;40m;[0;31;40m;[0;34;40m:[0;32;40m:[0;31;40m:[0;34;40m.[0;32;40m.[0;31;40m.[0;34;40m.[0;32;40m.[0;31;40m    [0;34;40m                                        [0m
[0;34;40m                     :[0;33;5;40;100m%[0;37;5;40;100mX[0;1;37;97;47m.S88[0;37;5;47;107m88@XXXXSS8[0;1;37;97;47m@t[0;1;30;90;47m.;%t%%SSX@88@@XS%t%%%SX8[0;37;5;40;100m88[0;36;5;40;100m   [0;30;5;40;100mS[0;1;30;90;40m8[0;34;40m.[0;32;40m.[0;31;40m    [0;34;40m                       [0m
[0;34;40m                    [0;30;5;40;100m8[0;1;37;97;47m.[0;37;5;47;107mt           %[0;1;37;97;47mt[0;1;30;90;47m8[0;37;5;40;100mS8[0;1;30;90;47m:[0;1;37;97;47mS[0;37;5;47;107m8t:.. ...:tS@[0;1;37;97;47m8Xt[0;1;30;90;47m tS8[0;37;5;40;100m888[0;1;30;90;47m88XSXX8[0;33;5;40;100m.[0;30;5;40;100mSSS@8[0;1;30;90;40mX[0;31;40m.[0;34;40m.[0;32;40m    [0;34;40m                [0m
[0;34;40m                  [0;1;30;90;40m@[0;1;30;90;47m8[0;37;5;47;107mX   .    .t[0;1;37;97;47mX[0;1;30;90;47mt@S[0;1;37;97;47m.8[0;37;5;47;107m%%X8[0;1;37;97;47mXSt;;::::::tX8[0;37;5;47;107m@St;::.:;t;ttt%SSX@8[0;1;37;97;47m88S;[0;1;30;90;47m:[0;37;5;40;100m8[0;33;5;40;100m:[0;34;40mt[0;32;40m:[0;31;40m.[0;34;40m               [0m
[0;34;40m                :[0;37;5;40;100mX[0;37;5;47;107mX       .@[0;1;37;97;47m:[0;1;30;90;47mS%[0;1;37;97;47mt[0;37;5;47;107m@8[0;1;37;97;47m%[0;1;30;90;47m.t:[0;1;37;97;47m.tX[0;37;5;47;107m88SS%tttt;t%@[0;1;37;97;47m8@St;:;:;;;::...:;t%X@8@t[0;1;30;90;47mt[0;1;37;97;47m@[0;37;5;47;107m.;[0;1;37;97;47mX[0;1;30;90;47m8[0;30;5;40;100mX[0;34;40m:[0;31;40m.[0;32;40m     [0;34;40m       [0m
[0;34;40m               [0;32;40m.[0;37;5;40;100m8[0;37;5;47;107m:  . .  X[0;1;30;90;47mS[0;37;5;40;100m8[0;1;37;97;47mS[0;37;5;47;107m:;[0;1;37;97;47m.[0;37;5;40;100mS8[0;1;37;97;47m%[0;37;5;47;107m%S[0;1;37;97;47mX[0;1;30;90;47m.X88888[0;37;5;40;100m8[0;1;30;90;47m8[0;37;5;40;100m8[0;1;30;90;47m88 [0;37;5;47;107m%     .            .:;t;. .     . :[0;1;37;97;47m;[0;30;5;40;100mS[0;32;40m.[0;31;40m   [0;34;40m        [0m
[0;34;40m              [0;31;40m.[0;37;5;40;100mX[0;37;5;47;107mt     ;[0;1;37;97;47m@[0;1;30;90;47m:[0;1;37;97;47m [0;37;5;47;107m8t[0;1;37;97;47m8[0;1;30;90;47m;%[0;1;37;97;47m:S [0;1;30;90;47mt%:[0;1;37;97;47mt8[0;37;5;47;107m8S%t;;;;;%[0;1;30;90;47mS;[0;37;5;47;107m;         . ;[0;1;37;97;47mS[0;1;30;90;47m ....     :%[0;1;37;97;47m 8[0;37;5;47;107m..     .%[0;1;30;90;47m.[0;30;5;40;100m@[0;31;40m   [0;34;40m       [0m
[0;34;40m             :[0;37;5;40;100m8[0;37;5;47;107m:. . .@[0;1;37;97;47mt8[0;37;5;47;107m.t[0;1;30;90;47m.@[0;1;37;97;47mt[0;37;5;47;107m8[0;1;30;90;47m;@[0;1;37;97;47m.[0;37;5;47;107m8:              :[0;1;30;90;47mtS[0;37;5;47;107m. . .  .   .t@[0;1;37;97;47m8[0;37;5;47;107m       . .X[0;1;37;97;47m.[0;1;30;90;47m8.[0;37;5;47;107m;  .    X[0;33;5;40;100mt[0;34;40m         [0m
[0;34;40m            [0;32;40m [0;33;5;40;100m.[0;37;5;47;107mt         X[0;37;5;40;100m8[0;1;30;90;47m:[0;37;5;47;107m;[0;1;37;97;47m%[0;36;5;40;100m [0;1;37;97;47mt[0;37;5;47;107m:     .  .  .   .  %[0;1;30;90;47mS[0;37;5;47;107mt     .     :[0;1;30;90;47m8.[0;37;5;47;107m .  .      .;[0;37;5;40;100m8[0;1;30;90;47m.[0;37;5;47;107m      ..[0;37;5;40;100m8[0;34;40m.        [0m
[0;34;40m           [0;32;40m [0;30;5;40;100m@[0;1;37;97;47m8[0;37;5;47;107m  .  .   .88:[0;1;37;97;47m%[0;1;30;90;47m:[0;37;5;47;107mX  .%[0;1;37;97;47m8S [0;1;30;90;47mtX@@8@S [0;1;37;97;47mX[0;37;5;47;107mS.  .8:  .    . . @[0;1;30;90;47m@[0;37;5;47;107mt   .  .  .   @[0;1;37;97;47m:[0;37;5;47;107m: . .   [0;1;30;90;47m.[0;34;40m.        [0m
[0;34;40m           [0;33;5;40;100m;[0;1;37;97;47m8[0;37;5;47;107m        .  .  . ;[0;1;37;97;47mt[0;37;5;40;100mS[0;30;5;40;100m8[0;1;30;90;40m%[0;34;40m.[0;32;40m.[0;31;40m      [0;1;30;90;40m8[0;30;5;40;100mS[0;1;30;90;40m8[0;34;40m;[0;32;40mt[0;31;5;40;100mS[0;37;5;40;100mX[0;1;37;97;47m%[0;37;5;47;107mt  .   .      S[0;1;37;97;47mS[0;37;5;47;107m. .tX[0;1;37;97;47m88XXX@8[0;37;5;47;107m%.         .%[0;35;5;40;100m [0;34;40m        [0m
[0;34;40m          [0;36;5;40;100m;[0;1;37;97;47m;[0;37;5;47;107m88S: .  . .tSS:.[0;1;30;90;47m.[0;30;5;40;100m8[0;34;40m:[0;30;5;40;100mX[0;33;5;40;100m;[0;34;40m;[0;31;40m.      :[0;1;37;97;47m.[0;37;5;47;107m::S[0;1;37;97;47m%[0;37;5;40;100m@[0;30;5;40;100m@[0;34;40m:[0;1;30;90;40m8[0;37;5;40;100mX[0;1;37;97;47m8[0;37;5;47;107m      .  . .;[0;1;37;97;47mS[0;37;5;40;100m8[0;30;5;40;100mS[0;1;30;90;40mX[0;34;40m:[0;31;40m:[0;32;40m.[0;34;40m.[0;31;40m;[0;32;40mt[0;34;40m;[0;31;40m:[0;1;30;90;40mS[0;36;5;40;100m;[0;1;37;97;47m [0;37;5;47;107m:... .    t[0;1;30;90;47m8[0;1;30;90;40mS[0;34;40m      [0m
[0;34;40m       .[0;1;30;90;40m8[0;30;5;40;100mX[0;35;5;40;100m:[0;33;5;40;100m [0;1;30;90;47m8X.[0;1;37;97;47mS[0;37;5;47;107m8;   %[0;37;5;40;100m8[0;30;5;40;100m%[0;37;5;40;100mS[0;1;37;97;47m%[0;37;5;47;107m:[0;1;37;97;47m@[0;1;30;90;40mS[0;32;40m.[0;1;30;90;40m8[0;35;5;40;100m:[0;1;30;90;40mX[0;32;40m. [0;31;40m     [0;34;40m .[0;1;30;90;40m8[0;36;5;40;100m;[0;37;5;40;100m@[0;1;37;97;47mX[0;37;5;47;107m; ;[0;1;30;90;47mS[0;1;30;90;40mS[0;32;40m [0;30;5;40;100m8[0;1;37;97;47m8[0;37;5;47;107m  .     .t[0;1;37;97;47mS[0;35;5;40;100m [0;34;40m:[0;31;40m.[0;32;40m ..[0;34;40m.[0;32;40m.[0;34;40m:[0;1;30;90;40mXS[0;31;40m.[0;32;40m.[0;31;40m  [0;1;30;90;40m@[0;1;37;97;47m8S[0;37;5;40;100mX[0;1;30;90;47m8S;.[0;1;37;97;47m :S[0;37;5;47;107mX [0;1;37;97;47mX[0;30;5;40;100mX[0;34;40m     [0m
[0;34;40m     :[0;33;5;40;100m%[0;36;5;40;100m [0;1;30;90;47mX [0;1;37;97;47mt@88@X8[0;37;5;47;107m8@S;:8[0;1;37;97;47m%Xt[0;37;5;47;107m8%[0;37;5;40;100m8[0;35;5;40;100m%[0;30;5;40;100m8[0;32;5;40;100m@[0;35;5;40;100m%[0;33;5;40;100m [0;37;5;40;100m8[0;1;30;90;47m@S@[0;37;5;40;100m@88[0;36;5;40;100m [0;33;5;40;100mt[0;34;40m%[0;31;40mt[0;32;40mt[0;30;5;40;100mX[0;35;5;40;100m [0;1;30;90;47m;;[0;30;5;40;100m8[0;32;40m [0;1;30;90;40m@[0;1;37;97;47m@[0;37;5;47;107m    . [0;1;37;97;47m:[0;35;5;40;100m:[0;30;5;40;100m@[0;1;30;90;40m8[0;32;40m;[0;31;40m.[0;32;40m.[0;31;40m  .[0;34;40m;[0;1;30;90;40m@8[0;30;5;40;100m@S[0;35;5;40;100m;[0;36;5;40;100m.[0;33;5;40;100m [0;37;5;40;100m8[0;1;30;90;47mS[0;1;37;97;47mX[0;37;5;47;107m.t8[0;1;37;97;47m8[0;37;5;47;107m8[0;1;37;97;47m8[0;37;5;47;107m8[0;1;37;97;47m88@[0;1;30;90;47m;@[0;1;37;97;47mX[0;37;5;47;107m8[0;1;30;90;47m@[0;1;30;90;40mX[0;34;40m   [0m
[0;34;40m    [0;30;5;40;100mS[0;1;30;90;47m8[0;37;5;40;100mX[0;1;37;97;47mS[0;37;5;47;107m. t[0;1;30;90;47m.[0;35;5;40;100m [0;1;30;90;40m@[0;34;40m;[0;31;40mt[0;1;30;90;40m@SXSX[0;30;5;40;100m@[0;33;5;40;100m [0;1;30;90;47m.[0;1;37;97;47m8[0;37;5;47;107m8: .;... .8[0;1;30;90;47m@[0;30;5;40;100m8[0;1;30;90;40mS[0;1;30;90;47m;[0;37;5;47;107m  ..X[0;1;37;97;47m;[0;37;5;40;100m8[0;35;5;40;100mt[0;32;40mt[0;34;40m:[0;30;5;40;100m8[0;37;5;40;100m8[0;37;5;47;107m8 . .   ;[0;1;37;97;47mX[0;1;30;90;47m8[0;30;5;40;100m8[0;34;40m.[0;31;40m.[0;34;5;40;100m%[0;1;30;90;47m@[0;1;37;97;47m;[0;37;5;47;107m8%t:..:        .. . S[0;1;30;90;47m:t[0;1;37;97;47m;.[0;1;30;90;47m8t[0;1;37;97;47m@[0;30;5;40;100mX[0;34;40m  [0m
[0;34;40m   [0;33;5;40;100mt[0;1;37;97;47m%[0;37;5;40;100mS[0;1;37;97;47mX[0;37;5;47;107m  %[0;35;5;40;100m [0;31;40m.[0;34;40m.[0;33;5;40;100m;[0;1;30;90;47m%[0;1;37;97;47m@[0;37;5;47;107m8@8[0;1;37;97;47m%[0;37;5;40;100m%[0;1;30;90;40m@[0;32;40m.[0;34;40m.[0;31;40m:[0;30;5;40;100mX[0;37;5;40;100m@[0;1;37;97;47m;[0;37;5;47;107m@;.;@[0;1;37;97;47m.[0;33;5;40;100m [0;1;30;90;40mS[0;32;40m [0;1;30;90;40mS[0;1;30;90;47mt[0;37;5;47;107m:      ..8[0;1;37;97;47m;[0;37;5;47;107m%       .   .[0;1;30;90;47m [0;31;40m.[0;1;30;90;40mS[0;1;37;97;47m@[0;37;5;47;107m  .           .X[0;1;30;90;47m.[0;37;5;40;100m8S%@[0;1;30;90;47m%[0;1;37;97;47mS[0;37;5;47;107mt[0;1;37;97;47mX[0;35;5;40;100m [0;1;37;97;47m.[0;37;5;47;107m:[0;1;30;90;47mXtX[0;34;40m.[0;32;40m [0m
[0;34;40m  [0;34;5;40;100mS[0;1;37;97;47m8X[0;1;30;90;47mS[0;37;5;47;107m. X[0;36;5;40;100m [0;34;40m:[0;30;5;40;100m8[0;1;30;90;47m.[0;37;5;47;107mt ..[0;1;37;97;47mt[0;37;5;40;100m@[0;37;5;47;107m8.X[0;1;37;97;47mt[0;1;30;90;47m@[0;36;5;40;100m [0;30;5;40;100m@[0;1;30;90;40mX[0;32;40m;[0;1;30;90;40m@8[0;30;5;40;100m88[0;1;30;90;40m%[0;32;40m:[0;1;30;90;40mX[0;30;5;40;100mS[0;1;30;90;47m8[0;37;5;47;107m8.  . .     .  .  .       [0;1;37;97;47mS[0;32;40m;[0;31;40mt[0;1;37;97;47mS[0;37;5;47;107m.    . ;[0;1;37;97;47m;;[0;37;5;47;107m8;S[0;1;37;97;47m:[0;33;5;40;100m [0;1;30;90;40m8[0;31;40m;[0;30;5;40;100m8@X8[0;34;40m;[0;31;40m;[0;30;5;40;100mS[0;1;37;97;47mt8[0;37;5;40;100m8[0;37;5;47;107m:%[0;1;30;90;47m88[0;32;40m;[0;31;40m.[0m
[0;34;40m [0;1;30;90;40m8[0;1;37;97;47m8[0;37;5;47;107m [0;1;37;97;47mt.[0;37;5;47;107m ;[0;36;5;40;100m [0;32;40m.[0;33;5;40;100m;[0;37;5;47;107m8 .  [0;1;37;97;47m8[0;1;30;90;40m8[0;32;40m [0;1;30;90;40m8[0;1;30;90;47mX[0;1;37;97;47m8[0;37;5;47;107m.  .;S[0;1;37;97;47m8@SX@[0;37;5;47;107m8t. . .     . .     .   .  .  S[0;35;5;40;100mt[0;32;40m.[0;30;5;40;100m@[0;1;37;97;47m [0;37;5;47;107m% .   %[0;33;5;40;100m [0;1;30;90;40mX[0;32;40m:[0;1;30;90;40m@[0;31;40m;[0;34;40m:[0;32;40m:[0;30;5;40;100mS[0;1;37;97;47m@[0;37;5;47;107mt%..t[0;1;37;97;47mX[0;1;30;90;47mt[0;1;37;97;47m8[0;37;5;47;107m:[0;1;30;90;47m@[0;37;5;47;107m8.[0;1;30;90;47m:8[0;31;40mt[0;34;40m:[0m
[0;34;40m [0;33;5;40;100mt[0;37;5;47;107m% [0;1;37;97;47m;.[0;37;5;47;107m [0;1;37;97;47m8[0;1;30;90;40m8[0;34;40m;[0;1;37;97;47m:[0;37;5;47;107m   .[0;1;37;97;47m8[0;36;5;40;100m;[0;31;40m  [0;32;40m  [0;34;40m;[0;33;5;40;100m [0;1;37;97;47m%[0;37;5;47;107m; .               .:[0;1;37;97;47m8[0;37;5;47;107m:.8[0;1;37;97;47m;[0;1;30;90;47m.[0;1;37;97;47m.[0;37;5;47;107m8.      .  ..[0;1;37;97;47m8[0;36;5;40;100m%[0;31;40m  [0;30;5;40;100m@[0;1;30;90;47m.[0;37;5;47;107m: .   8[0;1;30;90;47m.8[0;37;5;40;100mX[0;33;5;40;100m [0;1;30;90;47mX[0;37;5;47;107m8.[0;1;30;90;47m:[0;30;5;40;100mX[0;1;37;97;47m8[0;37;5;47;107m     .[0;1;30;90;47mS[0;1;37;97;47m@[0;37;5;47;107m.[0;1;30;90;47mX8[0;34;40mt[0;32;40m:[0m
[0;34;40m [0;34;5;40;100mX[0;37;5;47;107m% [0;1;37;97;47mt[0;1;30;90;47m.[0;37;5;47;107m [0;1;37;97;47m8[0;1;30;90;40mS@[0;1;37;97;47m@[0;37;5;47;107m%[0;1;37;97;47m.[0;37;5;40;100m@[0;36;5;40;100m;[0;31;40mt[0;32;40m [0;31;40m [0;33;5;40;100m:[0;1;30;90;47mt[0;37;5;40;100m@[0;33;5;40;100mS[0;34;40m;[0;32;40m;[0;30;5;40;100m@[0;35;5;40;100m [0;1;30;90;47m.[0;1;37;97;47m8[0;37;5;47;107mt .  . .%8[0;1;37;97;47m888@X [0;37;5;40;100m8[0;1;37;97;47mS[0;36;5;40;100m [0;1;30;90;40mX[0;32;40m:[0;1;30;90;40m8[0;30;5;40;100m8[0;37;5;40;100mS[0;37;5;47;107m% . .       .S[0;1;37;97;47m;[0;33;5;40;100m [0;31;40mt[0;32;40m;[0;35;5;40;100mS[0;1;30;90;47m8[0;37;5;47;107mS    .     ;[0;36;5;40;100m [0;34;40m.[0;37;5;40;100m8[0;37;5;47;107m..  ;[0;1;37;97;47m:[0;37;5;40;100m@[0;37;5;47;107mXX[0;37;5;40;100mXX[0;32;40m:[0;31;40m.[0m
[0;34;40m [0;32;40m;[0;1;37;97;47m:[0;37;5;47;107m [0;1;37;97;47m8[0;1;30;90;47mX[0;37;5;47;107m:%[0;33;5;40;100mt[0;34;40m [0;37;5;40;100mX[0;37;5;47;107m8[0;1;30;90;47mt8[0;37;5;40;100m@[0;36;5;40;100m [0;30;5;40;100m8[0;34;40m [0;30;5;40;100m@[0;37;5;47;107m@.;%[0;1;37;97;47m%[0;37;5;40;100m8[0;35;5;40;100m%[0;32;40m:[0;34;40m:[0;1;30;90;40m@[0;33;5;40;100mt[0;37;5;40;100m%[0;1;30;90;47m.[0;1;37;97;47m8[0;37;5;47;107m%.:[0;1;37;97;47m8%X@8[0;37;5;47;107m8@%:[0;1;30;90;47m%[0;32;40m [0;30;5;40;100mX[0;1;37;97;47m8[0;37;5;47;107m;S[0;1;37;97;47mX:.:%[0;37;5;47;107m8t .  .    S[0;30;5;40;100mS[0;32;40m ..[0;1;30;90;40mS[0;37;5;40;100m@[0;1;37;97;47m8[0;37;5;47;107m;  .   .[0;1;37;97;47mX[0;1;30;90;40mX[0;32;40m :[0;37;5;40;100m@[0;37;5;47;107m; :[0;1;37;97;47m;;[0;37;5;47;107mt8[0;37;5;40;100m8[0;36;5;40;100m [0;1;30;90;40m8[0;31;40m.[0;34;40m [0m
[0;34;40m [0;31;40m.[0;33;5;40;100m;[0;37;5;47;107mX%[0;37;5;40;100m@[0;37;5;47;107mX.[0;1;30;90;47m%[0;34;40m:[0;1;30;90;40m@[0;1;37;97;47m8[0;37;5;47;107m .  [0;1;37;97;47m:[0;34;40m:[0;32;40m.[0;37;5;40;100mX[0;37;5;47;107m;   .t[0;1;37;97;47mS[0;33;5;40;100m;[0;34;40m  . .[0;30;5;40;100m8[0;35;5;40;100m [0;1;37;97;47m [0;37;5;47;107m8:   .   [0;1;30;90;47mS[0;34;40m.[0;1;30;90;40m@[0;1;37;97;47m%[0;37;5;47;107m.[0;1;30;90;47mX[0;31;40m.[0;34;40m   [0;31;40m [0;32;40m:[0;35;5;40;100m [0;37;5;47;107m;     . %[0;33;5;40;100m [0;32;40m.[0;34;40m [0;1;30;90;40mS[0;32;40mt[0;35;5;40;100m [0;1;30;90;47mt[0;37;5;40;100m8S[0;1;30;90;47m@[0;37;5;47;107m8.   :[0;36;5;40;100m [0;32;40m.[0;31;40m.[0;34;40m.[0;31;40mt[0;1;37;97;47m@[0;37;5;47;107m. .[0;1;37;97;47m8[0;37;5;40;100m8%[0;1;37;97;47m.[0;33;5;40;100mt[0;34;40m   [0m
[0;34;40m  .[0;37;5;40;100m@[0;37;5;47;107mt[0;1;37;97;47m.[0;1;30;90;47m8[0;37;5;47;107m@;[0;1;30;90;47m8[0;30;5;40;100m8[0;1;37;97;47mt[0;37;5;47;107m    8[0;1;30;90;40m8[0;31;40m [0;34;40m.[0;35;5;40;100m;[0;1;30;90;47mt[0;37;5;47;107m@:  .8[0;30;5;40;100m8[0;34;40m [0;34;5;40;100mS[0;33;5;40;100m .[0;30;5;40;100m8[0;1;30;90;40mS[0;31;40m:[0;1;30;90;40mS[0;30;5;40;100m8[0;36;5;40;100m.[0;37;5;40;100m@[0;1;30;90;47m:[0;1;37;97;47mX[0;37;5;47;107mX; 8[0;33;5;40;100m.[0;34;40m:[0;30;5;40;100mX[0;37;5;47;107m@[0;1;37;97;47m@[0;36;5;40;100m [0;1;30;90;47m:[0;1;37;97;47m;t%X[0;37;5;47;107m8: :[0;1;37;97;47m8[0;37;5;47;107m@:%[0;1;30;90;47m [0;31;5;40;100m%[0;32;40m.[0;30;5;40;100m@[0;1;30;90;47m;[0;37;5;47;107mSS; .S[0;1;37;97;47mX[0;1;30;90;47m:[0;1;37;97;47mS[0;37;5;47;107m.:[0;1;37;97;47mX[0;36;5;40;100m [0;1;30;90;40mS[0;34;40m.[0;1;30;90;40m@[0;32;40m;[0;34;40m;[0;1;30;90;47mX[0;37;5;47;107m:[0;1;37;97;47mS[0;1;30;90;47m; [0;1;37;97;47m@[0;37;5;47;107mS[0;37;5;40;100mS[0;34;40m.   [0m
[0;34;40m    [0;35;5;40;100mt[0;1;37;97;47m;[0;37;5;47;107m8[0;1;30;90;47m%@.[0;1;37;97;47m8[0;37;5;47;107m:   ..[0;1;37;97;47m%[0;30;5;40;100mS[0;32;40m.  :[0;30;5;40;100m@[0;37;5;40;100mS[0;1;37;97;47m [0;37;5;47;107m@t[0;36;5;40;100m [0;31;40m [0;33;5;40;100m%[0;37;5;47;107m@ :%8[0;1;37;97;47m:[0;1;30;90;47mX[0;35;5;40;100m ;[0;1;30;90;40mS[0;32;40m;[0;31;40m;[0;1;30;90;40m8[0;30;5;40;100mS[0;36;5;40;100m [0;1;30;90;47m8@:[0;37;5;47;107m8;    . .  [0;1;37;97;47m8[0;30;5;40;100mS[0;32;40m;[0;1;30;90;40m8[0;31;40mt[0;34;40m;[0;36;5;40;100mX[0;1;30;90;47m:[0;37;5;47;107m: .     :@[0;1;37;97;47m:[0;37;5;40;100mX[0;30;5;40;100m8[0;32;40m.[0;34;40m:[0;1;30;90;40m@[0;31;40m.[0;34;40mt[0;1;30;90;40mX[0;31;40m.[0;33;5;40;100m [0;37;5;47;107m:   :[0;37;5;40;100m8[0;34;40m:[0;32;40m [0;34;40m   [0m
[0;34;40m   [0;32;40m  :[0;30;5;40;100mS[0;1;30;90;47m8[0;1;37;97;47mS[0;37;5;47;107m88t .    %[0;37;5;40;100m8[0;32;40m:[0;31;40m.[0;30;5;40;100mX[0;35;5;40;100m [0;30;5;40;100m8[0;34;40m:[0;32;40m;[0;1;30;90;40m8[0;34;40mt[0;31;40m.[0;34;40m [0;1;30;90;40m@[0;1;30;90;47m%[0;37;5;47;107m%..   ::S[0;1;37;97;47m:[0;1;30;90;40m8[0;34;40m ;[0;1;30;90;40m8X[0;31;40m:[0;32;40m;[0;1;30;90;40m@[0;30;5;40;100m8[0;34;5;40;100m%[0;33;5;40;100m:[0;37;5;40;100mX[0;1;30;90;47m@;[0;1;37;97;47m.%X88S.;S[0;37;5;47;107m8SX@[0;1;37;97;47m8X:[0;1;30;90;47mS[0;37;5;40;100mX[0;35;5;40;100m:[0;1;30;90;40m@[0;32;40m:[0;31;40m;[0;30;5;40;100m8[0;1;30;90;40m8[0;31;40m.[0;1;30;90;40mS[0;1;37;97;47m:[0;33;5;40;100m.[0;31;40m.[0;32;40m. [0;30;5;40;100mS[0;37;5;47;107m%.  [0;1;37;97;47m%[0;1;30;90;40mS[0;32;40m.[0;31;40m [0;34;40m   [0m
[0;34;40m  [0;32;40m    [0;34;40m  [0;36;5;40;100m [0;1;37;97;47m;[0;37;5;47;107m:     .  :[0;1;30;90;47m8[0;34;40m.[0;31;40m;[0;1;30;90;47m:[0;37;5;47;107m;[0;1;37;97;47mS[0;35;5;40;100m [0;1;30;90;40mX[0;31;40m.[0;32;40m [0;34;40m  .[0;30;5;40;100m@[0;1;30;90;47mX[0;1;37;97;47m%[0;37;5;47;107mS.    [0;1;37;97;47m8[0;1;30;90;40m8[0;32;40m.[0;1;30;90;47m8[0;37;5;47;107mtX[0;1;37;97;47m@[0;1;30;90;47m.[0;37;5;40;100m8[0;33;5;40;100m.[0;1;30;90;40m8[0;31;40mt[0;32;40m:[0;34;40m.[0;31;40m   [0;34;40m :[0;31;40m;[0;34;40mt[0;1;30;90;40m@X@@XS[0;31;40m;[0;34;40m;[0;32;40m.[0;31;40m  [0;32;40m  .[0;35;5;40;100mt[0;1;37;97;47m.[0;37;5;47;107m;[0;1;37;97;47m@[0;1;30;90;40mS[0;32;40m:[0;1;37;97;47m%t[0;32;40m;[0;31;40m.[0;34;40m [0;30;5;40;100m8[0;37;5;47;107mX   [0;1;30;90;47m%[0;31;40m.[0;34;40m     [0m
[0;34;40m  [0;32;40m     [0;34;40m [0;1;30;90;40mS[0;1;30;90;47m8[0;37;5;47;107mt.  .    .;[0;1;30;90;47mX[0;34;40mt[0;32;40m%[0;1;30;90;47mX[0;37;5;47;107m; [0;1;37;97;47m@[0;30;5;40;100mS[0;32;40m  .[0;31;40m  [0;32;40m :[0;1;30;90;40m@[0;30;5;40;100mS[0;37;5;40;100mS[0;1;30;90;47m:[0;1;37;97;47m@[0;37;5;47;107mS[0;1;30;90;47mS[0;34;40m.[0;1;30;90;40m8[0;1;37;97;47m8[0;37;5;47;107m  . .:;S[0;1;37;97;47mS[0;36;5;40;100m [0;34;40m.[0;31;40m.[0;36;5;40;100m [0;1;30;90;47m@8[0;37;5;40;100m8S[0;33;5;40;100m.[0;30;5;40;100mX[0;32;40m.[0;31;40m.[0;1;30;90;40mS[0;36;5;40;100m:[0;33;5;40;100m [0;37;5;40;100m8[0;1;30;90;47m@t:[0;33;5;40;100m [0;32;40m;[0;34;40m:[0;1;30;90;47m%[0;37;5;47;107m  [0;1;37;97;47m8[0;1;30;90;40m@[0;34;40m.[0;1;30;90;47mt[0;37;5;47;107m8[0;1;30;90;40m8[0;34;40m.[0;31;40m [0;30;5;40;100m8[0;37;5;47;107m@ . [0;1;30;90;47m@[0;32;40m.[0;34;40m     [0m
[0;34;40m [0;32;40m       [0;34;40m  [0;36;5;40;100m;[0;1;37;97;47m8[0;37;5;47;107m    .     [0;1;37;97;47m8[0;35;5;40;100m%[0;32;40m [0;30;5;40;100mX[0;1;37;97;47m%[0;37;5;47;107m.[0;1;30;90;47mS[0;32;40m [0;1;30;90;40m8[0;1;37;97;47mt.[0;37;5;40;100m8[0;35;5;40;100m%[0;32;40mt[0;34;40m:[0;31;40m.[0;32;40m  .[0;34;40m;[0;32;40m;[0;31;40m:[0;1;30;90;40m@[0;1;30;90;47mS[0;1;37;97;47mS[0;37;5;47;107m8t..  . [0;1;37;97;47m@[0;1;30;90;40mS[0;32;40mt[0;1;37;97;47mX[0;37;5;47;107m     S[0;30;5;40;100mS[0;31;40m [0;37;5;40;100m8[0;37;5;47;107m.     ;[0;35;5;40;100m:[0;32;40m [0;33;5;40;100m.[0;37;5;47;107m%@[0;37;5;40;100m8[0;32;40m.[0;31;40m.[0;34;40m;[0;1;30;90;40mX[0;31;40m.  [0;30;5;40;100m8[0;37;5;47;107mX   [0;1;30;90;47m8[0;34;40m      [0m
[0;34;40m [0;32;40m       [0;34;40m [0;31;40m  [0;35;5;40;100m.[0;37;5;47;107mt .   .  ..@[0;33;5;40;100m.[0;32;40m [0;1;30;90;40mS[0;1;30;90;47m8[0;33;5;40;100m:[0;34;40m [0;35;5;40;100m.[0;37;5;47;107mt .:S[0;1;30;90;47m:[0;33;5;40;100m [0;1;30;90;40m@[0;34;40m.[0;31;40m...[0;34;40m.[0;32;40m  .[0;31;40m;[0;30;5;40;100m8[0;36;5;40;100m.[0;37;5;40;100mX[0;1;30;90;47m%.[0;1;37;97;47m:t[0;1;30;90;47m8[0;34;40m:[0;31;40m:[0;35;5;40;100m [0;1;37;97;47m.:;;;[0;1;30;90;47m:[0;30;5;40;100m8[0;31;40m [0;30;5;40;100m@[0;1;30;90;47m; .:X[0;37;5;40;100mX[0;33;5;40;100m:[0;34;40m;[0;32;40m.:[0;30;5;40;100m@[0;1;30;90;40m%[0;34;40m.[0;31;40m [0;32;40m.[0;31;40m.[0;32;40m   [0;31;40m [0;30;5;40;100m@[0;37;5;47;107mS . [0;37;5;40;100m8[0;34;40m      [0m
[0;34;40m  [0;32;40m      [0;31;40m   :[0;37;5;40;100m8[0;37;5;47;107m:   .     .S[0;1;30;90;47m8[0;1;30;90;40mS[0;34;40m  t[0;1;37;97;47mS[0;37;5;47;107m.     :X[0;1;37;97;47m@[0;1;30;90;47m@[0;1;30;90;40mX[0;32;40m :[0;1;30;90;40mS[0;31;40m.[0;34;40m.[0;32;40m.[0;34;40m  [0;31;40m  [0;34;40m.[0;32;40m:[0;34;40m:[0;31;40m.[0;32;40m.[0;34;40m.[0;31;40m.[0;34;40m:[0;32;40m:[0;31;40m:[0;32;40m:[0;34;40m:[0;31;40m:[0;32;40m.[0;34;40m [0;31;40m .[0;32;40m.[0;31;40m.[0;32;40m.[0;31;40m.[0;34;40m  [0;31;40m..[0;34;40m.[0;31;40m [0;32;40m  [0;31;40m  [0;32;40m     [0;33;5;40;100m%[0;37;5;47;107mX   [0;1;30;90;47m8[0;34;40m      [0m
[0;34;40m  [0;32;40m    [0;34;40m [0;31;40m    [0;32;40m.[0;34;40m.[0;37;5;40;100mS[0;37;5;47;107m%    .     .X[0;1;30;90;47m@[0;30;5;40;100m8[0;32;40m:[0;33;5;40;100m%[0;1;30;90;47mS[0;37;5;47;107mX. .    8[0;30;5;40;100m8[0;34;40m [0;1;30;90;47m8[0;37;5;47;107m%8[0;1;37;97;47m.[0;1;30;90;47m@[0;36;5;40;100m [0;30;5;40;100mS[0;1;30;90;40mX[0;31;40m:[0;34;40m.[0;31;40m.[0;32;40m.[0;31;40m.[0;34;40m [0;32;40m .[0;31;40m.[0;34;40m..[0;31;40m.[0;32;40m.[0;34;40m      .[0;31;40m [0;34;40m   [0;32;40m      [0;31;40m  [0;32;40m  .[0;31;40m.[0;34;40m [0;36;5;40;100m [0;37;5;47;107m   .[0;1;30;90;47m8[0;34;40m      [0m
[0;34;40m       [0;31;40m      [0;32;40m.[0;35;5;40;100m [0;37;5;47;107m: .   .  .  .X[0;37;5;40;100mS[0;34;40m;[0;31;40m [0;1;30;90;40m8[0;1;30;90;47mt[0;37;5;47;107mt   ..[0;1;30;90;47mS[0;32;40m.[0;34;40m:[0;1;37;97;47m:[0;37;5;47;107m .   .%[0;1;37;97;47mX[0;1;30;90;47m8[0;30;5;40;100m8[0;34;40m  .[0;31;40m    [0;32;40m  [0;34;40m       [0;31;40m  [0;34;40m   [0;32;40m      [0;31;40m  [0;32;40m [0;31;40m.[0;36;5;40;100m;[0;31;40m;[0;34;40m;[0;1;30;90;47m:[0;37;5;47;107m .  [0;37;5;40;100m8[0;34;40m      [0m
[0;34;40m       [0;31;40m      .[0;34;40m:[0;33;5;40;100m.[0;1;37;97;47mX[0;37;5;47;107m.  .        :[0;1;37;97;47m@[0;37;5;40;100mX[0;1;30;90;40m@[0;34;40m.[0;30;5;40;100m8[0;37;5;40;100m8[0;1;37;97;47m@[0;37;5;47;107m; [0;1;37;97;47m@[0;1;30;90;40m8[0;31;40m:[0;37;5;40;100mS[0;37;5;47;107mt      . .[0;1;37;97;47m.[0;32;40m:[0;31;40m;[0;1;30;90;47mt[0;1;37;97;47mt[0;1;30;90;47m.S8[0;37;5;40;100m@[0;33;5;40;100m [0;36;5;40;100m [0;34;40m:[0;32;40m.[0;31;40m.[0;1;30;90;40mS@@X[0;34;40m:[0;32;40m.[0;31;40m :[0;30;5;40;100m@[0;36;5;40;100mt[0;30;5;40;100m8[0;31;40m.[0;32;40m [0;30;5;40;100mS[0;37;5;40;100m8[0;32;40m;[0;34;40m:[0;33;5;40;100m:[0;35;5;40;100m%[0;32;40m.[0;32;5;40;100mX[0;37;5;47;107m@    [0;1;30;90;47m8[0;34;40m      [0m
[0;34;40m        [0;31;40m    [0;34;40m [0;32;40m.[0;31;40m.[0;34;40m :[0;33;5;40;100m.[0;1;37;97;47mt[0;37;5;47;107m: :;.  .t:   t[0;1;37;97;47mt[0;33;5;40;100m [0;1;30;90;40m8[0;32;40m.[0;1;30;90;40m8[0;35;5;40;100m:[0;1;30;90;40mS[0;32;40m.[0;36;5;40;100m%[0;37;5;47;107m8    .     [0;1;37;97;47m;[0;34;40mt[0;1;30;90;40m8[0;37;5;47;107m8   .  X[0;30;5;40;100m8[0;31;40m.[0;36;5;40;100m [0;37;5;47;107mt:;;[0;1;30;90;47m8[0;31;40m.[0;1;30;90;40mX[0;1;37;97;47m:[0;37;5;47;107m:[0;1;37;97;47m8[0;30;5;40;100m@[0;31;40m [0;33;5;40;100m [0;37;5;47;107mS[0;1;30;90;47m;[0;31;40mt[0;1;30;90;40mX[0;30;5;40;100mS[0;32;40m.[0;30;5;40;100m8[0;1;37;97;47m8[0;37;5;47;107m . . [0;1;30;90;47m8[0;34;40m      [0m
[0;34;40m          [0;31;40m  [0;34;40m   [0;32;40m .[0;31;40m :[0;37;5;40;100m8[0;37;5;47;107mS%[0;1;30;90;47mt;[0;37;5;47;107m@. [0;1;37;97;47m8[0;1;30;90;47m;.[0;37;5;47;107m@.   X[0;1;30;90;47mX[0;30;5;40;100m8[0;32;40m. [0;31;40m t[0;37;5;40;100mS[0;1;37;97;47m:[0;37;5;47;107mX:   .   [0;1;37;97;47m.[0;32;40m;[0;30;5;40;100m8[0;37;5;47;107m8 .    [0;1;37;97;47m;[0;32;40m:[0;34;40mt[0;1;37;97;47m@[0;37;5;47;107m.  [0;1;37;97;47m8[0;1;30;90;40m8[0;32;40m [0;36;5;40;100m [0;37;5;47;107m .[0;37;5;40;100mS[0;31;40m [0;1;30;90;40m8[0;1;37;97;47m8[0;37;5;47;107m.[0;1;37;97;47m8[0;36;5;40;100m [0;34;40m;[0;32;40m.[0;30;5;40;100m8[0;1;37;97;47m@[0;37;5;47;107m     .[0;1;30;90;47m8[0;34;40m.[0;31;40m   [0;32;40m [0;34;40m [0m
[0;34;40m           [0;31;40m [0;34;40m  [0;32;40m [0;31;40m  [0;34;40m .[0;32;40m.[0;30;5;40;100m8[0;1;30;90;47m8[0;1;37;97;47m88[0;1;30;90;47m:%:[0;1;37;97;47m@[0;37;5;47;107mS[0;1;37;97;47m8:[0;1;30;90;47mt:[0;1;37;97;47m%[0;37;5;47;107mS..%[0;1;37;97;47mS[0;1;30;90;47mS[0;35;5;40;100m [0;30;5;40;100mS[0;1;30;90;40mS[0;31;40m;[0;1;30;90;40mX[0;30;5;40;100m@[0;36;5;40;100m:[0;37;5;40;100m%[0;1;30;90;47mX.[0;1;37;97;47mt@@[0;37;5;40;100mS[0;31;40m.[0;1;30;90;40m@[0;1;37;97;47m@[0;37;5;47;107m:.. .;[0;37;5;40;100m@[0;31;40m:[0;30;5;40;100m@[0;37;5;47;107m@.:[0;1;37;97;47m8[0;33;5;40;100m:[0;34;40m t[0;1;37;97;47m.@[0;1;30;90;47m;[0;1;30;90;40m8[0;31;40m [0;1;30;90;40m@[0;33;5;40;100m;[0;30;5;40;100m8[0;1;30;90;40mSS[0;30;5;40;100mX[0;1;30;90;47m8[0;37;5;47;107m8. . .  [0;1;30;90;47mS[0;31;40m.[0;32;40m    [0;34;40m [0m
[0;34;40m               [0;31;40m  [0;32;40m  [0;34;40m   :[0;30;5;40;100mX[0;1;30;90;47m8[0;1;37;97;47mS[0;37;5;47;107m@[0;1;37;97;47mS[0;1;30;90;47m;%.[0;1;37;97;47m@[0;37;5;47;107mX[0;1;37;97;47m8.[0;1;30;90;47m%t[0;1;37;97;47m:8[0;37;5;47;107m: .;@[0;1;37;97;47m%[0;1;30;90;47m;8[0;35;5;40;100m  [0;30;5;40;100mS8[0;1;30;90;40m@X[0;31;40m;[0;34;40m;[0;32;40m;[0;1;30;90;40mX8[0;30;5;40;100m8888[0;1;30;90;40m@[0;34;40m.[0;32;40m.[0;31;40m:[0;1;30;90;40m8[0;30;5;40;100m8[0;1;30;90;40m8[0;34;40m;[0;32;40m.[0;31;40m;[0;32;40mt[0;1;30;90;40mSX8[0;30;5;40;100mX[0;33;5;40;100m;[0;35;5;40;100m.[0;37;5;40;100mX[0;1;30;90;47mt[0;1;37;97;47m%[0;37;5;47;107mX:.  .    .[0;1;30;90;47m;[0;34;40m.[0;32;40m.[0;31;40m    [0m
[0;34;40m              [0;31;40m   [0;32;40m  [0;34;40m  [0;32;40m .[0;31;40m  .[0;1;30;90;40m@[0;36;5;40;100m [0;1;30;90;47m:[0;37;5;47;107m8X[0;1;37;97;47mX[0;1;30;90;47m:t:[0;1;37;97;47m%[0;37;5;47;107m@8[0;1;37;97;47m%[0;1;30;90;47m;%:[0;1;37;97;47mt[0;37;5;47;107m8;     .;;tSX8[0;1;37;97;47m888@XXXXXX@8[0;37;5;47;107m88X%t;.:.  . :[0;1;37;97;47mXX[0;37;5;47;107m.  ...  [0;1;37;97;47m:[0;31;40m:[0;34;40m.[0;32;40m    [0m
[0;34;40m                [0;32;40m   [0;34;40m [0;32;40m [0;31;40m   [0;32;40m   .[0;34;40m [0;31;40m;[0;36;5;40;100m [0;1;37;97;47mt[0;37;5;47;107m;.%[0;1;37;97;47m.[0;37;5;40;100m@8[0;1;37;97;47m.[0;37;5;47;107mX.%[0;1;37;97;47m;[0;37;5;40;100m8S[0;1;30;90;47mS[0;1;37;97;47mX[0;37;5;47;107m; .     .  .X8[0;1;37;97;47m8@@@@8[0;37;5;47;107m88;  .     .  8[0;37;5;40;100mX@[0;37;5;47;107mX   %[0;1;37;97;47mX[0;37;5;47;107m.  [0;1;37;97;47m%[0;1;30;90;40mS[0;32;40m     [0m
[0;34;40m                 [0;32;40m  [0;34;40m [0;31;40m     [0;32;40m [0;34;40m  .[0;32;40m.[0;34;40m.[0;32;40m:[0;30;5;40;100m8[0;35;5;40;100m [0;1;30;90;47mS[0;1;37;97;47mX[0;37;5;47;107mSX[0;1;37;97;47m@;[0;1;30;90;47m  [0;1;37;97;47m:t%;[0;1;30;90;47m ;;:[0;1;37;97;47m:%@[0;37;5;47;107m88XS%t%%%%tttt;;..   ..:S[0;1;37;97;47m8 [0;1;30;90;47mXS[0;1;37;97;47mS[0;37;5;47;107m;  .:[0;1;30;90;47m.:[0;37;5;47;107m   [0;1;37;97;47m8[0;1;30;90;40m@[0;32;40m     [0m
[0;34;40m                    [0;31;40m     [0;32;40m [0;34;40m [0;31;40m [0;32;40m [0;34;40m .[0;31;40m.[0;34;40m   [0;31;40m [0;1;30;90;40mS[0;30;5;40;100mS[0;37;5;40;100m8[0;1;37;97;47m:[0;37;5;47;107mX.;X[0;1;37;97;47m8%.[0;1;30;90;47m:;tt%t;[0;1;37;97;47m :;t;::  [0;1;30;90;47m.....  [0;1;37;97;47m. ..;:;%@[0;37;5;47;107mS: . ;[0;1;37;97;47m8[0;1;30;90;47mS[0;37;5;40;100m8[0;1;37;97;47m.[0;37;5;47;107m; . X[0;30;5;40;100m@[0;32;40m     [0m
[0;34;40m                    [0;31;40m     [0;32;40m [0;34;40m [0;32;40m [0;34;40m [0;32;40m  [0;34;40m     [0;31;40m [0;34;40m [0;31;40m [0;34;40m [0;1;30;90;40mX[0;36;5;40;100m [0;1;37;97;47mX[0;37;5;47;107m;     .t8[0;1;37;97;47m%[0;1;30;90;47m:@[0;37;5;40;100m88[0;1;30;90;47m@;[0;1;37;97;47m.X[0;37;5;47;107m8@t;:.. .           .%[0;1;37;97;47m:[0;36;5;40;100m [0;37;5;40;100m@[0;1;37;97;47mX[0;37;5;47;107m  .   @[0;30;5;40;100m8[0;32;40m     [0m
[0;34;40m                      [0;31;40m  [0;34;40m   [0;32;40m     [0;34;40m        .[0;32;40m.[0;31;40m;[0;30;5;40;100m@[0;36;5;40;100m [0;1;37;97;47m.[0;37;5;47;107m8;.      .;S8[0;1;37;97;47m8@S%tt;;;;;;;:;;;ttt;.[0;1;30;90;47m.;[0;1;37;97;47m:[0;37;5;47;107m@:     .:[0;1;30;90;47m8[0;34;40m.[0;32;40m     [0m
[0;34;40m                            [0;32;40m    [0;34;40m       [0;32;40m [0;31;40m.[0;34;40m.[0;32;40m.[0;34;40m.[0;32;40m.[0;34;40m .[0;1;30;90;40m8[0;33;5;40;100m [0;1;30;90;47m8[0;1;37;97;47mt[0;37;5;47;107m8t..               .                .  .8[0;35;5;40;100m [0;31;40m:[0;32;40m.[0;34;40m [0;32;40m   [0;34;40m [0m
[0;34;40m                            [0;32;40m    [0;34;40m      [0;32;40m      [0;34;40m.[0;31;40m [0;32;40m     ;[0;30;5;40;100mS[0;35;5;40;100m [0;1;30;90;47m8[0;1;37;97;47m.8[0;37;5;47;107mt.   .  .  .    . . . .  .  .   .[0;1;37;97;47m%[0;30;5;40;100m8[0;34;40m.[0;32;40m.[0;34;40m      [0m
[0;34;40m                                      [0;32;40m            [0;31;40m.[0;34;40m.[0;31;40m.[0;34;40m.[0;31;40m .[0;34;40mt[0;1;30;90;40m8[0;30;5;40;100mS[0;33;5;40;100m [0;37;5;40;100m@[0;1;30;90;47m%[0;1;37;97;47m;8[0;37;5;47;107mS;.    .        .     :8[0;1;30;90;47m.[0;35;5;40;100m [0;1;30;90;40m%[0;31;40m [0;32;40m [0;34;40m       [0m
[0;34;40m                                      [0;32;40m           [0;34;40m    [0;31;40m [0;32;40m .[0;31;40m:[0;32;40m.[0;31;40m     .[0;32;40m;[0;1;30;90;40m8[0;30;5;40;100m@[0;35;5;40;100m.[0;33;5;40;100m [0;37;5;40;100m8[0;1;30;90;47mXt:[0;1;37;97;47m.:;;;ttt; [0;1;30;90;47mt[0;37;5;40;100m8[0;36;5;40;100m [0;1;30;90;40m8[0;34;40m:[0;31;40m.[0;32;40m.[0;34;40m [0;31;40m  [0;34;40m       [0m
[0;34;40m                                       [0;32;40m          [0;34;40m    [0;31;40m  [0;32;40m .[0;31;40m     [0;32;40m [0;34;40m.[0;31;40m.[0;34;40m  [0;32;40m .[0;31;40m   [0;32;40m [0;31;40m    [0;34;40m         [0;32;40m .[0;31;40m     [0;34;40m       [0m";

            core.WriteLine(new Core.MessageProperties { Time = null, Label = null }, trolling);
            core.Delay(1000);

            for (var x =0;x < 40; x++)
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
