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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Achievements
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class InitBodies : MonoBehaviour
    {
        void Start()
        {
            Body.InitAllBodies();
        }
    }

    public class Body
    {
        internal static readonly Body SUN = new Body(Localizer.Format("#LOC_Ach_14"), Localizer.Format("#LOC_Ach_15"));

        internal static readonly Body MOHO = new Body(Localizer.Format("#LOC_Ach_16"));
        internal static readonly Body EVE = new Body(Localizer.Format("#LOC_Ach_17"));
        internal static readonly Body KERBIN = new Body(Localizer.Format("#LOC_Ach_18"));
        internal static readonly Body DUNA = new Body(Localizer.Format("#LOC_Ach_19"));
        internal static readonly Body DRES = new Body(Localizer.Format("#LOC_Ach_20"));
        internal static readonly Body JOOL = new Body(Localizer.Format("#LOC_Ach_21"));
        internal static readonly Body EELOO = new Body(Localizer.Format("#LOC_Ach_22"));

        internal static readonly Body GILLY = new Body(Localizer.Format("#LOC_Ach_23"));
        internal static readonly Body MUN = new Body(Localizer.Format("#LOC_Ach_24"), Localizer.Format("#LOC_Ach_25"));
        internal static readonly Body MINMUS = new Body(Localizer.Format("#LOC_Ach_26"));
        internal static readonly Body IKE = new Body(Localizer.Format("#LOC_Ach_27"));
        internal static readonly Body LAYTHE = new Body(Localizer.Format("#LOC_Ach_28"));
        internal static readonly Body VALL = new Body(Localizer.Format("#LOC_Ach_29"));
        internal static readonly Body TYLO = new Body(Localizer.Format("#LOC_Ach_30"));
        internal static readonly Body BOP = new Body(Localizer.Format("#LOC_Ach_31"));
        internal static readonly Body POL = new Body(Localizer.Format("#LOC_Ach_32"));
#if false
        internal static readonly Body ABLATE = new Body(Localizer.Format("#LOC_Ach_33"));
        internal static readonly Body ASCENSION = new Body(Localizer.Format("#LOC_Ach_34"));
        internal static readonly Body ERIN = new Body(Localizer.Format("#LOC_Ach_35"));
        internal static readonly Body POCK = new Body(Localizer.Format("#LOC_Ach_36"));
        internal static readonly Body RINGLE = new Body(Localizer.Format("#LOC_Ach_37"));
        internal static readonly Body SENTAR = new Body(Localizer.Format("#LOC_Ach_38"));
        internal static readonly Body SKELTON = new Body(Localizer.Format("#LOC_Ach_39"));
        internal static readonly Body THUD = new Body(Localizer.Format("#LOC_Ach_40"));
        internal static readonly Body SERIOUS = new Body(Localizer.Format("#LOC_Ach_41"));
        internal static readonly Body JOKER = new Body(Localizer.Format("#LOC_Ach_42"));
        internal static readonly Body INACCESSABLE = new Body(Localizer.Format("#LOC_Ach_43"));
#endif
        internal static readonly IEnumerable<Body> STOCK_ALL = new Body[] { SUN, MOHO, EVE, KERBIN, DUNA, DRES, JOOL, EELOO, GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
        //internal static readonly IEnumerable<Body> STOCK_PLANETS = new Body[] { MOHO, EVE, KERBIN, DUNA, DRES, JOOL, EELOO };
        //internal static readonly IEnumerable<Body> STOCK_PLANETS_WITHOUT_KERBIN = new Body[] { MOHO, EVE, DUNA, DRES, JOOL, EELOO };
        //internal static readonly IEnumerable<Body> STOCK_MOONS = new Body[] { GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
        internal static readonly IEnumerable<Body> STOCK_LANDABLE = new Body[] { MOHO, EVE, KERBIN, DUNA, DRES, EELOO, GILLY, MUN, MINMUS, IKE, LAYTHE, VALL, TYLO, BOP, POL };
        //internal static readonly IEnumerable<Body> STOCK_SPLASHABLE = new Body[] { EVE, KERBIN, LAYTHE };
        //internal static readonly IEnumerable<Body> STOCK_WITH_ATMOSPHERE = new Body[] { EVE, KERBIN, DUNA, JOOL, LAYTHE };
#if false
		internal static readonly  IEnumerable <Body> SENTAR_ALL = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, POCK, RINGLE, SENTAR, SKELTON, THUD, SERIOUS, JOKER };
		internal static readonly  IEnumerable <Body> SENTAR_PLANETS = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, RINGLE, SENTAR, SKELTON, THUD, JOKER };
		internal static readonly  IEnumerable <Body> SENTAR_MOONS = new Body[] { POCK };
		internal static readonly  IEnumerable <Body> SENTAR_LANDABLE = new Body[] { ABLATE, ASCENSION, ERIN, INACCESSABLE, POCK, RINGLE, SKELTON, THUD, JOKER };
		internal static readonly  IEnumerable <Body> SENTAR_SPLASHABLE = new Body[] { ERIN };
		internal static readonly  IEnumerable <Body> SENTAR_WITH_ATMOSPHERE = new Body[] { ERIN, SENTAR, SKELTON };

		internal static readonly  IEnumerable <Body> ALL = flatten(STOCK_ALL, SENTAR_ALL);
		internal static readonly  IEnumerable <Body> ALL_PLANETS = flatten(STOCK_PLANETS, SENTAR_PLANETS);
		internal static readonly  IEnumerable <Body> ALL_PLANETS_WITHOUT_KERBIN = flatten(STOCK_PLANETS_WITHOUT_KERBIN, SENTAR_PLANETS);
		internal static readonly  IEnumerable <Body> ALL_MOONS = flatten(STOCK_MOONS, SENTAR_MOONS);
		internal static readonly  IEnumerable <Body> ALL_LANDABLE = flatten(STOCK_LANDABLE, SENTAR_LANDABLE);
		internal static readonly  IEnumerable <Body> ALL_SPLASHABLE = flatten(STOCK_SPLASHABLE, SENTAR_SPLASHABLE);
		internal static readonly  IEnumerable <Body> ALL_WITH_ATMOSPHERE = flatten(STOCK_WITH_ATMOSPHERE, SENTAR_WITH_ATMOSPHERE);
