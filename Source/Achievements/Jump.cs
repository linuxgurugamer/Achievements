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

namespace Achievements {
	internal class JumpFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new SpaceDunk()
			};
		}

		public Category getCategory() {
			return Category.GROUND_OPERATIONS;
		}
	}

	internal class SpaceDunk : AchievementBase {
		// -29.2269035483408, 236.104528768504
		private const double MIN_HEIGHT = 240;

		private bool onSurfaceStep;
		private double startAltitude;
		private double startFuel;
		private Body startBody;

		private void onVesselChange(Vessel vessel) {
			onSurfaceStep = false;
		}

		public override bool check(Vessel vessel) {
			if ((vessel != null) && vessel.isEVA()) {
				if (!onSurfaceStep) {
					onSurfaceStep = vessel.isOnSurface();
				}

				if (vessel.isOnSurface()) {
					startAltitude = vessel.altitude;
					startFuel = getFuel(vessel);
					startBody = vessel.getCurrentBody();
				}

				return onSurfaceStep && !vessel.isOnSurface() &&
					((vessel.altitude - startAltitude) >= MIN_HEIGHT) && (getFuel(vessel) == startFuel) &&
					vessel.getCurrentBody().Equals(startBody);
			} else {
				return false;
			}
		}

		private double getFuel(Vessel vessel) {
			return (double) vessel.FindPartModulesImplementing<KerbalEVA>().Sum(eva => eva.Fuel);
		}

		public override string getTitle() {
			return Localizer.Format("#LOC_Ach_189");
		}

		// This is here to avoid having crazy code to avoid localization in the getText and getKey methods below
        #region NO_LOCALIZATION
        const string F0 = "F0";
        #endregion

        public override string getText() {
			return Localizer.Format("#LOC_Ach_190") + MIN_HEIGHT.ToString(F0) + Localizer.Format("#LOC_Ach_191");
		}

		public override string getKey() {
			return Localizer.Format("#LOC_Ach_192") + MIN_HEIGHT.ToString(F0);
		}
	}
}
