//
//  LLTexture2D.m
//  LLLibSet
//


#import "LLTexture2D.h"
#import "LLDrawingContext.h"
#import <OpenGLES/ES2/gl.h>
#import <OpenGLES/ES2/glext.h>
#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>
#import <Metal/Metal.h>
#import "LLMetalContext.h"


#define CHECK_GL_ERROR() ({ GLenum __error = glGetError(); if(__error) printf("OpenGL error 0x%04X in %s:%ld\n", __error, __FUNCTION__, (long)(__LINE__)); })



@interface LLTexture2DGL : LLTexture2D


@end




@implementation LLTexture2DGL
{
    GLuint						_name;
    NSUInteger					_width;
    NSUInteger                  _height;
}


+ (LLDrawingContextPixelFormat)drawingPixelFormatForrequestedFormat:(LLTexture2DFormat)pixelFormat
{
    switch (pixelFormat)
    {
        case Format_A8:
            return LLDrawingContextPixelFormat_A8;

        case Format_RGB16:
            return LLDrawingContextPixelFormat_RGB565;

        default:
            return LLDrawingContextPixelFormat_RGBA8888;
    }
}


- (instancetype)initWithImagePath:(NSString *)imagePath pixelFormat:(LLTexture2DFormat)pixelFormat doMipMaps:(BOOL)doMipMaps
{
    UIImage *curImage = nil;
    
    if ([imagePath isAbsolutePath] && [[NSFileManager defaultManager] fileExistsAtPath:imagePath])
    {
        curImage = [UIImage imageWithContentsOfFile:imagePath];
    }
    else
    {
        NSString *pathWithoutExtension = [imagePath stringByDeletingPathExtension];
        NSString *name = [pathWithoutExtension lastPathComponent];
        
        if ([name rangeOfString:@"@"].location != NSNotFound)
        {
            if ([name rangeOfString:@"@2x"].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:@"@2x" withString:@""];
            }
            else if ([name rangeOfString:@"@4x"].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:@"@4x" withString:@""];
            }
            else if ([name rangeOfString:@"@1x"].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:@"@1x" withString:@""];
            }
            else
            {
                name = [name stringByReplacingOccurrencesOfString:@"@" withString:@""];
            }
        }
        
        curImage = [UIImage imageNamed:name];
    }
    
    if (curImage == nil)
    {
        return nil;
    }

    return [self initWithCGImage:curImage.CGImage pixelFormat:pixelFormat doMipMaps:doMipMaps];
}


- (instancetype)initWithCGImage:(CGImageRef)cgImage pixelFormat:(LLTexture2DFormat)_pixelFormat doMipMaps:(BOOL)doMipMaps
{
    if (cgImage == NULL)
    {
        return nil;
    }

    LLDrawingContextPixelFormat curPixelFormat = [self.class drawingPixelFormatForrequestedFormat:_pixelFormat];

    LLDrawingContext *drawContext = [LLDrawingContext drawingContextWithCGImage:cgImage
                                                                    pixelFormat:curPixelFormat];
    return [self initWithDrawingContext:drawContext doMipMaps:doMipMaps];
}


