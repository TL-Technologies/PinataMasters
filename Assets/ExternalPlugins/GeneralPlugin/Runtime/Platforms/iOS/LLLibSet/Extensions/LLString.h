//
//  LLString.h
//  LLLibSet
//

#import "LLLibSet.h"

@interface NSString (libset_extentions)

+ (instancetype)stringWithUnityString:(LLUnityString)unityString;

@end


FOUNDATION_EXPORT LLUnityString LLUnityStringFromNSString(NSString *string);
FOUNDATION_EXPORT NSString* LLNSStringFromUnityString(LLUnityString string);
