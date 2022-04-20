using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JSON;

namespace Mabot {

    public class LoggingService {
        public LoggingService(DiscordSocketClient client, CommandService command) {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }
        private Task LogAsync(LogMessage message) {
            if (message.Exception is CommandException cmdException) {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            } else
                Console.WriteLine($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }
    }

    public class StreamTrackService {
        public static string DIRECTORY { get { return Mabot.PREFS_PATH + "SteamTrack"; } }
        public static string PATH { get { return DIRECTORY + "/"; } } 

        static int DELAY = 3;

        DiscordSocketClient client;

        Dictionary<ulong, Dictionary<string, ulong>> tracked;
        Dictionary<string, bool> live;
        List<string> stream_list;

        bool tracking = false;
        bool continue_tracking = false;

        string client_id;

        public StreamTrackService(DiscordSocketClient client, string client_id) {
            tracked = new Dictionary<ulong, Dictionary<string, ulong>>();
            stream_list = new List<string>();
            live = new Dictionary<string, bool>();
            this.client_id = client_id;
            this.client = client;

            foreach (SocketGuild guild in client.Guilds) {
                LoadPrefs(guild.Id);
            }
        }

        public bool AddStream(string stream, ulong channel_id, ulong guild_id) {
            if (!tracked.ContainsKey(guild_id)) {
                tracked.Add(guild_id, new Dictionary<string, ulong>(new CaseInsensitiveStringComparer()));
            }
            if (!tracked[guild_id].ContainsKey(stream)) {
                tracked[guild_id].Add(stream, channel_id);
                if (AddIfNotDuplicate(stream)) {
                    live.Add(stream, false);
                }
                SavePrefs(guild_id);

                if (tracked.Count == 1) {
                    StartTrack();
                }
                return true;
            }

            return false;
        }

        public bool RemoveStream(string stream, ulong guild_id) {
            if (tracked[guild_id].ContainsKey(stream)) {
                tracked[guild_id].Remove(stream);
                bool is_last = false;

                foreach (SocketGuild guild in client.Guilds) {
                    if (tracked[guild.Id].ContainsKey(stream)) {
                        is_last = true;
                        break;
                    }
                }
                if (is_last) {
                    live.Remove(stream);
                    stream_list.Remove(stream);
                }
                SavePrefs(guild_id);
                return true;
            }
            return false;
        }

        public void StartTrack() {
            if (!tracking) {
                continue_tracking = true;
                Task.Run(async () => await TrackStreams()).GetAwaiter().GetResult();
            }
        }

        public void EndTrack() {
            if (tracking) {
                continue_tracking = false;
            }
        }

        void LoadPrefs(ulong guild) {
            Directory.CreateDirectory(DIRECTORY);
            if (!File.Exists(PATH + guild + ".streamprefs")) {
                SavePrefs(guild);
            }

            string prefs = File.ReadAllText(PATH + guild + ".streamprefs");

            tracked.Add(guild, new Dictionary<string, ulong>(new CaseInsensitiveStringComparer()));

            if (prefs == "") {
                return;
            }

            string[] streams = prefs.Split(",");
            foreach (string str in streams) {
                if (str=="") {
                    continue;
                }
                string[] split_str = str.Split(":");
                tracked[guild].Add(split_str[0], ulong.Parse(split_str[1]));
                AddIfNotDuplicate(split_str[0]);
                if (!live.ContainsKey(split_str[0])) {
                    live.Add(split_str[0], false);
                }
            }
        }

        void SavePrefs(ulong guild) {
            Directory.CreateDirectory(DIRECTORY);

            string prefs = "";
            foreach (var pair in tracked[guild]) {
                prefs += pair.Key + ":" + pair.Value + ",";
            }

            File.WriteAllText(PATH + guild + ".streamprefs", prefs);
        }

        bool AddIfNotDuplicate(string s) {
            if (!stream_list.Contains(s, new CaseInsensitiveStringComparer())) {
                stream_list.Add(s);
                return true;
            }
            return false;
        }


        async Task TrackStreams() {


            int count = 0;

            tracking = true;

            await Task.Yield();

            while (continue_tracking) {
                await Task.Delay(DELAY * tracked.Count * 1000);

                HttpClient web_client = new HttpClient();

                if (stream_list.Count == 0) {
                    EndTrack();
                }
                count = (count + 1) % stream_list.Count;

                string name = stream_list[count];
                HttpResponseMessage response = await GetResponse(web_client, GetStreamURLByUser(name));                

                if (!response.IsSuccessStatusCode) {
                    Console.WriteLine(response.StatusCode.ToString());
                    web_client.Dispose();
                    continue;
                }

                JSONObject data = Parser.Parse(await response.Content.ReadAsStringAsync());
                data = data["data"];

                if (data.is_table_array) {
                    // Stream Up
                    if (!live[name]) {
                        live[name] = true;

                        foreach (SocketGuild guild in client.Guilds) {
                            if (tracked[guild.Id].ContainsKey(name)) {
                                ISocketMessageChannel channel = client.GetChannel(tracked[guild.Id][name]) as ISocketMessageChannel;
                                await channel.SendMessageAsync((channel as IGuildChannel).Guild.EveryoneRole.Mention + " " + name + " is live!");
                                await channel.SendMessageAsync("https://www.twitch.tv/" + name);
                            }
                        }

                    }
                } else {
                    // Stream Down
                    live[name] = false;
                }

                web_client.Dispose();
            }



            await Task.Yield();

            tracking = false;
        }

        async Task<HttpResponseMessage> GetResponse(HttpClient client, string url) {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.TryAddWithoutValidation("Client-ID", client_id);
            request.RequestUri = new Uri(url);

            return await client.SendAsync(request);
        }

