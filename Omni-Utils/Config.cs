using Exiled.API.Enums;
using Exiled.API.Interfaces;
using Omni_Utils.Customs;
using PlayerRoles;
using Respawning;
using System.Collections.Generic;
using System.ComponentModel;

namespace Omni_Utils
{
    public class Config : IConfig
    {
        [Description("Indicates plugin enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indicates debug mode enabled or not")]
        public bool Debug { get; set; } = false;
        [Description("The string that will be displayed when a player uses the command .ohelp")]
        public string ImportantCommands { get; set; } =
            "Put important info here";

        [Description("Make sure all the names are different. You can use UCR custom role IDs")]
        public List<CustomSquad> customSquads { get; set; } = new List<CustomSquad>
        {
            new CustomSquad()
            {
                UseCassieAnnouncement=true,
                SquadName= "minutemen",
                SquadType=Respawning.SpawnableTeamType.NineTailedFox,
                EntranceAnnouncement = $"MTFUnit epsilon 11 designated %division% hasentered AllRemaining",
                EntranceAnnouncementSubs = $"Mobile Task Force Unit Epsilon-11 %divison% has entered the facility.<split>All remaining personnel are advised to proceed with standard evacuation protocols until an MTF squad reaches your destination.",
                customCaptain=new OverallRoleType
                {
                    RoleId=12,RoleType=RoleVersion.BaseGameRole
                },
                customSergeant=new OverallRoleType
                {
                    RoleId=11,RoleType=RoleVersion.BaseGameRole
                },
                customPrivate=new OverallRoleType
                {
                    RoleId=13,RoleType=RoleVersion.BaseGameRole
                },
            },
            new CustomSquad()
            {
                UseCassieAnnouncement=true,
                SquadName= "swat",
                SquadType=Respawning.SpawnableTeamType.ChaosInsurgency,
                EntranceAnnouncement = $"the Secret jam_1_1 weapons and tactical team from an core jam_40_2 agent p d hasentered",
                EntranceAnnouncementSubs = $"The Special Weapons and Tactical team from Anchorage PD has entered the facility.",
                customCaptain=new OverallRoleType
                {
                    RoleId=1104,RoleType=RoleVersion.UcrRole
                },
                customSergeant=new OverallRoleType
                {
                    RoleId=1104,RoleType=RoleVersion.UcrRole
                },
                customPrivate=new OverallRoleType
                {
                    RoleId=1104,RoleType=RoleVersion.UcrRole
                },
            },
        };

        [Description("Whether jumping shall consume stamina")]
        public bool JumpingStamina { get; set; } = true;
        [Description("Percent of stamina to consume when jumping")]
        public float StaminaUseOnJump { get; set; } = 30;

