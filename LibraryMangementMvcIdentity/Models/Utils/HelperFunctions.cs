namespace LibraryMangementMvcIdentity.Models.Utils
{
    public static class HelperFunctions
    {
        public static string ShowErrorMessage(List<string> errors)
        {
            var messages = "";

            foreach (var error in errors)
            {
                messages += error + Environment.NewLine;
            }
            return messages;
        }
    }
}
