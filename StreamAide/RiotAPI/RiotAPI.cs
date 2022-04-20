using JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static RiotAPI.RiotApi;

namespace RiotAPI {
    public class RiotApi {
        private string api_key;
        public static RuneData rune_data { get; private set; }

        public class Runes {
            public int primary_tree_id { get; private set; }
            public int secondary_tree_id { get; private set; }
            public int[] primary_rune_ids { get; private set; }
            public int[] secondary_rune_ids { get; private set; }
            public int[] tertiary_rune_ids { get; private set; }

            public Runes(JSONObject runes) {
                primary_tree_id = runes["perkStyle"].int_value;
                secondary_tree_id = runes["perkSubStyle"].int_value;

                primary_rune_ids = new int[4];
                secondary_rune_ids = new int[2];
                tertiary_rune_ids = new int[3];

                var rune_array = runes["perkIds"];

                primary_rune_ids[0] = rune_array[0].int_value;
                primary_rune_ids[1] = rune_array[1].int_value;
                primary_rune_ids[2] = rune_array[2].int_value;
                primary_rune_ids[3] = rune_array[3].int_value;
                secondary_rune_ids[0] = rune_array[4].int_value;
                secondary_rune_ids[1] = rune_array[5].int_value;
                tertiary_rune_ids[0] = rune_array[6].int_value;
                tertiary_rune_ids[1] = rune_array[7].int_value;
                tertiary_rune_ids[2] = rune_array[8].int_value;
            }

            public override string ToString() {
                string to_ret = "";

                for (int i = 0; i < primary_rune_ids.Length; i++) {
                    to_ret += rune_data.GetRune(primary_rune_ids[i])["name"].string_value + Environment.NewLine;
                }
                for (int i = 0; i < secondary_rune_ids.Length; i++) {
                    to_ret += rune_data.GetRune(secondary_rune_ids[i])["name"].string_value + Environment.NewLine;
                }
                for (int i = 0; i < tertiary_rune_ids.Length; i++) {
                    to_ret += rune_data.GetRune(tertiary_rune_ids[i])["name"].string_value + Environment.NewLine;
                }

                return to_ret;
            }
            public string ToStringOneLine() {
                string to_ret = "";

                for (int i = 0; i < primary_rune_ids.Length; i++) {
                    to_ret += rune_data.GetRune(primary_rune_ids[i])["name"].string_value + ", ";
                }
                to_ret = to_ret.Substring(0, to_ret.Length - 2);
                to_ret += " | ";
                for (int i = 0; i < secondary_rune_ids.Length; i++) {
                    to_ret += rune_data.GetRune(secondary_rune_ids[i])["name"].string_value + ", ";
                }
                to_ret = to_ret.Substring(0, to_ret.Length - 2);
                to_ret += " | ";
                for (int i = 0; i < tertiary_rune_ids.Length; i++) {
                    to_ret += rune_data.GetRune(tertiary_rune_ids[i])["name"].string_value + ", ";
                }

                to_ret = to_ret.Substring(0, to_ret.Length - 2);

                return to_ret;
            }
        }

        public class MatchInfo : Data {

            public MatchInfo(JSONObject data) : base(data) {
                
            }

            public MatchInfo() {

            }

            public Participant GetParticipantById(string id) {
                return new Participant(FindParticipant(data, id));
            }

            public Participant GetParticipantByName(string name) {
                return new Participant(FindParticipantByName(data, name));
            }

            internal override void LoadData(JSONObject data) {

            }

            public class Participant : SummonerInfo {
                public Runes runes { get; private set; }

                public Participant(JSONObject json) : base() {
                    LoadData(json);
                }

                public Participant() : base() {

                }

                internal override void LoadData(JSONObject json) {
                    try {
                        summoner_name = json["summonerName"].string_value;
                        summoner_id = json["summonerId"].string_value;
                        runes = new Runes(json["perks"]);

                        found = true;
                    } catch (System.Exception e) {
                        found = false;
                    }
                }

