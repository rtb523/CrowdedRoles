﻿using CrowdedRoles.Api.Extensions;
using CrowdedRoles.Api.Roles;
using HarmonyLib;
using UnityEngine;

namespace CrowdedRoles.Api.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    internal static class PlayerControlPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerControl.FixedUpdate))]
        static void FixedUpdate_Postfix(ref PlayerControl __instance)
        {
            BaseRole? role = __instance.GetRole();
            
            role?.PlayerControl_FixedUpdate(__instance);

            if (__instance.AmOwner && role != null) // probably will be reworked
            {
                if (role.AbleToKill &&  __instance.CanMove && !__instance.Data.IsDead)
                {
                    __instance.SetKillTimer(Mathf.Max(0, __instance.killTimer - Time.fixedDeltaTime));
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(__instance.CustomFindClosetTarget());
                }
                else
                {
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                }
            }
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerControl.SetKillTimer))]
        static bool KillCooldownFlickFix(ref PlayerControl __instance, [HarmonyArgument(0)] float time) // https://gist.github.com/Galster-dev/5ff8fcc48dfb566817565bddb1a99e4f
        {
            __instance.killTimer = time;
            if (__instance.AmOwner)
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(
                    PlayerControl.GameOptions.KillCooldown > 0 ? time : 0,
                    PlayerControl.GameOptions.KillCooldown
                );
            }

            return false;
        }
    }
}