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

namespace Achievements {
	internal class Category {
		internal static readonly Category CREW_OPERATIONS = new Category(Localizer.Format("#LOC_Ach_94"));
		internal static readonly Category GENERAL_FLIGHT = new Category(Localizer.Format("#LOC_Ach_95"));
		internal static readonly Category GROUND_OPERATIONS = new Category(Localizer.Format("#LOC_Ach_96"));
		internal static readonly Category LANDING = new Category(Localizer.Format("#LOC_Ach_97"));
		internal static readonly Category RESEARCH_AND_DEVELOPMENT = new Category(Localizer.Format("#LOC_Ach_98"));
		internal static readonly Category SPACEFLIGHT = new Category(Localizer.Format("#LOC_Ach_99"));
        internal static readonly Category CONTRACTS = new Category(Localizer.Format("#LOC_Ach_100"));
        internal static readonly Category FUNDS = new Category(Localizer.Format("#LOC_Ach_101"));
        internal static readonly Category REPUTATION = new Category(Localizer.Format("#LOC_Ach_102"));
        //internal static readonly Category KSP_Worlds_First = new Category("Ksp Worlds First Orginization");

		internal readonly string title;

		private Category(string title) {
			this.title = title;
		}
	}
}
