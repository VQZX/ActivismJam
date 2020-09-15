using System;
using System.IO;
using UnityEngine;

namespace Flusk.Utility
{
    public static class PlayerPrefsUtility
    {
        public static T GetData<T>(string key)
        {
            var type = typeof(T);
            if (type == typeof(string))
            {
                return (T)(object)PlayerPrefs.GetString(key);
            }

            if (type == typeof(int))
            {
                return (T) (object) PlayerPrefs.GetInt(key);
            }

            if (type == typeof(float))
            {
                return (T) (object) PlayerPrefs.GetFloat(key);
            }

            if (type == typeof(bool))
            {
                var prefsData =  PlayerPrefs.GetInt(key);
                switch (prefsData)
                {
                    case 0:
                        return (T) (object)false;
                    case 1:
                        return (T) (object)true;
                    default:
                        throw new InvalidDataException($"PlayerPrefs data not formatted correctly for flag with {key}. " +
                                                       $"Returned data is {prefsData}");
                }
            }

            throw new InvalidDataException($"No such format as {typeof(T)} in PlayerPrefs");
        }

        public static bool TryGetData<T>(string key, ref T data)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return false;
            }
            data = PlayerPrefsUtility.GetData<T>(key);
            return true;
        }

        public static void SetData<T>(string key, T data)
        {
            var type = typeof(T);
            if (type == typeof(string))
            {
                PlayerPrefs.SetString(key, (string) (object)data);
                return;
            }

            if (type == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int) (object)data);
                return;
            }

            if (type == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float) (object)data);
                return;
            }

            if (type == typeof(Boolean) || type == typeof(bool))
            {
                var boolData = (bool) (object) data;
                var prefsData = boolData ? 1 : 0;
                PlayerPrefs.SetInt(key, prefsData);
                return;
            }

            throw new InvalidDataException($"No such format as {typeof(T)} in PlayerPrefs");
        }
    }
}