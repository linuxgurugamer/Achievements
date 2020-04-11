using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Achievements
{
#if false
    	BODIES
	{
		name = 
		body = name1
		body = name2

		or
		bodies = name1 name2 ...
	}
	SURFACESAMPLE
	{
		key = keyname
		title =
		text =
		bodies =
	}
	ORBITACHIEVEMENT
	{
		key = keyname
		title =
		text = 
		bodies =
	}

	LOCATION
	{
		name =
		body =
		// Following defines a rectangle if the second pair is set
		latitude =
		longitude =
		// following second corner of rectangle, optional
		latitude2 =   float N/S
		longitude2 =  float E/W
		// If second pair not set, then radius is needed
		radius =
	}


	LANDING
	{
		key = 
		title =
		text =
		bodies =
		// following are all optional
		splash = true
		stableOrbit = false (if true, minAltitude MUST be set)
		minAltitude =
		maxDegreesLatitudeFromEquator =
		locations = location1 location2
	}

#endif

#if false
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class AchievementLoad : MonoBehaviour
#else
    public class AchievementLoad
#endif
    {
        //const string CONFIG = "GameData/Achievements/PluginData/achievements.cfg";

        public class Bodies
        {
            public string name;
            public List<string> bodies = new List<string>();

            public bool predefined = false;

            public Bodies(string name)
            {
                this.name = name;
            }
            public Bodies()
            {
                name = "";
            }
            public Bodies(string name, string bodyName)
            {
                this.name = name;
                bodies.Add(bodyName);
                predefined = true;
            }
            public override string ToString()
            {
                string str = name + ": ";
                foreach (var b in bodies)
                    str += b + " ";
                return str;
            }
        }
        internal Dictionary<string, Bodies> AllBodies = new Dictionary<string, Bodies>();


        List<string> GetAllbodies(string bname)
        {
            var ab = new List<string>();
            foreach (var b1 in bname.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries))
            {
                if (AllBodies.ContainsKey(b1) && !AllBodies[b1].predefined)
                {
                    var body = AllBodies[b1];
                    string s = "";
                    foreach (var s1 in body.bodies)
                        s += s1 + " ";
                    var slist = GetAllbodies(s);
                    ab.AddRange(slist);
                }
                else
                {
                    ab.Add(bname);
                }
            }

            return ab;
        }

        public class SurfaceSample
        {
            public string key;
            public string title;
            public string text;
            public List<string> bodies = new List<string>();
            public bool requireAll = false;
            public bool individual = false;
            public HashSet<string> bodiesHash;

            public SurfaceSample(string key, string title, string text)
            {
                this.key = key;
                this.title = title;
                this.text = text;
            }

            public override string ToString()
            {
                string str = key + ", " + title + ", " + text + ": " + bodies.ToString();
                return str;
            }
        }

        public bool AddBodiesTo(SurfaceSample ss, string bodies)
        {
            ss.bodies.AddRange(GetAllbodies(bodies));
            ss.bodiesHash = new HashSet<string>(ss.bodies);
            return ss.bodies.Count > 0;
        }

        internal Dictionary<string, SurfaceSample> allSurfaceSamples = new Dictionary<string, SurfaceSample>();

        internal const double MAX_ECCENTRICITY = 0.999999;
        public class OrbitAchievement
        {
            public string key;
            public string title;
            public string text;
            public List<string> bodies;
            public bool requireAll = false;
            public bool individual = false;
            public HashSet<string> bodiesHash;
            public double minAltitude = -1;
            public double maxAltitude = -1;
            public double minEccentricity = 0;
            public double maxEccentricity = MAX_ECCENTRICITY;
            public double minInclination = 0;
            public double maxInclination = 90;

            public OrbitAchievement(string key, string title, string text)
            {
                this.key = key;
                this.title = title;
                this.text = text;
            }

            public override string ToString()
            {
                return key + ", " + title + ", " + text + ": " + bodies.ToString() +
                    ", minAltitude: " + minAltitude +
                    ", maxAltitude: " + maxAltitude +
                    ", minEccentricity: " + minEccentricity +
                    ", maxEccentricity: " + maxEccentricity +
                    ", minInclination: " + minInclination +
                    ", maxInclination: " + maxInclination
                ;

            }
        }
        public bool AddBodiesTo(OrbitAchievement ss, string bodies)
        {
            ss.bodies = GetAllbodies(bodies);
            ss.bodiesHash = ss.bodiesHash = new HashSet<string>(ss.bodies);

            return ss.bodies.Count > 0;
        }

        internal Dictionary<String, OrbitAchievement> allOrbitAchievements = new Dictionary<string, OrbitAchievement>();


        public class CfgLocation
        {
            public string name;
            public List<string> bodies;
            public HashSet<string> bodiesHash;
            public double latitude;
            public double longitude;
            public double latitude2 = 0;
            public double longitude2 = 0;
            public double radius = -1;

            public CfgLocation(string name)
            {
                this.name = name;
            }
            public override string ToString()
            {
                string s = "";
                foreach (var b in bodies)
                    s += b + ", ";
                return name + ", " + s+
                    ", latitude: " + latitude +
                    ", longitude: " + longitude +
                    ", latitude2: " + latitude2 +
                    ", longitude2: " + longitude2 +
                    ", radius: " + radius;
            }
        }

        public bool AddBodiesTo(CfgLocation ss, string bodies)
        {
            ss.bodies = GetAllbodies(bodies);
            ss.bodiesHash = new HashSet<string>(ss.bodies);

            return ss.bodies.Count > 0;
        }
        internal Dictionary<string, CfgLocation> allLocations = new Dictionary<string, CfgLocation>();


        public class Landing
        {
            public string key;
            public string title;
            public string text;
            public List<string> bodies;
            public HashSet<string> bodiesHash;
            public bool requireAll = false;
            public bool individual = false;
            public bool splash = false;
            public bool stableOrbit = false;
            public double minAltitude = -1;
            public double maxDegreesLatitudeFromEquator = -1;
            public List<CfgLocation> locations = new List<CfgLocation>();

            public Landing(string key, string title, string text)
            {
                this.key = key;
                this.title = title;
                this.text = text;
            }
            public override string ToString()
            {
                return key + ", " + title + ", " + text + ": " + bodies.ToString() +
                    ", splash: " + splash +
                    ", stableOrbit: " + stableOrbit +
                    ", minAltitude: " + minAltitude +
                    ", maxDegreesLatitudeFromEquator: " + maxDegreesLatitudeFromEquator +
                    ", locations: " + locations.ToString();
            }
        }

        public bool AddBodiesTo(Landing ss, string bodies)
        {
            ss.bodies = GetAllbodies(bodies);
            ss.bodiesHash = new HashSet<string>(ss.bodies);

            return ss.bodies.Count > 0;
        }
        internal Dictionary<string, Landing> allLandings = new Dictionary<string, Landing>();


        //============================

        char[] charSeparators = new char[] { ',', ' ' };

        void AddPredefined()
        {
            AllBodies.Add("STOCK_ALL", new Bodies("STOCK_ALL", "STOCK_ALL"));
            AllBodies.Add("STOCK_LANDABLE", new Bodies("STOCK_LANDABLE", "STOCK_LANDABLE"));
            AllBodies.Add("ALL_PLANETS", new Bodies("ALL_PLANETS", "ALL_PLANETS"));
            AllBodies.Add("ALL_PLANETS_WITHOUT_HOMEWORLD", new Bodies("ALL_PLANETS_WITHOUT_HOMEWORLD", "ALL_PLANETS_WITHOUT_HOMEWORLD"));
            AllBodies.Add("ALL_MOONS", new Bodies("ALL_MOONS", "ALL_MOONS"));
            AllBodies.Add("ALL_LANDABLE", new Bodies("ALL_LANDABLE", "ALL_LANDABLE"));
            AllBodies.Add("ALL_NONLANDABLE", new Bodies("ALL_NONLANDABLE", "ALL_NONLANDABLE"));
            AllBodies.Add("ALL_SPLASHABLE", new Bodies("ALL_SPLASHABLE", "ALL_SPLASHABLE"));
            AllBodies.Add("ALL_WITH_ATMOSPHERE", new Bodies("ALL_WITH_ATMOSPHERE", "ALL_WITH_ATMOSPHERE"));

            AllBodies.Add(Body.SUN.name, new Bodies(Body.SUN.name, Body.SUN.name));
            AllBodies.Add(Body.MOHO.name, new Bodies(Body.MOHO.name, Body.MOHO.name));
            AllBodies.Add(Body.EVE.name, new Bodies(Body.EVE.name, Body.EVE.name));
            AllBodies.Add(Body.KERBIN.name, new Bodies(Body.KERBIN.name, Body.KERBIN.name));
            AllBodies.Add(Body.DUNA.name, new Bodies(Body.DUNA.name, Body.DUNA.name));
            AllBodies.Add(Body.DRES.name, new Bodies(Body.DRES.name, Body.DRES.name));
            AllBodies.Add(Body.JOOL.name, new Bodies(Body.JOOL.name, Body.JOOL.name));
            AllBodies.Add(Body.EELOO.name, new Bodies(Body.EELOO.name, Body.EELOO.name));
            AllBodies.Add(Body.GILLY.name, new Bodies(Body.GILLY.name, Body.GILLY.name));
            AllBodies.Add(Body.MUN.name, new Bodies(Body.MUN.name, Body.MUN.name));
            AllBodies.Add(Body.MINMUS.name, new Bodies(Body.MINMUS.name, Body.MINMUS.name));
            AllBodies.Add(Body.IKE.name, new Bodies(Body.IKE.name, Body.IKE.name));
            AllBodies.Add(Body.LAYTHE.name, new Bodies(Body.LAYTHE.name, Body.LAYTHE.name));
            AllBodies.Add(Body.VALL.name, new Bodies(Body.VALL.name, Body.VALL.name));
            AllBodies.Add(Body.TYLO.name, new Bodies(Body.TYLO.name, Body.TYLO.name));
            AllBodies.Add(Body.BOP.name, new Bodies(Body.BOP.name, Body.BOP.name));
            AllBodies.Add(Body.POL.name, new Bodies(Body.POL.name, Body.POL.name));
        }

        internal void  Start()
        {
#if false
            StartCoroutine(WaitForBody());
#else
            LoadCfgAchievements();
#endif
        }

        private IEnumerable<Body> flatten(List<string> bodies)
        {
            List<Body> result = new List<Body>();
            foreach (var bs in bodies)
            {
                if (Body.allBodiesDict.ContainsKey(bs))
                {
                    var body = Body.allBodiesDict[bs];
                    result.Add(body);
                }
                else
                {
                    bool b = false;
                    switch (bs)
                    {
                        case "STOCK_ALL":
                            b = true; result.AddRange(Body.STOCK_ALL);
                            break;
                        case "STOCK_LANDABLE":
                            b = true; result.AddRange(Body.STOCK_LANDABLE);
                            break;
                        case "ALL_PLANETS":
                            b = true; result.AddRange(Body.ALL_PLANETS);
                            break;
                        case "ALL_PLANETS_WITHOUT_HOMEWORLD":
                            b = true; result.AddRange(Body.ALL_PLANETS_WITHOUT_HOMEWORLD);
                            break;
                        case "ALL_MOONS":
                            b = true; result.AddRange(Body.ALL_MOONS);
                            break;
                        case "ALL_LANDABLE":
                            b = true; result.AddRange(Body.ALL_LANDABLE);
                            break;
                        case "ALL_NONLANDABLE":
                            b = true; result.AddRange(Body.ALL_NONLANDABLE);
                            break;
                        case "ALL_SPLASHABLE":
                            b = true; result.AddRange(Body.ALL_SPLASHABLE);
                            break;
                        case "ALL_WITH_ATMOSPHERE":
                            b = true; result.AddRange(Body.ALL_WITH_ATMOSPHERE);
                            break;
                    }
                    if (!b)
                        Log.info("flatten.  Missing body: " + bs);
                    else
                        Log.info("flatten: Adding new body range to result: " + bs);

                }
            }
            return result;
        }

        private List<Location> CfgLocationsToLocations(List<CfgLocation> cfgList)
        {
            var locations = new List<Location>();

            foreach (var cfg in cfgList)
            {
                foreach (var b in cfg.bodies)
                {
                    if (cfg.radius < 0)
                    {
                        locations.Add(new Location(Body.allBodiesDict[b], cfg.latitude, cfg.longitude, cfg.latitude2, cfg.longitude2));
                    }
                    else
                    {
                        locations.Add(new Location(Body.allBodiesDict[b], cfg.latitude, cfg.longitude, cfg.radius));
                    }
                }
            }

            return locations;
        }

        internal void LoadCfgAchievements()
        { 
            AddPredefined();
            var achievements = GameDatabase.Instance.GetConfigNodes("ACHIEVEMENTS");

            //List<Achievement> loadedAchievements = new List<Achievement>();
            List<Achievement> loadedSurfaceSampleAchievements = new List<Achievement>();
            List<Achievement> loadedOrbitAchievements = new List<Achievement>();
            List<Achievement> loadedLandingAchievements = new List<Achievement>();

            foreach (var achievementGroup in achievements)
            {
                GetAllBodies(achievementGroup);
                GetAllSurfaceSamples(achievementGroup);
                GetAllOrbitAchievements(achievementGroup);
                GetAllLocations(achievementGroup);
                GetAllLandings(achievementGroup);


#if false
                Log.info("Allbodies dump=================================================");
                foreach (var b in AllBodies)
                    Log.info(b.ToString());

                Log.info("allSurfaceSamples dump=================================================");
                foreach (var a in allSurfaceSamples)
                {
                    var ass = a.Value;
                    Log.info(a.ToString());
                    if (ass.individual)
                    {
                        foreach (var f in ass.bodies)
                        {
                            if (Body.allBodiesDict.ContainsKey(f))
                            {
                                var body = Body.allBodiesDict[f];
                                List<Body> l = new List<Body>();
                                l.Add(body);
                                loadedSurfaceSampleAchievements.Add(
                                   new AllBodiesSurfaceSample(l,
                                                               ass.title,
                                                               ass.text,
                                                               ass.key).addon());
                            }
                        }
                    }
                    if (ass.requireAll)
                    {

                    }
                    if (!ass.individual && !ass.requireAll)
                    {
                        var f = flatten(ass.bodies);
                        loadedSurfaceSampleAchievements.Add(
                            new AllBodiesSurfaceSample(f,
                                                        ass.title,
                                                        ass.text,
                                                        ass.key).addon());
                    }
                }



                Log.info("allOrbitAchievements dump=================================================");
                foreach (var a in allOrbitAchievements)
                {
                    Log.info(a.ToString());

                    var aoa = a.Value;
                    if (aoa.individual)
                    {
                        foreach (var f in aoa.bodies)
                        {
                            if (Body.allBodiesDict.ContainsKey(f))
                            {
                                var body = Body.allBodiesDict[f];
                                List<Body> l = new List<Body>();
                                l.Add(body);
                                Log.info("Adding existing body to allOrbitAchievements: " + body.name);
                                loadedOrbitAchievements.Add(
                                 new
                              SpecifiedOrbitAchievement(
                                     l,
                                     aoa.title,
                                     aoa.text,
                                     aoa.minAltitude,
                                     aoa.maxAltitude,
                                     aoa.minEccentricity,
                                     aoa.maxEccentricity,
                                     aoa.minInclination,
                                     aoa.maxInclination));
                            }
                        }
                    }
                    if (aoa.requireAll)
                    {

                    }
                    if (!aoa.individual && !aoa.requireAll)
                    {
                        var f = flatten(aoa.bodies);
                        loadedOrbitAchievements.Add(
                             new
                          SpecifiedOrbitAchievement(
                                 f,
                                 aoa.title,
                                 aoa.text,
                                 aoa.minAltitude,
                                 aoa.maxAltitude,
                                 aoa.minEccentricity,
                                 aoa.maxEccentricity,
                                 aoa.minInclination,
                                 aoa.maxInclination));
                    }
                }
                Log.info("allLocations dump=================================================");
                foreach (var l in allLocations)
                {
                    Log.info(l.ToString());
                }
                Log.info("allLandings dump=================================================");
                foreach (var l in allLandings)
                {
                    Log.info(l.Value.ToString());
                    var al = l.Value;
                    foreach (CfgLocation ll in al.locations)
                        Log.info("BodyLanding.location: " + ll.ToString());

                    if (al.locations.Count > 0)
                    {

                        loadedLandingAchievements.Add(new BodyLanding(Body.allBodiesDict[al.bodies.First()], al.splash, al.stableOrbit, al.minAltitude, al.maxDegreesLatitudeFromEquator,
                             CfgLocationsToLocations(al.locations), al.title, al.text, al.key));


                    }
                    else
                    {
                        foreach (var b in al.bodies)
                        {
                            //loadedLandingAchievements.Add(new BodyLanding(Body.allBodiesDict[b], al.splash, al.title));

                            loadedLandingAchievements.Add(new BodyLanding(Body.allBodiesDict[b], al.splash,
                                al.stableOrbit, al.minAltitude, al.maxDegreesLatitudeFromEquator,
                                new Location[0], al.title, al.text, al.key));

                        }

#if false
                          internal BodyLanding(Body body, bool splash, string title)
            : this(body, splash, false, -1, -1, new Location[0], title,
                splash ? "Splash into an ocean on the surface of " + body.theName + "." : "Land on the surface of " + body.theName + ".",
                splash ? "landing.splash." + body.name : "landing." + body.name)
        {
            Log.info("BodyLanding, body: " + body.name + ", splash: " + splash +  title + ", text: " + text);

            this.body = body;
            this.splash = splash;
            this.title = title;
        }
#endif

                    }
                }
                Log.info("End dump=================================================");
#endif
                    }
            Log.info("Total new body achievements loaded from cfg: " + loadedSurfaceSampleAchievements.Count());
            Log.info("Total new orbit achievements loaded from cfg: " + loadedOrbitAchievements.Count());
            Log.info("Total new landing achievements loaded from cfg: " + loadedLandingAchievements.Count());

            var achievementList = (List<Achievement>)EarnedAchievements.instance.achievements[Category.RESEARCH_AND_DEVELOPMENT];
            achievementList.AddRange(loadedSurfaceSampleAchievements);

            achievementList = (List<Achievement>)EarnedAchievements.instance.achievements[Category.SPACEFLIGHT];
            achievementList.AddRange(loadedOrbitAchievements);

            achievementList = (List<Achievement>)EarnedAchievements.instance.achievements[Category.LANDING];
            achievementList.AddRange(loadedLandingAchievements);

            EarnedAchievements.instance.achievementsList.AddRange(loadedSurfaceSampleAchievements);
            EarnedAchievements.instance.achievementsList.AddRange(loadedOrbitAchievements);
            EarnedAchievements.instance.achievementsList.AddRange(loadedLandingAchievements);
        }



        void GetAllLandings(ConfigNode achievements)
        {
            foreach (var node in achievements.GetNodes("LANDING"))
            {
                string key = "", title = "", text = "", bodies = "";

                if (node.TryGetValue("key", ref key) &&
                    node.TryGetValue("title", ref title) &&
                    node.TryGetValue("text", ref text) &&
                    node.TryGetValue("bodies", ref bodies))
                {
                    Landing ss = new Landing(key, title, text);
                    if (AddBodiesTo(ss, bodies))
                    {
                        string b = "any";
                        node.TryGetValue("bodyRequirement", ref b);
                        if (b != "")
                        {
                            ss.requireAll = (b.ToLower() == "all");
                            ss.individual = (b.ToLower() == "individual");
                        }
                        b = "false";
                        node.TryGetValue("splash", ref b);
                        if (b != "")
                            bool.TryParse(b, out ss.splash);

                        b = "false";
                        node.TryGetValue("stableOrbit", ref b);
                        if (b != "")
                            bool.TryParse(b, out ss.stableOrbit);
                        b = "-1";
                        node.TryGetValue("minAltitude", ref b);
                        if (b != "")
                            double.TryParse(b, out ss.minAltitude);

                        b = "-1";
                        node.TryGetValue("maxDegreesLatitudeFromEquator", ref b);
                        if (b != "")
                            double.TryParse(b, out ss.maxDegreesLatitudeFromEquator);

                        string locations = "";
                        if (node.TryGetValue("locations", ref locations))
                        {
                            List<string> list = locations.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var l in list)
                            {
                                if (allLocations.ContainsKey(l))
                                {
                                    ss.locations.Add(allLocations[l]);
                                }
                                else
                                    Log.error("Missing location: " + l);
                            }
                        }
                            allLandings.Add(key, ss);
                        
                    }
                }
            }
        }

        double Latitude(string str)
        {
            double l = -1;
            str = String.Join(" ", str.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
            int i = str.IndexOf(' ');
            string num = str;
            if (i != -1) num = str.Substring(0, i);
            double.TryParse(num, out l);

            // sanity check
            if (l > 90) l = 90;
            if (l < -90) l = -90;

            if (i != -1)
            {
                string ns = str.Substring(i + 1, 1);
                switch (ns.ToUpper())
                {
                    case "N": l = l.north(); break;
                    case "S": l = l.south(); break;
                }
            }
            return l;
        }

        double Longitude(string str)
        {
            double l = -1;
            str = String.Join(" ", str.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
            int i = str.IndexOf(' ');
            string num = str;
            if (i != -1) num = str.Substring(0, i);
            double.TryParse(num, out l);

            // sanity check
            if (l > 90) l = 180;
            if (l < -90) l = -180;

            if (i != -1)
            {
                string ns = str.Substring(i + 1, 1);
                switch (ns.ToUpper())
                {
                    case "E": l = l.east(); break;
                    case "W": l = l.west(); break;
                }
            }
            return l;
        }


        void GetAllLocations(ConfigNode achievements)
        {
            foreach (var node in achievements.GetNodes("LOCATION"))
            {
                string name = "", bodies = "";
                if (node.TryGetValue("body", ref bodies) ||
                    node.TryGetValue("bodies", ref bodies))
                { 
                    if (node.TryGetValue("name", ref name))
                    {
                        CfgLocation location = new CfgLocation(name);
                        if (AddBodiesTo(location, bodies))
                        {
                            string lat = "", lon = "";

                            if (node.TryGetValue("latitude", ref lat) &&
                                node.TryGetValue("longitude", ref lon))
                            {
                                location.latitude = Latitude(lat);
                                location.longitude = Longitude(lon);
                            }
                            if (node.TryGetValue("latitude2", ref lat) &&
                                node.TryGetValue("longitude2", ref lon))
                            {
                                location.latitude2 = Latitude(lat);
                                location.longitude2 = Longitude(lon);
                            }
                            string radius = "";
                            if (node.TryGetValue("radius", ref radius))
                            {
                                double.TryParse(radius.Trim(), out location.radius);
                            }

                            allLocations.Add(name, location);
                        }
                        else
                            Log.error("No valid locations found for LANDING");
                    }
                
                }
            }
        }

        void GetAllOrbitAchievements(ConfigNode achievements)
        {
            foreach (var node in achievements.GetNodes("ORBITACHIEVEMENT"))
            {
                string key = "", title = "", text = "", bodies = "";

                if (node.TryGetValue("key", ref key) &&
                    node.TryGetValue("title", ref title) &&
                    node.TryGetValue("text", ref text) &&
                    node.TryGetValue("bodies", ref bodies))
                {
                    OrbitAchievement ss = new OrbitAchievement(key, title, text);

                    if (AddBodiesTo(ss, bodies))
                    {
                        string b = "any";
                        node.TryGetValue("bodyRequirement", ref b);
                        if (b != "")
                        {
                            ss.requireAll = (b.ToLower() == "all");
                            ss.individual = (b.ToLower() == "individual");
                        }
                        if (node.HasValue("minAltitude"))
                        {
                            double.TryParse(node.GetValue("minAltitude"), out ss.minAltitude);
                            ss.minAltitude = Math.Max(ss.minAltitude, -1);
                        }

                        if (node.HasValue("maxAltitude"))
                        {
                            double.TryParse(node.GetValue("maxAltitude"), out ss.maxAltitude);
                            ss.maxAltitude = Math.Max(ss.maxAltitude, -1);
                        }

                        if (node.HasValue("minEccentricity"))
                        {
                            double.TryParse(node.GetValue("minEccentricity"), out ss.minEccentricity);
                            ss.minEccentricity = Math.Min(Math.Max(ss.minEccentricity, 0), MAX_ECCENTRICITY);
                        }

                        if (node.HasValue("maxEccentricity"))
                        {
                            double.TryParse(node.GetValue("maxEccentricity"), out ss.maxEccentricity);
                            ss.maxEccentricity = Math.Max(Math.Min(ss.maxEccentricity, MAX_ECCENTRICITY), 0);
                        }

                        if (node.HasValue("minInclination"))
                        {
                            double.TryParse(node.GetValue("minInclination"), out ss.minInclination);
                            ss.minInclination = Math.Min(Math.Max(ss.minInclination, 0), 90);
                        }

                        if (node.HasValue("maxInclination"))
                        {
                            double.TryParse(node.GetValue("maxInclination"), out ss.maxInclination);
                            ss.maxInclination = Math.Max(Math.Min(ss.maxInclination, 90), 0);
                        }


                        allOrbitAchievements.Add(key, ss);
                    }
                }
            }
        }



        void GetAllSurfaceSamples(ConfigNode achievements)
        {
            foreach (var node in achievements.GetNodes("SURFACESAMPLE"))
            {
                string key = "", title = "", text = "", bodies = "";

                if (node.TryGetValue("key", ref key) &&
                    node.TryGetValue("title", ref title) &&
                    node.TryGetValue("text", ref text) &&
                    node.TryGetValue("bodies", ref bodies))
                {
                    SurfaceSample ss = new SurfaceSample(key, title, text);
                    if (AddBodiesTo(ss, bodies))
                    {
                        string b = "any";
                        node.TryGetValue("bodyRequirement", ref b);
                        if (b != "")
                        {
                            ss.requireAll = (b.ToLower() == "all");
                            ss.individual = (b.ToLower() == "individual");
                        }
                        allSurfaceSamples.Add(key, ss);

                    }
                }
            }
        }
        void GetAllBodies(ConfigNode achievements)
        {
            foreach (var node in achievements.GetNodes("BODIES"))
            {
                string name = "";
                string bodies = "";
                if (node.TryGetValue("name", ref name))
                {
                    Bodies bodiesNode = new Bodies(name);
                    if (node.TryGetValue("bodies", ref bodies))
                    {
                        List<string> list = bodies.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                        bodiesNode.bodies.AddRange(list);
                    }
                    foreach (var body in node.GetValues("body"))
                    {
                        bodiesNode.bodies.Add(body);
                    }
                    AllBodies.Add(name, bodiesNode);
                }
            }
        }
    }
}
