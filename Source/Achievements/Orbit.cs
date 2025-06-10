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
//using System.Numeric;
using UnityEngine;

namespace Achievements
{
    internal class OrbitFactory : AchievementFactory
    {
        public IEnumerable<Achievement> getAchievements()
        {
            List<Achievement> achievements = new List<Achievement>();
            foreach (Body body in Body.ALL)
            {
                achievements.Add(new BodyOrbit(body).addon(!body.isStock()));
            }
            achievements.AddRange(new Achievement[] {
                new OrbitAchievement(),
                new ExtraKerbalPlanetOrbit(),
                new MoonOrbit(),
                new KesslerSyndrome(),
                new SSTO(),
                new JetPackOrbit(),
                new SunEscapeTrajectory(),
                new PreventAsteroidImpact(),
                new MoveAsteroidIntoStableOrbit()
            });
            return achievements;
        }

        public Category getCategory()
        {
            return Category.SPACEFLIGHT;
        }
    }

    internal class SpecifiedOrbitAchievement : AchievementBase
    {
        string title;
        string text;
        IEnumerable<Body> bodies;
        double minAltitude;
        double maxAltitude;
        double minEccentricity;
        double maxEccentricity;
        double minInclination;
        double maxInclination;
        internal SpecifiedOrbitAchievement(IEnumerable<Body> bodies,
            string title,
            string text,            
            double minAltitude = -1,
            double maxAltitude = -1,
            double minEccentricity = 0,
            double maxEccentricity = AchievementLoad.MAX_ECCENTRICITY,
            double minInclination = 0,
            double maxInclination = 90)
        {
            this.title = title;
            this.text = text;
            this.bodies = bodies;
            this.minAltitude = minAltitude;
            this.maxAltitude = maxAltitude;
            this.minEccentricity = minEccentricity;
            this.maxEccentricity = maxEccentricity;
            this.minInclination = minInclination;
            this.maxInclination = maxInclination;
        }
        public override bool check(Vessel vessel)
        {
            if (vessel != null && vessel.isInStableOrbit())
            {
                foreach (Body body in bodies)
                {
                    if (vessel.getCurrentBody().Equals(body))
                    {
                        Orbit orbit = vessel.orbit;
                        if (orbit.altitude >= minAltitude &&
                            orbit.altitude <= maxAltitude &&
                            orbit.eccentricity >= minEccentricity &&
                            orbit.eccentricity <= maxEccentricity &&
                            orbit.inclination >= minInclination &&
                            orbit.inclination <= maxInclination)
                            return true;
                        break;
                    }
                }
            }
            return false;
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
            #region NO_LOCALIZATION
            string str = "stableOrbit." + minAltitude.ToString("F0") + "." +maxAltitude.ToString("F0") +"."+
                minEccentricity.ToString("F0") + "." + maxEccentricity.ToString("F0") + "." +
                minInclination.ToString("F0") + "." + maxInclination.ToString("F0");
            #endregion
            foreach (Body body in bodies)
                str += "." + body.name;
            return str;
                //return "stableOrbit" ;
        }
    }

