namespace Update_Tag
{
    public static class ArrayExtensions
    {
        public static T SecondOrDefault<T>(this T[] array) where T:class
        {
            return array != null && array.Length > 1
                ? array[1]
                : null;
        }
    }
}