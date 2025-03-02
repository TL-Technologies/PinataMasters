//
//  LLDrawingContext.h
//  LLLibSet
//

#import <CoreGraphics/CoreGraphics.h>



typedef enum
{
    LLDrawingContextPixelFormat_RGBA8888,
    LLDrawingContextPixelFormat_RGB565,
    LLDrawingContextPixelFormat_A8,
    LLDrawingContextPixelFormat_RGBA4444,
    LLDrawingContextPixelFormat_RGB5A1,
}
LLDrawingContextPixelFormat;



@interface LLDrawingContext : NSObject
{
    LLDrawingContextPixelFormat _pixelFormat;
    CGContextRef                _cgContext;
    NSMutableData               *_contextData;
}

@property (nonatomic, readonly) CGContextRef cgContext;
@property (nonatomic, readonly) NSData *data;
@property (nonatomic, readonly) NSUInteger width;
@property (nonatomic, readonly) NSUInteger height;
@property (nonatomic, readonly) NSUInteger area;
@property (nonatomic, readonly) BOOL hasPremultipliedAlpha;
@property (nonatomic, readonly) LLDrawingContextPixelFormat pixelFormat;
@property (nonatomic, readonly) NSUInteger bytesPerRow;
@property (nonatomic, readonly) NSUInteger bytesPerPixel;

- (instancetype)initWithPixelFormat:(LLDrawingContextPixelFormat)pixelFormat
                               size:(CGSize)size;

- (instancetype)initWithCGImage:(CGImageRef)imageRef
                    pixelFormat:(LLDrawingContextPixelFormat)pixelFormat;

+ (instancetype)drawingContextWithPixelFormat:(LLDrawingContextPixelFormat)pixelFormat
                                         size:(CGSize)size;

+ (instancetype)drawingContextWithCGImage:(CGImageRef)imageRef
                              pixelFormat:(LLDrawingContextPixelFormat)pixelFormat;


- (void)clear;

@end