- (instancetype)initWithDrawingContext:(LLDrawingContext *)context doMipMaps:(BOOL)doMipMaps
{
    self = [super init];
	if (self) {
        _width = context.width;
        _height = context.height;
        _name = 0;

        glGenTextures(1, &_name);
        glBindTexture(GL_TEXTURE_2D, _name);

		// Specify OpenGL texture image
		GLenum internalFormat = 0;
        GLenum dataFormat = 0;
        GLenum dataType = 0;
        GLint unpackAlignment = (GLint)context.bytesPerPixel;
		switch(context.pixelFormat)
		{
			case LLDrawingContextPixelFormat_RGBA8888:
                internalFormat = GL_RGBA8_OES;
                dataFormat = GL_RGBA;
                dataType = GL_UNSIGNED_BYTE;
				break;
			case LLDrawingContextPixelFormat_RGBA4444:
                internalFormat = GL_RGBA4;
                dataFormat = GL_RGBA;
                dataType = GL_UNSIGNED_SHORT_4_4_4_4;
				break;
			case LLDrawingContextPixelFormat_RGB5A1:
                internalFormat = GL_RGB5_A1;
                dataFormat = GL_RGBA;
                dataType = GL_UNSIGNED_SHORT_5_5_5_1;
				break;
			case LLDrawingContextPixelFormat_RGB565:
                internalFormat = GL_RGB565;
                dataFormat = GL_RGB;
                dataType = GL_UNSIGNED_SHORT_5_6_5;
				break;
			case LLDrawingContextPixelFormat_A8:
                internalFormat = GL_ALPHA8_EXT;
                dataFormat = GL_ALPHA;
                dataType = GL_UNSIGNED_BYTE;
				break;
			default:
				[NSException raise:NSInternalInconsistencyException format:@""];
		}
        

        glPixelStorei(GL_UNPACK_ALIGNMENT, unpackAlignment);

        glTexStorage2DEXT(GL_TEXTURE_2D, 1, internalFormat, (GLsizei)_width, (GLsizei)_height);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAX_LEVEL_APPLE, 0);
        glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, (GLsizei)_width, (GLsizei)_height, dataFormat, dataType, context.data.bytes);
        CHECK_GL_ERROR();


        if (doMipMaps)
        {
            glGenerateMipmap(GL_TEXTURE_2D);
        }

        glFinish();
	}
    
	return self;
}


- (void)dealloc
{
    glDeleteTextures(1, &_name);
}


- (NSString *)description
{
	return [NSString stringWithFormat:@"<%@ = %08lX | Name = %i | Dimensions = %lix%li>",
            [self class],
            (u_long)self,
            _name,
            (u_long)_width,
            (u_long)_height];
}


- (LLUnityIntPtr)name
{
    return (LLUnityIntPtr)(size_t)_name;
}


- (NSUInteger)width
{
    return _width;
}


- (NSUInteger)height
{
    return _height;
}


@end








MTLPixelFormat LLDrawingContextPixelFormatToMetalPixelFormat(LLDrawingContextPixelFormat format)
{
    switch (format)
    {
        case LLDrawingContextPixelFormat_A8:
            return MTLPixelFormatA8Unorm;

        case LLDrawingContextPixelFormat_RGBA8888:
            return MTLPixelFormatRGBA8Unorm;

        case LLDrawingContextPixelFormat_RGB565:
            return MTLPixelFormatB5G6R5Unorm;

        default:
            [NSException raise:@"LLTextureHelper" format:@"Not implemented format %@", @(format)];
            return MTLPixelFormatRGBA8Unorm;
    }
}


@interface LLTexture2DMetal : LLTexture2D
{
    id <MTLTexture>             _metalTexture;
    NSUInteger					_width;
    NSUInteger                  _height;
}

@end



@implementation LLTexture2DMetal


+ (LLDrawingContextPixelFormat)drawingPixelFormatForrequestedFormat:(LLTexture2DFormat)pixelFormat
{
    switch (pixelFormat)
    {
        case Format_A8:
            return LLDrawingContextPixelFormat_A8;

        case Format_RGB16:
            return LLDrawingContextPixelFormat_RGB565;

        default:
            return LLDrawingContextPixelFormat_RGBA8888;
    }
}


- (instancetype)initWithContext:(LLMetalContext *)context imagePath:(NSString *)imagePath pixelFormat:(LLTexture2DFormat)pixelFormat doMipMaps:(BOOL)doMipMaps
{
    UIImage *curImage = nil;
    
    if ([imagePath isAbsolutePath] && [[NSFileManager defaultManager] fileExistsAtPath:imagePath])
    {
        curImage = [UIImage imageWithContentsOfFile:imagePath];
    }
    else
    {
        NSString *pathWithoutExtension = [imagePath stringByDeletingPathExtension];
        NSString *name = [pathWithoutExtension lastPathComponent];
        
        if ([name rangeOfString:@"@"].location != NSNotFound)
        {
            if ([name rangeOfString:@"@2x"].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:@"@2x" withString:@""];
            }
            else if ([name rangeOfString:@"@4x"].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:@"@4x" withString:@""];
            }
            else if ([name rangeOfString:@"@1x"].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:@"@1x" withString:@""];
            }
            else
            {
                name = [name stringByReplacingOccurrencesOfString:@"@" withString:@""];
            }
        }
        
        curImage = [UIImage imageNamed:name];
    }
    
    if (curImage == nil)
    {
        return nil;
    }

    return [self initWithContext:context
                         CGImage:curImage.CGImage
                     pixelFormat:pixelFormat
                       doMipMaps:doMipMaps];
}


