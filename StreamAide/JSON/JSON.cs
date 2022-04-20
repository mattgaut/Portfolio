using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JSON {
    public class JSONObject {
        public string string_value { get; private set; }
        public int int_value { get; private set; }
        public bool bool_value { get; private set; }
        public double double_value { get; private set; }

        public bool is_string { get; private set; }
        public bool is_int { get; private set; }
        public bool is_table { get; private set; }
        public bool is_table_array { get; private set; }
        public bool is_bool { get; private set; }
        public bool is_double { get; private set; }
        public bool is_object_array { get; private set; }
        public bool is_null { get; private set; }

        public int count {
            get {
                if (is_object_array) {
                    return object_array_value.count;
                }
                if (is_table_array) {
                    return table_array_value.count;
                }
                return -1;
            }
        }

        JSONTable table_value { get; set; }
        JSONTableArray table_array_value { get; set; }
        JSONObjectArray object_array_value { get; set; }

        public JSONObject this[string name] {
            get { return table_value[name]; }
            set { table_value[name] = value; }
        }

        public JSONObject this[int index] {
            get {
                if (is_object_array) {
                    return object_array_value[index];
                } else if (is_table_array) {
                    return table_array_value[index];
                } else {
                    return null;
                }
            }
        }

        public bool HasValue(string value) {
            return is_table && table_value.HasValue(value);
        }

        public JSONObject() {
            is_null = true;
        }

        public JSONObject(string value) {
            string_value = value;
            is_string = true;
        }

        public JSONObject(int value) {
            int_value = value;
            is_int = true;
        }

        public JSONObject(double value) {
            double_value = value;
            is_double = true;
        }

        public JSONObject(bool value) {
            bool_value = value;
            is_bool = true;
        }

        internal JSONObject(JSONTable value) {
            table_value = value;
            is_table = true;
        }

        internal JSONObject(JSONTableArray value) {
            table_array_value = value;
            is_table_array = true;
        }

        internal JSONObject(JSONObjectArray value) {
            object_array_value = value;
            is_object_array = true;
        }

        public void Print() {
            Console.WriteLine(ToString());
        }

        public string ToString(int depth = 0) {
            string to_ret = "";

            if (is_bool) {
                to_ret += bool_value.ToString().ToLower();
            } else if (is_int) {
                to_ret += int_value;
            } else if (is_double) {
                to_ret += double_value;
            } else if (is_table) {
                to_ret += table_value.ToString(depth + 1);
            } else if (is_table_array) {
                to_ret += table_array_value.ToString(depth + 1);
            } else if (is_object_array) {
                to_ret += object_array_value.ToString(depth + 1);
            } else if (is_string) {
                to_ret += "\"" + string_value + "\"";
            } else if (is_null) {
                to_ret += "null";
            }

            return to_ret;
        }
    }

    internal class JSONObjectArray {
        List<JSONObject> objects;

        public JSONObject this[int index] {
            get { return objects[index]; }
        }
        public int count {
            get { return objects.Count; }
        }

        public JSONObjectArray(string json) {
            objects = new List<JSONObject>();
            string[] strings = Parser.SplitJSONString(json);
            for (int i = 0; i < strings.Length; i++) {
                objects.Add(Parser.ParseInternal(strings[i]));
            }
        }

        public void Print() {
            Console.WriteLine(ToString());
        }

        public string ToString(int depth = 0) {
            string to_ret = "[";
            if (objects.Count > 0) {
                to_ret += Helper.TabbedNewline(depth);
                for (int i = 0; i < objects.Count; i++) {
                    to_ret += objects[i].ToString(depth + 1);
                    if (i < objects.Count - 1) {
                        to_ret += ",";
                        to_ret += Helper.TabbedNewline(depth);
                    } else {
                        to_ret += Helper.TabbedNewline(depth - 1);
                    }
                }
            }
            to_ret += "]";

            return to_ret;
        }
    }

    internal class JSONTable {
        Dictionary<string, JSONObject> table;

        public JSONObject this[string name] {
            get { return table[name]; }
            set { table[name] = value; }
        }

        public bool HasValue(string name) {
            return table.ContainsKey(name);
        }

        public JSONTable(string json) {
            table = new Dictionary<string, JSONObject>();
            string[] strings = Parser.SplitJSONString(json);
            for (int i = 0; i < strings.Length; i++) {
                int colon = Parser.IndexOfFirstColon(strings[i]);
                table.Add(strings[i].Substring(1, colon - 1), Parser.ParseInternal(strings[i].Substring(colon + 2)));
            }
        }

        internal JSONTable(string json, bool clean_whitespace) {
            table = new Dictionary<string, JSONObject>();
            string[] strings = Parser.SplitJSONString(json);
            for (int i = 0; i < strings.Length; i++) {
                int colon = Parser.IndexOfFirstColon(strings[i]);
                JSONObject child_json;
                if (clean_whitespace) {
                    child_json = Parser.Parse(strings[i].Substring(colon + 2));
                } else {
                    child_json = Parser.ParseInternal(strings[i].Substring(colon + 2));
                }
                table.Add(strings[i].Substring(1, colon - 1), child_json);
            }
        }

        public void Print() {
            Console.WriteLine(ToString());
        }

        public string ToString(int depth = 0) {
            string to_ret = "{" + Helper.TabbedNewline(depth);

            int count = 0;
            foreach (KeyValuePair<string, JSONObject> pair in table) {
                count++;
                to_ret += "\"" + pair.Key + "\"" + " : " + pair.Value.ToString(depth);
                if (count < table.Count) {
                    to_ret += ",";
                    to_ret += Helper.TabbedNewline(depth);
                } else {
                    to_ret += Helper.TabbedNewline(depth - 1);
                }
            }
           
            to_ret += "}";

            return to_ret;
        }
    }

    internal class JSONTableArray {
        List<JSONTable> tables;

        public int count { get { return tables.Count; } }

        public JSONObject this[int index] {
            get { return new JSONObject(tables[index]); }
        }

        public JSONTableArray(string json) {

            tables = new List<JSONTable>();
            if (json.Length > 2) {
                string[] strings = Parser.SplitJSONString(json);
                for (int i = 0; i < strings.Length; i++) {
                    tables.Add(new JSONTable(strings[i].Substring(1, strings[i].Length - 2)));
                }
            }
        }

        public void Print() {
            Console.WriteLine(ToString());
        }

        public string ToString(int depth = 0) {
            string to_ret = "[";
            if (tables.Count > 0) {
                to_ret += Helper.TabbedNewline(depth);
                for (int i = 0; i < tables.Count; i++) {
                    to_ret += tables[i].ToString(depth+ 1);
                    if (i < tables.Count - 1) {
                        to_ret += ",";
                        to_ret += Helper.TabbedNewline(depth);
                    } else {
                        to_ret += Helper.TabbedNewline(depth - 1);
                    }
                }
            }
            to_ret += "]";

            return to_ret;
        }
    }

    public static class Parser {
        public static JSONObject Parse(string json) {
            json = RemoveWhiteSpaceOutsideQuotes(json);
            return ParseInternal(json);
        }
        
        internal static JSONObject ParseInternal(string json) {
            JSONObject json_object = null;

            int n;
            double d;
            bool b;
            if (json.StartsWith('[')) {
                if(json.StartsWith("[{")) {
                    JSONTableArray table_array = new JSONTableArray(json.Substring(1, json.Length - 2));
                    json_object = new JSONObject(table_array);
                } else {
                    JSONObjectArray object_array = new JSONObjectArray(json.Substring(1, json.Length - 2));
                    json_object = new JSONObject(object_array);
                }
            } else if (json.StartsWith('{')) {
                JSONTable table = new JSONTable(json.Substring(1, json.Length - 2));
                json_object = new JSONObject(table);
            } else if (json.StartsWith('"')) {
                json_object = new JSONObject(json.Substring(1, json.Length - 2));
            } else if (int.TryParse(json, out n)) {
                json_object = new JSONObject(n);
            } else if (double.TryParse(json, out d)) {
                json_object = new JSONObject(d);
            } else if (bool.TryParse(json, out b)) {
                json_object = new JSONObject(b);
            } else if (json == "null") {
                json_object = new JSONObject();
            } else {
                throw new Exception("could not parse " + json + " : " + json.Length);
            }

            return json_object;
        }

        public static string[] SplitJSONString(string to_split) {
            if (to_split.Length == 0) {
                return new string[0];
            }
            char[] chars = to_split.ToCharArray();
            List<string> strings = new List<string>();

            bool inside_quote = false;
            int inside_brace = 0, inside_bracket = 0;
            int last_split = 0;

            for (int i = 0; i < chars.Length; i++) {
                if (chars[i] == '"') {
                    inside_quote = !inside_quote;
                }
                if (!inside_quote) {
                    if (chars[i] == '{') {
                        inside_brace++;
                    }
                    if (chars[i] == '}') {
                        inside_brace--;
                    }
                    if (chars[i] == '[') {
                        inside_bracket++;
                    }
                    if (chars[i] == ']') {
                        inside_bracket--;
                    }
                }
                if (!inside_quote && inside_brace == 0 && inside_bracket == 0 && chars[i] == ',') {
                    strings.Add(to_split.Substring(last_split, i - last_split));
                    last_split = i + 1;
                }
            }
            strings.Add(to_split.Substring(last_split));

            return strings.ToArray();
        }

        public static int IndexOfFirstColon(string to_split) {
            char[] chars = to_split.ToCharArray();
            bool inside_quote = false;

            for (int i = 0; i < chars.Length; i++) {
                if (chars[i] == '"') {
                    inside_quote = !inside_quote;
                }
                if (!inside_quote) {
                    return i;
                }
            }

            return -1;
        }

        public static string RemoveWhiteSpaceOutsideQuotes(string str) {
            char[] characters = str.ToCharArray();
            string to_ret = "";

            bool inside_quotes = false;
            for (int i = 0; i < characters.Length; i++) {
                if (characters[i] == '"') {
                    inside_quotes = !inside_quotes;
                }
                if (inside_quotes) {
                    to_ret += characters[i];
                } else if (!char.IsWhiteSpace(characters[i])) {
                    to_ret += characters[i];
                }
            }
            return to_ret;
        }
    }

    public static class Helper {
        public static string TabbedNewline(int tab_count) {
            string to_ret=Environment.NewLine;
            for (int i = 0; i < tab_count; i++) {
                to_ret += "\t";
            }
            return to_ret;
        }
    }
}
