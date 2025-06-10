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
using Smooth.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Achievements
{
    internal class BodyEVAFactory : AchievementFactory
    {
        internal class evaAchievements : CountingAchievement
        {
            List<IEnumerable<Body>> bodies;
            string title;
            string descr;
            string key;

            internal evaAchievements(List<IEnumerable<Body>> bodies, string title, string descr, string key) : base(1)
            {
                this.bodies = bodies;
                this.title = title;
                this.descr = descr;
                this.key = key;
            }
            public override string getTitle()
            {
                return title;
            }

            public override string getText()
            {
                return descr;
            }

            public override string getKey()
            {
                return key;
            }

        }
        string[] quotes =
        {
            Localizer.Format("#LOC_Ach_132"),
            Localizer.Format("#LOC_Ach_133"),
            Localizer.Format("#LOC_Ach_134"),
            Localizer.Format("#LOC_Ach_135"),
            Localizer.Format("#LOC_Ach_136"),
            Localizer.Format("#LOC_Ach_137"),
            Localizer.Format("#LOC_Ach_138"),
            Localizer.Format("#LOC_Ach_139"),
            Localizer.Format("#LOC_Ach_140"),
            Localizer.Format("#LOC_Ach_141"),
            Localizer.Format("#LOC_Ach_142"),
            Localizer.Format("#LOC_Ach_143"),
            Localizer.Format("#LOC_Ach_144"),
            Localizer.Format("#LOC_Ach_145"),
            Localizer.Format("#LOC_Ach_146"),
            Localizer.Format("#LOC_Ach_147"),
            Localizer.Format("#LOC_Ach_148"),
            Localizer.Format("#LOC_Ach_149"),
            Localizer.Format("#LOC_Ach_150"),
            Localizer.Format("#LOC_Ach_151"),
            Localizer.Format("#LOC_Ach_152"),
            Localizer.Format("#LOC_Ach_153"),
            Localizer.Format("#LOC_Ach_154"),
            Localizer.Format("#LOC_Ach_155"),
            Localizer.Format("#LOC_Ach_156"),
            Localizer.Format("#LOC_Ach_157"),
            Localizer.Format("#LOC_Ach_158"),
            Localizer.Format("#LOC_Ach_159"),
            Localizer.Format("#LOC_Ach_160"),
            Localizer.Format("#LOC_Ach_161"),
            Localizer.Format("#LOC_Ach_162"),
            Localizer.Format("#LOC_Ach_163"),
            Localizer.Format("#LOC_Ach_164"),
            Localizer.Format("#LOC_Ach_165")
        };
       


        public IEnumerable<Achievement> getAchievements()
        {
            List<Achievement> aList = new List<Achievement>();
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            int quoteCnt = UnityEngine.Random.Range(0, quotes.Length-1);

            foreach (var a in Body.ALL_LANDABLE)
            {
                List<Body> bodies = new List<Body>();
                bodies.Add(a);
                IEnumerable<Body> b = Body.flatten(bodies);
                List<IEnumerable<Body>> ienumBodyList = new List<IEnumerable<Body>>();
                ienumBodyList.Add(b);
                aList.Add(new evaAchievements(ienumBodyList,Localizer.Format("#LOC_Ach_166") + a.theName, quotes[quoteCnt],  Localizer.Format("#LOC_Ach_167") + a.name));
                quoteCnt++;
                if (quoteCnt >= quotes.Length)
                    quoteCnt = 0;
            }

            aList.Add(new AllBodiesEVA(Body.ALL_LANDABLE, Localizer.Format("#LOC_Ach_168"), Localizer.Format("#LOC_Ach_169"), Localizer.Format("#LOC_Ach_170")));
            return aList.ToArray();
#if false
            return new Achievement[] {
                //new AllBodiesEVA(Body.STOCK_LANDABLE, "Steps in the Sand", "Set foot on every planet and moon.", "landing.allBodiesEVA"),
                new AllBodiesEVA(Body.ALL_LANDABLE, Localizer.Format("#LOC_Ach_168"), Localizer.Format("#LOC_Ach_169"), Localizer.Format("#LOC_Ach_170"))
            };
#endif
        }

        public Category getCategory()
        {
            return Category.GROUND_OPERATIONS;
        }
    }

    internal class AllBodiesEVA : CountingAchievement
    {
        private IEnumerable<Body> bodies;
        private IEnumerable<Location> locations;
        private string title;
        private string text;
        private string key;
        private bool inSpace;
        private Dictionary<Body, bool> landedBodies = new Dictionary<Body, bool>();
        private Dictionary<Location, bool> landedLocations = new Dictionary<Location, bool>();

        protected AllBodiesEVA() : base(1) { }
        internal AllBodiesEVA(IEnumerable<Body> bodies, string title, string text, string key)
            : base(bodies.Count())
        {
            this.bodies = bodies;
            this.title = title;
            this.text = text;
            this.key = key;
        }

        internal AllBodiesEVA(IEnumerable<Location> locations, string title, string text, string key) : base((locations.Count()))
        {
            this.locations = locations;
            this.title = title;
            this.text = text;
            this.key = key;
        }

        public override void init(ConfigNode node)
        {
            inSpace = false;
            if (locations == null)
            {
                foreach (Body body in bodies)
                {
                    bool landed = false;
                    if (node.HasValue(body.name))
                    {
                        landed = bool.Parse(node.GetValue(body.name));
                    }
                    if (!landedBodies.ContainsKey(body))
                        landedBodies.Add(body, landed);
                    else
                    {
                        landedBodies[body] = landed;
                    }
                    if (landed)
                    {
                        increaseCounter();
                    }
                }
            }
            else
            {
                foreach (Location loc in locations)
                {
                    bool landed = false;

                    if (!landedLocations.ContainsKey(loc))
                        landedLocations.Add(loc, landed);
                    else
                    {
                        landedLocations[loc] = false;
                    }
                    if (landed)
                    {
                        increaseCounter();
                    }
                }

            }
        }

        public override void save(ConfigNode node)
        {
            if (locations == null)
            {
                foreach (Body body in landedBodies.Keys)
                {
                    if (landedBodies[body])
                    {
                        if (node.HasValue(body.name))
                        {
                            node.RemoveValue(body.name);
                        }
                        node.AddValue(body.name, landedBodies[body].ToString());
                    }
                }
            }
            else
            {
                foreach (Location loc in landedLocations.Keys)
                {
                    if (landedLocations[loc])
                    {
                        if (node.HasValue(loc.ToString()))
                        {
                            node.RemoveValue(loc.ToString());
                        }
                        node.AddValue(loc.ToString(), landedLocations[loc].ToString());
                    }
                }

            }
        }

        public override bool check(Vessel vessel)
        {
            if ((vessel != null) && vessel.isEVA())
            {
                if (locations == null)
                {
                    if (vessel.isOnSurface())
                    {
                        Body body = vessel.getCurrentBody();
                        if (bodies.Contains(body))
                        {
                            if (landedBodies.ContainsKey(body))
                            {
                                landedBodies.Remove(body);
                            }
                            landedBodies.Add(body, true);

                            resetCounter();
                            foreach (var x in landedBodies.Where(kv => kv.Value))
                            {
                                increaseCounter();
                            }
                        }
                    }
                }
                else
                {
                    if (vessel.isOnSurface() && !inSpace)
                    {
                        foreach (var loc in locations)
                        {
                            if (loc.isAtLocation(vessel))
                            {
                                if (landedLocations.ContainsKey(loc))
                                {
                                    landedLocations.Remove(loc);
                                }
                                landedLocations.Add(loc, true);

                                resetCounter();
                                foreach (var x in landedLocations.Where(kv => kv.Value))
                                {
                                    increaseCounter();
                                }

                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
            return base.check(vessel);
        }

        public override string getTitle()
        {
            return title;
        }

        public override string getText()
        {
            return text;
        }

        public override string getKey()
        {
            return key;
        }
    }
}
