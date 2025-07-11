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

namespace Achievements
{
    internal class SurfaceSampleFactory : AchievementFactory
    {
        public IEnumerable<Achievement> getAchievements()
        {
            List<Achievement> achievements = new List<Achievement>();
            foreach (Body body in Body.ALL_LANDABLE)
            {
                achievements.Add(new BodySurfaceSample(body).addon(!body.isStock()));
            }
            achievements.AddRange(new Achievement[] {
                new SurfaceSample(),
                new AllBodiesSurfaceSample(Body.ALL_LANDABLE, Localizer.Format("#LOC_Ach_313"), Localizer.Format("#LOC_Ach_314"), Localizer.Format("#LOC_Ach_315")),
                new AsteroidSample()
            });
            return achievements;
        }

        public Category getCategory()
        {
            return Category.RESEARCH_AND_DEVELOPMENT;
        }
    }

    internal class SurfaceSample : AchievementBase
    {
        public override bool check(Vessel vessel)
        {
            return (vessel != null) && vessel.isEVA() && vessel.hasSurfaceSample();
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_316");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_317");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_54");
        }
    }

    internal class BodySurfaceSample : SurfaceSample
    {
        private Body body;

        internal BodySurfaceSample(Body body)
        {
            this.body = body;
        }

        public override bool check(Vessel vessel)
        {
            return base.check(vessel) && vessel.getCurrentBody().Equals(body);
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_318") + body.name;
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_319") + body.theName + ".";
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_320") + body.name;
        }
    }

    internal class AllBodiesSurfaceSample : CountingAchievement
    {
        private IEnumerable<Body> bodies;
        private string title;
        private string text;
        private string key;
        private Dictionary<Body, bool> sampleBodies = new Dictionary<Body, bool>();

        internal AllBodiesSurfaceSample(IEnumerable<Body> bodies, string title, string text, string key)
            : base(bodies.Count())
        {
            this.bodies = bodies;
            this.title = title;
            this.text = text;
            this.key = key;
        }

        public override void init(ConfigNode node)
        {
            foreach (Body body in bodies)
            {
                bool surfaceSample = false;
                if (node.HasValue(body.name))
                {
                    surfaceSample = bool.Parse(node.GetValue(body.name));
                }
                if (!sampleBodies.ContainsKey(body))
                    sampleBodies.Add(body, surfaceSample);
                else
                    sampleBodies[body] = surfaceSample;
                if (surfaceSample)
                {
                    increaseCounter();
                }
            }
        }

        public override void save(ConfigNode node)
        {
            foreach (Body body in sampleBodies.Keys)
            {
                if (sampleBodies[body])
                {
                    if (node.HasValue(body.name))
                    {
                        node.RemoveValue(body.name);
                    }
                    node.AddValue(body.name, sampleBodies[body].ToString());
                }
            }
        }

        public override bool check(Vessel vessel)
        {
            if ((vessel != null) && vessel.isEVA() && vessel.hasSurfaceSample())
            {
                Body body = vessel.getCurrentBody();
                if (bodies.Contains(body))
                {
                    if (sampleBodies.ContainsKey(body))
                    {
                        sampleBodies.Remove(body);
                    }
                    sampleBodies.Add(body, true);

                    resetCounter();
                    foreach (var x in sampleBodies.Where(kv => kv.Value))
                    {
                        increaseCounter();
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

    internal class AsteroidSample : AchievementBase
    {
        public override bool check(Vessel vessel)
        {
            return (vessel != null) && vessel.isEVA() && vessel.hasAsteroidSample();
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_321");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_322");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_55");
        }
    }
}