    internal class OrbitAchievement : AchievementBase
    {
        public override bool check(Vessel vessel)
        {
            return (vessel != null) && vessel.isInStableOrbit();
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_274");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_275");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_276");
        }
    }

    internal class BodyOrbit : OrbitAchievement
    {
        private Body body;

        internal BodyOrbit(Body body)
        {
            this.body = body;
        }

        public override bool check(Vessel vessel)
        {
            return base.check(vessel) && vessel.getCurrentBody().Equals(body);
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_277") + body.name;
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_278") + body.theName + ".";
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_279") + body.name;
        }
    }

    internal abstract class AnyBodyOrbit : OrbitAchievement
    {
        private IEnumerable<Body> bodies;

        internal AnyBodyOrbit(IEnumerable<Body> bodies)
        {
            this.bodies = bodies;
        }

        public override bool check(Vessel vessel)
        {
            return base.check(vessel) && bodies.Contains(vessel.getCurrentBody());
        }
    }






    internal class ExtraKerbalPlanetOrbit : AnyBodyOrbit
    {
        internal ExtraKerbalPlanetOrbit()
            : base(Body.ALL_PLANETS_WITHOUT_HOMEWORLD)
        {
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_280");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_281");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_282");
        }
    }

    internal class MoonOrbit : AnyBodyOrbit
    {
        internal MoonOrbit()
            : base(Body.ALL_MOONS)
        {
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_283");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_284");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_285");
        }
    }

    internal class KesslerSyndrome : CountingAchievement
    {
        private bool counted;

        internal KesslerSyndrome()
            : base(100)
        {
            registerOnVesselCreate(onVesselCreate);
        }

        private void onVesselCreate(Vessel vessel)
        {
            counted = false;
        }

        public override bool check(Vessel vessel)
        {
            if (FlightGlobals.fetch != null)
            {
                if (!counted)
                {
                    resetCounter();
                    foreach (Vessel v in FlightGlobals.Vessels.Where((v =>
                            (v.vesselType == VesselType.Debris) && v.getCurrentBody().Equals(Body.KERBIN) && v.isInStableOrbit())))
                    {

                        increaseCounter();
                    }
                    counted = true;
                }

                return base.check(vessel);
            }
            else
            {
                return false;
            }
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_286");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_287");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_288");
        }
    }

    internal class SSTO : OrbitAchievement
    {
        private bool preLaunchStep;
        private int numParts;

        internal SSTO()
        {
            registerOnVesselChange(onVesselChange);
        }

        private void onVesselChange(Vessel vessel)
        {
        }

        public override bool check(Vessel vessel)
        {
            if (vessel != null)
            {
                if (!preLaunchStep)
                {
                    preLaunchStep = vessel.isPreLaunched();
                }

                if (vessel.isPreLaunched())
                {
                    numParts = getNumParts(vessel);
                }

                return base.check(vessel) && preLaunchStep && (getNumParts(vessel) == numParts) && vessel.getCurrentBody().Equals(Body.KERBIN); // replace with homeworld
            }
            else
            {
                return false;
            }
        }

        private int getNumParts(Vessel vessel)
        {
            IEnumerable<Part> launchClampParts = vessel.FindPartModulesImplementing<LaunchClamp>().ConvertAll(lc => lc.part);
            return vessel.parts.Count() - launchClampParts.Count();
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_289");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_290");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_291");
        }
    }

    internal class JetPackOrbit : OrbitAchievement
    {
        private bool surfaceStep;

        internal JetPackOrbit()
        {
            registerOnVesselChange(onVesselChange);
        }

        private void onVesselChange(Vessel vessel)
        {
            surfaceStep = false;
        }

        public override bool check(Vessel vessel)
        {
            if ((vessel != null) && vessel.isEVA())
            {
                if (!surfaceStep)
                {
                    surfaceStep = vessel.isOnSurface();
                }

                return surfaceStep && base.check(vessel);
            }
            else
            {
                return false;
            }
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_292");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_293");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_294");
        }
    }

    internal class SunEscapeTrajectory : AchievementBase
    {
        public override bool check(Vessel vessel)
        {
            return (vessel != null) && vessel.getCurrentBody().Equals(Body.SUN) &&
                (vessel.orbit.eccentricity >= 1d);
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_295");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_296");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_297");
        }
    }

    internal class PreventAsteroidImpact : AchievementBase
    {
        private bool impactStep;

        internal PreventAsteroidImpact()
        {
            registerOnVesselChange(onVesselChange);
        }

        private void onVesselChange(Vessel vessel)
        {
            impactStep = false;
        }

        public override bool check(Vessel vessel)
        {
            if ((vessel != null) && vessel.hasGrabbedAsteroid() && !vessel.isAsteroid())
            {
                bool impact = vessel.isOnImpactTrajectory();

                if (!impactStep)
                {
                    impactStep = impact;
                }

                return impactStep && !impact;
            }
            else
            {
                return false;
            }
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_298");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_299");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_300");
        }
    }

    internal class MoveAsteroidIntoStableOrbit : OrbitAchievement
    {
        private bool escapeStep;

        internal MoveAsteroidIntoStableOrbit()
        {
            registerOnVesselChange(onVesselChange);
        }

        private void onVesselChange(Vessel vessel)
        {
            escapeStep = false;
        }

        public override bool check(Vessel vessel)
        {
            if ((vessel != null) && vessel.hasGrabbedAsteroid() && !vessel.isAsteroid() && !vessel.orbit.referenceBody.Equals(Body.SUN.getCelestialBody()))
            {
                bool escape = vessel.isOnEscapeTrajectory();

                if (!escapeStep)
                {
                    escapeStep = escape;
                }

                return escapeStep && !escape && vessel.isInStableOrbit();
            }
            else
            {
                return false;
            }
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_301");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_302");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_303");
        }
    }
}
