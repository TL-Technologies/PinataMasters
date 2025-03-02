//
//  LLTextureHelper.m
//  LLLibSet
//

#import "LLTextureHelper.h"
#import "LLTexture2D.h"
#import <UIKit/UIKit.h>
#import <Metal/Metal.h>


#import "LLDrawingContext.h"
#import "LLMetalContext.h"




NSString * const LLTHLoadResultKeyTexturePtr 	= @"texturePtr";
NSString * const LLTHLoadResultKeyWidth 		= @"width";
NSString * const LLTHLoadResultKeyHeight 		= @"height";
NSString * const LLTHLoadResultKeyLoadKey		= @"loadKey";


@interface LLTHLoadResult : NSObject

@property (nonatomic, readonly) const void *texturePtr;
@property (nonatomic, readonly) int width;
@property (nonatomic, readonly) int height;
@property (nonatomic, readonly) NSString *loadKey;

@property (nonatomic, readonly) NSDictionary *JSONDescription;

- (instancetype)initWithJSONDescription:(NSDictionary *)JSONDescription;
- (instancetype)initWithTexturePtr:(const void *)texPtr width:(int)width height:(int)height loadKey:(NSString *)loadKey;
- (instancetype)initWithTexture2D:(LLTexture2D *)texture2d loadKey:(NSString *)loadKey;

@end


@implementation LLTHLoadResult


- (instancetype)initWithTexturePtr:(const void *)texPtr width:(int)width height:(int)height loadKey:(NSString *)loadKey
{
    self = [super init];
    if (self)
    {
        _texturePtr = texPtr;
        _width = width;
        _height = height;
        _loadKey = [loadKey copy];
    }
    
    return self;
}


- (instancetype)initWithTexture2D:(LLTexture2D *)texture2d loadKey:(NSString *)loadKey
{
    return [self initWithTexturePtr:texture2d.name
                              width:(int)texture2d.width
                             height:(int)texture2d.height
                            loadKey:loadKey];
}


- (instancetype)initWithJSONDescription:(NSDictionary *)JSONDescription
{
    return [self initWithTexturePtr:[[JSONDescription objectForKey:LLTHLoadResultKeyTexturePtr] pointerValue]
                              width:[[JSONDescription objectForKey:LLTHLoadResultKeyWidth] intValue]
                             height:[[JSONDescription objectForKey:LLTHLoadResultKeyHeight] intValue]
                            loadKey:[JSONDescription objectForKey:LLTHLoadResultKeyLoadKey]];
}


- (NSDictionary *)JSONDescription
{
    return @{
             LLTHLoadResultKeyTexturePtr    : @((NSUInteger)_texturePtr),
             LLTHLoadResultKeyWidth         : @(_width),
             LLTHLoadResultKeyHeight        : @(_height),
             LLTHLoadResultKeyLoadKey       : _loadKey
             };
}


- (NSString *)description
{
    return self.JSONDescription.description;
}


@end




@interface NSObject (Texture_AsyncLoad)

-(void)performTextureOnMainThreadWithObject:(id)object waitUntilDone:(BOOL)wait;
-(void)performTextureOnThread:(NSThread *)thr withObject:(id)arg waitUntilDone:(BOOL)wait;

@end


@implementation NSObject (Texture_AsyncLoad)


-(void)performBlockWithObject:(id)object
{
    void (^block)(id object) = (id)self;
    block(object);
}


-(void)performTextureOnMainThreadWithObject:(id)object waitUntilDone:(BOOL)wait
{
    id myCopy = [self copy];
    [myCopy performSelectorOnMainThread:@selector(performBlockWithObject:)
                             withObject:object
                          waitUntilDone:wait];
    myCopy = nil;
}


-(void)performTextureOnThread:(NSThread *)thr withObject:(id)arg waitUntilDone:(BOOL)wait
{
    id myCopy = [self copy];
    [myCopy performSelector:@selector(performBlockWithObject:)
                   onThread:thr
                 withObject:arg
              waitUntilDone:wait];
    myCopy = nil;
}

