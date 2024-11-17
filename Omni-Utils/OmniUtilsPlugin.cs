using Exiled.API.Features;
using Exiled.Events.Handlers;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Omni_CustomSquads.Extensions;
using Omni_Utils.EventHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using Map = Exiled.Events.Handlers.Map;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace Omni_Utils
{
    public class OmniUtilsPlugin : Plugin<Config>
    {
        public static OmniUtilsPlugin pluginInstance;


        public override string Name => "Omni-2 Roleplay Utilities (public)";

        public override string Author => "icedchqi";

        public override string Prefix => "omni-utils";

        public override Version Version => new(3, 0, 0);



        #region customSquad stuff
        //squadsToIndex is used to go from the squadname to the index in Config.customSquads, to 
        //allow accessing other properties of the squad from just the name.
        public static Dictionary<string, int> squadNameToIndex = new Dictionary<string, int>();
        public static List<string> SquadStrings = new List<string>();
        public static string NextWaveMtf = null;
        public static string NextWaveCi = null;
        public static CustomSquad TryGetCustomSquad(string squadName)
        {
            try
            {
                return pluginInstance.Config.customSquads[OmniUtilsPlugin.squadNameToIndex[squadName]];
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                return null;
            }

        }
        public static CustomSquad TryGetCustomSquad(int squadIndex)
        {
            try
            {
                return pluginInstance.Config.customSquads[squadIndex];
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                return null;
            }
        }
        #endregion

        PluginEventHandler EventHandler;
        public override void OnEnabled()
        {

            pluginInstance = this;

            for (int i = 0; i <= Config.customSquads.Count - 1; i++)
            {
                CustomSquad squad = Config.customSquads[i];
                squadNameToIndex.Add(squad.SquadName.ToLower(), i);
                Log.Info($"{squad.SquadName} {i}");
            }


            //Look under MyPatcher.cs in /Patches
            MyPatcher.DoPatching();
            RegisterEvents();

        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            pluginInstance = null;
        }
        private void RegisterEvents()
        {
            EventHandler = new PluginEventHandler();
            if (Config.JumpingStamina)
            {
                Player.Jumping += EventHandler.OnPlayerJump;
            }
            
            Player.Dying += EventHandler.OnPlayerDeath;
            Player.ChangingRole += EventHandler.OnChangingRole;
            Player.ChangingNickname += EventHandler.OnChangingNickname;
            Player.ChangingItem += EventHandler.OnChangingItem;
            Player.TriggeringTesla += EventHandler.OnTeslaBooming;



            Map.AnnouncingNtfEntrance += EventHandler.OnNTFAnnounced;
            Map.AnnouncingScpTermination += EventHandler.OnAnnouncingScpTermination;
            Server.RespawningTeam += EventHandler.OnSpawnWave;

            Item.UsingRadioPickupBattery += EventHandler.OnPickupRadioDrain;
        }

        private void UnregisterEvents()
        {

            if (Config.JumpingStamina)
            {
                Player.Jumping -= EventHandler.OnPlayerJump;
            }
            Player.Dying -= EventHandler.OnPlayerDeath;
            Player.ChangingRole -= EventHandler.OnChangingRole;
            Player.ChangingNickname -= EventHandler.OnChangingNickname;
            Player.ChangingItem -= EventHandler.OnChangingItem;
            Player.TriggeringTesla -= EventHandler.OnTeslaBooming;



            Map.AnnouncingNtfEntrance -= EventHandler.OnNTFAnnounced;
            Map.AnnouncingScpTermination -= EventHandler.OnAnnouncingScpTermination;
            Server.RespawningTeam -= EventHandler.OnSpawnWave;

            Item.UsingRadioPickupBattery -= EventHandler.OnPickupRadioDrain;
            EventHandler = null;

        }
    }
}
