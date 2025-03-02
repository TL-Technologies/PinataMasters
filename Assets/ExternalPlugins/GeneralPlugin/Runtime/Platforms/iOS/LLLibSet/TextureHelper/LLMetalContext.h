//
//  LLMetalContext.h
//  LLLibSet
//

#import <Metal/Metal.h>


@interface LLMetalContext : NSObject

- (instancetype) initWithDevice:(id <MTLDevice>)device;


- (id <MTLTexture>)newTextureWithDescriptor:(MTLTextureDescriptor *)descriptor;
- (void)generateMipmapsForTexture:(id <MTLTexture>)texture;

@end
