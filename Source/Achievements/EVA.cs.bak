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
            "One small step for a kerbal",
            "You need to be content with small steps",
            "Each step reveals a new horizon",
            "I can see my house from here",
            "Success!",
            "Hot Dog, that may have been a small one for Neil, but that's a long one for me!",
            "Hot dog, we made it!",
            "It may be just rocks, but its new rocks!",
            "That's one sm- oof! hold on. I tripped. Lemme try that again.",
            "Wait... we actually made it?",
            "Hey look, we didn't die!",
            "That's one small step- who put this thing here?!",
            "What the heck is tha- [CRACKLING]",
            "Okay. Now what?",
            "Where are the snacks?",
            "Docking with mothership comple--wait, that's a rock.",
            "Yep, thats me. You're probably wondering how i got myself into this situation.",
            "that's one big tumble from the hatch, one giant leap for kerbalkind. how do i get back up?",
            "Finally back home! ...oh....oh no...",
            "I left the snacks in the lander, can we do this over?",
            "uhh... i can't think of anything, cut the camera. we need to do this shot over again.",
            "Alright, wheres the flag, i want to get home.",
            "This landing was sponsored by the K Cola corporation.",
            "finally some fresh air. wait what do you mean i cant take my helmet off?",
            "This landing was sponsored by SpaceTux",
            "flag, science, and get out of here",
            "To infinity and beyond lol...",
            "Weeeee!",
            "Where's the bathroom?",
            "What's that smell??",
            "Elon lied to me !",
            "Damn my legs hurt",
            "I thought it would be less quiet",
            "I can't see my house from here"
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
                aList.Add(new evaAchievements(ienumBodyList,"Set foot on " + a.theName, quotes[quoteCnt],  "landing." + a.name));
                quoteCnt++;
                if (quoteCnt >= quotes.Length)
                    quoteCnt = 0;
            }

            aList.Add(new AllBodiesEVA(Body.ALL_LANDABLE, "Steps in the Sand", "Set foot on every planet and moon.", "landing.allBodiesEVA"));
            return aList.ToArray();
#if false
            return new Achievement[] {
                //new AllBodiesEVA(Body.STOCK_LANDABLE, "Steps in the Sand", "Set foot on every planet and moon.", "landing.allBodiesEVA"),
                new AllBodiesEVA(Body.ALL_LANDABLE, "Steps in the Sand", "Set foot on every planet and moon.", "landing.allBodiesEVA")
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
