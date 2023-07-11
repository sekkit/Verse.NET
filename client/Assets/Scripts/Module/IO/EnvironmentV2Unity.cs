using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Module.Shared;
using UnityEngine;
using Zio;

namespace Module.IO
{
    class EnvironmentV2Unity : EnvironmentV2 {

        public EnvironmentV2Unity() : base(new UnitySystemInfo()) { }

        /// <summary>
        /// Base impl. would return C:\Users\User123\AppData\Local\Temp\ 
        /// Unity impl. will return C:\Users\User123\AppData\Local\Temp\DefaultCompany\UnityTestsA\
        /// </summary>
        protected override DirectoryInfo GetRootTempDirInfo() {
            return new DirectoryInfo(Application.temporaryCachePath);
        }

        public override DirectoryEntry GetOrAddAppDataFolder(string appDataSubfolderName) {
            return GetPersistentDataPath().GetChildDir(appDataSubfolderName).CreateV2().ToRootDirectoryEntry();
        }

        public override DirectoryEntry GetCurrentDirectory() {
            if (isWindows || isMacOs || isLinux || isUnityEditor) {
                var appDataPath = new DirectoryInfo(Application.dataPath);
                if (isUnityEditor) {
                    // Return "/TestApplicationData" to protect the rest of the Unity project folders:
                    return appDataPath.Parent.GetChildDir("TestApplicationData").CreateV2().ToRootDirectoryEntry();
                }
                // On Windows, Linux and MacOS it makes sense to return the install folder:
                return appDataPath.ToRootDirectoryEntry();
            }
            // On all other platforms there is no install folder so return the normal GetPersistentDataPath:
            return GetPersistentDataPath().ToRootDirectoryEntry();
        }

        private static DirectoryInfo GetPersistentDataPath() { return new DirectoryInfo(Application.persistentDataPath); }

        public override CultureInfo CurrentCulture { get => Application.systemLanguage.ToCultureInfo(); set => throw new NotSupportedException(); }

        public override CultureInfo CurrentUICulture { get => CurrentCulture; set => CurrentCulture = value; }

        public static bool isUnityEditor {
            get {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isWindows {
            get {
#if UNITY_STANDALONE_WIN 
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isMacOs {
            get {
#if UNITY_STANDALONE_OSX 
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isLinux {
            get {
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isAndroid {
            get {
#if UNITY_ANDROID
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isIos {
            get {
#if UNITY_IOS
                return true;
#else
                return false;
#endif
            }
        }

    }
}