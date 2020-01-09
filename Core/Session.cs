using Communication;

namespace Core
{
    public static class Session
    {
        public static User currentUser = new User();

        public static bool isLoggedIn = false;
    }
}