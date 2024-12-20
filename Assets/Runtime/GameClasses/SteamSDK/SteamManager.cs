// The SteamManager is designed to work with Steamworks.NET
// This file is released into the public domain.
// Where that dedication is not recognized you are granted a perpetual,
// irrevocable license to copy and modify this file as you see fit.
//
// Version: 1.0.13

#if !(UNITY_STANDALONE_WIN || UNITY_SWITCH || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

using UnityEngine;

#if !DISABLESTEAMWORKS
using Steamworks;
#endif

#region Generic
namespace GameClasses {
    public interface ISteamManager {
        void Init(uint appID, bool isFailedToQuit);
        void Tick();
        void TearDown();
        void Ach_Unlock(string achievementId);
        void Ach_CleanAll();
    }
}
#endregion

#region Steam
#if !DISABLESTEAMWORKS
namespace GameClasses {

    //
    // The SteamManager provides a base implementation of Steamworks.NET on which you can build upon.
    // It handles the basics of starting up and shutting down the SteamAPI for use.
    //
    [DisallowMultipleComponent]
    public class SteamManager : ISteamManager {

        SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
        bool isInit;

        [AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
        static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText) {
            Debug.LogWarning(pchDebugText);
        }

        public SteamManager() {
            isInit = false;
        }

        public void Init(uint appID, bool isFailedToQuit) {

            if (!Packsize.Test()) {
                Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
            }

            if (!DllCheck.Test()) {
                Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
            }

            try {
                // If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
                // Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.
                // Note that this will run which ever version you have installed in steam. Which may not be the precise executable
                // we were currently running.

                // Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
                // remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
                // See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown

                if (SteamAPI.RestartAppIfNecessary(new AppId_t(appID))) {
                    Debug.LogWarning("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");
                    if (isFailedToQuit) {
                        Application.Quit();
                    }
                    return;
                }
            } catch (System.DllNotFoundException e) { // We catch this exception here, as it will be the first occurrence of it.
                Debug.LogWarning("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);
                if (isFailedToQuit) {
                    Application.Quit();
                }
                return;
            }

            // Initializes the Steamworks API.
            // If this returns false then this indicates one of the following conditions:
            // [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
            // [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
            // [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
            // [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
            // [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
            // Valve's documentation for this is located here:
            // https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
            isInit = SteamAPI.Init();
            if (!isInit) {
                Debug.LogWarning("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
                if (isFailedToQuit) {
                    Application.Quit();
                }
                return;
            }

            // Set up our callback to receive warning messages from Steam.
            // You must launch with "-debug_steamapi" in the launch args to receive warnings.
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);

        }

        public void Tick() {
            if (!isInit) {
                return;
            }
            SteamAPI.RunCallbacks();
        }

        // OnApplicationQuit gets called too early to shutdown the SteamAPI.
        // Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
        // Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
        public void TearDown() {
            SteamAPI.Shutdown();
        }

        // 解锁成就
        public void Ach_Unlock(string achievementId) {
            if (!isInit) {
                return;
            }
            SteamUserStats.SetAchievement(achievementId);
            SteamUserStats.StoreStats();
        }

        // 清空所有成就
        public void Ach_CleanAll() {
            if (!isInit) {
                return;
            }
            SteamUserStats.ResetAllStats(true);
            SteamUserStats.StoreStats();
        }

    }
}
#endif
#endregion

#region Disable Steam
#if DISABLESTEAMWORKS
namespace GameClasses {
    public class SteamManager : ISteamManager {

        public SteamManager() {
        }

        public void Init(uint appID, bool isFailedToQuit) {
        }

        public void Tick() {
        }

        public void TearDown() {
        }

        public void Ach_Unlock(string achievementId) {
        }

        public void Ach_CleanAll() {
        }

    }
}
#endif
#endregion