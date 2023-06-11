using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class Descriptors
    {
        public static Dictionary<string, string> SkillDescriptions;
        private static void LoadSkillDescriptions()
        {
            SkillDescriptions = new Dictionary<string, string>();

            SkillDescriptions.Add("Fencing", string.Format("{7}\r\n{0}\r\n\r\nLevel 1 unlocked at level {1}. [{2}] Experience to rank up.\r\nLevel 2 unlocked at level {3}. [{4}] Experience to rank up.\r\nLevel 3 unlocked at level {5}. {6} Experience to rank up.",
                "Increase the accuracy of melee attacks.", // Skill description
                1, // Grade 1 learnable level
                270, // Grade 1 experience
                7, // Grade 2 learnable level
                600, // Grade 2 experience
                12, // Grade 3 learnable level
                1300, // Grade 3 experience
                "(Passive)" )); // Activation type

            SkillDescriptions.Add("Slaying", string.Format("{7}\r\n{0}\r\n\r\nLevel 1 unlocked at level {1}. [{2}] Experience to rank up.\r\nLevel 2 unlocked at level {3}. [{4}] Experience to rank up.\r\nLevel 3 unlocked at level {5}. {6} Experience to rank up.",
                "Increased chance to land a critical strike during melee combat.", // Skill description
                15, // Grade 1 learnable level
                500, // Grade 1 experience
                17, // Grade 2 learnable level
                1100, // Grade 2 experience
                20, // Grade 3 learnable level
                1800, // Grade 3 experience
                "(Passive)" )); // Activation type

            SkillDescriptions.Add("Thrusting", string.Format("{7}\r\n{0}\r\n\r\nLevel 1 unlocked at level {1}. [{2}] Experience to rank up.\r\nLevel 2 unlocked at level {3}. [{4}] Experience to rank up.\r\nLevel 3 unlocked at level {5}. {6} Experience to rank up.",
                "While active, increase the range of attack directly in front\r\nyou by 1 if there is an attackable body there,", // Skill description
                22, // Grade 1 learnable level
                2000, // Grade 1 experience
                24, // Grade 2 learnable level
                3500, // Grade 2 experience
                27, // Grade 3 learnable level
                6000, // Grade 3 experience
                "(Toggle)")); // Activation type
        }

        public static void LoadAll()
        {
            LoadSkillDescriptions();
        }
    }
}