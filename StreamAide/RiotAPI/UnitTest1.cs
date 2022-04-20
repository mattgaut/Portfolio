using JSON;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiotAPI;
using System.IO;
using static RiotAPI.RiotApi;

namespace RiotApiUnitTest {
    [TestClass]
    public class UnitTest1 {
        EnvironmentVariables variables = new EnvironmentVariables();

        [TestMethod]
        public void TestMethod0() {
            Assert.AreEqual(rune_data.GetRune(8010)["id"].int_value, 8010);
            Assert.AreEqual(rune_data.GetRune(8010)["name"].string_value, "Conqueror");
        }

        [TestMethod]
        public void TestMethod1() {
            JSONObject obj = Parser.Parse(File.ReadAllText("Test1.json"));
            MatchInfo match_info = new MatchInfo(obj);
            MatchInfo.Participant info = match_info.GetParticipantByName("dense terd");


            Assert.AreEqual(info.summoner_name, "dense terd");
            Assert.AreEqual(info.runes.primary_rune_ids[0], 8128);
            Assert.AreEqual(info.runes.secondary_rune_ids[0], 9111);
            Assert.IsNotNull(rune_data);
            Assert.IsNotNull(info.runes.ToString());
        }

        [TestMethod]
        public void TestMethod2() {
            var response = variables.riotAPI.GetSummonerRankedInfo("Mabud");

            response.Wait();

            var info = response.Result.data;

            Assert.IsTrue(response.IsCompletedSuccessfully);
            Assert.AreEqual(response.Result.code, Response.Code.Success);
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.data);
            Assert.IsTrue(info.found);
            Assert.AreEqual("Mabud", info.summoner_name);

        }

        [TestMethod]
        public void TestMethod3() {
            var response = variables.riotAPI.GetSummonerRankedInfo("Days");

            response.Wait();

            var info = response.Result.data;

            Assert.AreEqual(response.Result.code, Response.Code.Success);
            Assert.IsNotNull(response.Result.data.ToStringOneLine(RankedInfo.Queue.Type.SoloDuo));

        }

        //[TestMethod]
        //public void TestMethod2() {
        //    var info2 = variables.riotAPI.GetSummonerMatchInfo("dense terd");
        //    info2.Wait();
        //    Assert.AreEqual(info2.Result.summoner_name, "dense terd");
        //    Assert.IsNotNull(info2.Result.runes);
        //    Assert.AreEqual(info2.Result.runes.primary_rune_ids[0], 8214);
        //    Assert.IsNotNull(info2.Result.runes.ToString());
        //}
    }

    [TestClass]
    public class UnitTest2 {
        EnvironmentVariables variables = new EnvironmentVariables();

        [TestMethod]
        public void TestMethod0() {
            var summoner_info = variables.riotAPI.GetSummonerInfo("Thornshot");
            summoner_info.Wait();

            Assert.AreEqual("zsitvSRdx4kyjGOQry0npuTLGVIcz7-BkSUamxxrJ5i16MU", summoner_info.Result.data.summoner_id);
        }

        [TestMethod]
        public void TestMethod1() {
            var match_info = variables.riotAPI.GetMatchInfo("Thornshot");
            match_info.Wait();

            Assert.IsTrue(match_info.Result.code == Response.Code.Success || match_info.Result.code == Response.Code.NotFound);
        }

        [TestMethod]
        public void TestMethod2() {
            var match_info = variables.riotAPI.GetMatchInfo("Thornshot");
            match_info.Wait();

            MatchInfo info = match_info.Result.data;

            Assert.IsTrue(match_info.Result.code == Response.Code.Success || match_info.Result.code == Response.Code.NotFound);
        }

        [TestMethod]
        public void TestMethod3() {
            var match_info = variables.riotAPI.GetMatchInfoById("zsitvSRdx4kyjGOQry0npuTLGVIcz7-BkSUamxxrJ5i16MU");
            match_info.Wait();

            Assert.IsTrue(match_info.Result.code == Response.Code.Success || match_info.Result.code == Response.Code.NotFound);
        }
        [TestMethod]
        public void TestMethod4() {
            var match_info = variables.riotAPI.GetMatchInfoById("zsitvSRdx4kyjGOQry0npuTLGVIcz7-BkSUamxxrJ5i16MU");
            match_info.Wait();

            MatchInfo info = match_info.Result.data;

            Assert.IsTrue(match_info.Result.code == Response.Code.Success || match_info.Result.code == Response.Code.NotFound);
        }
    }

    class EnvironmentVariables {
        public string riot_key { get; private set; }
        public RiotApi riotAPI { get; private set; }

        JSONObject variables;

        public EnvironmentVariables() {
            variables = Parser.Parse(File.ReadAllText("ENV.json"));

            riot_key = variables["RiotApi"].string_value;
            riotAPI = new RiotApi(riot_key);
        }
    }
}