- (instancetype)initWithContext:(LLMetalContext *)context
                        CGImage:(CGImageRef)cgImage
                    pixelFormat:(LLTexture2DFormat)pixelFormat
                      doMipMaps:(BOOL)doMipMaps
{
    if (cgImage == NULL)
    {
        return nil;
    }

    LLDrawingContextPixelFormat curPixelFormat = [self.class drawingPixelFormatForrequestedFormat:pixelFormat];

    LLDrawingContext *drawContext = [LLDrawingContext drawingContextWithCGImage:cgImage
                                                                    pixelFormat:curPixelFormat];
    return [self initWithContext:context drawingContext:drawContext doMipMaps:doMipMaps];
}


- (instancetype)initWithContext:(LLMetalContext *)metalContext drawingContext:(LLDrawingContext *)context doMipMaps:(BOOL)doMipMaps
{
    self = [super init];
    if (self)
    {
        _width = context.width;
        _height = context.height;


        MTLTextureDescriptor *newDescriptor = [MTLTextureDescriptor texture2DDescriptorWithPixelFormat:LLDrawingContextPixelFormatToMetalPixelFormat(context.pixelFormat)
                                                                                                 width:_width
                                                                                                height:_height
                                                                                             mipmapped:doMipMaps];

        _metalTexture = [metalContext newTextureWithDescriptor:newDescriptor];

        MTLRegion uploadRegion = MTLRegionMake2D(0, 0, _width, _height);


        [_metalTexture replaceRegion:uploadRegion
                         mipmapLevel:0
                           withBytes:context.data.bytes
                         bytesPerRow:context.bytesPerRow];


        if (doMipMaps)
        {
            [metalContext generateMipmapsForTexture:_metalTexture];
        }
    }

    return self;
}


- (LLUnityIntPtr)name
{
    return (__bridge LLUnityIntPtr)_metalTexture;
}


- (NSUInteger)width
{
    return _width;
}


- (NSUInteger)height
{
    return _height;
}


- (void)dealloc
{
    CFBridgingRelease(self.name);
}


- (NSString *)description
{
    return [NSString stringWithFormat:@"<%@ | metal texture = %@>", NSStringFromClass(self.class), _metalTexture];
}


@end





@implementation LLTexture2D


+ (instancetype)glTextureWithImagePath:(NSString *)imagePath pixelFormat:(LLTexture2DFormat)pixelFormat doMipMaps:(BOOL)doMipMaps
{
    return [[LLTexture2DGL alloc] initWithImagePath:imagePath pixelFormat:pixelFormat doMipMaps:doMipMaps];
}


+ (instancetype)metalTextureWithContext:(LLMetalContext *)context imagePath:(NSString *)imagePath pixelFormat:(LLTexture2DFormat)pixelFormat doMipMaps:(BOOL)doMipMaps
{
    return [[LLTexture2DMetal alloc] initWithContext:context imagePath:imagePath pixelFormat:pixelFormat doMipMaps:doMipMaps];
}


- (LLUnityIntPtr)name
{
    [NSException raise:@"LLTexture2D" format:@"-name not implemented"];
    return NULL;
}


- (NSUInteger)width
{
    [NSException raise:@"LLTexture2D" format:@"-width not implemented"];
    return 0;
}


- (NSUInteger)height
{
    [NSException raise:@"LLTexture2D" format:@"-height not implemented"];
    return 0;
}


@end



