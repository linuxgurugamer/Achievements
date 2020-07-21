/*
Achievements - Brings achievements to Kerbal Space Program.
Copyright (C) 2013-2014 Maik Schreiber

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSPAchievements;
using ToolbarControl_NS;
using System.Diagnostics.Eventing.Reader;
using KRASH;

namespace Achievements
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(Achievements.MODID, Achievements.MODNAME);
        }

    }

    internal  class KRASH_Interface : MonoBehaviour
    {
        static double lastTimeChecked = 0;
        static bool simActive = false;
        internal static  bool KRASH_Active()
        {
            if (Planetarium.GetUniversalTime() - lastTimeChecked > 0)
            {
                KRASHPersistent krashObject = FindObjectOfType<KRASHPersistent>();

                if (krashObject != null)
                    simActive = krashObject.shelterSimulationActive;
                Log.info("KRASH_Active: " + simActive);
            }
            return simActive;
        }

    }
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class Achievements : MonoBehaviour
    {
        static public Achievements fetch;
        internal const string UNKNOWN_VESSEL = "unknown";
        internal const long VERSION = 19;

        private const long CHECK_INTERVAL = 1500;
        private const float REPUTATION_REWARD = 10;

        private long lastCheck = 0;
        private AudioClip achievementEarnedClip;
        private AudioSource achievementEarnedAudioSource;
        private Toast toast;
        private Dictionary<string, Achievement> queuedEarnedAchievements = new Dictionary<string, Achievement>();
        private AchievementsWindow achievementsWindow;
        private static ToolbarControl toolbarControl;

        public Texture2D AchieveButton;
#if LOCATION_PICKER
		private LocationPicker locationPicker;
#endif
        private bool showGui = true;


        public void Start()
        {
            Log.debug("Achievements.Start");
            fetch = this;

            achievementEarnedClip = GameDatabase.Instance.GetAudioClip("Achievements/achievement");
            achievementEarnedAudioSource = gameObject.AddComponent<AudioSource>();

            achievementEarnedAudioSource.clip = achievementEarnedClip;
            achievementEarnedAudioSource.panStereo = 0;
            achievementEarnedAudioSource.playOnAwake = false;
            achievementEarnedAudioSource.loop = false;
            achievementEarnedAudioSource.Stop();
            //windowButton = ToolbarManager.Instance.add("achievements", "achievements");
            //windowButton.TexturePath = "Achievements/button-normal";
            //windowButton.ToolTip = "Achievements";
            //windowButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.EDITOR);
            //windowButton.OnClick += (e) => toggleAchievementsWindow();

            GameEvents.onShowUI.Add(onShowUI);
            GameEvents.onHideUI.Add(onHideUI);
            StartCoroutine("SlowUpdate");
            DontDestroyOnLoad(this);
        }
        public void Awake()
        {
            LoadTextures();
            CreateButtons();
        }
        public void LoadTextures()
        {
            AchieveButton = new Texture2D(2, 2);
            if (!ToolbarControl.LoadImageFromFile(ref AchieveButton, "GameData/Achievements/PluginData/Textures/AchievmentTrophyButton"))
                Log.error("Unable to load AchievmentTrophyButton from file");
        }

        internal const string MODID = "Achievements_NS";
        internal const string MODNAME = "Achievements";
        public void CreateButtons()
        {
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();

                toolbarControl.AddToAllToolbars(this.AchieveButtonOn, this.AchievButtonOff,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT,
                    MODID,
                    "achButton",
                    "Achievements/PluginData/Textures/button-normal-38",
                    "Achievements/PluginData/Textures/button-normal-24",
                    MODNAME
                );

            }
            else { Debug.Log("achievment CreateButtons Failed"); }
        }
        private void AchieveButtonOn()
        {
            toggleAchievementsWindow();
            toolbarControl.SetTexture("Achievements/PluginData/Textures/button-highlight-38", "Achievements/PluginData/Textures/button-highlight-24");
        }

        internal void AchievTextureOff()
        {
            toolbarControl.SetTexture("Achievements/PluginData/Textures/button-normal-38", "Achievements/PluginData/Textures/button-normal-24");
        }
        internal void AchievButtonOff()
        {
            toggleAchievementsWindow();
            AchievTextureOff();
        }
        public void DestroyButtons()
        {
            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                UnityEngine.Object.Destroy(toolbarControl);
            }
        }
        internal void OnDestroy()
        {
            //windowButton.Destroy();
            DestroyButtons();
            GameEvents.onShowUI.Remove(onShowUI);
            GameEvents.onHideUI.Remove(onHideUI);
            StopCoroutine("SlowUpdate");
        }

        IEnumerator SlowUpdate()
        {
            while (true)
            {
                DoUpdate();
                yield return new WaitForSeconds(0.25f);
            }
        }

        
        internal void DoUpdate()
        {
            // Since bodies are now generated from the database rather than being hardcoded,
            // the following "if" was added to wait until all the bodies and achievements had been initialized
            // This is necessary since the hard-coded version didn't need to wait becuase they
            // were initialized at the compile time
            if (EarnedAchievements.instance != null && EarnedAchievements.instance.allAchievementsCreated)
            {
                if (SpaceTuxUtility.HasMod.hasMod("KRASH"))
                {
                    Log.info("KRASH found");
                    if (KRASH_Interface.KRASH_Active())
                        return;
                }
                updateAchievements();
                checkAchievements();

                if (achievementsWindow != null)
                {
                    achievementsWindow.update();
                }

                //UpdateChecker.update();
            }
        }

        private void updateAchievements()
        {
            if (EarnedAchievements.instance != null)
            {
                foreach (Achievement achievement in EarnedAchievements.instance.achievementsList)
                {
                    try
                    {
                        achievement.update();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        private void checkAchievements()
        {
            long now = DateTime.UtcNow.Ticks / 10000; // 1 millisecond
            if ((now - lastCheck) >= CHECK_INTERVAL) // this delays it to every 1.5 seconds
            {
                Vessel vessel = (FlightGlobals.fetch != null) ? FlightGlobals.ActiveVessel : null;
                if (vessel != null && EarnedAchievements.instance != null)
                {
                    foreach (Achievement achievement in EarnedAchievements.instance.achievementsList)
                    {
                        if (EarnedAchievements.instance.earnedAchievements == null || !EarnedAchievements.instance.earnedAchievements.ContainsKey(achievement.getKey()))
                        {

                            string key = achievement.getKey();

                            //Vessel vessel = (FlightGlobals.fetch != null) ? FlightGlobals.ActiveVessel : null;
                            //if (vessel != null)
                            //{
                            try
                            {
                                if (achievement.check(vessel))
                                {
                                    string key2 = achievement.getKey();
                                    AchievementEarn earn = new AchievementEarn(now, (vessel != null) ? vessel.vesselName : Achievements.UNKNOWN_VESSEL, achievement);
                                    EarnedAchievements.instance.earnedAchievements.Add(achievement.getKey(), earn);

                                    // queue for later display
                                    queuedEarnedAchievements.Add(achievement.getKey(), achievement);
                                }
                            }

                            catch (Exception e)
                            {
                                Debug.Log("checkAchievements 1.5");
                                Debug.LogException(e);
                            }
                        }
                    }
                }

                //long done = DateTime.UtcNow.Ticks / 10000;
                //Debug.LogWarning("checking achievements took " + (done - now) + " ms");

                if ((queuedEarnedAchievements.Count() > 0) && (toast == null))
                {

                    Achievement achievement = queuedEarnedAchievements[queuedEarnedAchievements.Keys.Min()];
                    queuedEarnedAchievements.Remove(achievement.getKey());
                    //if (EarnedAchievements.instance.earnedAchievements.ContainsKey(achievement))
                    toast = null;
                    bool found = false;
                    foreach (var e in EarnedAchievements.instance.earnedAchievements)
                    {
                        if (e.Key == achievement.getKey())
                        {
                            toast = new Toast(achievement, e.Value);
                            playAchievementEarnedClip();
                            awardReputation(achievement);
                            found = true;
                            break;
                        }
                    }

                }

                lastCheck = now;
            }
        }

        internal void OnGUI()
        {
            if (!showGui)
            {
                return;
            }

            // auto-close achievements list window
            if (EarnedAchievements.instance == null)
            {
                achievementsWindow = null;
            }

#if LOCATION_PICKER
			if (SHOW_LOCATION_PICKER_BUTTON && (HighLogic.LoadedScene == GameScenes.FLIGHT) && MapView.MapIsEnabled) {
				drawLocationPickerButton();
			}
#endif

            if (toast != null)
            {
                if (!toast.isTimedOut())
                {
                    toast.draw();
                }
                else
                {
                    // auto-close toast after timeout
                    toast = null;
                }
            }

            if (achievementsWindow != null)
            {
                achievementsWindow.draw();
            }

#if LOCATION_PICKER
			if (locationPicker != null) {
				locationPicker.draw();
			}
#endif
        }

        private void onShowUI()
        {
            showGui = true;
        }

        private void onHideUI()
        {
            showGui = false;
        }

        private void drawLocationPickerButton()
        {
            GUI.depth = -100;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0, 0, -90)), Vector3.one);
            if (GUI.Button(new Rect(-700, Screen.width - 25, 120, 25), "Location"))
            {
                toggleLocationPicker();
            }
            GUI.matrix = Matrix4x4.identity;
            GUI.depth = 0;
        }

        public void toggleAchievementsWindow()
        {
            if (achievementsWindow == null)
            {
                achievementsWindow = new AchievementsWindow(EarnedAchievements.instance.achievements, EarnedAchievements.instance.earnedAchievements /*,false */);
                achievementsWindow.closeCallback = () =>
                {
                    achievementsWindow = null;
                };

                // reset toolbar button
                //windowButton.TexturePath = "Achievements/button-normal";
            }
            else
            {
                achievementsWindow = null;
            }
        }

        private void toggleLocationPicker()
        {
#if LOCATION_PICKER
			if (locationPicker == null) {
				MapView.EnterMapView();
				locationPicker = new LocationPicker();
				locationPicker.closeCallback = () => {
					locationPicker.destroy();
					locationPicker = null;
				};
			} else {
				locationPicker.destroy();
				locationPicker = null;
			}
#endif
        }

        private void playAchievementEarnedClip()
        {
            if (!achievementEarnedAudioSource.isPlaying)
            {
                achievementEarnedAudioSource.volume = GameSettings.UI_VOLUME;
                achievementEarnedAudioSource.Play();
            }
        }

        private void awardReputation(Achievement achievement)
        {
            if (Reputation.Instance != null)
            {
                //Reputation.Instance.AddReputation(REPUTATION_REWARD, "Achievement: " + achievement.getTitle());
                Reputation.Instance.AddReputation(REPUTATION_REWARD, TransactionReasons.None);
            }
        }
    }
}
