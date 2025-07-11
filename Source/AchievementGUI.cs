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
using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ToolbarControl_NS;
using static Achievements.Achievements;


namespace Achievements {
	internal class AchievementGUI {
		internal static readonly int TEX_WIDTH = 340;
		internal static readonly int TEX_HEIGHT = 75;
		internal static readonly int TEX_BORDER = 10;
		internal static readonly int EXPANDED_INSET = 50;

		private static readonly int TEX_HEIGHT_EXPANSION = 32;

		private static readonly int FONT_SIZE = 13;
		private static readonly int TITLE_FONT_SIZE = 15;
		private static readonly int FONT_SIZE_EXPANSION = 11;
		private static readonly int TEXT_MARGIN = TEX_BORDER * 2;

		internal Callback clickCallback;

		private Achievement achievement;
        //private Achievements achievements;
		private AchievementEarn earn;
		private bool addon;       
        private Texture2D toastTex;
		private Texture2D toastNotEarnedTex;
		private Texture2D toastAddonTex;
		private Texture2D toastAddonNotEarnedTex;
		private Texture2D toastExpandedTex;
		private Texture2D toastAddonExpandedTex;       
		private GUIStyle bgStyle;
		private GUIStyle bgNotEarnedStyle;
		private GUIStyle bgAddonStyle;
		private GUIStyle bgAddonNotEarnedStyle;
		private GUIStyle bgExpandedStyle;
		private GUIStyle bgAddonExpandedStyle;
		private GUIStyle textStyle;
		private GUIStyle textNotEarnedStyle;
		private GUIStyle textExpansionStyle;
		private GUIStyle titleStyle;
		private GUIStyle titleNotEarnedStyle;
		private bool hover;
		private bool mouseDown;

        internal AchievementGUI(Achievement achievement, AchievementEarn earn) {
			this.achievement = achievement;
			this.earn = earn;

			// achievements from other assemblies are always addons
			addon = achievement.isAddon() || !achievement.GetType().Assembly.Equals(typeof(Achievements).Assembly);
#if false
#region NO_LOCALIZATION
		    toastTex = GameDatabase.Instance.GetTexture("Achievements/toast", false);
			toastNotEarnedTex = GameDatabase.Instance.GetTexture("Achievements/toast-not-earned", false);
			toastAddonTex = GameDatabase.Instance.GetTexture("Achievements/toast-addon", false);
			toastAddonNotEarnedTex = GameDatabase.Instance.GetTexture("Achievements/toast-addon-not-earned", false);
			toastExpandedTex = GameDatabase.Instance.GetTexture("Achievements/toast-expanded", false);
			toastAddonExpandedTex = GameDatabase.Instance.GetTexture("Achievements/toast-addon-expanded", false);            
#endregion
#endif
			toastTex = new Texture2D(2, 2);
			toastNotEarnedTex = new Texture2D(2, 2);
			toastAddonTex = new Texture2D(2, 2);
			toastAddonNotEarnedTex = new Texture2D(2, 2);
			toastExpandedTex = new Texture2D(2, 2);
			toastAddonExpandedTex = new Texture2D(2, 2);
			if (!ToolbarControl.LoadImageFromFile(ref toastTex, KSPUtil.ApplicationRootPath+"GameData/Achievements/PluginData/Textures/toast"))
				Log.Error("Unable to load toast from file");
			if (!ToolbarControl.LoadImageFromFile(ref toastNotEarnedTex, KSPUtil.ApplicationRootPath + "GameData/Achievements/PluginData/Textures/toast-not-earned"))
				Log.Error("Unable to load toast-not-earned from file");
			if (!ToolbarControl.LoadImageFromFile(ref toastAddonTex, KSPUtil.ApplicationRootPath + "GameData/Achievements/PluginData/Textures/toast-addon"))
				Log.Error("Unable to load toast-addon from file");
			if (!ToolbarControl.LoadImageFromFile(ref toastAddonNotEarnedTex, KSPUtil.ApplicationRootPath + "GameData/Achievements/PluginData/Textures/toast-addon-not-earned"))
				Log.Error("Unable to load toast-addon-not-earned from file");
			if (!ToolbarControl.LoadImageFromFile(ref toastExpandedTex, KSPUtil.ApplicationRootPath + "GameData/Achievements/PluginData/Textures/toast-expanded"))
				Log.Error("Unable to load toast-expanded from file");
			if (!ToolbarControl.LoadImageFromFile(ref toastAddonExpandedTex, KSPUtil.ApplicationRootPath + "GameData/Achievements/PluginData/Textures/toast-addon-expanded"))
				Log.Error("Unable to load toast-addon-expanded from file");
			int width = TEX_WIDTH + 300;
			int height = Screen.height / 2;

			bgStyle = new GUIStyle();
			bgStyle.normal.background = toastTex;
			bgStyle.fixedWidth = TEX_WIDTH;
			bgStyle.fixedHeight = TEX_HEIGHT + TEX_BORDER * 2;

			bgNotEarnedStyle = new GUIStyle();
			bgNotEarnedStyle.normal.background = toastNotEarnedTex;
			bgNotEarnedStyle.fixedWidth = TEX_WIDTH;
			bgNotEarnedStyle.fixedHeight = TEX_HEIGHT + TEX_BORDER * 2;

			bgAddonStyle = new GUIStyle(bgStyle);
			bgAddonStyle.normal.background = toastAddonTex;

			bgAddonNotEarnedStyle = new GUIStyle(bgNotEarnedStyle);
			bgAddonNotEarnedStyle.normal.background = toastAddonNotEarnedTex;

			bgExpandedStyle = new GUIStyle(bgStyle);
			bgExpandedStyle.normal.background = toastExpandedTex;
			bgExpandedStyle.fixedHeight = TEX_HEIGHT + TEX_HEIGHT_EXPANSION + TEX_BORDER * 2;

			bgAddonExpandedStyle = new GUIStyle(bgStyle);
			bgAddonExpandedStyle.normal.background = toastAddonExpandedTex;
			bgAddonExpandedStyle.fixedHeight = TEX_HEIGHT + TEX_HEIGHT_EXPANSION + TEX_BORDER * 2;

			textStyle = new GUIStyle();
			textStyle.alignment = TextAnchor.MiddleCenter;
			textStyle.fontSize = FONT_SIZE;
			textStyle.normal.textColor = Color.black;
			textStyle.padding = new RectOffset(TEXT_MARGIN, TEXT_MARGIN, 0, 0);
			textStyle.wordWrap = true;
			textStyle.fixedWidth = TEX_WIDTH;

			textNotEarnedStyle = new GUIStyle();
			textNotEarnedStyle.alignment = TextAnchor.MiddleCenter;
			textNotEarnedStyle.fontSize = FONT_SIZE;
			textNotEarnedStyle.normal.textColor = Color.grey;
			textNotEarnedStyle.padding = new RectOffset(TEXT_MARGIN, TEXT_MARGIN, 0, 0);
			textNotEarnedStyle.wordWrap = true;
			textNotEarnedStyle.fixedWidth = TEX_WIDTH;

			textExpansionStyle = new GUIStyle(textStyle);
			textExpansionStyle.fontSize = FONT_SIZE_EXPANSION;
			textExpansionStyle.fixedWidth = TEX_WIDTH - EXPANDED_INSET;

			titleStyle = new GUIStyle(textStyle);
			titleStyle.fontSize = TITLE_FONT_SIZE;
			titleStyle.fontStyle = FontStyle.Bold;

			titleNotEarnedStyle = new GUIStyle(textNotEarnedStyle);
			titleNotEarnedStyle.fontSize = TITLE_FONT_SIZE;
			titleNotEarnedStyle.fontStyle = FontStyle.Bold;
		}
      