@end




@interface LLTextureHelper ()

@property (nonatomic, readonly) BOOL isMetalAvailable;

@end



@implementation LLTextureHelper
{
    NSThread                *_backgroundThread;
    LLMetalContext          *_curMetalContext;
    
    NSMutableDictionary     *_retainedTextures;
}


+(instancetype)sharedInstance
{
    static LLTextureHelper *__sharedInstance = nil;
    if (__sharedInstance == nil)
    {
        __sharedInstance = [[LLTextureHelper alloc] init];
    }
    
    return __sharedInstance;
}


-(id)init
{
    self = [super init];
    if (self)
    {
        _retainedTextures = [[NSMutableDictionary alloc] init];
        
        if([EAGLContext currentContext] == NULL)
        {
            if (MTLCreateSystemDefaultDevice != NULL)
            {
                id <MTLDevice> curMetalDevice = MTLCreateSystemDefaultDevice();
                if (curMetalDevice != nil)
                {
                    _curMetalContext = [[LLMetalContext alloc] initWithDevice:curMetalDevice];
                }
            }
            
            
            if (self.isMetalAvailable)
            {
                _backgroundThread = [[NSThread alloc] initWithTarget:self
                                                            selector:@selector(backgroundLoopWithMetalContext:)
                                                              object:_curMetalContext];
            }
        }
        else
        {
            EAGLContext * backgroundContext = [[EAGLContext alloc] initWithAPI:[EAGLContext currentContext].API
                                                                    sharegroup:[EAGLContext currentContext].sharegroup];
            
            _backgroundThread = [[NSThread alloc] initWithTarget:self
                                                        selector:@selector(backgroundLoopWithContext:)
                                                          object:backgroundContext];
        }
        
        
        _backgroundThread.threadPriority = 0.;
        _backgroundThread.name = @"LLTextureHelper";
        
        [_backgroundThread start];
    }
    
    return self;
}


- (void)dealloc
{
    [_backgroundThread cancel];
}



- (BOOL)isMetalAvailable
{
    return _curMetalContext != nil;
}



- (void)backgroundLoop
{
    @autoreleasepool
    {
        NSRunLoop *threadLoop = [NSRunLoop currentRunLoop];
        
        while (![NSThread currentThread].isCancelled)
        {
            @autoreleasepool
            {
                [threadLoop runMode:NSDefaultRunLoopMode beforeDate:[NSDate dateWithTimeIntervalSinceNow:10.]];
            }
        }
    }
}


- (void)backgroundLoopWithContext:(EAGLContext *)glContext
{
    [EAGLContext setCurrentContext:glContext];
    
    [self backgroundLoop];
}


- (void)backgroundLoopWithMetalContext:(LLMetalContext *)context
{
    [self backgroundLoop];
}



- (LLTexture2D *)createTextureWithImagePath:(NSString *)imagePath
                             requestMipMaps:(BOOL)doMipMaps
                                pixelFormat:(LLTexture2DFormat)pixelFormat
{
    LLTexture2D *newTexture = nil;
    
    if (self.isMetalAvailable)
    {
        newTexture = [LLTexture2D metalTextureWithContext:_curMetalContext
                                                imagePath:imagePath
                                              pixelFormat:pixelFormat
                                                doMipMaps:doMipMaps];
    }
    else
    {
        
        
        newTexture = [LLTexture2D glTextureWithImagePath:imagePath
                                             pixelFormat:pixelFormat
                                               doMipMaps:doMipMaps];
    }
    
    return newTexture;
}


- (LLTexture2D *)loadImageAtPath:(NSString *)imagePath
                  requestMipMaps:(BOOL)doMipMaps
                     pixelFormat:(LLTexture2DFormat)pixelFormat
{
    __block LLTexture2D *newTexture = nil;
    
    
    // do in background with lock to not broke Unity context and use our own
    [(id) ^
     {
         newTexture = [self createTextureWithImagePath:imagePath
                                        requestMipMaps:doMipMaps
                                           pixelFormat:pixelFormat];
     }
     performTextureOnThread:_backgroundThread withObject:nil waitUntilDone:YES];
    
    
    if(newTexture != nil)
    {
        [_retainedTextures setObject:newTexture
                              forKey:[NSValue valueWithPointer:newTexture.name]];
    }
    
    return newTexture;
}


