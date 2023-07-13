using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using UnityEngine;

namespace Module.Helper
{
    public static class LocalSave
    {
        public static string ReadString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static int ReadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static float ReadFloat(string key, float defaultValue = 0.0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public static void Save(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static void Save(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static void Save(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void Remove(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public static bool Has(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}
