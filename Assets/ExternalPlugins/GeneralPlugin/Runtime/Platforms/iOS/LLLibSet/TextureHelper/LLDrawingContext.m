//
//  LLDrawingContext.m
//  LLLibSet
//

#import "LLDrawingContext.h"



NSUInteger LLDrawingContextPixelFormatByteSize(LLDrawingContextPixelFormat format)
{
    switch (format)
    {
        case LLDrawingContextPixelFormat_RGB565:
        case LLDrawingContextPixelFormat_RGBA4444:
        case LLDrawingContextPixelFormat_RGB5A1:
            return 2;

        case LLDrawingContextPixelFormat_A8:
            return 1;

        default:
            return 4;
    }
}





@implementation LLDrawingContext

@synthesize cgContext = _cgContext;
@synthesize pixelFormat = _pixelFormat;


+ (instancetype)drawingContextWithCGImage:(CGImageRef)imageRef
                              pixelFormat:(LLDrawingContextPixelFormat)pixelFormat
{
    return [[self alloc] initWithCGImage:imageRef
                             pixelFormat:pixelFormat];
}


+ (instancetype)drawingContextWithPixelFormat:(LLDrawingContextPixelFormat)pixelFormat size:(CGSize)size
{
    return [[self alloc] initWithPixelFormat:pixelFormat size:size];
}


- (instancetype)initWithPixelFormat:(LLDrawingContextPixelFormat)pixelFormat size:(CGSize)size
{
    self = [super init];
    if (self) {
        NSUInteger contextArea = size.width * size.height;

        // Create the bitmap graphics context
        CGColorSpaceRef contextColorSpace = NULL;
        NSUInteger dataSize = 0;
        NSUInteger rowSize = 0;
        CGBitmapInfo contextInfo = 0;

        switch(pixelFormat)
        {
            case LLDrawingContextPixelFormat_RGBA8888:
            case LLDrawingContextPixelFormat_RGBA4444:
            case LLDrawingContextPixelFormat_RGB5A1:
            case LLDrawingContextPixelFormat_RGB565:
                contextColorSpace = CGColorSpaceCreateDeviceRGB();
                dataSize = contextArea * 4;
                rowSize = size.width * 4;
                contextInfo = ((pixelFormat != LLDrawingContextPixelFormat_RGB565) ? kCGImageAlphaPremultipliedLast : kCGImageAlphaNoneSkipLast) | kCGBitmapByteOrder32Big;
                break;

            case LLDrawingContextPixelFormat_A8:
                contextColorSpace = CGColorSpaceCreateDeviceGray();
                dataSize = contextArea;
                rowSize = size.width;
                contextInfo |= kCGImageAlphaNone;
                break;

            default:
                [NSException raise:NSInternalInconsistencyException format:@"Invalid pixel format"];
        }

        _contextData = [[NSMutableData alloc] initWithLength:dataSize];
        _cgContext = CGBitmapContextCreate(_contextData.mutableBytes,
                                           size.width,
                                           size.height,
                                           8,
                                           rowSize,
                                           contextColorSpace,
                                           contextInfo);
        _pixelFormat = pixelFormat;


        CGColorSpaceRelease(contextColorSpace);
    }

    return self;
}


- (void)dealloc
{
    CGContextRelease(_cgContext);
}


- (NSUInteger)width
{
    return CGBitmapContextGetWidth(_cgContext);
}


- (NSUInteger)height
{
    return CGBitmapContextGetHeight(_cgContext);
}


- (NSUInteger)area
{
    return self.width * self.height;
}


- (BOOL)hasPremultipliedAlpha
{
    CGBitmapInfo contextInfo = CGBitmapContextGetBitmapInfo(_cgContext);
    return (contextInfo & kCGImageAlphaPremultipliedLast || contextInfo & kCGImageAlphaPremultipliedFirst);
}


