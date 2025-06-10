/*
Achievements - Brings achievements to Kerbal Space Program.
Copyright (C) 2013-2014 Maik Schreiber && Danny Moffre

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
    internal class FundsFactory : AchievementFactory
    {
        public IEnumerable<Achievement> getAchievements()
        {
            return new Achievement[] {
				new FundsAllocations(Localizer.Format("#LOC_Ach_177"), Localizer.Format("#LOC_Ach_178"), Localizer.Format("#LOC_Ach_179"),100000.00),
                new FundsAllocations(Localizer.Format("#LOC_Ach_180"),Localizer.Format("#LOC_Ach_181"), Localizer.Format("#LOC_Ach_182"),1000000.00),
                new FundsAllocations(Localizer.Format("#LOC_Ach_183"),Localizer.Format("#LOC_Ach_184"),Localizer.Format("#LOC_Ach_185"),10000000.00),
			};
        }

        public Category getCategory()
        {
            return Category.FUNDS;
        }
    }
    internal class FundsAllocations : AchievementBase
    {
        private string title;
        private string text;
        private string key;
        private double fundsAmount;

        internal FundsAllocations(string title, string text, string key, double fundsAmount)
        {
            this.title = title;
            this.text = text;
            this.key = key;
            this.fundsAmount = fundsAmount;
        }

        public override bool check(Vessel vessel)
        {
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                if (Funding.Instance.Funds >= fundsAmount)
                {
                    return true;
                }
                else
                    return false;
            }
            else
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
            return key;
        }
    }   
}