        //Put a role on the left, and the name it should have on the right, and %subject% will be replaced
        //with that.
        public Dictionary<OverallRoleType, CustomAnnouncement> scpCassieString { get; set; } = new Dictionary<OverallRoleType, CustomAnnouncement>()
        {
            {new OverallRoleType{RoleId=(sbyte)RoleTypeId.Scp049,
                RoleType=RoleVersion.BaseGameRole},
                new CustomAnnouncement{ words="scp 0 4 9",translation="SCP-049"} },
            {new OverallRoleType { RoleId =(sbyte) RoleTypeId.Scp079,
                RoleType=RoleVersion.BaseGameRole },
                new CustomAnnouncement{ words="scp 0 7 9",translation="SCP-079"}},
            {new OverallRoleType { RoleId =(sbyte) RoleTypeId.Scp096,
                RoleType=RoleVersion.BaseGameRole },
                new CustomAnnouncement { words = "scp 0 9 6", translation = "SCP-096" } },
            {new OverallRoleType { RoleId =(sbyte) RoleTypeId.Scp106 ,
                RoleType=RoleVersion.BaseGameRole},
                new CustomAnnouncement{ words="scp 1 0 6",translation="SCP-106"}},
            {new OverallRoleType { RoleId =(sbyte) RoleTypeId.Scp173 ,
                RoleType=RoleVersion.BaseGameRole},
                new CustomAnnouncement { words = "scp 1 7 3", translation = "SCP-173" } },
            {new OverallRoleType { RoleId =(sbyte) RoleTypeId.Scp939 ,
                RoleType=RoleVersion.BaseGameRole},
                new CustomAnnouncement{ words="scp 9 3 9",translation="SCP-939"}},
            {new OverallRoleType { RoleId =(sbyte) RoleTypeId.Scp3114 ,
                RoleType=RoleVersion.BaseGameRole},
                new CustomAnnouncement { words = "scp 3 1 1 4", translation = "SCP-3114" } },
            {new OverallRoleType{RoleId=7,RoleType=RoleVersion.UcrRole},
                
                new CustomAnnouncement{words="o5 personnel", translation="O5 Personnel"}},
        };
        [Description("Use %subject% in the announcements for the termination's name. First int is Ucr ID, second int is Announcement index from cassie_announcements")]
        public Dictionary<OverallRoleType, string> ScpTerminationAnnouncementIndex { get; set; } = new Dictionary<OverallRoleType, string>
        {
            { new OverallRoleType{RoleId=1104,RoleType=RoleVersion.UcrRole},
                "goi_anchorage_pd" },
            {new OverallRoleType{RoleId=11,RoleType=RoleVersion.BaseGameRole}, "mtf_e11" },
            {new OverallRoleType{RoleId=12,RoleType=RoleVersion.BaseGameRole}, "mtf_e11" },
            {new OverallRoleType{RoleId=13,RoleType=RoleVersion.BaseGameRole}, "mtf_e11" },
        };
        public Dictionary<RoleTypeId, int> InternalScpTerminationAnnouncementIndex { get; set; } = new Dictionary<RoleTypeId, int>
        {

        };
        public Dictionary<string, CustomAnnouncement> ScpTerminationCassieAnnouncements { get; set; } = new Dictionary<string, CustomAnnouncement>
        {
            {"mtf_e11" ,
                new CustomAnnouncement{words="%subject% containedsuccessfully by mtfunit epsilon 11", translation=
                    "%subject% contained successfully by Mobile Task Force Unit Epsilon-11."} },
            { "goi_anchorage_pd" ,
                new CustomAnnouncement{words="%subject% terminated by an core jam_40_2 agent p d" ,translation="%subject% terminated by Anchorage PD." } },
        };
        public Dictionary<RoleTypeId, string> roleRoleNames { get; set; } = new Dictionary<RoleTypeId, string> {

            { RoleTypeId.ClassD, "Class-D Personnel" },
            { RoleTypeId.Scientist, "Research Personnel" },
            { RoleTypeId.FacilityGuard, "FAC-SEC Personnel" },

            { RoleTypeId.NtfCaptain, "Nine-Tailed Fox Captain (%division%)" },
            { RoleTypeId.NtfSergeant, "Nine-Tailed Fox Sergeant (%division%)" },
            { RoleTypeId.NtfPrivate, "Nine-Tailed Fox Private (%division%)" },

            { RoleTypeId.Tutorial, "Unknown Personnel" },

            { RoleTypeId.ChaosConscript, "Chaos Insurgency Conscript" },
            { RoleTypeId.ChaosMarauder, "Chaos Insurgency Marauder" },
            { RoleTypeId.ChaosRepressor, "Chaos Insurgency Repressor" },
            { RoleTypeId.ChaosRifleman, "Chaos Insurgency Rifleman" },
        };


    }
    public class CustomSquad
    {
        [Description("Whether to make a CASSIE announcement")]
        public bool UseCassieAnnouncement { get; set; }
        [Description("Name used to refer to the squad in commands and logs")]
        public string SquadName { get; set; }
        [Description("Respawn wave this will replace. Use NineTailedFox to get the NATO divisions (Juliet-15), and ChaosInsurgency to not.")]
        public SpawnableTeamType SquadType { get; set; }
        [Description("Announcement CASSIE will say when the custom squad enters")]
        public string EntranceAnnouncement { get; set; }
        public string EntranceAnnouncementSubs { get; set; }

        [Description("UCR Role for each role in the spawn wave")]
        public OverallRoleType customCaptain { get; set; }
        public OverallRoleType customSergeant { get; set; }
        public OverallRoleType customPrivate { get; set; }

    }
}
