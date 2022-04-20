using JSON;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace JSONParserUnitTest {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            var json = Parser.Parse(File.ReadAllText("Test1Input.json"));

            Assert.AreEqual("KarasmaiBestKayn", json["participants"][3]["summonerName"].string_value);
            Assert.AreEqual(8010, json["participants"][3]["perks"]["perkIds"][0].int_value);
        }

        [TestMethod]
        public void TestMethod2() {
            var json = Parser.Parse(File.ReadAllText("Test1Input.json"));

            File.WriteAllText("Test1Output.json", json.ToString());

            var json2 = Parser.Parse(File.ReadAllText("Test1Output.json"));

            Assert.AreEqual(json.ToString(), json2.ToString());
        }

        [TestMethod]
        public void TestMethod3() {
            var json = Parser.Parse(File.ReadAllText("Test2Input.json"));

            File.WriteAllText("Test2Output.json", json.ToString());

            var json2 = Parser.Parse(File.ReadAllText("Test2Output.json"));

            Assert.AreEqual(json.ToString(), json2.ToString());

            json2["flags"] = new JSONObject("flag");

            Assert.AreEqual(json2["flags"].string_value, "flag");
        }
    }
}