        internal void draw(bool showCounter, bool showAddonIndicator, bool expanded) {
			if (earn == null) {
				expanded = false;
			}

			GUIStyle bgStyle;
			if (earn != null) {
				bgStyle = (showAddonIndicator && addon) ? (expanded ? bgAddonExpandedStyle : bgAddonStyle) : (expanded ? bgExpandedStyle : this.bgStyle);
			} else {
				bgStyle = (showAddonIndicator && addon) ? bgAddonNotEarnedStyle : bgNotEarnedStyle;
			}
			GUILayout.BeginVertical(bgStyle, GUILayout.Width(TEX_WIDTH),
				GUILayout.Height(TEX_HEIGHT + TEX_BORDER * 2 + (expanded ? TEX_HEIGHT_EXPANSION : 0)),
				GUILayout.ExpandWidth(true));

			GUILayout.Space(TEX_BORDER);

			GUILayout.BeginVertical(GUIStyle.none, GUILayout.Width(TEX_WIDTH), GUILayout.Height(TEX_HEIGHT), GUILayout.ExpandWidth(true));
			GUILayout.FlexibleSpace();
			GUILayout.Label(achievement.getTitle(), (earn != null) ? titleStyle : titleNotEarnedStyle,
				GUILayout.Width(TEX_WIDTH), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);
			string text = achievement.getText();
			if ((achievement is CountingAchievement) && showCounter && (earn == null)) {
				int minRequired = ((CountingAchievement) achievement).minRequired;
				if (minRequired > 1) {
					// clamp to minRequired
					int count = Math.Min(((CountingAchievement) achievement).getCount(), minRequired);
                    #region NO_LOCALIZATION
                    text += " (" + count.ToString("D0") + "/" + minRequired.ToString("D0") + ")";
                    #endregion
                }
            }
			GUILayout.Label(text, (earn != null) ? textStyle : textNotEarnedStyle, GUILayout.Width(TEX_WIDTH), GUILayout.ExpandWidth(true));
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

			if (expanded) {
				GUILayout.BeginVertical(GUIStyle.none, GUILayout.Width(TEX_WIDTH), GUILayout.Height(TEX_HEIGHT_EXPANSION), GUILayout.ExpandWidth(true));
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal(GUIStyle.none);
				GUILayout.Space(EXPANDED_INSET);
				string flightName = (earn.flightName == Achievements.UNKNOWN_VESSEL) ? Localizer.Format("#LOC_Ach_1") : earn.flightName;
				string date = new DateTime(earn.time * 10000).ToShortDateString();
				GUILayout.Label(flightName + ", " + date, textExpansionStyle, GUILayout.Width(TEX_WIDTH - EXPANDED_INSET), GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
			}

			GUILayout.Space(TEX_BORDER);

			GUILayout.EndVertical();

			if (Event.current.type == EventType.Repaint) {
				hover = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
			}
		}

		internal void update() {
			if (hover && (clickCallback != null)) {
				if (Input.GetMouseButtonDown(0)) {
					mouseDown = true;
				}

				if (mouseDown && Input.GetMouseButtonUp(0)) {
					mouseDown = false;
					clickCallback.Invoke();
				}
			} else {
				mouseDown = false;
			}
		}
	}
}
