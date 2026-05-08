namespace LibraryBot.Helper
{
    public static class MessageFormatter
    {
        public static string GetWelcomeMessage()
        {
            return @"📚 *Welcome to library bot\!*
        
            I can help you browse our library catalog\.
            Choose an option:

            📖 *Books* \- Brose all books
            ✍️ *Authors* \- Brose all authors
            📂 *Categories* \- Brose all categories

            You can also use direct commands:
            `/book:id` \- Get book by ID
            `/author:id` \- Get author by ID
            `/category:id` \- Get category by ID";
        }

        public static string GetNoDataMessage()
        {
            return "No data to return\n" + GetAvailableCommands();
        }

        public static string GetAvailableCommands()
        {
            return "/book:<id> - returns book by Id \n" +
                "/books:<pageSize>:<pageIndex> - returns paginated books \n" +
                "/author:<id> - returns author by Id \n" +
                "/authors:<pageSize>:<pageIndex> - returns paginated authors \n" +
                "/category:<id> - returns category by Id \n" +
                "/categories:<pageSize>:<pageIndex> - returns paginated categories \n"; 
        }
    }
}
