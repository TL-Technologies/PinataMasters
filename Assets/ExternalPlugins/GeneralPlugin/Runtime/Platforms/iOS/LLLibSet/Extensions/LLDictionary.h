//
//  LLDictionary.h
//  LLLibSet
//

#import "LLLibSet.h"
#import "LLLibSetExtentions.h"

@interface NSDictionary (libset_extentions)

- (NSString *)JSONString;

@end

FOUNDATION_EXPORT NSDictionary* LLNSDictionaryFromUnityStrings(int lengthDictionary, LLUnityString keys[], LLUnityString vals[]);
