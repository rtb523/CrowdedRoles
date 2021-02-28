﻿using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.IL2CPP;
using CrowdedRoles.Extensions;
using CrowdedRoles.Options;
using UnityEngine;

namespace CrowdedRoles.Roles
{
    public abstract class BaseRole
    {
        internal RoleData Data { get; }
        
        public abstract string Name { get; }
        public abstract Color Color { get; }
        
        public virtual Team Team { get; } = Team.Crewmate;
        public virtual Visibility Visibility { get; } = Visibility.Myself;
        public virtual string Description { get; } = "Do nothing but [FF0000FF]kiss";
        public virtual PatchFilter PatchFilterFlags { get; } = PatchFilter.None;

        public virtual bool CanKill(PlayerControl? target) => false;
        public virtual bool CanVent(Vent vent) => false;
        public virtual bool CanSabotage(SystemTypes? sabotage) => false;

        protected BaseRole(BasePlugin plugin)
        {
            var guid = MetadataHelper.GetMetadata(plugin).GUID;
            
            if (!RoleManager.Roles.ContainsKey(guid))
            {
                RoleManager.Roles.Add(guid, new Dictionary<byte, BaseRole>());
            }

            Dictionary<byte, BaseRole> localRoles = RoleManager.Roles[guid];
            Data = new RoleData(guid, (byte)localRoles.Count);
            
            localRoles.Add((byte)localRoles.Count, this);
            RoleManager.Roles[guid] = localRoles;
            RoleManager.Limits.Add(this, 0);
            OptionsManager.AddLimitOptionIfNecessary(this, guid);
        }
        
        public virtual string FormatName(GameData.PlayerInfo player) => player.PlayerName;

        public virtual bool PreKill(ref PlayerControl killer, ref PlayerControl target, ref CustomMurderOptions options)
        {
            return true;
        }

        public virtual IEnumerable<GameData.PlayerInfo> SelectHolders(IEnumerable<GameData.PlayerInfo> unusedPlayers, byte limit)
        {
            var rand = new System.Random();
            return unusedPlayers.OrderBy(_ => rand.Next()).Take(limit).ToList();
        }

        public static bool operator ==(BaseRole? me, BaseRole? other) => me?.Data == other?.Data;
        public static bool operator !=(BaseRole? me, BaseRole? other) => me?.Data != other?.Data;
        private bool Equals(BaseRole other)
        {
            return Data.Equals(other.Data);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) &&
                   obj.GetType() == GetType() && 
                   Equals((BaseRole) obj);
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}