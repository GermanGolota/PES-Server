namespace Core.Extensions
{
    public static class GenericExtensions
    {
        public static bool IsNotNull(this object obj)
        {
            return obj is object;
        }
    }
}
