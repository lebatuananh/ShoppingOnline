namespace ShoppingOnline.Utilities.Constants
{
    public static class SystemConstants
    {
        public const string ConnectionString = "DefaultConnection";

        public class TokenProvider
        {
            public const string EmailConfirm = "EmailConfirmationTokenProvider";
            public const string Passwordless = "passwordless-auth";
        }

        public class AuthenticationScheme
        {
            public const string AdminSide = "AdminSide";
            public const string ClientSide = "ClientSide";
        }
    }
}