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
	internal class HyperEditFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new HyperEdit().addon()
			};
		}

		public Category getCategory() {
			return Category.GENERAL_FLIGHT;
		}
	}

	internal class HyperEdit : AchievementBase {
		private const long MIN_FLIGHT_TIME = 10000;

		private bool onSurfaceStep;
		private long lastSeenOnSurface;

		internal HyperEdit() {
			registerOnVesselChange(onVesselChange);
		}

		private void onVesselChange(Vessel vessel) {
			onSurfaceStep = false;
		}

		public override bool check(Vessel vessel) {
			if (vessel != null) {
				if (!onSurfaceStep) {
					onSurfaceStep = vessel.isOnSurface();
				}

				long now = DateTime.UtcNow.Ticks / 10000;
				if (vessel.isOnSurface()) {
					lastSeenOnSurface = now;
				}

				return onSurfaceStep && vessel.isInStableOrbit() && ((now - lastSeenOnSurface) <= MIN_FLIGHT_TIME);
			} else {
				return false;
			}
		}

		public override string getTitle() {
			return Localizer.Format("#LOC_Ach_186");
		}

		public override string getText() {
			return Localizer.Format("#LOC_Ach_187");
		}

		public override string getKey() {
			return Localizer.Format("#LOC_Ach_188");
		}
	}
}