                public override string ToString() {
                    return "Summoner: " + summoner_name + Environment.NewLine + runes.ToString();
                }
                public string ToStringOneLine() {
                    return "Summoner: " + summoner_name + " Runes: " + runes.ToStringOneLine();
                }
            }
        }

        public class SummonerInfo : Data {
            public string summoner_name { get; protected set; }
            public string summoner_id { get; protected set; }

            public bool found { get; protected set; }

            public static SummonerInfo GetNotFoundSummoner(string name) {
                return new SummonerInfo(name);
            }

            public SummonerInfo(JSONObject json) : base (json) {
                LoadData(json);
            }

            public SummonerInfo(string name, string id) {
                summoner_name = name;
                summoner_id = id;
                found = true;
            }

            public SummonerInfo() {
                found = false;
            }

            SummonerInfo(string name) {
                summoner_name = name;
                found = false;
            }

            internal override void LoadData(JSONObject json) {
                try {
                    summoner_name = json["name"].string_value;
                    summoner_id = json["id"].string_value;
                    found = true;
                } catch (System.Exception e) {
                    found = false;
                }
            }
        }

        public class RankedInfo : SummonerInfo {
            public class Queue {
                public enum Type { SoloDuo, Flex, None }

                public Type queue_type { get; private set; }

                public int lp { get; private set; }
                public string tier { get; private set; }
                public string rank { get; private set; }

                public bool is_in_promos { get; private set; }
                public string promo_progress { get; private set; }

                public int wins { get; private set; }
                public int losses { get; private set; }

                public Queue(JSONObject queue_info) {
                    queue_type = ParseQueueType(queue_info["queueType"].string_value);
                    lp = queue_info["leaguePoints"].int_value;
                    tier = queue_info["tier"].string_value;
                    rank = queue_info["rank"].string_value;

                    wins = queue_info["wins"].int_value;
                    losses = queue_info["losses"].int_value;

                    is_in_promos = queue_info.HasValue("miniSeries");
                    if (is_in_promos) {
                        promo_progress = "";
                        foreach (char character in queue_info["miniSeries"]["progress"].string_value) {
                            if (character == 'W') {
                                promo_progress += "O ";
                            } else if (character == 'L') {
                                promo_progress += "X ";
                            } else if (character == 'N') {
                                promo_progress += "- ";
                            }
                        }
                        promo_progress = promo_progress.Trim();
                    }
                }

                public override string ToString() {
                    string to_return =
                        "Queue: " + queue_type + "\n" +
                        tier + " " + rank + " " + lp + " LP\n";

                    if (is_in_promos) {
                        to_return += promo_progress + "\n";
                    }

                    to_return += wins + " Wins " + losses + " Losses (" + ((double)wins / (wins + losses)).ToString("p") + ")";

                    return to_return;
                }

                public string ToStringOneLine(bool include_queue_type = false) {
                    string to_return = "";
                    if (include_queue_type) {
                        to_return += queue_type.ToString() + " ";
                    }
                    to_return += tier + " " + rank + " " + lp + " LP ";

                    if (is_in_promos) {
                        to_return += "(" + promo_progress + ") ";
                    }

                    to_return += "(" + wins + " Wins : " + losses + " Losses) ";

                    return to_return;
                }

                private Type ParseQueueType(string queue) {
                    if (queue == "RANKED_SOLO_5x5") {
                        return Type.SoloDuo;
                    } else if (queue == "RANKED_FLEX_SR") {
                        return Type.Flex;
                    }
                    return Type.None;
                }
            }

            public Queue[] ranked_queues { get; private set; }


            public RankedInfo(JSONObject ranked_json) : base(ranked_json) {
                LoadData(ranked_json);
            }

            public RankedInfo() {

            }

            public override string ToString() {
                string to_return = "Summoner: " + summoner_name;
                if (ranked_queues == null || ranked_queues.Length == 0) {
                    to_return += "\n\nNo Ranked Info To Report";
                }
                foreach (Queue q in ranked_queues) {
                    to_return += "\n\n" + q.ToString();
                }
                return to_return;
            }

