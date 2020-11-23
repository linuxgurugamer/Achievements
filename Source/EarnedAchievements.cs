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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Achievements.Achievements;

namespace Achievements
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION)]
    internal class EarnedAchievements : ScenarioModule
    {
        private static List<Type> achievementFactoryTypes;

        internal static EarnedAchievements instance;
#if false
        {
            get
            {
                installScenario();

                Game game = HighLogic.CurrentGame;
                if (game != null)
                {
                    return game.scenarios.Select(s => s.moduleRef).OfType<EarnedAchievements>().SingleOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }
#endif

        public override void OnAwake()
        {
            //installScenario();
            instance = this;
        }

        internal Dictionary<Category, IEnumerable<Achievement>> achievements
        {
            get;
            private set;
        }

#if false
        static private List<Achievement> achievementsList_ = null;
        static internal int achievementsListCnt = 0;
#else
         private List<Achievement> achievementsList_ = null;
         internal int achievementsListCnt = 0;

#endif
        internal List<Achievement> achievementsList
        {
            get
            {
                if (achievementsList_ == null || achievementsList_.Count() == 0)
                {
                    achievementsList_ = getAchievementsList();
                }
                return achievementsList_;
            }
        }

        internal Dictionary<string, AchievementEarn> earnedAchievements
        {
            get;
            private set;
        }

        // Since bodies are now generated from the database rather than being hardcoded,
        // the following was added to wait until all the bodies had been initialized
        // This is necessary since the hard-coded version didn't need to wait becuase they
        // were initialized at the compile time

        // The various modules need to be initialized in a specific order:
        //
        // Modules to run before ready to go, in order of need

        //  Body
        // All static achievements via getAchievements()
        // AchievementLoad
        // EarnedAchievements
        public override void OnLoad(ConfigNode node)
        {
            StartCoroutine(WaitForBody(node));
        }
        IEnumerator WaitForBody(ConfigNode node)
        {
            WaitForSeconds wfs = new WaitForSeconds(0.25f);

            // First wait for the Body to be initialized, check ever 1/4 second until it is
            while (!Body.initted)
                yield return wfs;

            // Now the static achievements
            achievements = createAchievements(); // which calls the getAchievements() methods in all the classes

            // Now load the custom achievements from any/all configs
            if (node.HasNode("achievements"))
            {
                node = node.GetNode("achievements");

                AchievementLoad al = new AchievementLoad();
                al.LoadCfgAchievements();

                earnedAchievements = loadEarnedAchievements(node);
            }
#if false
            Log.Info("Achievement Dump Start");
            int cat = 0, cnt = 0;
            foreach (var c in achievements)
            {
                cat++;
                cnt = 0;
                foreach (var e in c.Value)
                {
                    cnt++;
                    Log.Info(cat + "." + cnt + ":  " + e.getKey() + ": " + e.getTitle() + ":" + e.getText());
                }
            }
            Log.Info("Achievement Dump End");
#endif

            yield return null;
        }

        public override void OnSave(ConfigNode node)
        {
            if (node.HasNode("achievements"))
            {
                node.RemoveNode("achievements");
            }
            node = node.AddNode("achievements");
            saveEarnedAchievements(node);
        }

        internal void OnDestroy()
        {
            if (achievementsList_ != null)
            {
                foreach (Achievement achievement in achievementsList_)
                {
                    achievement.destroy();
                }
            }
        }

        private Dictionary<string, AchievementEarn> loadEarnedAchievements(ConfigNode node)
        {
            Dictionary<string, AchievementEarn> result = new Dictionary<string, AchievementEarn>();

            // new way
            foreach (Achievement achievement in achievementsList)
            {
                string key = achievement.getKey();
                if (node.HasNode(key))
                {
                    ConfigNode achievementNode = node.GetNode(key);
                    achievement.init(achievementNode);

                    if (achievementNode.HasValue("time") && achievementNode.HasValue("flight"))
                    {
                        long time = long.Parse(achievementNode.GetValue("time"));
                        string flightName = achievementNode.HasValue("flight") ? achievementNode.GetValue("flight") : null;
                        AchievementEarn earn = new AchievementEarn(time, flightName, achievement);
                        if (result.ContainsKey(achievement.getKey()))
                        {
                            Log.Info("loadEarnedAchievements, duplicate key: " + achievement.getKey());
                        }
                        else
                        {
                            result.Add(achievement.getKey(), earn);
                            Log.Info("loadEarnedAchievements, adding key: " + achievement.getKey());
                        }
                    }
                }
            }

            return result;
        }

        private void saveEarnedAchievements(ConfigNode node)
        {
            if (achievementsList == null || earnedAchievements == null)
                return;
            foreach (Achievement achievement in achievementsList)
            {
                ConfigNode achievementNode = node.AddNode(achievement.getKey());

                achievement.save(achievementNode);

                AchievementEarn earn = earnedAchievements.ContainsKey(achievement.getKey()) ? earnedAchievements[achievement.getKey()] : null;
                if (earn != null)
                {
                    achievementNode.AddValue("time", earn.time);
                    if (earn.flightName != null)
                    {
                        achievementNode.AddValue("flight", earn.flightName);
                    }
                }
            }
        }


        private List<Achievement> getAchievementsList()
        {
            List<Achievement> achList = new List<Achievement>();
            if (Body.initted && this.achievements != null)
            {
                foreach (IEnumerable<Achievement> categoryAchievements in this.achievements.Values.AsEnumerable())
                {
                    achList.AddRange(categoryAchievements);
                }
            }
            return achList;
        }

        // This MUST be finished before other stuff happens
        internal bool allAchievementsCreated { get; private set; } = false;

        private Dictionary<Category, IEnumerable<Achievement>> createAchievements()
        {
            // Can't do anything if Bodies have not yet been initialized
            if (!Body.initted) return null;

            IEnumerable<Type> factoryTypes = getAchievementFactoryTypes();
            Dictionary<Category, IEnumerable<Achievement>> achievements = new Dictionary<Category, IEnumerable<Achievement>>();
            foreach (Type type in factoryTypes)
            {
                try
                {
                    // Find all classes derived from the class AchievementFactory and invoke the constructor
                    // This instantiates all the classes without having to explictly list each one
                    AchievementFactory factory = (AchievementFactory)type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    Category category = factory.getCategory();
                    List<Achievement> categoryAchievements;
                    if (achievements.ContainsKey(category))
                    {
                        categoryAchievements = (List<Achievement>)achievements[category];
                    }
                    else
                    {
                        categoryAchievements = new List<Achievement>();
                        achievements.Add(category, categoryAchievements);
                    }
                    IEnumerable<Achievement> factoryAchievements = factory.getAchievements();
                    categoryAchievements.AddRange(factoryAchievements);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            allAchievementsCreated = true;
            return achievements;
        }

        private static IEnumerable<Type> getAchievementFactoryTypes()
        {
            if (achievementFactoryTypes == null)
            {
                achievementFactoryTypes = new List<Type>();
                Type achievementFactoryType = typeof(AchievementFactory);
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        achievementFactoryTypes.AddRange(assembly.GetTypes().Where<Type>(isAchievementFactoryType));
                    }
                    catch (Exception)
                    {
                        Debug.LogWarning("exception while loading types, ignoring assembly: " + assembly.FullName);
                    }
                }
            }
            return achievementFactoryTypes;
        }

        private static bool isAchievementFactoryType(Type type)
        {
            if (type.IsClass)
            {
                Type interfaceType = type.GetInterface(typeof(AchievementFactory).FullName);
                return (interfaceType != null) && interfaceType.Equals(typeof(AchievementFactory));
            }
            else
            {
                return false;
            }
        }
    }
}
