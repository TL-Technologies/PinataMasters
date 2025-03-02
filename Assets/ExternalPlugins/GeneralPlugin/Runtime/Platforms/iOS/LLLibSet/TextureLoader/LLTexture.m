//
//  LLTexture.m
//  LLLibSet
//


#import "LLTexture.h"
#import <GLKit/GLKit.h>
#import "LLCGLoader.h"
#ifdef METAL_ENABLE
#import <MetalKit/MetalKit.h>
#endif


FOUNDATION_EXTERN MTLDeviceObject          LLTextureGetMetalDevice();


@interface LLOperationQueue : NSObject
{
    dispatch_queue_t        _loadQueue;
    dispatch_queue_t        _syncQueue;
    dispatch_semaphore_t    _loadSemaphore;
    dispatch_semaphore_t    _syncSemaphore;
}

+ (instancetype)sharedInstance;
- (void)addOperationBackgroundBlock:(void (^)())blockBack andMainBlock:(void (^)())blockMain;

@end




@implementation LLOperationQueue

+ (instancetype)sharedInstance
{
    static LLOperationQueue *__sharedInstance = nil;
    if (__sharedInstance == nil)
    {
        __sharedInstance = [[LLOperationQueue alloc] init];
    }
    
    return __sharedInstance;
}


- (instancetype)init
{
    self = [super init];
    if (self != nil)
    {
        _loadQueue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0);
        _syncQueue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0);
        _loadSemaphore = dispatch_semaphore_create(1);
        _syncSemaphore = dispatch_semaphore_create(0);
    }
    return self;
}


- (void)addOperationBackgroundBlock:(void (^)())blockBack andMainBlock:(void (^)())blockMain
{
    dispatch_async(_loadQueue, ^{
        dispatch_semaphore_wait(_loadSemaphore, DISPATCH_TIME_FOREVER);
        blockBack();
        dispatch_semaphore_signal(_syncSemaphore);
    });
    dispatch_async(_syncQueue, ^{
        dispatch_semaphore_wait(_syncSemaphore, DISPATCH_TIME_FOREVER);
        dispatch_async(dispatch_get_main_queue(),^{
            blockMain();
            dispatch_semaphore_signal(_loadSemaphore);
        });
    });
}

@end




@interface LLTextureMetal : LLTexture
{
    MTLTextureObject               _metalTexture;
    MTLTextureObject               _tempTexture;
    BOOL                            _isLoad;
    LLUnityIntPtr                   _metalPointer;
    NSUInteger					    _width;
    NSUInteger                      _height;
}

@end


#warning Not work correctly
@implementation LLTextureMetal


- (instancetype)initWithImage:(UIImage *)image doMipMaps:(BOOL)mipMaps
{
    self = [super init];
    if (self != nil)
    {
        _width = image.size.width * image.scale;
        _height = image.size.height * image.scale;

        @autoreleasepool
        {
#ifdef METAL_ENABLE
            // Load Metal texture
            LLMTLoader *loader = [LLMTLoader loaderWithCGImage:image.CGImage hasMipMaps:mipMaps];
            _metalTexture = loader.name;
            _isLoad = YES;
            loader = nil;
            
            _metalPointer = (__bridge LLUnityIntPtr)_metalTexture;
#endif
        }
    }
    return self;
}


- (instancetype)initWithImage:(UIImage *)image doMipMaps:(BOOL)mipMaps callback:(void (^)(LLTexture*, LLUnityIntPtr))callback
{
    self = [super init];
    if (self != nil)
    {
        _width = image.size.width * image.scale;
        _height = image.size.height * image.scale;
        
        @autoreleasepool
        {
#ifdef METAL_ENABLE
            MTLDeviceObject device = LLTextureGetMetalDevice();

            // Generate white Metal texture.
            {
                unsigned char emptyData = 0;
                MTLTextureDescriptor *tempDescriptor = [MTLTextureDescriptor texture2DDescriptorWithPixelFormat:MTLPixelFormatA8Unorm width:1 height:1 mipmapped:NO];
                _tempTexture = [device newTextureWithDescriptor:tempDescriptor];
                [_tempTexture replaceRegion:MTLRegionMake2D(0, 0, 1, 1) mipmapLevel:0 withBytes:&emptyData bytesPerRow:1];
                _metalPointer = (__bridge LLUnityIntPtr)_tempTexture;

            }
            
            
            // Load Metal texture
            {
                __block MTLTextureObject newTexture = nil;
                
                void (^backBlock)(void) = ^
                {
                    LLMTLoader *loader = [LLMTLoader loaderWithCGImage:image.CGImage hasMipMaps:mipMaps];
                    newTexture = loader.name;
                    loader = nil;
                };
                
                void (^mainBlock)(void) = ^
                {
                    LLUnityIntPtr oldTexture = _metalPointer;
                    
                    _metalTexture = newTexture;
                    _metalPointer = (__bridge LLUnityIntPtr)_metalTexture;
                    _isLoad = YES;
                    
                    if (callback != nil)
                    {
                        callback(self, oldTexture);
                    }
                };
                
                [[LLOperationQueue sharedInstance] addOperationBackgroundBlock:backBlock andMainBlock:mainBlock];
            }
#endif
        }
    }
    return self;
}


- (void)dealloc
{
}


- (NSString *)description
{
    return [NSString stringWithFormat:@"<%@ | metal texture = %@>", NSStringFromClass(self.class), _metalTexture];
}


- (LLUnityIntPtr)name
{
    return _metalPointer;
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





@implementation LLTexture

+ (instancetype)createTextureFromImage:(UIImage *)image hasMipMaps:(BOOL)mipMaps
{
    return [[LLTextureMetal alloc] initWithImage:image doMipMaps:mipMaps];
}


+ (instancetype)createTextureFromImage:(UIImage *)image hasMipMaps:(BOOL)mipMaps  callback:(void (^)(LLTexture*, LLUnityIntPtr))callback
{
    return [[LLTextureMetal alloc] initWithImage:image doMipMaps:mipMaps callback:callback];
}


- (LLUnityIntPtr)name
{
    [NSException raise:@"LLTexture" format:@"-name not implemented"];
    return NULL;
}


- (NSUInteger)width
{
    [NSException raise:@"LLTexture" format:@"-width not implemented"];
    return 0;
}


- (NSUInteger)height
{
    [NSException raise:@"LLTexture" format:@"-height not implemented"];
    return 0;
}

@end
