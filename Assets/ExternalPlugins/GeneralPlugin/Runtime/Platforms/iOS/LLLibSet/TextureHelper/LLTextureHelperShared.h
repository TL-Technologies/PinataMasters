//
//  LLTextureHelperShared.h
//  LLLibSet
//

#ifndef LLLibSet_LLTextureHelperShared_h
#define LLLibSet_LLTextureHelperShared_h



#define IntPtr LLUnityIntPtr
#define readonly
#define string const char *

typedef enum Format LLTexture2DFormat;
typedef struct LoadResult LLTextureHelperLoadResult;

enum Format
{
    Format_RGBA32 		= 0,
    
    Format_RGB16		= 8,
    
    Format_A8			= 16,
};

struct LoadResult
{
    IntPtr	texturePtr;
    int		width;
    int 		height;
};

#undef IntPtr
#undef readonly
#undef string



#endif

