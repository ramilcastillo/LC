using System;

namespace LifeCouple.WebApi.Test.FunctionalTests
{
    public static class ExtensionMethods
    {
        public static string ExGetFileNameFromAssemblyPath(this string filePath)
        {

            try
            {
                return System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            catch
            {
                return "???";
            }
        }


        public static DateTime ExGetDateOfBirth(this DateTime dateOfBirth)
        {
            return new DateTime(dateOfBirth.Year, dateOfBirth.Month, dateOfBirth.Day, 0, 0, 0, DateTimeKind.Unspecified);
        }
    }
}