- (NSString *)loadImageAsyncAtPath:(NSString *)imagePath
                    requestMipMaps:(BOOL)doMipMaps
                       pixelFormat:(LLTexture2DFormat)pixelFormat
{
    NSString *loadKey = [NSUUID UUID].UUIDString;
    
    
    [(id) ^
     {
         LLTexture2D *newTexture = [self createTextureWithImagePath:imagePath
                                                     requestMipMaps:doMipMaps
                                                        pixelFormat:pixelFormat];
         
         [(id) ^
          {
              if(newTexture != nil)
              {
                  [_retainedTextures setObject:newTexture
                                        forKey:[NSValue valueWithPointer:newTexture.name]];
              }
              
              if (_textureHelperCallback != NULL)
              {
                  _textureHelperCallback(LLUnityStringFromNSString([[LLTHLoadResult alloc] initWithTexture2D:newTexture loadKey:loadKey].JSONDescription.JSONString));
              }
          }
          performTextureOnMainThreadWithObject:nil waitUntilDone:NO];
     }
     performTextureOnThread:_backgroundThread withObject:nil waitUntilDone:NO];
    
    return loadKey;
}


- (void)releaseTextureByName:(LLUnityIntPtr)textureName
{
    if(textureName != NULL)
    {
        [_retainedTextures removeObjectForKey:[NSValue valueWithPointer:textureName]];
    }
}



#pragma mark - Static Interface

+ (LLTexture2D *)loadImageAtPath:(NSString *)imagePath
                  requestMipMaps:(BOOL)doMipMaps
                     pixelFormat:(LLTexture2DFormat)pixelFormat
{
    return [[self sharedInstance] loadImageAtPath:imagePath
                                   requestMipMaps:doMipMaps
                                      pixelFormat:pixelFormat];
}


+ (NSString *)loadImageAsyncAtPath:(NSString *)imagePath
                    requestMipMaps:(BOOL)doMipMaps
                       pixelFormat:(LLTexture2DFormat)pixelFormat
{
    return [[self sharedInstance] loadImageAsyncAtPath:imagePath
                                        requestMipMaps:doMipMaps
                                           pixelFormat:pixelFormat];
}


+ (void)releaseTextureByName:(LLUnityIntPtr)textureName
{
    [[self sharedInstance] releaseTextureByName:textureName];
}


@end





LLTextureHelperLoadResult LLTextureHelperLoadImageAtPath(LLUnityString imagePath, LLUnityInt requestMipMaps, LLUnityInt format)
{
    LLTexture2D *newTexture = [LLTextureHelper loadImageAtPath:[NSString stringWithUnityString:imagePath]
                                                requestMipMaps:requestMipMaps != 0
                                                   pixelFormat:(LLTexture2DFormat)format];
    
    LLTextureHelperLoadResult result;
    result.texturePtr = newTexture.name;
    result.width = (int)newTexture.width;
    result.height = (int)newTexture.height;
    return result;
}


LLUnityString LLTextureHelperLoadImageAtPathAsync(LLUnityString imagePath, LLUnityInt requestMipMaps, LLUnityInt format)
{
    return LLUnityStringFromNSString([LLTextureHelper loadImageAsyncAtPath:[NSString stringWithUnityString:imagePath]
                                                            requestMipMaps:requestMipMaps != 0
                                                               pixelFormat:(LLTexture2DFormat)format]);
}


void LLTextureHelperReleaseTexture(LLUnityIntPtr textureName)
{
    [LLTextureHelper releaseTextureByName:textureName];
}


void LLTextureHelperRegisterCallback(LLLibSetCallbackString unityCallback)
{
    _textureHelperCallback = unityCallback;
}