        string GetStreamURLByUser(string user) {
            return "https://api.twitch.tv/helix/streams?user_login=" + user;
        }
    }

    public class ModerationService {

        Regex white_space;

        DiscordSocketClient client;

        Dictionary<ulong, Dictionary<ulong, ChannelPermissions>> guild_to_channel_to_permisions;
        Dictionary<ulong, Regex> guild_to_filter;
        Dictionary<ulong, bool> guild_has_filter;

        public ModerationService(DiscordSocketClient client) {
            this.client = client;

            guild_to_channel_to_permisions = new Dictionary<ulong, Dictionary<ulong, ChannelPermissions>>();
            guild_to_filter = new Dictionary<ulong, Regex>();
            guild_has_filter = new Dictionary<ulong, bool>();

            white_space = new Regex("\\s");

            Console.WriteLine("Guilds" + " : " + client.Guilds.Count);
            foreach (SocketGuild guild in client.Guilds) {
                InitializeLanguageFilter(guild.Id);
                LoadLanguageFilter(guild.Id);
            }

            client.JoinedGuild += InitializeLanguageFilter;
            client.MessageReceived += ModerateLanguage;
        }

        public string GetFilter(ulong guild) {
            return guild_to_filter[guild].ToString();
        }

        public void ClearWordFilter(ulong guild) {
            SaveLanguageFilter(guild, new string[] { });
            LoadLanguageFilter(guild);
        }

        public void AddWordFilter(ulong guild, string word) {
            string text = File.ReadAllText("filters/" + guild + ".txt");
            text += Environment.NewLine + word;
            File.WriteAllText("filters/" + guild + ".txt", text);
            LoadLanguageFilter(guild);
        }

        public bool RemoveWordFilter(ulong guild, string word) {
            string text = File.ReadAllText("filters/" + guild + ".txt");
            List<string> words = new List<string>();
            words.AddRange(text.Split(Environment.NewLine, '\n'));

            bool removed = false;

            for (int i = words.Count - 1; i >= 0; i--) {
                if (words[i] == word) {
                    words.RemoveAt(i);
                    removed = true;
                }
            }
            SaveLanguageFilter(guild, words);
            LoadLanguageFilter(guild, words);
            return removed;
        }

        private void LoadLanguageFilter(ulong guild) {
            List<string> words = new List<string>();

            Directory.CreateDirectory("filters");
            if (File.Exists("filters/" + guild + ".txt")) {
                string text = File.ReadAllText("filters/" + guild + ".txt");
                words.AddRange(text.Split(Environment.NewLine, '\n'));
            } else {
                File.Create("filters/" + guild + ".txt");
            }

            LoadLanguageFilter(guild, words);
        }

        private void LoadLanguageFilter(ulong guild, IEnumerable<string> words) {
            string text = "";
            foreach (string word in words) {
                text += word + "|";
            }
            text = text.Trim('|');

            if (words.Count() == 0 || text == "") {
                guild_has_filter[guild] = false;
                guild_to_filter[guild] = new Regex("", RegexOptions.IgnoreCase);
                return;
            }

            Console.WriteLine(guild + " : " + text);
            guild_to_filter[guild] = new Regex(text, RegexOptions.IgnoreCase);
            guild_has_filter[guild] = true;
        }

        private void SaveLanguageFilter(ulong guild, IEnumerable<string> words) {
            string text = "";
            foreach (string word in words) {
                text += word + Environment.NewLine;
            }
            Directory.CreateDirectory("filters");
            File.WriteAllText("filters\\" + guild + ".txt", text);
        }

        private async Task InitializeLanguageFilter(SocketGuild guild) {
            InitializeLanguageFilter(guild.Id);
            LoadLanguageFilter(guild.Id);
            await Task.Yield();
        }

        private void InitializeLanguageFilter(ulong id) {
            Console.WriteLine("Initialiaze : " + id);
            guild_to_filter.Add(id, new Regex(""));
            guild_has_filter.Add(id, false);
        }

        private async Task ModerateLanguage(SocketMessage message) {
            var channel = message.Channel;
            var guild_channel = channel as IGuildChannel;
            if (message.Author.Id != client.CurrentUser.Id && guild_has_filter[guild_channel.GuildId]) {
                string content = message.Content;
                content = white_space.Replace(content, "");
                if (guild_to_filter[guild_channel.GuildId].IsMatch(content)) {
                    var author = message.Author;
                    if (await HasChannelPermissions(guild_channel)) {
                        await message.DeleteAsync();
                        await channel.SendMessageAsync($"{author.Mention} We don't use that kind of language here.");
                    }
                }                 
            }
        }

        private async Task<bool> HasChannelPermissions(IGuildChannel channel) {
            if (!guild_to_channel_to_permisions.ContainsKey(channel.GuildId)) {
                guild_to_channel_to_permisions.Add(channel.GuildId, new Dictionary<ulong, ChannelPermissions>());
            }
            if (!guild_to_channel_to_permisions[channel.GuildId].ContainsKey(channel.Id)) {
                var perms = (await channel.Guild.GetCurrentUserAsync()).GetPermissions(channel);

                guild_to_channel_to_permisions[channel.GuildId].Add(channel.Id, perms);
            }

            return guild_to_channel_to_permisions[channel.GuildId][channel.Id].ManageMessages;
        }
    }
}

public class CaseInsensitiveStringComparer : IComparer<string>, IEqualityComparer<string> {
    public int Compare(string x, string y) {
        return string.Compare(x, y, true);
    }

    public bool Equals(string x, string y) {
        return string.Compare(x, y, true) == 0;
    }

    public int GetHashCode(string obj) {
        return obj.ToLower().GetHashCode();
    }
}