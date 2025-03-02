//
//  LLDictionary.m
//  LLLibSet
//

#import "LLDictionary.h"

@implementation NSDictionary (libset_extentions)


- (NSString *)JSONString
{
    return [NSJSONSerialization JSONStringWithObject:self];
}

@end


NSDictionary* LLNSDictionaryFromUnityStrings(int lengthDictionary, LLUnityString keys[], LLUnityString vals[])
{
    NSDictionary *params = [NSDictionary dictionary];
    if ((lengthDictionary > 0) && (keys != NULL) && (vals != NULL))
    {
        NSMutableDictionary *temp = [NSMutableDictionary dictionaryWithCapacity:lengthDictionary];
        for(int i = 0; i < lengthDictionary; i++)
        {
            if ((vals[i] != NULL) && (vals[i] != 0) && (keys[i] != NULL) && keys[i] != 0)
            {
                temp[[NSString stringWithUTF8String:keys[i]]] = [NSString stringWithUTF8String:vals[i]];
            }
        }
        params = [NSDictionary dictionaryWithDictionary:temp];
    }
    return params;
}