            public string ToStringOneLine() {
                string to_return = "Summoner: " + summoner_name + " ";
                if (ranked_queues != null && ranked_queues.Length > 0) {
                    foreach (Queue q in ranked_queues) {
                        to_return += q.ToStringOneLine(true);
                    }
                } else {
                    to_return += "  No Ranked Info To Report";
                }
                return to_return;
            }

            public string ToStringOneLine(Queue.Type type) {
                string to_return = "Summoner: " + summoner_name + " ";
                bool found = false;
                if (ranked_queues != null) {
                    foreach (Queue q in ranked_queues) {
                        if (type != q.queue_type) {
                            continue;
                        }
                        to_return += q.ToStringOneLine();
                        found = true;
                    }
                }                
                if (!found) {
                    to_return += "  No Ranked Info To Report";
                }
                return to_return;
            }

            internal override void LoadData(JSONObject ranked_json) {
                try {
                    summoner_name = ranked_json[0]["summonerName"].string_value;
                    summoner_id = ranked_json[0]["summonerId"].string_value;
                    ranked_queues = new Queue[ranked_json.count];
                    for (int i = 0; i < ranked_json.count; i++) {
                        ranked_queues[i] = new Queue(ranked_json[i]);
                    }
                    found = true;
                } catch (System.Exception e) {
                    found = false;
                }
            }
        }

        public class RuneData : Data {
            Dictionary<int, JSONObject> runes;

            internal RuneData() {
            }

            public RuneData(JSONObject rune_data) : base(rune_data) {
                LoadData(rune_data);
            }

            public JSONObject GetRune(int id) {
                if (runes.ContainsKey(id)) {
                    return runes[id];
                }
                return null;
            }

            internal override void LoadData(JSONObject rune_data) {
                runes = new Dictionary<int, JSONObject>();

                for (int i = 0; i < rune_data.count; i++) {
                    runes.Add(rune_data[i]["id"].int_value, rune_data[i]);
                }
            }
        }

        public abstract class Data {
            public JSONObject data { get; private set; }

            public Data(JSONObject data) {
                this.data = data;
                LoadData(data);
            }

            internal Data() {

            }

            internal static T LoadNewData<T>(JSONObject json) where T : Data, new() {
                T new_data_container = new T();
                new_data_container.data = json;
                new_data_container.LoadData(new_data_container.data);
                return new_data_container;
            }

            internal abstract void LoadData(JSONObject data);
        }

        public async Task<Response<MatchInfo>> GetMatchInfo(string summoner_name, string region = "NA1") {
            HttpClient client = new HttpClient();
            var summoner_info = await GetSummonerInfo(client, region, summoner_name, api_key);
            var match_info = await GetLiveMatchInfo(client, region, summoner_info.data.summoner_id, api_key);

            client.Dispose();

            return match_info;
        }

        public async Task<Response<MatchInfo>> GetMatchInfoById(string summoner_id, string region = "NA1") {
            HttpClient client = new HttpClient();  
            var match_info = await GetLiveMatchInfo(client, region, summoner_id, api_key);

            client.Dispose();

            return match_info;
        }

        public async Task<Response<SummonerInfo>> GetSummonerInfo(string summoner_name, string region = "NA1") {
            return await GetSummonerInfo(summoner_name, api_key, region);
        }

        public async Task<Response<RankedInfo>> GetSummonerRankedInfo(string summoner_name, string region = "NA1") {
            return await GetSummonerRankedInfo(summoner_name, api_key, region);
        }

        public async Task<Response<RankedInfo>> GetSummonerRankedInfoById(string summoner_id, string region = "NA1") {
            return await GetSummonerRankedInfoById(summoner_id, api_key, region);
        }

        internal static JSONObject FindParticipant(JSONObject obj, string participant_id) {
            for (int i = 0; i < obj["participants"].count; i++) {
                if (obj["participants"][i]["summonerId"].string_value == participant_id) {
                    return obj["participants"][i];
                }
            }
            return null;
        }

