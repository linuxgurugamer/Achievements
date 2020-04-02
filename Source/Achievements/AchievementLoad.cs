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

    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class AchievementLoad : MonoBehaviour
    {
        const string CONFIG = "GameData/Achievements/PluginData/achievements.cfg";

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
            Log.info("allBodies, bname: " + bname);
            var ab = new List<string>();
            foreach (var b1 in bname.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries))
            {
                if (AllBodies.ContainsKey(b1) && !AllBodies[b1].predefined)
                {
                    var body = AllBodies[b1];
                    string s = "";
                    foreach (var s1 in body.bodies)
                        s += s1 + " ";
                    Log.info("AllBodies.ContainsKey, s: " + s);
                    var slist = GetAllbodies(s);
                    ab.AddRange(slist);
                }
                else
                    ab.Add(bname);
            }

            return ab;
        }
        public class SurfaceSample
        {
            public string key;
            public string title;
            public string text;
            public List<string> bodies = new List<string>();
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
            Log.info("SurfaceSample AddBodiesTo, bodies: " + bodies);

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


        public class Location
        {
            public string name;
            public List<string> bodies;
            public HashSet<string> bodiesHash;
            public double latitude;
            public double longitude;
            public double latitude2 = -1;
            public double longitude2 = -1;
            public double radius = -1;

            public Location(string name)
            {
                this.name = name;
            }
            public override string ToString()
            {
                return name + ", " + bodies.ToString() +
                    ", latitude: " + latitude +
                    ", longitude: " + longitude +
                    ", latitude2: " + latitude2 +
                    ", longitude2: " + longitude2 +
                    ", radius: " + radius;
            }
        }

        public bool AddBodiesTo(Location ss, string bodies)
        {
            ss.bodies = GetAllbodies(bodies);
            ss.bodiesHash = new HashSet<string>(ss.bodies);

            return ss.bodies.Count > 0;
        }
        internal Dictionary<string, Location> allLocations = new Dictionary<string, Location>();


        public class Landing
        {
            public string key;
            public string title;
            public string text;
            public List<string> bodies;
            public HashSet<string> bodiesHash;

            public bool splash = false;
            public bool stableOrbit = false;
            public double minAltitude = -1;
            public double maxDegreesLatitudeFromEquator = -1;
            public List<Location> locations;

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

        void Start()
        {
            StartCoroutine(WaitForBody());
        }

        private IEnumerable<Body> flatten(List<string> bodies)
        {
            List<Body> result = new List<Body>();
            foreach (var bs in bodies)
            {
                var body = Body.allBodiesDict[bs];
                result.Add(body);
            }
            return result;
        }


        IEnumerator WaitForBody()
        {
            WaitForSeconds wfs = new WaitForSeconds(0.25f);
            while (!Body.initted || EarnedAchievements.instance == null || !EarnedAchievements.instance.allAchievementsCreated)
                yield return wfs;
            Log.info("AchievementLoad.WaitForBody, bodyInitted");
            AddPredefined();
            var achievements = GameDatabase.Instance.GetConfigNodes("ACHIEVEMENTS");

            foreach (var achievementGroup in achievements)
            {
                GetAllBodies(achievementGroup);
                GetAllSurfaceSamples(achievementGroup);
                GetAllOrbitAchievements(achievementGroup);
                GetAllLocations(achievementGroup);

                List<Achievement> loadedAchievements = new List<Achievement>();

#if DEBUG
                Log.info("Allbodies dump=================================================");
                foreach (var b in AllBodies)
                    Log.info(b.ToString());
                Log.info("allSurfaceSamples dump=================================================");
                foreach (var a in allSurfaceSamples)
                {
                    var ass = a.Value;
                    Log.info(a.ToString());
                    var f = flatten(ass.bodies);
                    loadedAchievements.Add(
                        new AllBodiesSurfaceSample(f,
                                                    ass.title,
                                                    ass.text,
                                                    ass.key).addon());
                }



                Log.info("allOrbitAchievements dump=================================================");
                foreach (var a in allOrbitAchievements)
                {
                    Log.info(a.ToString());

                    var aoa = a.Value;
                    loadedAchievements.Add(
                        new
                     SpecifiedOrbitAchievement(
                        aoa.minAltitude,
                        aoa.maxAltitude,
                        aoa.minEccentricity,
                        aoa.maxEccentricity,
                        aoa.minInclination,
                        aoa.maxInclination));
                }
                Log.info("allLocations dump=================================================");
                foreach (var l in allLocations)
                {
                    Log.info(l.ToString());

                    var al = l.Value;

                }

                Log.info("End dump=================================================");
#endif
            }
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
                        string b = "false";
                        node.TryGetValue("splash", ref b);
                        bool.TryParse(b, out ss.splash);

                        b = "false";
                        node.TryGetValue("stableOrbit", ref b);
                        bool.TryParse(b, out ss.stableOrbit);

                        b = "-1";
                        node.TryGetValue("minAltitude", ref b);
                        double.TryParse(b, out ss.minAltitude);

                        b = "-1";
                        node.TryGetValue("maxDegreesLatitudeFromEquator", ref b);
                        double.TryParse(b, out ss.maxDegreesLatitudeFromEquator);

                        string locations = "";
                        if (node.TryGetValue("locations", ref locations))
                        {
                            List<string> list = locations.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var l in list)
                            {
                                if (allLocations.ContainsKey(l))
                                    ss.locations.Add(allLocations[l]);
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
                if (node.TryGetValue("name", ref name) &&
                    node.TryGetValue("bodies", ref bodies))
                {
                    Location location = new Location(name);
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
                        if (node.HasValue("minAltitude"))
                        {
                            double.TryParse(node.GetValue("minAltitude"), out ss.minAltitude);
                            ss.minAltitude = Math.Max(ss.minAltitude, -1);
                        }

                        if (node.HasValue("maxAltitude"))
                        {
                            double.TryParse(node.GetValue("maxAltitude"), out ss.maxAltitude);
                            ss.maxAltitude = Math.Max(ss.maxAltitude, 0);
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
            Log.info("GetAllSurfaceSamples, node count: " + achievements.GetNodes("SURFACESAMPLE").Length);
            foreach (var node in achievements.GetNodes("SURFACESAMPLE"))
            {
                string key = "", title = "", text = "", bodies = "";

                if (node.TryGetValue("key", ref key) &&
                    node.TryGetValue("title", ref title) &&
                    node.TryGetValue("text", ref text) &&
                    node.TryGetValue("bodies", ref bodies))
                {
                    Log.info("Surface sample: key: " + key);
                    SurfaceSample ss = new SurfaceSample(key, title, text);
                    if (AddBodiesTo(ss, bodies))
                    {
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
