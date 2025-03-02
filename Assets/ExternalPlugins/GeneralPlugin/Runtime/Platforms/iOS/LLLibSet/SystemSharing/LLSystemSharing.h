//
//  LLSystemSharing.h
//  LLLibSet
//

#import "LLLibSet.h"
#import "LLLibSetExtentions.h"
#import <UIKit/UIKit.h>


@interface LLSystemSharing : NSObject

+ (void)shareText:(NSString *)text
        withImage:(UIImage *)image
           andURL:(NSURL *)url;

+ (void)shareText:(NSString *)text
    withImagePath:(NSString *)imagePath
     andURLString:(NSString *)urlString;

+ (void)shareText:(NSString *)text
    withImageData:(NSData *)imageData
     andURLString:(NSString *)urlString;

@end

#if !UNITY_TVOS
FOUNDATION_EXTERN void LLSystemSharingShareItems(LLUnityString text, LLUnityString imagePath, LLUnityString urlString);
FOUNDATION_EXTERN void LLSystemSharingShareItemsWithData(LLUnityString text, Byte *bytes, LLUnityInt width, LLUnityInt height, LLUnityString urlString);
#endif
