using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using static Mabot.Mabot;
using Discord;
using System.Collections.Generic;
using static RiotAPI.RiotApi;

namespace Mabot {
    public class OverlordModule : ModuleBase<SocketCommandContext> {
        [Command("bot")]
        [Summary("Assert who is in charge.")]
        public async Task SquareAsync() {
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync($"I for one welcome our new robot overlords.");
        }
    }

    public class BaseModule : ModuleBase<SocketCommandContext> {
        [Command("userinfo")]
        [Summary("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfoAsync (
            [Summary("The (optional) user to get info from")]
            SocketUser user) {
            if (user == null) {
                await ReplyAsync("User does not exist in Server.");
            } else {
                await ReplyAsync($"{user.Username}#{user.Discriminator}");
            }
        }

        [Command("Help")]
        [Summary("Returns Command List")]
        public async Task HelpAsync() {
            await ReplyAsync("!help\n!rank (summoner name)\n!rankoce (summoner name)\n!rankeuw (summoner name)\n!user (discord username)");
        }
    }

    public class ModeratorModule : ModuleBase<SocketCommandContext> {
        [Command("filter")]
        [Summary("prints the language filter")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Filter() {
            await ReplyAsync("Filter: " + moderation.GetFilter(Context.Guild.Id));
        }
        [Command("filterclear")]
        [Summary("Clears the language filter")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task FilterClear() {
            moderation.ClearWordFilter(Context.Guild.Id);
            await ReplyAsync("Filter Cleared.");
        }
        [Command("filterremove")]
        [Summary("Adds a word to the language filter")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task RemoveFilter(
        [Summary("Word to remove from filter")]
            string word) {
            if (moderation.RemoveWordFilter(Context.Guild.Id, word)) {
                await ReplyAsync(word + " removed from language filter.");
            } else {
                await ReplyAsync(word + " not found in language filter");
            }
        }
        [Command("filteradd")]
        [Summary("Adds a word to the language filter")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task AddFilter(
        [Summary("Word to add to filter")]
            string word) {
            moderation.AddWordFilter(Context.Guild.Id, word);
            await ReplyAsync("Added " + word + " to language filter");
        }

        [Command("purge")]
        [Summary("Deletes all messages from a user in all channels")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge (
            [Summary("The user to moderate")]
            SocketUser user) {
            await PurgeUserMessages(user);
        }

        [Command("purge")]
        [Summary("Deletes X messages from a user in current channels")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(
        [Summary("The user to moderate")] SocketUser user,
        [Summary("The amount of messages to delete.")] int count) {
            await PurgeUserMessages(user, Context.Channel, count);
        }

        private async Task PurgeUserMessages(SocketUser user) {
            foreach (SocketChannel c in Context.Guild.Channels) {
                IMessageChannel channel = c as IMessageChannel;
                if (channel == null) {
                    continue;
                }
                if (await channel.GetUserAsync(user.Id) == null) {
                    continue;
                }
                await PurgeUserMessages(user, channel, -1);
            }
        }

        private async Task PurgeUserMessages(SocketUser user, IMessageChannel channel, int limit = -1) {
            List<IMessage> to_delete = new List<IMessage>();
            List<IMessage> messages = new List<IMessage>(await channel.GetMessagesAsync(50).FlattenAsync());
            while (messages.Count != 0 && (limit == -1 || to_delete.Count < limit)) {
                foreach (IMessage message in messages) {
                    if (message.Author.Id == user.Id) {
                        to_delete.Add(message);
                        if (to_delete.Count >= limit) {
                            break;
                        }
                    }
                }
                messages = new List<IMessage>(await channel.GetMessagesAsync(messages[messages.Count - 1], Direction.Before, 50).FlattenAsync());
            }

            foreach (IMessage message in to_delete) {
                await channel.DeleteMessageAsync(message.Id);
            }
        }
    }

    public class RiotModule : ModuleBase<SocketCommandContext> {
        [Command("rank")]
        [Summary("Returns rank info about the given user.")]
        [Alias("lolrank")]
        public async Task UserInfoAsync(
        [Summary("The League Of Legends user to get info of")]
            string user, params string[] user_array) {
            try {
                for (int i = 0; i < user_array.Length; i++) {
                    user += " " + user_array[i];
                }
                var info = (await auth.RiotApi.GetSummonerRankedInfo(user));
                await ReplyAsync(info.data.ToString());
            } catch (Exception e) {
                await ReplyAsync("Error Retrieving user Data");
            }            
        }

        [Command("rankoce")]
        [Summary("Returns rank info about the given user.")]
        [Alias("lolrankoce")]
        public async Task UserOCEInfoAsync(
        [Summary("The League Of Legends user to get info of")]
            string user, params string[] user_array) {
            try {
                for (int i = 0; i < user_array.Length; i++) {
                    user += " " + user_array[i];
                }
                RankedInfo info = (await auth.RiotApi.GetSummonerRankedInfo(user, "OC1")).data;
                await ReplyAsync(info.ToString());
            } catch (Exception e) {
                await ReplyAsync("Error Retrieving user Data");
            }
        }

        [Command("rankeuw")]
        [Summary("Returns rank info about the given user.")]
        [Alias("lolrankeuw")]
        public async Task UserEUWInfoAsync(
        [Summary("The League Of Legends user to get info of")]
            string user, params string[] user_array) {
            try {
                for (int i = 0; i < user_array.Length; i++) {
                    user += " " + user_array[i];
                }
                RankedInfo info = (await auth.RiotApi.GetSummonerRankedInfo(user, "EUW1")).data;
                await ReplyAsync(info.ToString());
            } catch (Exception e) {
                await ReplyAsync("Error Retrieving user Data");
            }
        }
    }

    public class TwitchModule : ModuleBase<SocketCommandContext> {
        [Command("trackstream")]
        [Summary("Adds a stream to the stream tracker")]
        [Alias("track")]
        public async Task TrackStreamAsync(
        [Summary("The name of the stream")]
            string user) {
            try {
                if (stream_track_service.AddStream(user, Context.Channel.Id, Context.Guild.Id)) {
                    await ReplyAsync("Added https://www.twitch.tv/" + user + " to tracker.");
                } else {
                    await ReplyAsync("Stream already in tracker. (Possibly in another channel)");
                }
            } catch (Exception e) {
                await ReplyAsync("Error");
            }
        }

        [Command("untrackstream")]
        [Summary("Removes a stream from the stream tracker")]
        [Alias("untrack")]
        public async Task UntrackStreamAsync(
        [Summary("The name of the stream")]
            string user) {
            try {
                if (stream_track_service.RemoveStream(user, Context.Guild.Id)) {
                    await ReplyAsync("Removed https://www.twitch.tv/" + user + " from tracker.");
                } else {
                    await ReplyAsync("Stream not found in tracker.");
                }
            } catch (Exception e) {
                await ReplyAsync("Error");
            }
        }
    }
}
