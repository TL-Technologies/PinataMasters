//
//  LLArray.m
//  LLLibSet
//

#import "LLArray.h"

@implementation NSArray (libset_extentions)


- (NSString *)JSONString
{
    return [NSJSONSerialization JSONStringWithObject:self];
}


@end


NSArray* LLNSArrayFromUnityStrings(int lengthArray, LLUnityString vals[])
{
    NSArray *params = [NSArray array];
    if ((lengthArray > 0) && (vals != NULL))
    {
        NSMutableArray *temp = [NSMutableArray arrayWithCapacity:lengthArray];
        for(int i = 0; i < lengthArray; i++)
        {
            if ((vals[i] != NULL) && (vals[i] != 0))
            {
                temp[i] = [NSString stringWithUTF8String:vals[i]];
            }
        }
        params = [NSArray arrayWithArray:temp];
    }
    return params;
}


NSArray* LLNSArrayFromFloats(int lengthArray, float vals[])
{
    NSArray *result = [NSArray array];
    
    if ((lengthArray > 0) && (vals != NULL))
    {
        NSMutableArray *temp = [NSMutableArray arrayWithCapacity:lengthArray];
        for (int i = 0; i < lengthArray; i++)
        {
            [temp addObject:[NSNumber numberWithFloat:vals[i]]];
        }
        
        result = [NSArray arrayWithArray:temp];
    }
    
    return result;
}
