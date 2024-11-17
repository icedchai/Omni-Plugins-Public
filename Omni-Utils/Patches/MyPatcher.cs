using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Features;
using HarmonyLib;
using Omni_Utils.Commands;
using System;
using UncomplicatedCustomRoles.Extensions;
using Utils;

namespace Omni_CustomSquads.Extensions
{
    using HarmonyLib;

    public class MyPatcher
    {
        // make sure DoPatching() is called at start either by
        // the mod loader or by your injector

        public static void DoPatching()
        {
            var harmony = new Harmony("me.icedchai.patch");
            harmony.PatchAll();
        }
    }
}

[HarmonyPatch(typeof(CassieCommand), nameof(CassieCommand.Execute))]
// CassieCmdPatch by icedchqi
// Documented November 7th 2024
public class CassieCmdPatch
{
    //CassieCmdPatch adds functionality to allow admins to send subtitled CASSIE announcements, by 
    //separating the words and the subtitles using a ';'
    //(e.g "cassie mtfunit epsilon 11 designated ninetailedfox hasentered; Mobile Task Force Unit Epsilon-11 has entered 
    //the facility.")
    public static bool Prefix(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        //Overriding the original function requires a Prefix that executes my own code, and then returns before the rest of 
        //the code can run (i.e the original function)
        response = null;
        Execute(arguments, sender, out response);
        //Returns false to show red text. This was originally a bug because I didn't understand patches, but I think it's fine
        //and works somewhat as clarification.
        return false;
    }
    public static bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        //COPIED AND PASTED FROM THE VANILLA CASSIE COMMAND
        if (!sender.CheckPermission(PlayerPermissions.Announcer, out response))
        {
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "To execute this command provide at least 1 argument!";
            return false;
        }

        string text = RAUtils.FormatArguments(arguments, 0);
        //splits by the ';' into the Words and Translation

        string[] announcements = text.Split(';');
        ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started a cassie announcement: " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);

        //messagetranslated sends a CASSIE announcement w/ subtitles
        if (announcements[1] == null)
        {
            announcements[1] = announcements[0];
        }
        Cassie.MessageTranslated(announcements[0], announcements[1]);
        response = "Announcement sent.";
        return true;
    }

}
[HarmonyPatch(typeof(ChangeCustomPlayerInfoCommand), nameof(ChangeCustomPlayerInfoCommand.Execute))]
//CustomInfoPatch by icedchqi
//Documented November 7th 2024

//Notes:
//I implemented this because there is a feature elsewhere in the plugin that contains all PlayerInfo
//in the CustomInfo (to allow customization of role names)
public class CustomInfoPatch
{
    public static bool Prefix(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        //Read Prefix(arguments, sender, out response) in CassieCmdPatch.

        //This code is probably kind of fucked up but I prefer to keep everything consistent so that
        //I have to duplicate code (when changing functions, during a major update for instance) as 
        //little as possible.

        //I implemented this command as a patch because I want admins to be able to change their habits
        //as little as possible while still using my own code. So this replaces the 'custominfo' cmd

        CustomInfoCmdRa cmd = new CustomInfoCmdRa();
        cmd.Execute(arguments, sender, out response);
        return false;
    }
}
[HarmonyPatch(typeof(PlayerExtension), nameof(PlayerExtension.ApplyCustomInfoAndRoleName))]
//CustomRoleNamePatch by icedchqi
//Documented November 7th 2024

//Notes:
//This patch patches the UCR function "ApplyCustomInfoAndRoleName"
//This way, I can piggyback off the UCR functions and do minimal work.
public class CustomRoleNamePatch
{
    //ApplyCustomInfoAndRoleName doesn't get called if the custom_info field in a custom role is empty,
    //so I added a feature so that if you set it to 'none' it will be empty.
    public static string ProcessCustomInfo(string customInfo)
    {
        if (customInfo.Contains("none"))
        {
            return "";
        }
        return customInfo.Replace("[br]", "\n");
    }
    public static bool Prefix(Exiled.API.Features.Player player, string customInfo, string role)
    {
        //I'm not sure what the advantage to using the nicknameSync compared to the InfoArea is, and
        //I couldn't get it to work. I'm leaving it in as a comment just in case.
        /*player.ReferenceHub.nicknameSync.Network_playerInfoToShow |= PlayerInfoArea.CustomInfo;
        player.ReferenceHub.nicknameSync.Network_playerInfoToShow &= ~PlayerInfoArea.Role; // Hide role
        player.ReferenceHub.nicknameSync.Network_playerInfoToShow &= ~PlayerInfoArea.Nickname;*/

        //Hides the entire InfoArea except the CustomInfo and Badge. InfoArea is the text that displays
        //when you hover over another player.
        player.InfoArea = PlayerInfoArea.CustomInfo | PlayerInfoArea.Badge;

        if (role.Contains("</"))
            Log.Error($"Failed to apply CustomInfo with Role name at PlayerExtension::ApplyCustomInfoAndRoleName(%Player, string, string): role name can't contains any end tag like </color>, </b>, </size> etc...!\nCustomInfo won't be applied to player {player.Nickname} ({player.Id}) -- Found: {role}");

        if (customInfo.StartsWith("<"))
            Log.Error($"Failed to apply CustomInfo with Role name at PlayerExtension::ApplyCustomInfoAndRoleName(%Player, string, string): role custom_info can't contains any tag like </olor>, <b>, <size> etc...!\nCustomInfo won't be applied to player {player.Nickname} ({player.Id}) -- Found: {customInfo}");
        //player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"{player.CustomName}\n{ProcessCustomInfo(customInfo)}\n{role}";

        string info = player.CustomInfo;
        if (player.HasCustomName)
        {
            //CustomInfo supports line breaks, so I have the "customInfo", then the CustomName, then the rolename
            //to simulate how the InfoArea is organized in vanilla
            //(e.g:
            //Custom info
            //Jonny
            //Tutorial)
            info = $"{ProcessCustomInfo(customInfo)}\n{player.CustomName}*\n{role}";
        }
        else
        {
            info = $"{ProcessCustomInfo(customInfo)}\n{player.CustomName}\n{role}";
        }
        //Replaces %division% with the player's UnitName, if applicable. This allows for custom roles to have the name 
        //of their MTF unit in their role name (like XRAY-12)
        //eg Hammer-Down Captain (%division%)
        //turns into
        //Hammer-Down Captain (GOLF-09)
        info = info.Replace("%division%", player.UnitName);
        player.CustomInfo = info;
        return false;
    }
}

