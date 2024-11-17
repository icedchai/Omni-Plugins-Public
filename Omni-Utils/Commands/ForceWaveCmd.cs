using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace Omni_Utils.Commands
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    //ForceWaveCmd.cs by icedchqi
    //Documented November 16th 2024
    public class ForceNextWave : ICommand
    {
        public string Command { get; } = "forcecustomwave";

        public string[] Aliases { get; } = new[]
        {

            "forcenextwave" ,
            "forcewave",
            "fwave",
        };

        public string Description { get; } = "Force next wave to be a specific custom squad. Only evaluates first argument.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission(PlayerPermissions.RoundEvents))
            {
                response = "You do not have permission to use this command! Permission: PlayerPermissions.RoundEvents";
                return false;
            }
            if (arguments.Count == 0)
            {
                response = "List of available squads:";
                foreach (string crew in OmniUtilsPlugin.squadNameToIndex.Keys)
                {
                    response += $"\n{crew} - {OmniUtilsPlugin.TryGetCustomSquad(crew).SquadType}";
                }
                return false;
            }

            string arg0 = arguments.At(0).ToLower();
            int squadIndex;
            if (!OmniUtilsPlugin.squadNameToIndex.TryGetValue(arg0, out squadIndex))
            {
                response = "Please input a squad";
                return false;
            }

            if (OmniUtilsPlugin.TryGetCustomSquad(squadIndex).SquadType == Respawning.SpawnableTeamType.NineTailedFox)
            {
                OmniUtilsPlugin.NextWaveMtf = arg0;
                response = $"Set next MTF Spawnwave to {arg0}";
                Log.Info($"{player.Nickname} ({player.UserId}) {response}");
                return true;
            }
            if (OmniUtilsPlugin.TryGetCustomSquad(squadIndex).SquadType == Respawning.SpawnableTeamType.ChaosInsurgency)
            {
                OmniUtilsPlugin.NextWaveCi = arg0;
                response = $"Set next CI Spawnwave to {arg0}";
                Log.Info($"{player.Nickname} ({player.UserId}) {response}");
                return true;
            }
            else
            {
                response = "Your squad is not configured properly.";
                return false;
            }
        }
    }
}
