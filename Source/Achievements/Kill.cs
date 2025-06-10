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
	internal class KillFactory : AchievementFactory {
		public IEnumerable<Achievement> getAchievements() {
			return new Achievement[] {
				new Kill(1, Localizer.Format("#LOC_Ach_193"), Localizer.Format("#LOC_Ach_194"), true),
				new Kill(10, Localizer.Format("#LOC_Ach_195"), Localizer.Format("#LOC_Ach_196"), true),
				new Kill(10, Localizer.Format("#LOC_Ach_197"), Localizer.Format("#LOC_Ach_198"), false),
				new Kill(100, Localizer.Format("#LOC_Ach_199"), Localizer.Format("#LOC_Ach_200"), false),
				new KillJebediahAgain().hide()
			};
		}

		public Category getCategory() {
			return Category.CREW_OPERATIONS;
		}
	}

	internal class Kill : CountingAchievement {
		private string title;
		private string text;
		private bool resetOnVesselChange;
		private HashSet<string> killedCrewNames = new HashSet<string>();

		internal Kill(int minKilled, string title, string text, bool resetOnVesselChange)
			: base(minKilled) {
			this.title = title;
			this.text = text;
			this.resetOnVesselChange = resetOnVesselChange;

			if (resetOnVesselChange) {
				registerOnVesselChange(reset);
			}
			registerOnCrewKilled(onCrewKilled);
		}

		private void reset(Vessel vessel) {
			resetCounter();
			killedCrewNames.Clear();
		}

		private void onCrewKilled(EventReport report) {
			string crewName = report.sender;
			// make sure to not double-count
			if (!killedCrewNames.Contains(crewName)) {
				killedCrewNames.Add(crewName);

				increaseCounter();
			}
		}

		public override string getTitle() {
			return title;
		}

		public override string getText() {
			return text;
		}

		public override string getKey() {
			return resetOnVesselChange ? Localizer.Format("#LOC_Ach_201") + minRequired : Localizer.Format("#LOC_Ach_202") + minRequired;
		}
	}

	internal class KillJebediahAgain : CountingAchievement {
		private bool killed;

		internal KillJebediahAgain()
			: base(2) {
			registerOnVesselChange(onVesselChange);
			registerOnCrewKilled(onCrewKilled);
		}

		private void onVesselChange(Vessel vessel) {
			killed = false;
		}

		private void onCrewKilled(EventReport report) {
			string crewName = report.sender;
			if (crewName == Localizer.Format("#LOC_Ach_203")) {
				// make sure to not double-count
				if (!killed) {
					increaseCounter();
					killed = true;
				}
			}
		}

		public override string getTitle() {
			return Localizer.Format("#LOC_Ach_204");
		}

		public override string getText() {
			return Localizer.Format("#LOC_Ach_205");
		}

		public override string getKey() {
			return Localizer.Format("#LOC_Ach_206");
		}
	}
}
