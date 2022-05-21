using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheGameOfAzazel
{
    public static class SaveManager
    {
        private static readonly string _SaveFile = "Save.dat";
        private static readonly Dictionary<string, string> _SaveData = new Dictionary<string, string>();
        public static void Initalize()
        {
            if (File.Exists(_SaveFile))
            {
                foreach (string Line in File.ReadAllLines(_SaveFile))
                {
                    string[] parts = Line.Split(',');
                    _SaveData.Add(parts[0].ToLower(), parts[1].ToLower());
                }
            }
            else
            {
                GenerateSaveData();
                SaveData();
            }
        }
        private static void GenerateSaveData()
        {
            _SaveData.Add("money", "0");
            _SaveData.Add("health", "1");
            _SaveData.Add("shield", "1");
            _SaveData.Add("light", "1");
            _SaveData.Add("speed", "1");
            _SaveData.Add("dash", "1");
            _SaveData.Add("firerate", "1");
            _SaveData.Add("firepower", "1");
            _SaveData.Add("luck", "1");
            _SaveData.Add("startallies", "1");
            _SaveData.Add("coinmagnet", "1");
        }

        public static void SaveData()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> dataEntry in _SaveData)
            {
                sb.Append(dataEntry.Key + "," + dataEntry.Value + Environment.NewLine);
            }

            File.WriteAllText(_SaveFile, sb.ToString());
        }

        public static void SetValue(string Key, object value)
        {
            _SaveData[Key.ToLower()] = value.ToString();
        }
        public static string GetValueString(string Key)
        {
            return _SaveData[Key.ToLower()];
        }
        public static int GetValueInt(string Key)
        {
            return Convert.ToInt32(_SaveData[Key.ToLower()]);
        }

        public static double GetValueDouble(string Key)
        {
            return Convert.ToDouble(_SaveData[Key.ToLower()]);
        }
    }
}
