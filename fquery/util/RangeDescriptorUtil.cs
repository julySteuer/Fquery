using fquery.model;
using Range = fquery.model.Range;

namespace fquery.util;

public static class RangeDescriptorUtil
{
    public static bool IsInRangeDescriptor(RangeDescriptor rangeDescriptor, int num)
    {
        var rdn = rangeDescriptor.Size;
        return rangeDescriptor.Range switch
            {
                Range.GreaterThan => rdn > num,
                Range.SmallerThan => rdn < num,
                Range.Equals => rdn == num
            }
            ;
    }
    
}