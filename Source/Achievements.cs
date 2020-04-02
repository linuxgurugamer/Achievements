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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSPAchievements;
using ToolbarControl_NS;


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

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class Achievements : MonoBehaviour
    {
        internal const string UNKNOWN_VESSEL = "unknown";
        internal const long VERSION = 19;

        private const long CHECK_INTERVAL = 1500;
        private const float REPUTATION_REWARD = 10;

        private long lastCheck = 0;
        private AudioClip achievementEarnedClip;
        private AudioSource achievementEarnedAudioSource;
        private Toast toast;
        private HashSet<Achievement> queuedEarnedAchievements = new HashSet<Achievement>();
        private AchievementsWindow achievementsWindow;
        //private ApplicationLauncherButton AchButton;
        ToolbarControl toolbarControl;

        //private AchievementGUI AchievementGUI;
        public Texture2D AchieveButton;
#if LOCATION_PICKER
		private LocationPicker locationPicker;
#endif
        //private IButton windowButton;
        private bool showGui = true;

        protected void Start()
        {
            //if (UpdateChecker == null)
            //{
            //    UpdateChecker = new UpdateChecker();
            //}

            Log.debug("Achievements.Start");
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
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && this.toolbarControl == null)
            {
#if false
				this.AchButton = ApplicationLauncher.Instance.AddModApplication(
                    this.AchieveButtonOn,
                    this.AchievButtonOff,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    AchieveButton                 
                    );
#endif

                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(this.AchieveButtonOn, this.AchievButtonOff,
                    ApplicationLauncher.AppScenes.SPACECENTER,
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

        private void AchievButtonOff()
        {
            toggleAchievementsWindow();
            toolbarControl.SetTexture("Achievements/PluginData/Textures/button-normal-38", "Achievements/PluginData/Textures/button-normal-24");
        }
        public void DestroyButtons()
        {
#if false
			if (this.AchButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(this.AchButton);
            }
#endif
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
        }

        internal void Update()
        {
            // Since bodies are now generated from the database rather than being hardcoded,
            // the following "if" was added to wait until all the bodies and achievements had been initialized
            // This is necessary since the hard-coded version didn't need to wait becuase they
            // were initialized at the compile time
            if (EarnedAchievements.instance != null && EarnedAchievements.instance.allAchievementsCreated)
            {
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
            if (EarnedAchievements.instance != null )
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
            long now = DateTime.UtcNow.Ticks / 10000;
            if ((now - lastCheck) >= CHECK_INTERVAL)
            {
                if (EarnedAchievements.instance != null)
                {
                    foreach (Achievement achievement in EarnedAchievements.instance.achievementsList)
                    {
                        if (!EarnedAchievements.instance.earnedAchievements.ContainsKey(achievement))
                        {
                            Vessel vessel = (FlightGlobals.fetch != null) ? FlightGlobals.ActiveVessel : null;
                            try
                            {
                                if (achievement.check(vessel))
                                {
                                    string key = achievement.getKey();
                                    Debug.Log("achievement earned: " + key);
                                    AchievementEarn earn = new AchievementEarn(now, (vessel != null) ? vessel.vesselName : Achievements.UNKNOWN_VESSEL);
                                    EarnedAchievements.instance.earnedAchievements.Add(achievement, earn);

                                    // queue for later display
                                    queuedEarnedAchievements.Add(achievement);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }

                    //long done = DateTime.UtcNow.Ticks / 10000;
                    //Debug.LogWarning("checking achievements took " + (done - now) + " ms");

                    if ((queuedEarnedAchievements.Count() > 0) && (toast == null))
                    {
                        Achievement achievement = queuedEarnedAchievements.First<Achievement>();
                        queuedEarnedAchievements.Remove(achievement);

                        toast = new Toast(achievement, EarnedAchievements.instance.earnedAchievements[achievement]);
                        playAchievementEarnedClip();
                        awardReputation(achievement);
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