- (NSData *)data
{
    // Repack the pixel data into the right format
    switch (_pixelFormat)
    {
        case LLDrawingContextPixelFormat_RGB565: {
            //Convert "RRRRRRRRRGGGGGGGGBBBBBBBBAAAAAAAA" to "RRRRRGGGGGGBBBBB"
            NSUInteger area = self.area;
            NSUInteger dataSize = area * 2;
            void *tempData = malloc(dataSize);
            const uint32_t *inPixel32 = (const uint32_t*)_contextData.bytes;
            uint16_t *outPixel16 = (uint16_t*)tempData;
            for(unsigned int i = 0; i < area; ++i, ++inPixel32)
            {
                *outPixel16++ = ((((*inPixel32 >> 0) & 0xFF) >> 3) << 11) |
                ((((*inPixel32 >> 8) & 0xFF) >> 2) << 5) |
                ((((*inPixel32 >> 16) & 0xFF) >> 3) << 0);
            }

            return [NSData dataWithBytesNoCopy:tempData length:dataSize freeWhenDone:YES];
        }
            break;

        case LLDrawingContextPixelFormat_RGBA4444: {
            //Convert "RRRRRRRRRGGGGGGGGBBBBBBBBAAAAAAAA" to "RRRRGGGGBBBBAAAA"
            NSUInteger area = self.area;
            NSUInteger dataSize = area * 2;
            void *tempData = malloc(dataSize);
            const uint32_t *inPixel32 = (const uint32_t*)_contextData.bytes;
            uint16_t *outPixel16 = (uint16_t*)tempData;
            for(unsigned int i = 0; i < area; ++i, ++inPixel32)
                *outPixel16++ =
                ((((*inPixel32 >> 0) & 0xFF) >> 4) << 12) | // R
                ((((*inPixel32 >> 8) & 0xFF) >> 4) << 8) | // G
                ((((*inPixel32 >> 16) & 0xFF) >> 4) << 4) | // B
                ((((*inPixel32 >> 24) & 0xFF) >> 4) << 0); // A
            
            return [NSData dataWithBytesNoCopy:tempData length:dataSize freeWhenDone:YES];
        }
            break;

        case LLDrawingContextPixelFormat_RGB5A1: {
            //Convert "RRRRRRRRRGGGGGGGGBBBBBBBBAAAAAAAA" to "RRRRRGGGGGBBBBBA"
            NSUInteger area = self.area;
            NSUInteger dataSize = area * 2;
            void *tempData = malloc(dataSize);
            const uint32_t *inPixel32 = (const uint32_t*)_contextData.bytes;
            uint16_t *outPixel16 = (uint16_t*)tempData;
            for(unsigned int i = 0; i < area; ++i, ++inPixel32)
                *outPixel16++ =
                ((((*inPixel32 >> 0) & 0xFF) >> 3) << 11) | // R
                ((((*inPixel32 >> 8) & 0xFF) >> 3) << 6) | // G
                ((((*inPixel32 >> 16) & 0xFF) >> 3) << 1) | // B
                ((((*inPixel32 >> 24) & 0xFF) >> 7) << 0); // A


            return [NSData dataWithBytesNoCopy:tempData length:dataSize freeWhenDone:YES];
        }
            break;
            
        default:
            return _contextData;
    }
}


- (instancetype)initWithCGImage:(CGImageRef)imageRef
                    pixelFormat:(LLDrawingContextPixelFormat)pixelFormat
{
    CGSize imageSize = CGSizeMake(CGImageGetWidth(imageRef), CGImageGetHeight(imageRef));
    self = [self initWithPixelFormat:pixelFormat size:imageSize];
    if (self)
    {
        [self clear];

        CGContextTranslateCTM(self.cgContext, 0, self.height);
        CGContextScaleCTM(self.cgContext, 1, -1);
        CGContextDrawImage(self.cgContext, (CGRect) { .origin = CGPointZero, .size = imageSize }, imageRef);
    }

	return self;
}


- (void)clear
{
    CGRect imageRect = (CGRect) { CGPointZero, (CGSize) { .width = self.width, .height = self.height } };

    CGContextClearRect(self.cgContext, imageRect);
}


- (NSUInteger)bytesPerRow
{
    return self.width * self.bytesPerPixel;
}


- (NSUInteger)bytesPerPixel
{
    return LLDrawingContextPixelFormatByteSize(self.pixelFormat);
}


@end

