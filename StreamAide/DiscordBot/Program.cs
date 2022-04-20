using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JSON;
using RiotAPI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Mabot {

    public class Mabot {

        public static Auth auth { get; private set; }
        public static ModerationService moderation { get; private set; }
        public static StreamTrackService stream_track_service { get; private set; }

        public static string PREFS_PATH { get { return "Prefs/"; } }

        private DiscordSocketClient client;
        private CommandHandler command;
        private LoggingService logger;
        private CommandService command_service;

        public static void Main(string[] args) => new Mabot().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            client = new DiscordSocketClient();

            client.Ready += LoadServices;

            command_service = new CommandService();

            auth = new Auth("auth.json");

            command = new CommandHandler(client, command_service);
            logger = new LoggingService(client, command_service);



            await client.LoginAsync(TokenType.Bot, auth.DiscordToken);
            await client.StartAsync();

            await command.InstallCommandsAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task LoadServices() {
            moderation = new ModerationService(client);
            //stream_track_service = new StreamTrackService(client, auth.TwitchToken);
            //stream_track_service.StartTrack();
            await Task.Yield();
        }
    }

    public class Auth {

        public RiotApi RiotApi { get; private set; }
        public string DiscordToken { get; private set; }
        public string TwitchToken { get; private set; }

        public Auth(string filename) {
            string json_text = File.ReadAllText(filename);
            JSONObject json = Parser.Parse(json_text);

            RiotApi = new RiotApi(json["RiotApiKey"].string_value);
            DiscordToken = json["DiscordToken"].string_value;
            TwitchToken = json["Twitch"].string_value;
        }
    }
}
