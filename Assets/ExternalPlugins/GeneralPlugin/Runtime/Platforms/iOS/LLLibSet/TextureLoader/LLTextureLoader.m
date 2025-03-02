
//  LLTextureLoader.m
//  LLLibSet
//

#import "LLTextureLoader.h"
#import "LLTexture.h"
#import <OpenGLES/ES2/gl.h>
#ifdef METAL_ENABLE
#import <MetalKit/MetalKit.h>
#endif


FOUNDATION_EXTERN MTLDeviceObject       UnityGetMetalDevice();


#define INT_PTR_ZERO (LLUnityIntPtr)0
NSString * const STREAMING_ASSETS_FOLDER = @"StreamingAssets/";
NSString * const BUNDLE_ASSETS_FOLDER = @"Data/Raw";
NSString * const TK2D_PLATFORM_POSTFIX = @"@";
NSString * const TK2D_PLATFORM_1X_POSTFIX = @"@1x";
NSString * const TK2D_PLATFORM_2X_POSTFIX = @"@2x";
NSString * const TK2D_PLATFORM_4X_POSTFIX = @"@4x";
NSString * const STRING_EMPTY = @"";


FOUNDATION_EXTERN MTLDeviceObject          LLTextureGetMetalDevice()
{
    return UnityGetMetalDevice();
}


@implementation LLTextureLoader
{
    NSMutableDictionary     *_retainedTextures;
    NSMutableSet            *_loadingTextures;
    
    LLLibSetCallbackIntPtr  _callbackLoad;
    LLLibSetCallbackIntPtr  _callbackFail;

    LLTextureInfo           _result;
}


+ (instancetype)sharedInstance
{
    static LLTextureLoader *__sharedInstance = nil;
    if (__sharedInstance == nil)
    {
        __sharedInstance = [[LLTextureLoader alloc] init];
    }
    
    return __sharedInstance;
}


- (instancetype)init
{
    self = [super init];
    if (self)
    {
        _retainedTextures = [[NSMutableDictionary alloc] init];
        _loadingTextures = [[NSMutableSet alloc] init];

#ifdef METAL_ENABLE
        MTLDeviceObject metalDevice = UnityGetMetalDevice();
        if (metalDevice == nil)
        {
            metalDevice = MTLCreateSystemDefaultDevice();;
        }
#else
        [NSException raise:@"LLTextureLoader" format:@"-METAL not supported"];
#endif
    }
    
    return self;
}


- (void)setCallbackLoad:(LLLibSetCallbackIntPtr)callbackLoad
           callbackFail:(LLLibSetCallbackIntPtr)callbackFail
{
    _callbackLoad = callbackLoad;
    _callbackFail = callbackFail;
}


- (UIImage *)imageAtPath:(NSString *)imagePath
{
    UIImage *curImage = nil;
    
    NSRange rangeStreamingFolder = [imagePath rangeOfString:STREAMING_ASSETS_FOLDER];
    if (rangeStreamingFolder.location != NSNotFound)
    {
        imagePath = [imagePath substringFromIndex:rangeStreamingFolder.location + STREAMING_ASSETS_FOLDER.length];
        imagePath = [NSString stringWithFormat:@"%@/%@/%@", [NSBundle mainBundle].resourcePath, BUNDLE_ASSETS_FOLDER, imagePath];
    }
    
    if ([[NSFileManager defaultManager] fileExistsAtPath:imagePath])
    {
        curImage = [UIImage imageWithContentsOfFile:imagePath];
    }
    else
    {
        NSString *pathWithoutExtension = [imagePath stringByDeletingPathExtension];
        NSString *name = [pathWithoutExtension lastPathComponent];
        
        if ([name rangeOfString:TK2D_PLATFORM_POSTFIX].location != NSNotFound)
        {
            if ([name rangeOfString:TK2D_PLATFORM_2X_POSTFIX].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:TK2D_PLATFORM_2X_POSTFIX withString:STRING_EMPTY];
            }
            else if ([name rangeOfString:TK2D_PLATFORM_4X_POSTFIX].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:TK2D_PLATFORM_4X_POSTFIX withString:STRING_EMPTY];
            }
            else if ([name rangeOfString:TK2D_PLATFORM_1X_POSTFIX].location != NSNotFound)
            {
                name = [name stringByReplacingOccurrencesOfString:TK2D_PLATFORM_1X_POSTFIX withString:STRING_EMPTY];
            }
            else
            {
                name = [name stringByReplacingOccurrencesOfString:TK2D_PLATFORM_POSTFIX withString:STRING_EMPTY];
            }
        }
        
        curImage = [UIImage imageNamed:name];
    }
    
    return curImage;
}


- (LLTextureInfo)textureInfoFromImage:(NSString *)imagePath
{
    _result.exist = false;
    _result.width = 0;
    _result.height = 0;
    UIImage *image = [self imageAtPath:imagePath];
    if (image != nil)
    {
        _result.exist = true;
        _result.width = image.size.width * image.scale;
        _result.height = image.size.height * image.scale;
    }
    return _result;
}


