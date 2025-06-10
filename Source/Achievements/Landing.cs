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
using static Achievements.Achievements;


namespace Achievements
{
    internal class LandingFactory : AchievementFactory
    {
        public IEnumerable<Achievement> getAchievements()
        {
            List<Achievement> achievements = new List<Achievement>();
            achievements.AddRange(new Achievement[] {
                new Landing(false, -1),

                new BodyLanding(Body.MOHO, false, Localizer.Format("#LOC_Ach_207")),
                new BodyLanding(Body.EVE, false, Localizer.Format("#LOC_Ach_208")),
                new BodyLanding(Body.KERBIN, false, Localizer.Format("#LOC_Ach_209")),
                new BodyLanding(Body.DUNA, false, Localizer.Format("#LOC_Ach_210")),
                new BodyLanding(Body.DRES, false, Localizer.Format("#LOC_Ach_211")),
                new BodyLanding(Body.EELOO, false, Localizer.Format("#LOC_Ach_212")),
                new BodyLanding(Body.GILLY, false, Localizer.Format("#LOC_Ach_213")),
                new BodyLanding(Body.MUN, false, Localizer.Format("#LOC_Ach_214")),
                new BodyLanding(Body.MINMUS, false, Localizer.Format("#LOC_Ach_215")),
                new BodyLanding(Body.IKE, false, Localizer.Format("#LOC_Ach_216")),
                new BodyLanding(Body.LAYTHE, false, Localizer.Format("#LOC_Ach_217")),
                new BodyLanding(Body.VALL, false, Localizer.Format("#LOC_Ach_218")),
                new BodyLanding(Body.TYLO, false, Localizer.Format("#LOC_Ach_219")),
                new BodyLanding(Body.BOP, false, Localizer.Format("#LOC_Ach_220")),
                new BodyLanding(Body.POL, false, Localizer.Format("#LOC_Ach_221")),

                new BodyLanding(Body.KERBIN, true, Localizer.Format("#LOC_Ach_222")),
                new BodyLanding(Body.EVE, true, Localizer.Format("#LOC_Ach_223")),
                new BodyLanding(Body.LAYTHE, true, Localizer.Format("#LOC_Ach_224"))
            });

            foreach (Body body in Body.ALL_LANDABLE.Where(b => !b.isStock()))
            {
                achievements.Add(new BodyLanding(body, false, Localizer.Format("#LOC_Ach_225") + body.name).addon());
            }
            foreach (Body body in Body.ALL_SPLASHABLE.Where(b => !b.isStock()))
            {
                achievements.Add(new BodyLanding(body, true, Localizer.Format("#LOC_Ach_226") + body.name).addon());
            }

            achievements.AddRange(new Achievement[] {
                new AllCrewAliveLanding(false,
                    Localizer.Format("#LOC_Ach_227"), Localizer.Format("#LOC_Ach_228"), Localizer.Format("#LOC_Ach_229")),
                new AllCrewAliveLanding(true,
                    Localizer.Format("#LOC_Ach_230"), Localizer.Format("#LOC_Ach_231"), Localizer.Format("#LOC_Ach_232")),
                new EnginesDestroyedLanding(),

                new BodyLanding(Body.MUN, false, true, -1, -1, new Location[] {
                    new Location(Body.MUN, 10.886.south(), 81.182.east(), 44000),
                    new Location(Body.MUN, 11.260.north(), 22.229.east(), 54000),
                    new Location(Body.MUN, 38.988.south(), 4.574.east(), 42000),
                    new Location(Body.MUN, 61.936.north(), 32.985.west(), 48000),
                    new Location(Body.MUN, 2.063.north(), 56.534.west(), 39000),
                    new Location(Body.MUN, 5.672.north(), 151.283.west(), 37000)
                }, Localizer.Format("#LOC_Ach_233"), Localizer.Format("#LOC_Ach_234"), Localizer.Format("#LOC_Ach_235")),
                new BodyLanding(Body.KERBIN, false, true, -1, -1, new Location[] { Location.KSC }, Localizer.Format("#LOC_Ach_236"), Localizer.Format("#LOC_Ach_237"), Localizer.Format("#LOC_Ach_238")),
                new BodyLanding(Body.KERBIN, false, true, -1, -1, new Location[] { Location.KERBIN_NORTH_POLE, Location.KERBIN_SOUTH_POLE },
                    Localizer.Format("#LOC_Ach_239"), Localizer.Format("#LOC_Ach_240"), Localizer.Format("#LOC_Ach_241")),
                new BodyLanding(Body.KERBIN, false, false, 10000, -1, new Location[] { Location.KSC_LAUNCH_PAD },
                    Localizer.Format("#LOC_Ach_242"), Localizer.Format("#LOC_Ach_243"), Localizer.Format("#LOC_Ach_244")),
                new BodyLanding(Body.KERBIN, false, false, 10000, -1, new Location[] { Location.KSC_HELICOPTER_PAD },
                    Localizer.Format("#LOC_Ach_245"), Localizer.Format("#LOC_Ach_246"), Localizer.Format("#LOC_Ach_247")),
                new BodyLanding(Body.KERBIN, false, false, 10000, -1, new Location[] { Location.KSC_RUNWAY },
                    Localizer.Format("#LOC_Ach_248"), Localizer.Format("#LOC_Ach_249"), Localizer.Format("#LOC_Ach_250")),
                new BodyLanding(Body.KERBIN, false, false, 10000, -1, new Location[] { Location.ISLAND_RUNWAY },
                    Localizer.Format("#LOC_Ach_251"), Localizer.Format("#LOC_Ach_252"), Localizer.Format("#LOC_Ach_253")),
                new BodyLanding(Body.MUN, false, true, -1, -1, new Location[] { Location.ARMSTRONG_MEMORIAL },
                    Localizer.Format("#LOC_Ach_254"), Localizer.Format("#LOC_Ach_255"), Localizer.Format("#LOC_Ach_256")).hide()
            });

            var l = new BodyLanding(Body.KERBIN, false, true, -1, -1, new Location[] { Location.KERBIN_NORTH_POLE, Location.KERBIN_SOUTH_POLE },
                    Localizer.Format("#LOC_Ach_239"), Localizer.Format("#LOC_Ach_240"), Localizer.Format("#LOC_Ach_241"));
            return achievements;
        }

