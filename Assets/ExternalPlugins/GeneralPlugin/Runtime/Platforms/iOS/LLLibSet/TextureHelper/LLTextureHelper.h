//
//  LLTextureHelper.h
//  LLLibSet
//

#import "LLLibSet.h"
#import "LLLibSetExtentions.h"
#import "LLTexture2D.h"

static LLLibSetCallbackString _textureHelperCallback;

@interface LLTextureHelper : NSObject
{

}

@end


FOUNDATION_EXPORT LLTextureHelperLoadResult LLTextureHelperLoadImageAtPath(LLUnityString imagePath, LLUnityInt requestMipMaps, LLUnityInt format);
FOUNDATION_EXPORT LLUnityString LLTextureHelperLoadImageAtPathAsync(LLUnityString imagePath, LLUnityInt requestMipMaps, LLUnityInt format);

FOUNDATION_EXPORT void LLTextureHelperReleaseTexture(LLUnityIntPtr textureName);

FOUNDATION_EXTERN void LLTextureHelperRegisterCallback(LLLibSetCallbackString unityCallback);