- (LLUnityIntPtr)loadTextureFromImageAtPath:(NSString *)imagePath
                                 hasMipMaps:(BOOL)mipMaps
{
    return [self loadTextureFromImage:[self imageAtPath:imagePath] hasMipMaps:mipMaps];
}


- (LLUnityIntPtr)loadTextureFromImage:(UIImage *)image
                           hasMipMaps:(BOOL)mipMaps
{
    LLUnityIntPtr result = INT_PTR_ZERO;
    
    if (image != nil)
    {
        LLTexture *newTexture = [LLTexture createTextureFromImage:image hasMipMaps:mipMaps];
        
        if (newTexture == nil)
        {
            NSLog(@"!!!!!!LLTexture Instance Error");
        }
        else
        {
            result = newTexture.name;
//            NSLog(@"LLTextureLoader load == %lu", (unsigned long)newTexture.name);
            [_retainedTextures setObject:newTexture
                                  forKey:[NSValue valueWithPointer:newTexture.name]];
        }
        
    }
    
    return result;
}


- (LLUnityIntPtr)loadTextureAsyncFromImageAtPath:(NSString *)imagePath
                      hasMipMaps:(BOOL)mipMaps
{
    return [self loadTextureAsyncFromImage:[self imageAtPath:imagePath] hasMipMaps:mipMaps];
}


- (LLUnityIntPtr)loadTextureAsyncFromImage:(UIImage *)image
                  hasMipMaps:(BOOL)mipMaps
{
    LLUnityIntPtr result = INT_PTR_ZERO;
    
    if (image != nil)
    {
        
        LLTexture *newTexture = [LLTexture createTextureFromImage:image hasMipMaps:mipMaps callback:^(LLTexture *newTest, LLUnityIntPtr oldName) {
            if ([_loadingTextures containsObject:newTest])
            {
                [_loadingTextures removeObject:newTest];
            }
            
            if (newTest == nil)
            {
                NSLog(@"!!!!!!LLTexture Instance Error");
                
                if (_callbackFail != NULL)
                {
                    _callbackFail(oldName, INT_PTR_ZERO);
                }
            }
            else
            {
                if ([_retainedTextures objectForKey:[NSValue valueWithPointer:oldName]] != nil)
                {
                    [_retainedTextures removeObjectForKey:[NSValue valueWithPointer:oldName]];
                    [_retainedTextures setObject:newTest
                                          forKey:[NSValue valueWithPointer:newTest.name]];
                }
//                NSLog(@"LLTextureLoader update old == %lu", (unsigned long)oldName);
//                NSLog(@"LLTextureLoader update new == %lu", (unsigned long)newTest.name);
                
                if (_callbackLoad != NULL)
                {
                    _callbackLoad(oldName, newTest.name);
                }
            }
            
        }];
        
        if (newTexture == nil)
        {
            NSLog(@"!!!!!!LLTexture Instance Error");
        }
        else
        {
            result = newTexture.name;
//            NSLog(@"LLTextureLoader load == %lu", (unsigned long)result);
            [_retainedTextures setObject:newTexture
                                  forKey:[NSValue valueWithPointer:newTexture.name]];
            [_loadingTextures addObject:newTexture];
        }        
    }
    
    return result;
}


- (void)releaseTextureByName:(LLUnityIntPtr)textureName
{
    if (textureName != NULL)
    {
//        NSLog(@"LLTextureLoader unload == %lu", (unsigned long)textureName);
        [_retainedTextures removeObjectForKey:[NSValue valueWithPointer:textureName]];
    }
}


@end



void LLTextureLoaderInit(LLLibSetCallbackIntPtr callbackLoad, LLLibSetCallbackIntPtr callbackFail)
{
    [[LLTextureLoader sharedInstance] setCallbackLoad:callbackLoad callbackFail:callbackFail];
}


LLTextureInfo LLTextureLoaderGetInfo(LLUnityString imagePath)
{
    return [[LLTextureLoader sharedInstance] textureInfoFromImage:LLNSStringFromUnityString(imagePath)];
}


LLUnityIntPtr LLTextureLoaderLoadTexture(LLUnityString imagePath, bool mipMaps)
{
    return [[LLTextureLoader sharedInstance] loadTextureFromImageAtPath:LLNSStringFromUnityString(imagePath) hasMipMaps:mipMaps];
}


LLUnityIntPtr LLTextureLoaderLoadTextureAsync(LLUnityString imagePath, bool mipMaps)
{
    return [[LLTextureLoader sharedInstance] loadTextureAsyncFromImageAtPath:LLNSStringFromUnityString(imagePath) hasMipMaps:mipMaps];
}


void LLTextureLoaderReleaseTexture(LLUnityIntPtr textureName)
{
    [[LLTextureLoader sharedInstance] releaseTextureByName:textureName];
}
