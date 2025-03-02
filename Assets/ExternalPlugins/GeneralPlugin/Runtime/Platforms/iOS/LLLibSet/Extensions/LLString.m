//
//  LLString.m
//  LLLibSet
//

#import "LLString.h"

@implementation NSString (libset_extentions)

+ (instancetype)stringWithUnityString:(LLUnityString)unityString
{
    if (unityString != NULL)
    {
        return [NSString stringWithUTF8String:unityString];
    }
    else
    {
        return [NSString stringWithUTF8String:""];
    }
}

@end


NSString* LLNSStringFromUnityString(LLUnityString str)
{
    return [NSString stringWithUnityString:str];
}


LLUnityString LLUnityStringFromNSString(NSString *str)
{
    char *result = NULL;
    if (str == nil || (id)str == [NSNull null])
    {
        str = @"";
    }
    
    NSInteger resultLength = strlen(str.UTF8String) + 1;
    result = (char *)calloc(1, resultLength);

    strncpy(result, str.UTF8String, resultLength);
    return result;
}