        public Category getCategory()
        {
            return Category.LANDING;
        }
    }

    internal class Landing : AchievementBase
    {
        /* private*/
        internal bool stableOrbit;
        /* private*/
        internal double minAltitude;
        /* private*/
        internal bool flyingStep;
        /* private*/
        internal bool stableOrbitStep;
        /* private*/
        internal bool minAltitudeStep;

        internal Landing(bool stableOrbit, double minAltitude)
        {
            this.stableOrbit = stableOrbit;
            this.minAltitude = minAltitude;

            registerOnVesselChange(reset);
        }

        private void reset(Vessel vessel)
        {
            flyingStep = false;
            stableOrbitStep = false;
            minAltitudeStep = false;
        }

        public override bool check(Vessel vessel)
        {
            if (vessel != null)
                Log.Info("Landing.check, vessel: " + vessel.GetDisplayName());
            if ((vessel != null) && !vessel.isEVA())
            {
                if (!flyingStep)
                {
                    flyingStep = !vessel.isOnSurface();
                }
                Log.Info("flyingStep: " + flyingStep);

                if (!stableOrbitStep)
                {
                    stableOrbitStep = !stableOrbit || vessel.isInStableOrbit();
                }
                Log.Info("stableOrbitStep: " + stableOrbitStep);

                if (!minAltitudeStep)
                {
                    minAltitudeStep = (minAltitude < 0) || (vessel.altitude >= minAltitude);
                }
                Log.Info("minAltitudeStep: " + minAltitudeStep);
                return flyingStep && stableOrbitStep && minAltitudeStep && vessel.isOnSurface() && (vessel.horizontalSrfSpeed < 1d);
            }
            else
            {
                if (vessel != null)
                    Log.Info("vessel.isEVA");
                else
                    Log.Info("vessel is null");
                return false;
            }
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_257");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_258");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_259");
        }
    }

    internal class BodyLanding : Landing
    {
        /* private*/
        internal Body body;
        /* private*/
        internal bool splash;
        /* private*/
        internal double maxDegreesLatitudeFromEquator;
        /* private*/ internal  IEnumerable<Location> locations;
        /* private*/
        internal string title;
        /* private*/
        internal string text;
        /* private*/
        internal string key;

        internal BodyLanding(Body body, bool splash, string title)
            : this(body, splash, false, -1, -1, new Location[0], title,
                splash ? Localizer.Format("#LOC_Ach_260") + body.theName + "." : Localizer.Format("#LOC_Ach_261") + body.theName + ".",
                splash ? Localizer.Format("#LOC_Ach_262") + body.name : Localizer.Format("#LOC_Ach_167") + body.name)
        {
            this.body = body;
            this.splash = splash;
            this.title = title;
        }

        internal BodyLanding(Body body, bool splash, bool stableOrbit, double minAltitude, double maxDegreesLatitudeFromEquator,
            IEnumerable<Location> locations, string title, string text, string key)
            : base(stableOrbit, minAltitude)
        {

            this.body = body;
            this.splash = splash;
            this.maxDegreesLatitudeFromEquator = maxDegreesLatitudeFromEquator;
            this.locations = locations;
            this.title = title;
            this.text = text;
            this.key = key;
        }

        public override bool check(Vessel vessel)
        {
            return base.check(vessel) &&
                vessel.getCurrentBody().Equals(body) &&
                //vessel.getCurrentBody().name == body.name &&
                (splash ? vessel.isSplashed() : vessel.isLanded()) &&
                ((maxDegreesLatitudeFromEquator < 0) || (Math.Abs(vessel.latitude) <= maxDegreesLatitudeFromEquator)) &&
                isAtLocation(vessel);
        }

        private bool isAtLocation(Vessel vessel)
        {
            if (locations.Count() > 0)
            {
                foreach (Location location in locations)
                {
                    if (location.isAtLocation(vessel))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
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

    internal class AllCrewAliveLanding : AchievementBase
    {
        private bool mustUseAbort;
        private string title;
        private string text;
        private string key;
        private bool flyingStep;
        private int crewCount;
        private bool abortStep;

        internal AllCrewAliveLanding(bool mustUseAbort, string title, string text, string key)
        {
            this.mustUseAbort = mustUseAbort;
            this.title = title;
            this.text = text;
            this.key = key;

            registerOnVesselChange(reset);
        }

        private void reset(Vessel vessel)
        {
            flyingStep = false;
        }

        public override bool check(Vessel vessel)
        {
            if ((vessel != null) && !vessel.isEVA())
            {
                if (!flyingStep)
                {
                    crewCount = vessel.GetCrewCount();
                    flyingStep = !vessel.isOnSurface() && (crewCount > 0);
                }

                if (!abortStep)
                {
                    abortStep = !mustUseAbort || vessel.ActionGroups[KSPActionGroup.Abort];
                }

                return flyingStep && abortStep && vessel.isOnSurface() && (vessel.horizontalSrfSpeed < 1d) && (vessel.GetCrewCount() == crewCount);
            }
            else
            {
                return false;
            }
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

    internal class EnginesDestroyedLanding : Landing
    {
        private bool enginesStep;

        internal EnginesDestroyedLanding()
            : base(true, -1)
        {
            registerOnVesselChange(onVesselChange);
            registerOnPartRemove(onPartRemove);
        }

        private void onVesselChange(Vessel vessel)
        {
            reset();
        }

        private void onPartRemove(GameEvents.HostTargetAction<Part, Part> action)
        {
            if (!action.target.isEngine())
            {
                reset();
            }
        }

        private void reset()
        {
            enginesStep = false;
        }

        public override bool check(Vessel vessel)
        {
            if (vessel != null)
            {
                if (!enginesStep)
                {
                    enginesStep = vessel.getEnginesCount() > 0;
                }

                return base.check(vessel) && enginesStep && (vessel.getEnginesCount() == 0) && !vessel.getCurrentBody().Equals(Body.KERBIN); // replace with homeworld
            }
            else
            {
                return false;
            }
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_263");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_264");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_265");
        }
    }
}
