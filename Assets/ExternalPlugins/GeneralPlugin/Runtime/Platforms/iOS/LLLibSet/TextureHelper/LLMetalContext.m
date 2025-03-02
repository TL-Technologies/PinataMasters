//
//  LLMetalContext.m
//  LLLibSet
//

#import "LLMetalContext.h"

@implementation LLMetalContext
{
    id <MTLDevice>          _mtlDevice;
    id <MTLCommandQueue>    _mtlCommandQueue;
}


- (instancetype)initWithDevice:(id<MTLDevice>)device
{
    self = [super init];
    if (self)
    {
        _mtlDevice = device;
        _mtlCommandQueue = [device newCommandQueue];
    }

    return self;
}



- (id <MTLTexture>) newTextureWithDescriptor:(MTLTextureDescriptor *)descriptor
{
    return [_mtlDevice newTextureWithDescriptor:descriptor];
}


- (void) generateMipmapsForTexture:(id<MTLTexture>)texture
{
    id <MTLCommandBuffer> curBuffer = [_mtlCommandQueue commandBuffer];
    id <MTLBlitCommandEncoder> curEncoder = [curBuffer blitCommandEncoder];

    [curEncoder generateMipmapsForTexture:texture];
    [curEncoder endEncoding];

    [curBuffer commit];
    [curBuffer waitUntilCompleted];
}


@end
