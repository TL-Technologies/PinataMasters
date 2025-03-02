//
//  LLArray.h
//  LLLibSet
//

#import "LLLibSet.h"
#import "LLLibSetExtentions.h"


@interface NSArray (libset_extentions)

- (NSString *)JSONString;

@end

FOUNDATION_EXPORT NSArray* LLNSArrayFromUnityStrings(int lengthArray, LLUnityString vals[]);
FOUNDATION_EXPORT NSArray* LLNSArrayFromFloats(int lengthArray, float vals[]);
