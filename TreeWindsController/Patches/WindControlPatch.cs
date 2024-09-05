using Game.Rendering;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace TreeWindsController.Patches
{
    [HarmonyPatch(typeof(WindControl), "SetGlobalProperties")]
    public class WindControl_SetGlobalPropertiesPatch
    {

        public static bool Prefix(WindControl __instance, CommandBuffer __0, WindVolumeComponent __1)
        {
          
            var control = TreeWindsControl.Instance;
            if (control == null)
            {
                return true;
            }

            control.updateWindVolumeComponent(__1);
           
            return true;
        }
    }
}


