//
//  LLCGLoader.m
//  LLLibSet
//


#import "LLCGLoader.h"
#import <GLKit/GLKit.h>
#ifdef METAL_ENABLE
#import <MetalKit/MetalKit.h>
#endif


//#define GLKIT_LOADER

FOUNDATION_EXTERN MTLDeviceObject          LLTextureGetMetalDevice();

NSUInteger const TEXTURE_MAX_DIMENSION = 4096;

@implementation LLCGLoader

+ (instancetype)loaderWithCGImage:(CGImageRef)imageRef hasMipMaps:(BOOL)mipMaps
{
    return [[self alloc] initWithCGImage:imageRef hasMipMaps:mipMaps];
}


 - (instancetype)initWithCGImage:(CGImageRef)imageRef hasMipMaps:(BOOL)mipMaps
{
    self = [super init];
    return self;
}

@end



@interface LLGLLoader()
{
    GLuint      _name;
}

@end


@implementation LLGLLoader

@synthesize name = _name;

+ (instancetype)loaderWithCGImage:(CGImageRef)imageRef hasMipMaps:(BOOL)mipMaps
{
    return [[self alloc] initWithCGImage:imageRef hasMipMaps:mipMaps];
}


- (instancetype)initWithCGImage:(CGImageRef)imageRef hasMipMaps:(BOOL)mipMaps
{
    self = [super initWithCGImage:imageRef hasMipMaps:mipMaps];
    if (self)
    {
        @autoreleasepool
        {
#ifdef GLKIT_LOADER
            NSDictionary *options = @{ GLKTextureLoaderApplyPremultiplication : [NSNumber numberWithBool:NO],
                                       GLKTextureLoaderGenerateMipmaps : [NSNumber numberWithBool:mipMaps],
                                       GLKTextureLoaderOriginBottomLeft : [NSNumber numberWithBool:YES],
                                       GLKTextureLoaderSRGB : [NSNumber numberWithBool:NO] };
            
            NSError *error = nil;
            GLKTextureInfo *loader = [GLKTextureLoader textureWithCGImage:imageRef options:options error:&error];
            _name = loader.name;
            loader = nil;
#else
            NSUInteger                textureWidth, textureHeight;
            CGContextRef            context = nil;
            void*                    data = nil;
            CGColorSpaceRef            colorSpace;
            BOOL                    hasAlpha;
            CGImageAlphaInfo        info;
            CGSize                    imageSizeInPixels;
            
            info = CGImageGetAlphaInfo(imageRef);
            hasAlpha = ((info == kCGImageAlphaPremultipliedLast) || (info == kCGImageAlphaPremultipliedFirst) || (info == kCGImageAlphaLast) || (info == kCGImageAlphaFirst) ? YES : NO);
            
            colorSpace = CGImageGetColorSpace(imageRef);
            
            if (colorSpace)
            {
                if (hasAlpha)
                {
                    info = kCGImageAlphaPremultipliedLast;
                }
                else
                {
                    info = kCGImageAlphaNoneSkipLast;
                }
            }
            else
            {
                [NSException raise:@"LLTexture2D" format:@"-load texture with A8 format"];
            }
            
            textureWidth = CGImageGetWidth(imageRef);
            textureHeight = CGImageGetHeight(imageRef);
            
            if ((textureHeight > TEXTURE_MAX_DIMENSION) || (textureWidth > TEXTURE_MAX_DIMENSION))
            {
                [NSException raise:@"LLTextureGL" format:@"-load texture (%lu x %lu) is bigger than the supported %ld x %ld", (long)textureWidth, (long)textureHeight,
                 (long)TEXTURE_MAX_DIMENSION, (long)TEXTURE_MAX_DIMENSION];
            }
            
            imageSizeInPixels = CGSizeMake(CGImageGetWidth(imageRef), CGImageGetHeight(imageRef));
            
            colorSpace = CGColorSpaceCreateDeviceRGB();
            data = malloc(textureHeight * textureWidth * 4);
            context = CGBitmapContextCreate(data, textureWidth, textureHeight, 8, 4 * textureWidth, colorSpace, info | kCGBitmapByteOrder32Big);
            CGColorSpaceRelease(colorSpace);
            
            CGContextClearRect(context, CGRectMake(0, 0, textureWidth, textureHeight));
            CGContextTranslateCTM(context, 0, textureHeight - imageSizeInPixels.height);
            CGContextScaleCTM(context, 1.0, -1.0);
            CGContextTranslateCTM(context, 0, -imageSizeInPixels.height);
            CGContextDrawImage(context, CGRectMake(0, 0, CGImageGetWidth(imageRef), CGImageGetHeight(imageRef)), imageRef);
            
            // GLLoad
            {
                glGenTextures(1, &_name);
                glBindTexture(GL_TEXTURE_2D, _name);
                
                glPixelStorei(GL_UNPACK_ALIGNMENT,4);
                glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR );
                glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR );
                glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE );
                glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE );
                
                glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, (GLsizei)CGImageGetWidth(imageRef), (GLsizei)CGImageGetHeight(imageRef), 0, GL_RGBA, GL_UNSIGNED_BYTE, data);
                
                if (mipMaps)
                {
                    glGenerateMipmap(GL_TEXTURE_2D);
                    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_NEAREST);
                }
            }
            
            CGContextRelease(context);
            free(data);
#endif
        }
    }
    return self;
}

@end




@interface LLMTLoader()
{
    id<MTLTexture>  _name;
}

@end


@implementation LLMTLoader

@synthesize name = _name;

+ (instancetype)loaderWithCGImage:(CGImageRef)imageRef hasMipMaps:(BOOL)mipMaps
{
    return [[self alloc] initWithCGImage:imageRef hasMipMaps:mipMaps];
}


- (instancetype)initWithCGImage:(CGImageRef)imageRef hasMipMaps:(BOOL)mipMaps
{
    self = [super initWithCGImage:imageRef hasMipMaps:mipMaps];
    if (self)
    {
        @autoreleasepool
        {
#ifdef METAL_ENABLE
            MTLDeviceObject device = LLTextureGetMetalDevice();
            NSMutableDictionary *options = [NSMutableDictionary dictionaryWithCapacity:4];
            [options setValue:[NSNumber numberWithBool:mipMaps] forKey:MTKTextureLoaderOptionAllocateMipmaps];
            [options setValue:[NSNumber numberWithBool:NO] forKey:MTKTextureLoaderOptionSRGB];
            
            if (@available(ios 10.0, *))
            {
                [options setValue:[NSNumber numberWithBool:mipMaps] forKey:MTKTextureLoaderOptionGenerateMipmaps];
                [options setValue:MTKTextureLoaderOriginBottomLeft forKey:MTKTextureLoaderOptionOrigin];
            }
            
            MTKTextureLoader *loader = [[MTKTextureLoader alloc] initWithDevice:device];
            
            NSError *error = nil;
            _name = [loader newTextureWithCGImage:imageRef options:options error:&error];
            loader = nil;
#endif
        }
    }
    
    return self;
}

@end

