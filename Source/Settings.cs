using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;


namespace Achievements
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class AchOptions : GameParameters.CustomParameterNode
    {
        public override string Title { get { return Localizer.Format("#LOC_Ach_52"); } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return Localizer.Format("#LOC_Ach_2"); } }
        public override string DisplaySection { get { return Localizer.Format("#LOC_Ach_2"); } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Use KSP skin")]
        public bool useKSPskin = false;



        [GameParameters.CustomFloatParameterUI("Achievement Display Time (seconds)", minValue = 2f, maxValue = 20f,
          toolTip = "#LOC_Ach_53")]
        public float achievementDisplayTime = 10f;

       

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {

        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }


}
