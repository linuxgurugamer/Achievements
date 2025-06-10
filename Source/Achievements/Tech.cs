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
    internal class TechFactory : AchievementFactory
    {
        public IEnumerable<Achievement> getAchievements()
        {
            return new Achievement[] {
                new AllTechsResearched()
            };
        }

        public Category getCategory()
        {
            return Category.RESEARCH_AND_DEVELOPMENT;
        }
    }

    internal class AllTechsResearched : AchievementBase
    {
#if false
		private static  readonly string[] STOCK_TECH_IDS = new string[] {
			Localizer.Format("#LOC_Ach_323"),
			Localizer.Format("#LOC_Ach_324"),
			Localizer.Format("#LOC_Ach_325"),
			Localizer.Format("#LOC_Ach_326"),
			Localizer.Format("#LOC_Ach_327"),
			Localizer.Format("#LOC_Ach_328"),
			Localizer.Format("#LOC_Ach_329"),
			Localizer.Format("#LOC_Ach_330"),
			Localizer.Format("#LOC_Ach_331"),
			Localizer.Format("#LOC_Ach_259"),
			Localizer.Format("#LOC_Ach_332"),
			Localizer.Format("#LOC_Ach_333"),
			Localizer.Format("#LOC_Ach_334"),
			Localizer.Format("#LOC_Ach_335"),
			Localizer.Format("#LOC_Ach_336"),
			Localizer.Format("#LOC_Ach_337"),
			Localizer.Format("#LOC_Ach_338"),
			Localizer.Format("#LOC_Ach_339"),
			Localizer.Format("#LOC_Ach_340"),
			Localizer.Format("#LOC_Ach_341"),
			Localizer.Format("#LOC_Ach_342"),
			Localizer.Format("#LOC_Ach_343"),
			Localizer.Format("#LOC_Ach_344"),
			Localizer.Format("#LOC_Ach_345"),
			Localizer.Format("#LOC_Ach_346"),
			Localizer.Format("#LOC_Ach_347"),
			Localizer.Format("#LOC_Ach_348"),
			Localizer.Format("#LOC_Ach_349"),
			Localizer.Format("#LOC_Ach_350"),
			Localizer.Format("#LOC_Ach_351"),
			Localizer.Format("#LOC_Ach_352"),
			Localizer.Format("#LOC_Ach_353"),
			Localizer.Format("#LOC_Ach_354"),
			Localizer.Format("#LOC_Ach_355"),
			Localizer.Format("#LOC_Ach_356"),
			Localizer.Format("#LOC_Ach_357"),
			Localizer.Format("#LOC_Ach_358"),
			Localizer.Format("#LOC_Ach_359"),
			Localizer.Format("#LOC_Ach_360"),
			Localizer.Format("#LOC_Ach_361"),
			Localizer.Format("#LOC_Ach_362"),
			Localizer.Format("#LOC_Ach_363"),
			Localizer.Format("#LOC_Ach_364"),
			Localizer.Format("#LOC_Ach_365"),
			Localizer.Format("#LOC_Ach_366")
		};
#endif
        private static string[] TECH_IDS;

        private bool initialCheck;
        private bool allTechsResearched;

        internal AllTechsResearched()
        {
            registerOnTechnologyResearched(onTechResearched);
            GetAllTechs();
        }
        private static void GetAllTechs()
        {
            //TECH_IDS = STOCK_TECH_IDS.ToList();
            List<string> lst = new List<string>();
            var a = AssetBase.RnDTechTree.GetTreeTechs();
            foreach (var b in a)
                lst.Add(b.techID);
            TECH_IDS = lst.ToArray();
        }

        public override bool check(Vessel vessel)
        {
            if (!initialCheck)
            {
                initialCheck = checkTechs();
            }

            return allTechsResearched;
        }

        private bool checkTechs()
        {
            if (ResearchAndDevelopment.Instance != null)
            {
                bool allTechsResearched = true;
                //foreach (string techId in STOCK_TECH_IDS) {
                foreach (string techId in TECH_IDS)
                {
                    ProtoTechNode node = ResearchAndDevelopment.Instance.GetTechState(techId);
                    if ((node == null) || (node.state != RDTech.State.Available))
                    {
                        allTechsResearched = false;
                        break;
                    }
                }
                this.allTechsResearched = allTechsResearched;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void onTechResearched(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> action)
        {
            checkTechs();
        }

        public override string getTitle()
        {
            return Localizer.Format("#LOC_Ach_367");
        }

        public override string getText()
        {
            return Localizer.Format("#LOC_Ach_368");
        }

        public override string getKey()
        {
            return Localizer.Format("#LOC_Ach_369");
        }
    }
}
