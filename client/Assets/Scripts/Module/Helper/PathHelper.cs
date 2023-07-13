using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Module.Helper
{
    public class PathHelper
    {
        public static string BasePath => "Assets/Res/";

        public static string GetUIPath() => BasePath + "UI";

        public static string GetPrefabPath(string prefabName) => BasePath + "Prefabs/" + prefabName+".prefab";

        public static string GetPrefabUIPath(string prefabName) => BasePath + "Prefabs/UI/" + prefabName + ".prefab";


        //Get StreamingAssets path
        public static string GetSAUrl(string subname, bool isRelative = false)
        {
            subname = subname.Replace('\\', '/');
            if (subname.StartsWith("/"))
            {
                subname = subname.Substring(1);
            }
            string filePath = "";
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Application.streamingAssetsPath.StartsWith("/"))
                filePath = "file://" + Application.streamingAssetsPath + "/" + subname;
            else
                filePath = "file:///" + Application.streamingAssetsPath + "/" + subname;
#elif UNITY_IOS
            filePath = "file:///" + Application.streamingAssetsPath + "/" + subname;
#elif UNITY_ANDROID
            filePath = Application.streamingAssetsPath + "/" + subname;
#endif

            if (isRelative == true)
            {
                filePath = filePath.Replace(System.Environment.CurrentDirectory, "|").Split('|')[1].Substring(1);
            }

            return filePath;
        }

        public static string GetStoragePath(string subname = null, bool isRelative = false)
        {
            subname = subname.Replace('\\', '/');
            if (subname.StartsWith("/"))
            {
                subname = subname.Substring(1);
            }
            string _path = Application.persistentDataPath;
            if (isRelative == true)
            {
                _path = _path.Replace(System.Environment.CurrentDirectory, "|").Split('|')[1].Substring(1);
            }
            if (subname != null)
                _path = _path + "/" + subname;
            return _path;
        }

        public static string GetStreamingAssetsPathRelative(params string[] subnames)
        {
            string filePath = "";

            foreach (var subname in subnames)
            {
                if (filePath == "")
                    filePath = subname;
                else
                    filePath = filePath + "/" + subname;
            }

            return GetSAUrl(filePath, isRelative: true);
        }

        public static string GetSAUrl(params string[] subnames)
        {
            string filePath = "";

            foreach (var subname in subnames)
            {
                if (filePath == "")
                    filePath = subname;
                else
                    filePath = filePath + "/" + subname;
            }

            return GetSAUrl(filePath);
        }

        public static string GetSAPath(params string[] subnames)
        {
            string filePath = "";

            foreach (var subname in subnames)
            {
                if (filePath == "")
                    filePath = subname;
                else
                    filePath = filePath + "/" + subname;
            }

            return Application.streamingAssetsPath + "/" + filePath;
        }

        public static string PlatformAssetBundlePath(params string[] paths)
        {
            var fullPaths = paths.ToList();
            fullPaths.Insert(0, PlatformAssetBundleName);
            return GetSAUrl(fullPaths.ToArray());
        }

        public static string PlatformAssetBundleName
        {
            get
            {
                if (UnityEngine.Application.platform == RuntimePlatform.WindowsEditor ||
                    UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer)
                    return "windows";
                else if (UnityEngine.Application.platform == RuntimePlatform.OSXEditor ||
                    UnityEngine.Application.platform == RuntimePlatform.OSXPlayer)
                    return "osx";
                else if (UnityEngine.Application.platform == RuntimePlatform.IPhonePlayer)
                    return "ios";
                else if (UnityEngine.Application.platform == RuntimePlatform.Android)
                    return "android";
                else if (UnityEngine.Application.platform == RuntimePlatform.PS4)
                    return "ps4";
                else if (UnityEngine.Application.platform == RuntimePlatform.XboxOne)
                    return "xbx1";
                return "";
            }
        }
    }
}
