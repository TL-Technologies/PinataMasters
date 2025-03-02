//
//  LLTexture.h
//  LLLibSet
//

#import "LLTextureLoader.h"
#import <UIKit/UIKit.h>

@interface LLTexture : NSObject

@property (nonatomic, readonly) LLUnityIntPtr name;
@property (nonatomic, readonly) NSUInteger width;
@property (nonatomic, readonly) NSUInteger height;

+ (instancetype)createTextureFromImage:(UIImage *)image hasMipMaps:(BOOL)mipMaps;
+ (instancetype)createTextureFromImage:(UIImage *)image hasMipMaps:(BOOL)mipMaps callback:(void (^)(LLTexture*, LLUnityIntPtr oldName))callback;

@end
