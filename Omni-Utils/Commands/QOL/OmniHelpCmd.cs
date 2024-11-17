using CommandSystem;
using System;

namespace Omni_Utils.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    //OmniHelpCmd.cs by icedchqi
    //Documented November 16th 2024
    public class OmniHelpCmd : ICommand
    {
        public string Command { get; set; } = "info";

        public string[] Aliases { get; set; } = new string[]{
        "ohelp", "oinfo", "tellme"};

        public string Description { get; set; } = "Shows you a few of the most important commands for Omni.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = OmniUtilsPlugin.pluginInstance.Config.ImportantCommands;
            //returning false creates a big ass pink text
            return false;
        }
    }
}
