using CommandSystem;
using Exiled.API.Features;
using MEC;
using NorthwoodLib.Pools;
using Omni_Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using UncomplicatedCustomRoles.Extensions;
using Utils;

namespace Omni_Utils.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    //CustomInfoCmd.cs by icedchqi
    //documented on November 7th 2024
    public class CustomInfoCmd : ICommand
    {
        //This command is for players to modify their custominfo's top layer
        public string Command { get; set; } = "custominfo";
        public string[] Aliases { get; set; } = new string[] { "customi", "custominformation", "cinfo", "ci" };
        public string Description { get; set; } = "Sets the custom info (the little text that appears above your head)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            Player player = Player.Get(sender);
            if (arguments.Count <= 0)
            {
                response = "USAGE: custominfo (STRING)";
                return false;
            }
            if (player == null)
            {
                response = "You must exist to run this command!";
                return false;
            }
            string info = arguments.At(0);
            for (int i = 1; i < arguments.Count; i++)
            {
                info += $" {arguments.At(i)}";
            }

            Timing.CallDelayed(0.1f, () => player.ApplyCustomInfoAndRoleName(info,
                player.GetRoleName()));
            Log.Info($"{player.Nickname} ({player.UserId}) set custominfo to {info}");
            response = $"Set your custominfo1";
            return true;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    //CustomInfoCmdRa by icedchqi
    //documented on November 7th 2024
    public class CustomInfoCmdRa : ICommand
    {
        //this is the command that overrides the vanilla 'custominfo' 

        //Copied and pasted, with modifications from ChangeCustomPlayerInfoCommand

        public string Command { get; set; } = "setcustominfo";
        public string[] Aliases { get; set; } = new string[] { "omnicustominfo", "oci" };
        public string Description { get; set; } = "Sets the custom info string for players.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
            {
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!";
                return false;
            }

            string[] newargs;
            List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
            if (list == null)
            {
                response = "Cannot find player! Try using the player ID!";
                return false;
            }

            string text = ((newargs == null) ? null : string.Join(" ", newargs));
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            foreach (ReferenceHub item in list)
            {
                if (text == null)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} cleared custom info of player {item.PlayerId} ({item.nicknameSync.MyNick}).", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                    stringBuilder.AppendFormat("Reset {0}'s custom info.\n", item.LoggedNameFromRefHub());

                    Timing.CallDelayed(0.1f, () => Player.Get(item).ApplyCustomInfoAndRoleName("",
                        Player.Get(item).GetRoleName()));

                    continue;
                }

                Timing.CallDelayed(0.1f, () => Player.Get(item).ApplyCustomInfoAndRoleName(
                    text, Player.Get(item).GetRoleName()));

                ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} set custom info of player {item.PlayerId} ({item.nicknameSync.MyNick}) to \"{text}\".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                stringBuilder.AppendFormat("Set {0}'s custom info to: {1}\n", item.LoggedNameFromRefHub(), text);
            }

            response = stringBuilder.ToString().Trim();
            StringBuilderPool.Shared.Return(stringBuilder);
            return true;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CustomRoleNameCmdRa : ICommand
    {
        public string Command { get; set; } = "setcustomrolename";
        public string[] Aliases { get; set; } = new string[] { "rolename", "orn" };
        public string Description { get; set; } = "Sets the custom role name for players, USE THIS!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
            {
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!";
                return false;
            }

            string[] newargs;
            List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out newargs);
            if (list == null)
            {
                response = "Cannot find player! Try using the player ID!";
                return false;
            }

            string text = ((newargs == null) ? null : string.Join(" ", newargs));
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            foreach (ReferenceHub item in list)
            {
                if (text == null)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} cleared custom info of player {item.PlayerId} ({item.nicknameSync.MyNick}).", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                    stringBuilder.AppendFormat("Reset {0}'s custom rolename.\n", item.LoggedNameFromRefHub());

                    Timing.CallDelayed(0.1f, () => Player.Get(item).ApplyCustomInfoAndRoleName(
                        Player.Get(item).GetRoleName(), "unknown personnel"));

                    continue;
                }
                Timing.CallDelayed(0.1f, () => Player.Get(item).ApplyCustomInfoAndRoleName(Player.Get(item).GetCustomInfo(), text));

                ServerLogs.AddLog(ServerLogs.Modules.Administrative, $"{sender.LogName} set custom rolename of player {item.PlayerId} ({item.nicknameSync.MyNick}) to \"{text}\".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                stringBuilder.AppendFormat("Set {0}'s custom rolename to: {1}\n", item.LoggedNameFromRefHub(), text);
            }

            response = stringBuilder.ToString().Trim();
            StringBuilderPool.Shared.Return(stringBuilder);
            return true;
        }
    }
}

