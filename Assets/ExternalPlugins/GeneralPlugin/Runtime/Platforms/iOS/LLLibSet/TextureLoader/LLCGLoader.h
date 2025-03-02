//
//  LLCGLoader.h
//  LLLibSet
//

#import "LLTextureLoader.h"
#import <CoreGraphics/CoreGraphics.h>
#import <OpenGLES/gltypes.h>

@interface LLCGLoader : NSObject

+ (instancetype)loaderWithCGImage:(CGImageRef)imageRef hasMipMaps:(BOOL)mipMaps;

@end


@interface LLGLLoader : LLCGLoader

@property (nonatomic) GLuint name;

@end


@interface LLMTLoader : LLCGLoader

@property (nonatomic) MTLTextureObject name;

@end
