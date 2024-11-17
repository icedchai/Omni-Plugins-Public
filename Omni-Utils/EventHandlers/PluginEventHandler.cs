
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using Omni_Utils.Customs;
using Omni_Utils.Extensions;
using PlayerRoles;
using Respawning.NamingRules;
using System.Collections.Generic;
using UncomplicatedCustomRoles.Extensions;

namespace Omni_Utils.EventHandlers
{
    //PluginEventHandler.cs by icedchqi
    //Documented November 7th 2024

    //Notes:
    //Might be a bit messy. Sorry.
    public class PluginEventHandler
    {
        public void OnChangingNickname(ChangingNicknameEventArgs e)
        {
            //Calls the patched method, check MyPatcher.cs
            Timing.CallDelayed(0.1f, () => e.Player.ApplyCustomInfoAndRoleName(
                e.Player.GetCustomInfo(),
                e.Player.GetRoleName()
                ));
        }
        public void OnChangingRole(ChangingRoleEventArgs e)
        {
            //When a player changes to a basegame role, their roleName will be set to the configged roleName
            //This works with UCR, surprisingly. If it stops suddenly, like if this code starts running AFTER
            //ApplyCustomInfoAndRoleName, it's kind of fucked.
            string roleName = "";
            if (OmniUtilsPlugin.pluginInstance.Config.roleRoleNames.TryGetValue(e.NewRole, out roleName))
            {
                e.Player.ApplyCustomInfoAndRoleName("", roleName);
            }
        }
        public void OnPlayerJump(JumpingEventArgs e)
        {
            //Omni-2 has had a function to steal stamina when a player jumps since late-2022 to early-2023. 
            //The plugin broke and nobody bothered to write the literal nine-line solution. For years.
            //That is why I firmly do not believe Festive learned EXILED nor did he make any plugins.

            if (e.Player.IsHuman & e.Player.IsUsingStamina)
            {
                if (e.Player.IsEffectActive<Invigorated>() || e.Player.IsEffectActive<Scp207>())
                {
                    return;
                }
                else
                {
                    e.Player.Stamina -= (OmniUtilsPlugin.pluginInstance.Config.StaminaUseOnJump * 0.01f);
                }
            }
        }


        public void OnTeslaBooming(TriggeringTeslaEventArgs e)
        {
            e.IsAllowed = false;
        }        
        public void OnPickupRadioDrain(UsingRadioPickupBatteryEventArgs e)
        {
            e.Drain = -10;
        }
       
        public void OnChangingItem(ChangingItemEventArgs e)
        {
            //TODO:
            //Add a system so pulling out keycards prints a list of information about it to the player.
            //
            if (e.Item == null)
            {
                return;
            }
            if (e.Item.IsKeycard)
            {/*
                Keycard key = (Keycard)e.Item;
                KeycardPermissions permissions;
                permissions = key.Permissions;
                CustomKeycard customKeycard;
                customKeycard = OmniUtilsPlugin.Keycards[e.Item.Serial];

                SetElement element = new(300, $"name: {customKeycard.Name}\npermissions: {customKeycard.AssignedPermissions}");
                DisplayCore core;
                core = DisplayCore.Get(e.Player.ReferenceHub);
                RueI.Displays.Display display = new(core);
                display.Elements.Add(element);*/
            }
        }
        public void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs e)
        {
            //Disables SCP termination announcements
            e.IsAllowed = false;
            return;
        }
        //This makes a CASSIE announcement for any death you configure it to. This is so that
        //important subjects other than SCPs get announcements, for instance if an O5 level personnel
        //dies.
        public void AnnounceSubjectDeath(Player attacker, Player victim)
        {
            OverallRoleType attackerRoleType = attacker.GetOverallRole();
            OverallRoleType victimRoleType = victim.GetOverallRole();
            string announcementName = null;
            foreach (OverallRoleType newType in OmniUtilsPlugin.pluginInstance.Config.ScpTerminationAnnouncementIndex.Keys)
            {
                if (newType == attackerRoleType)
                {
                    if (!OmniUtilsPlugin.pluginInstance.Config.ScpTerminationAnnouncementIndex.TryGetValue(newType, out announcementName))
                    {
                        return;
                    }
                }
            }

            if (announcementName == null)
            {
                return;
            }
            string cassie;
            string subs;
            CustomAnnouncement announcement;
            OmniUtilsPlugin.pluginInstance.Config.ScpTerminationCassieAnnouncements.TryGetValue(
                announcementName, out announcement);
            cassie = announcement.words;
            subs = announcement.translation;


            CustomAnnouncement subjectName = null;
            foreach (OverallRoleType newType in OmniUtilsPlugin.pluginInstance.Config.scpCassieString.Keys)
            {
                if (newType == victimRoleType)
                {
                    OmniUtilsPlugin.pluginInstance.Config.scpCassieString.TryGetValue(newType, out subjectName);
                }
            }

            if (subjectName == null)
            {
                return;
            }
            cassie = cassie.Replace("%subject%", subjectName.words);
            subs = subs.Replace("%subject%", subjectName.translation);
            Cassie.MessageTranslated(cassie, subs);
        }
        public void OnPlayerDeath(DyingEventArgs e)
        {
            if (e.Attacker == null)
            {
                return;
            }
            if (e.Player == null)
            {
                return;
            }
            AnnounceSubjectDeath(e.Attacker, e.Player);
        }