#endif

        internal static IEnumerable<Body> ALL;
        internal static IEnumerable<Body> ALL_PLANETS;
        internal static IEnumerable<Body> ALL_PLANETS_WITHOUT_HOMEWORLD;
        internal static IEnumerable<Body> ALL_MOONS;
        internal static IEnumerable<Body> ALL_LANDABLE;
        internal static IEnumerable<Body> ALL_NONLANDABLE;
        internal static IEnumerable<Body> ALL_SPLASHABLE;
        internal static IEnumerable<Body> ALL_WITH_ATMOSPHERE;

        internal readonly string name;
        internal readonly string theName;

        private Body(string name)
            : this(name, name)
        {
        }

        private Body(string name, string theName)
        {
            this.name = name;
            this.theName = theName;
        }

        private static IEnumerable<Body> flatten(params IEnumerable<Body>[] bodies)
        {
            List<Body> result = new List<Body>();
            foreach (IEnumerable<Body> bs in bodies)
            {
                result.AddRange(bs);
            }
            return result;
        }

        internal static IEnumerable<Body> flatten(List<Body> bodies)
        {
            List<Body> result = new List<Body>();
            foreach (Body bs in bodies)
            {
                result.Add(bs);
            }
            return result;
        }
        internal bool isSame(CelestialBody b)
        {
            return name == b.name;
        }

        internal bool isStock()
        {
            return STOCK_ALL.Contains(this);
        }

        internal CelestialBody getCelestialBody()
        {
            return FlightGlobals.Bodies.First(b => b.name == name);
        }

        internal static Body get(CelestialBody b)
        {
            return ALL.Single(body => body.isSame(b));
        }

        internal static List<Body> allBodies = new List<Body>();
        internal static Dictionary<string, Body> allBodiesDict = new Dictionary<string, Body>();
        static List<Body> allPlanets = new List<Body>();
        static List<Body> allPlanetsWithoutHomeworld = new List<Body>();
        static List<Body> allBodiesWithoutMoons = new List<Body>();
        static List<Body> allMoons = new List<Body>();
        static List<Body> allLandable = new List<Body>();
        static List<Body> allNonLandable = new List<Body>();
        static List<Body> allSplashable = new List<Body>();
        static List<Body> allWithAtmo = new List<Body>();
        static List<Body> allStars = new List<Body>();
        static public bool initted = false;
        public static void InitAllBodies()
        {
            if (initted)
                return;

            foreach (CelestialBody p in FlightGlobals.Bodies)
            {
                Body b = null;
                if (!allBodiesDict.ContainsKey(p.name))
                {
                    foreach (var sa in STOCK_ALL)
                    {
                        if (sa.name == p.name)
                        {
                            b = sa;
                            break;
                        }
                    }
                    if (b== null)
                        b = new Body(p.name);
                    allBodies.Add(b);
                    allBodiesDict.Add(p.name, b);
                } else
                {
                    b = allBodiesDict[p.name];
                }
                if (p.referenceBody == Planetarium.fetch.Sun)
                {
                    allPlanets.Add(b);
                    if (!p.isHomeWorld)
                        allPlanetsWithoutHomeworld.Add(b);
                    if (p.orbitingBodies != null && p.orbitingBodies.Count == 0)
                        allBodiesWithoutMoons.Add(b);
                }
                if (p.referenceBody != Planetarium.fetch.Sun)
                    allMoons.Add(b);
                if (p.isStar)
                    allStars.Add(b);
                if (p.hasSolidSurface)
                    allLandable.Add(b);
                else
                    allNonLandable.Add(b);
                if (p.ocean)
                    allSplashable.Add(b);
                if (p.atmosphere)
                    allWithAtmo.Add(b);
            }

            ALL = flatten(allBodies);
            ALL_PLANETS = flatten(allPlanets);
            ALL_PLANETS_WITHOUT_HOMEWORLD = flatten(allPlanetsWithoutHomeworld);
            ALL_MOONS = flatten(allMoons);
            ALL_LANDABLE = flatten(allLandable);
            ALL_NONLANDABLE = flatten(allNonLandable);
            ALL_SPLASHABLE = flatten(allSplashable);
            ALL_WITH_ATMOSPHERE = flatten(allWithAtmo);

            initted = true;
        }
    }
}
