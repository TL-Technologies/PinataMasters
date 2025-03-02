//
//  LLTextureLoader.h
//  LLLibSet
//

#import "LLLibSet.h"

#define METAL_ENABLE

#ifdef METAL_ENABLE
@protocol MTLDevice;
@protocol MTLTexture;
@protocol MTLCommandBuffer;
@protocol MTLCommandQueue;
@protocol MTLCommandEncoder;

typedef id<MTLDevice>           MTLDeviceObject;
typedef id<MTLTexture>          MTLTextureObject;
#else
typedef id                      MTLDeviceObject;
typedef id                      MTLTextureObject;
#endif


#ifdef __cplusplus
extern "C"
{
#endif
	typedef struct 
	{
        bool            exist;
	    int				width;
	    int 			height;
	} LLTextureInfo;

    typedef void (*LLLibSetCallbackIntPtr)(LLUnityIntPtr, LLUnityIntPtr);
#ifdef __cplusplus
}
#endif


@interface LLTextureLoader : NSObject

+ (instancetype)sharedInstance;

@end


FOUNDATION_EXPORT void LLTextureLoaderInit(LLLibSetCallbackIntPtr callbackLoad, LLLibSetCallbackIntPtr callbackFail);

FOUNDATION_EXPORT LLTextureInfo LLTextureLoaderGetInfo(LLUnityString imagePath);

FOUNDATION_EXPORT LLUnityIntPtr LLTextureLoaderLoadTexture(LLUnityString imagePath, bool mipMaps);
FOUNDATION_EXPORT LLUnityIntPtr LLTextureLoaderLoadTextureAsync(LLUnityString imagePath, bool mipMaps);

FOUNDATION_EXPORT void LLTextureLoaderReleaseTexture(LLUnityIntPtr textureName);