        #region customsquad stuff
        //CustomSquad refugees
        public void OnNTFAnnounced(AnnouncingNtfEntranceEventArgs e)
        {
            if (OmniUtilsPlugin.NextWaveMtf == null)
            {
                return;
            }
            if (OmniUtilsPlugin.NextWaveMtf == "do_not_announce")
            {
                e.IsAllowed = false;
                OmniUtilsPlugin.NextWaveMtf = null;
                return;

            }
            else
            {
                string customSquadName = OmniUtilsPlugin.NextWaveMtf;
                CustomSquad customSquad = OmniUtilsPlugin.TryGetCustomSquad(customSquadName);
                //replaces %division% with the MTF unit name, (eg XRAY-12)
                string announcement = customSquad.EntranceAnnouncement.Replace("%division%", $"NATO_{e.UnitName[0]} {e.UnitNumber}");
                string announcementSubs = customSquad.EntranceAnnouncementSubs.Replace("%division%", $"{e.UnitName}-{e.UnitNumber}");

                Cassie.MessageTranslated(announcement,
                    announcementSubs);
                OmniUtilsPlugin.NextWaveMtf = null;
                e.IsAllowed = false;
            }
        }
        public void OnSpawnWave(RespawningTeamEventArgs e)
        {

            string customSquadName;
            CustomSquad customSquad;
            List<Player> players = new List<Player>();
            players = e.Players;
            Queue<RoleTypeId> queue = e.SpawnQueue;
            if (players.Count == 0)
            {
                return;
            }
            if (e.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
            {

                if (OmniUtilsPlugin.NextWaveCi != null)
                {
                    e.IsAllowed = false;
                    customSquadName = OmniUtilsPlugin.NextWaveCi;
                    customSquad = OmniUtilsPlugin.TryGetCustomSquad(customSquadName);
                    if (customSquad == null)
                    {
                        return;
                    }
                    OmniUtilsPlugin.NextWaveCi = null;
                    if (!queue.Contains(RoleTypeId.ChaosRepressor))
                    {
                        queue.Dequeue();
                        queue.Enqueue(RoleTypeId.ChaosRepressor);
                    }
                    foreach (RoleTypeId role in queue)
                    {
                        if (players.Count <= 0)
                            break;
                        Player player = players.RandomItem();
                        players.Remove(player);
                        switch (role)
                        {
                            case RoleTypeId.ChaosRepressor:
                                player.SetOverallRole(customSquad.customCaptain);
                                break;
                            case RoleTypeId.ChaosMarauder:
                                player.SetOverallRole(customSquad.customSergeant);
                                break;
                            case RoleTypeId.ChaosRifleman:
                                player.SetOverallRole(customSquad.customPrivate);
                                break;
                        }
                    }
                    if (!customSquad.UseCassieAnnouncement)
                    {
                        return;
                    }
                    Cassie.MessageTranslated(customSquad.EntranceAnnouncement, customSquad.EntranceAnnouncementSubs);
                }

            }

            if (e.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
            {
                if (OmniUtilsPlugin.NextWaveMtf != null)
                {
                    e.IsAllowed = false;
                    UnitNamingRule.TryGetNamingRule(e.NextKnownTeam, out UnitNamingRule unitNamingRule);
                    if (unitNamingRule != null)
                    {
                        UnitNameMessageHandler.SendNew(e.NextKnownTeam, unitNamingRule);
                    }
                    customSquadName = OmniUtilsPlugin.NextWaveMtf;
                    customSquad = OmniUtilsPlugin.TryGetCustomSquad(customSquadName);
                    if (customSquad == null)
                    {
                        return;
                    }


                    foreach (RoleTypeId role in queue)
                    {
                        if (players.Count <= 0)
                            break;
                        Player player = players.RandomItem();
                        players.Remove(player);
                        switch (role)
                        {
                            case RoleTypeId.NtfCaptain:
                                player.SetOverallRole(customSquad.customCaptain);
                                break;
                            case RoleTypeId.NtfSergeant:
                                player.SetOverallRole(customSquad.customSergeant);
                                break;
                            case RoleTypeId.NtfPrivate:
                                player.SetOverallRole(customSquad.customPrivate);
                                break;
                        }
                    }
                    if (!customSquad.UseCassieAnnouncement)
                    {
                        OmniUtilsPlugin.NextWaveMtf = "do_not_announce";
                        return;
                    }
                    Log.Info($"ENDED SPAWN WAVE");

                }

            }


            #endregion


        }
    }
}

