namespace FeatureSlice;

public static partial class From
{
    public static partial class Body
    {
        public static FromBodyJsonBinder<T> Json<T>()
            where T : notnull
        {
            return new ();
        }
    }
}