        internal static JSONObject FindParticipantByName(JSONObject obj, string participant_name) {
            for (int i = 0; i < obj["participants"].count; i++) {
                if (obj["participants"][i]["summonerName"].string_value.Equals(participant_name, StringComparison.CurrentCultureIgnoreCase)) {
                    return obj["participants"][i];
                }
            }
            return null;
        }

        private static async Task<Response<SummonerInfo>> GetSummonerInfo(string summoner_name, string api_key, string region = "NA1") {
            HttpClient client = new HttpClient();
            Response<SummonerInfo> info = await GetSummonerInfo(client, region, summoner_name, api_key);

            client.Dispose();

            return info;
        }

        private static async Task<Response<RankedInfo>> GetSummonerRankedInfo(string summoner_name, string api_key, string region = "NA1") {
            HttpClient client = new HttpClient();

            Response<SummonerInfo> summ = await GetSummonerInfo(client, region, summoner_name, api_key);
            if (summ.code != Response.Code.Success) {
                return new Response<RankedInfo>(summ.code);
            }

            Response<RankedInfo> rank = await GetRankedInfo(client, region, summ.data.summoner_id, api_key);

            client.Dispose();

            return rank;
        }

        private static async Task<Response<RankedInfo>> GetSummonerRankedInfoById(string summoner_id, string api_key, string region = "NA1") {
            HttpClient client = new HttpClient();

            Response<RankedInfo> rank = await GetRankedInfo(client, region, summoner_id, api_key);

            client.Dispose();

            return rank;
        }

        private static async Task<Response<SummonerInfo>> GetSummonerInfo(HttpClient client, string region, string summoner_name, string api_key) {
            HttpResponseMessage response = await client.GetAsync("https://" + region + ".api.riotgames.com/lol/summoner/v4/summoners/by-name/" + HttpUtility.HtmlEncode(summoner_name) + "?api_key=" + api_key);
            return await GetMessage<SummonerInfo>(response);
        }

        private static async Task<Response<RankedInfo>> GetRankedInfo(HttpClient client, string region, string summoner_id, string api_key) {
            HttpResponseMessage response = await client.GetAsync("https://" + region + ".api.riotgames.com/lol/league/v4/entries/by-summoner/" + HttpUtility.UrlEncode(summoner_id) + "?api_key=" + api_key);
            return await GetMessage<RankedInfo>(response);
        }

        private static async Task<Response<MatchInfo>> GetLiveMatchInfo(HttpClient client, string region, string summoner_id, string api_key) {
            HttpResponseMessage response = await client.GetAsync("https://" + region + ".api.riotgames.com/lol/spectator/v4/active-games/by-summoner/" + HttpUtility.UrlEncode(summoner_id) + "?api_key=" + api_key);
            return await GetMessage<MatchInfo>(response);
        }

        private static async Task<Response<T>> GetMessage<T>(HttpResponseMessage response) where T : Data, new() {
            return await Response<T>.GetRespone(response);
        }

        public RiotApi(string api_key) {
            this.api_key = api_key;
            rune_data = new RuneData(Parser.Parse(File.ReadAllText("runes.json")));
        }
    }

    public class Response {
        public enum Code { Error, Success, NotFound }
    }

    public class Response<T> : Response where T : Data, new() {

        public T data {
            get {
                if (_data == null && code == Code.Success) {
                    _data = Data.LoadNewData<T>(Parser.Parse(string_data));
                    data_loaded = true;
                }
                return _data;
            }
        }
        public bool data_loaded { get; private set; }

        public Code code { get; private set; }

        T _data;
        string string_data;

        public Response(Code code) {
            this.code = code;
        }

        Response() {

        }        

        public static async Task<Response<T>> GetRespone(HttpResponseMessage response) {
            Response<T> new_response = new Response<T>();

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                new_response.code = Code.Success;
            } else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                new_response.code = Code.NotFound;
            } else {
                new_response.code = Code.Error;
            }

            new_response.string_data = await response.Content.ReadAsStringAsync();

            return new_response;
        }

        public override string ToString() {
            return string_data;
        }
    }
}
