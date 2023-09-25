namespace gRPCClient.Helpers
{
    public static class GlobalVariables
    {
        public static bool LoginFlag { get; set; }
        public static bool WrongCreds { get; set; }
        public static bool UsernameExists { get; set; }
        public static bool ProductExists { get; set; }
        public static bool ReadStart { get; set; }
        public static bool IsSuccess { get; set; }



        // Constructor to initialize default values

        public static gRPC_Client.Inventory scanInv { get; set; } = new gRPC_Client.Inventory();
        static GlobalVariables()
        {
            LoginFlag = false; // Default value for LoginFlag
            WrongCreds = false;
            UsernameExists = false;
            ProductExists = false;
            ReadStart = false;
            IsSuccess = false;
        }
    }
}
