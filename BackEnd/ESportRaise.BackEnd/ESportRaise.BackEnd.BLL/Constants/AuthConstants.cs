namespace ESportRaise.BackEnd.BLL.Constants
{
    public static class AuthConstants
    {
        public const string ID_CLAIM_TYPE = "id";

        public const string ADMIN_ROLE = "Admin";

        public const string MEMBER_ROLE = "Member";

        public const string EMAIL_REGEX = @"^(([^<>()\[\]\\.,;:\s@""]+(\.[^<>()\[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";

        public const string USER_NAME_REGEX = @"^[A-Za-z0-9_]{3,20}$";

        public const string PASSWORD_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,20}$";
    }
}
