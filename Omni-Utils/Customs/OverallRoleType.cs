namespace Omni_Utils
{
    //OverallRoleType.cs by icedchqi 
    //Written November 8th 2024
    public class OverallRoleType
    {
        public bool Equals(OverallRoleType o)
        {
            return this == o;
        }
        public static bool operator ==(OverallRoleType left, OverallRoleType right)
        {
            if (left.RoleType != right.RoleType)
            {
                return false;
            }
            if (left.RoleId != right.RoleId)
            {
                return false;
            }
            return true;
        }

        public static bool operator !=(OverallRoleType left, OverallRoleType right)
        {
            return !(left == right);
        }
        public int RoleId { get; set; }
        public RoleVersion RoleType { get; set; }
    }
    public enum RoleVersion
    {
        UcrRole,
        CrRole,
        BaseGameRole,
    }
}
