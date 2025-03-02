//
//  LLSystemSharing.m
//  LLLibSet
//

#import "LLSystemSharing.h"


NSString * const LLStreamingAssetsFolderPathComponent = @"Data/Raw";


@implementation LLSystemSharing


+ (void)shareText:(NSString *)text withImage:(UIImage *)image andURL:(NSURL *)url
{
    NSMutableArray *items = [NSMutableArray array];

    if (text != nil && ![text isEqualToString:@""])
    {
        [items addObject:text];
    }

    if (image != nil)
    {
        [items addObject:image];
    }
    
    if (url != nil)
    {
        [items addObject:url];
    }

    
    UIActivityViewController *controller = [[UIActivityViewController alloc] initWithActivityItems:items
                                                                             applicationActivities:nil];
    
    if ([UIPopoverPresentationController class] != nil)
    {
        UIPopoverPresentationController *popover = controller.popoverPresentationController;
        
        if (popover)
        {
            popover.sourceView = [UIApplication sharedApplication].keyWindow.rootViewController.view;
            popover.permittedArrowDirections = UIPopoverArrowDirectionAny;
        }
    }
    
    [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:controller
                                                                                 animated:YES
                                                                               completion:nil];
}


+ (void)shareText:(NSString *)text withImagePath:(NSString *)imagePath andURLString:(NSString *)urlString
{
    NSString *streamingAssetsFolderPath = [[[NSBundle mainBundle] resourcePath] stringByAppendingPathComponent:LLStreamingAssetsFolderPathComponent];
    imagePath = [streamingAssetsFolderPath stringByAppendingPathComponent:imagePath];
    
    [self shareText:text
          withImage:[UIImage imageWithContentsOfFile:imagePath]
             andURL:[NSURL URLWithString:urlString]];
}


+ (void)shareText:(NSString *)text
    withImageData:(NSData *)imageData
     andURLString:(NSString *)urlString
{
    [self shareText:text
          withImage:[UIImage imageWithData:imageData]
             andURL:[NSURL URLWithString:urlString]];
}



@end



FOUNDATION_EXTERN void LLSystemSharingShareItems(LLUnityString text, LLUnityString imagePath, LLUnityString urlString)
{
    [LLSystemSharing shareText:[NSString stringWithUnityString:text]
                 withImagePath:[NSString stringWithUnityString:imagePath]
                  andURLString:[NSString stringWithUnityString:urlString]];
}


FOUNDATION_EXTERN void LLSystemSharingShareItemsWithData(LLUnityString text, Byte *bytes, LLUnityInt width, LLUnityInt height, LLUnityString urlString)
{
    NSInteger imageDataLength = width * height * sizeof(Byte);
    NSData *imageData = [[NSData alloc] initWithBytes:(void *)bytes
                                               length:imageDataLength];
    
    [LLSystemSharing shareText:[NSString stringWithUnityString:text]
                 withImageData:imageData
                  andURLString:[NSString stringWithUnityString:urlString]];
}

