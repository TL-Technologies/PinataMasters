//
//  LLTexture2D.h
//  LLLibSet
//

#import "LLLibSet.h"
#import "LLLibSetExtentions.h"
#import "LLTextureHelperShared.h"


@class LLMetalContext;

@interface LLTexture2D : NSObject

@property (nonatomic, readonly) LLUnityIntPtr name;
@property (nonatomic, readonly) NSUInteger width;
@property (nonatomic, readonly) NSUInteger height;

+ (instancetype)glTextureWithImagePath:(NSString *)imagePath pixelFormat:(LLTexture2DFormat)pixelFormat doMipMaps:(BOOL)doMipMaps;
+ (instancetype)metalTextureWithContext:(LLMetalContext *)context imagePath:(NSString *)imagePath pixelFormat:(LLTexture2DFormat)pixelFormat doMipMaps:(BOOL)doMipMaps;

@end


