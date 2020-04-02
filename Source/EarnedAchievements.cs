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

namespace Achievements
{

    internal class EarnedAchievements : ScenarioModule
    {
        private static List<Type> achievementFactoryTypes;

        internal static EarnedAchievements instance
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
        internal Dictionary<Category, IEnumerable<Achievement>> achievements
        {
            get;
            private set;
        }

        private List<Achievement> achievementsList_;
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

        internal Dictionary<Achievement, AchievementEarn> earnedAchievements
        {
            get;
            private set;
        }

        private static void installScenario()
        {
            Game game = HighLogic.CurrentGame;
            if ((game != null) && !game.scenarios.Any(s => s.moduleName == typeof(EarnedAchievements).Name))
            {
                // add scenario to game
                ProtoScenarioModule scenario = game.AddProtoScenarioModule(typeof(EarnedAchievements),
                    GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION);

                // load scenario instantly
                if (scenario.targetScenes.Contains(HighLogic.LoadedScene))
                {
                    scenario.Load(ScenarioRunner.Instance);
                }
            }
        }

        // Since bodies are now generated from the database rather than being hardcoded,
        // the following was added to wait until all the bodies had been initialized
        // This is necessary since the hard-coded version didn't need to wait becuase they
        // were initialized at the compile time
        public override void OnLoad(ConfigNode node)
        {
            StartCoroutine(WaitForBody(node));
        }
        IEnumerator WaitForBody(ConfigNode node)
        {
            WaitForSeconds wfs = new WaitForSeconds(0.1f);
            while (!Body.initted)
                yield return wfs;
            achievements = createAchievements();

            if (node.HasNode("achievements"))
            {
                Log.info("achievements node found");
                node = node.GetNode("achievements");
            }
            else
            {
                string settingsFile = KSPUtil.ApplicationRootPath + "GameData/Achievements/achievements.dat";
                if (System.IO.File.Exists(settingsFile))
                {
                    Debug.Log("converting earned achievements from global to current save");
                }
                node = ConfigNode.Load(settingsFile) ?? new ConfigNode();
            }
            earnedAchievements = loadEarnedAchievements(node);
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

        private Dictionary<Achievement, AchievementEarn> loadEarnedAchievements(ConfigNode node)
        {
            Log.info("loadEarnedAchievements");
            Dictionary<Achievement, AchievementEarn> result = new Dictionary<Achievement, AchievementEarn>();

            // old way of doing things
            List<ConfigNode> legacyNodes = new List<ConfigNode>();
            foreach (ConfigNode.Value value in node.values)
            {
                string key = value.name;

                // legacy
                if (key == "launch")
                {
                    key = "launch.1";
                }

                string time = value.value;
                ConfigNode legacyNode = new ConfigNode(key);
                legacyNode.AddValue("time", time);
                legacyNode.AddValue("flight", Achievements.UNKNOWN_VESSEL);
                legacyNodes.Add(legacyNode);
            }
            foreach (ConfigNode legacyNode in legacyNodes)
            {
                node.RemoveValue(legacyNode.name);
                node.AddNode(legacyNode);
            }

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
                        AchievementEarn earn = new AchievementEarn(time, flightName);
                        result.Add(achievement, earn);
                    }
                }
            }

            Debug.Log("loaded " + result.Count() + " earned achievements");
            return result;
        }

        private void saveEarnedAchievements(ConfigNode node)
        {
            Debug.Log("saving achievements (" + earnedAchievements.Count() + " earned)");
            foreach (Achievement achievement in achievementsList)
            {
                ConfigNode achievementNode = node.AddNode(achievement.getKey());

                achievement.save(achievementNode);

                AchievementEarn earn = earnedAchievements.ContainsKey(achievement) ? earnedAchievements[achievement] : null;
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
            Log.info("getAchievementsList, achievements loaded: " + achList.Count());
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
                    Log.info("EarnedAchievements.createAchievements completed");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            Debug.Log("number of achievements: " + achievements.getValuesCount() + " in " + achievements.Keys.Count() + " categories");
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
