using Exiled.API.Features;
using Exiled.CustomRoles.API;
using PlayerRoles;
using System.IO;
using UncomplicatedCustomRoles.API.Features;
using UncomplicatedCustomRoles.Extensions;

namespace Omni_Utils.Extensions
{
    public static class PlayerExtensions
    {
        public static string GetCustomInfo(this Player player)
        {
            string first;
            using (var reader = new StringReader(player.CustomInfo))
            {
                first = reader.ReadLine();
            }
            return first;
        }
        public static string GetNickname(this Player player)
        {

            string second;

            using (var reader = new StringReader(player.CustomInfo))
            {
                reader.ReadLine();
                second = reader.ReadLine();
            }
            return second;
        }
        public static string GetRoleName(this Player player)
        {
            string third;
            using (var reader = new StringReader(player.CustomInfo))
            {
                reader.ReadLine();
                reader.ReadLine();
                third = reader.ReadLine();
            }
            return third;
        }
        public static OverallRoleType GetOverallRole(this Player player)
        {
            if (SummonedCustomRole.TryGet(player, out SummonedCustomRole role))
            {
                return new OverallRoleType { RoleType = RoleVersion.UcrRole, RoleId = role.Role.Id };
            }
            //Put Exiled CustomRoles here
            else if (!player.GetCustomRoles().IsEmpty())
            {
                return new OverallRoleType { RoleType = RoleVersion.CrRole, RoleId = (int)player.GetCustomRoles()[0].Id };
            }
            else
            {
                return new OverallRoleType { RoleType = RoleVersion.BaseGameRole, RoleId = (sbyte)player.Role.Type };
            }
        }
        public static void SetOverallRole(this Player player, OverallRoleType roleType)
        {
            switch (roleType.RoleType)
            {
                case RoleVersion.BaseGameRole:
                    player.Role.Set((RoleTypeId)roleType.RoleId);
                    break;
                case RoleVersion.UcrRole:

                    player.SetCustomRole(roleType.RoleId);
                    break;
                case RoleVersion.CrRole:
                    //Put Exiled CR code here
                    break;

            }

        }

    }
}
