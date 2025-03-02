//
//  LLSystemPopUp.m
//  LLLibSet
//

#import "LLSystemPopUp.h"
#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();

@interface LLSystemPopUp () <UIAlertViewDelegate>
{
    NSMutableDictionary *_sendersPopUp;
    
    UIAlertController *_alertControllerWithoutButtons;
}

@end


static LLSystemPopUp *_sharedInstance = nil;


@implementation LLSystemPopUp

+ (instancetype)sharedInstance
{
    if (_sharedInstance == nil)
    {
        _sharedInstance = [[LLSystemPopUp alloc] init];
    }
    
    return _sharedInstance;
}


- (instancetype)init
{
    self = [super init];
    
    if (self != nil)
    {
        _sendersPopUp = [[NSMutableDictionary alloc] init];
    }
    
    return self;
}


- (void)showAlertControllerWithoutButtonsWithTitle:(NSString *)title
                                       withMessage:(NSString *)message
{
    if (_alertControllerWithoutButtons == nil)
    {
        _alertControllerWithoutButtons = [UIAlertController alertControllerWithTitle:title
                                                                             message:message
                                                                      preferredStyle:UIAlertControllerStyleAlert];
        
        UIViewController *unityViewController = UnityGetGLViewController();
        if (unityViewController.presentedViewController != nil)
        {
            [unityViewController dismissViewControllerAnimated:NO
                                                    completion:^
             {
                 [unityViewController presentViewController:_alertControllerWithoutButtons
                                                   animated:YES
                                                 completion:nil];
             }];
        }
        else
        {
            [unityViewController presentViewController:_alertControllerWithoutButtons
                                              animated:YES
                                            completion:nil];
        }
    }
}


- (void)hideAlertControllerWithoutButtons
{
    if (_alertControllerWithoutButtons != nil)
    {
        [_alertControllerWithoutButtons dismissViewControllerAnimated:YES
                                                           completion:nil];
        _alertControllerWithoutButtons = nil;
    }
}


- (void)showAlertControllerWithTitle:(NSString *)title withMessage:(NSString *)message withButton:(NSString *)button andCallback:(NSString *)callbackName
{
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:title message:message preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction *action = [UIAlertAction actionWithTitle:button style:UIAlertActionStyleDefault handler:^(UIAlertAction * action)
                             {
                                 [self performSelector:@selector(invokeCallback:) withObject:action];
                             }];
    
    [alertController addAction:action];
    
    UIPopoverPresentationController *popover = alertController.popoverPresentationController;
    
    if (popover)
    {
        popover.sourceView = [UIApplication sharedApplication].keyWindow.rootViewController.view;
        popover.permittedArrowDirections = UIPopoverArrowDirectionUp;
    }
    
    [UnityGetGLViewController() presentViewController:alertController
                                             animated:YES
                                           completion:nil];
    
    if (callbackName != nil)
    {
        [_sendersPopUp setObject:action forKey:callbackName];
    }
}


- (void)showAlertControllerWithTitle:(NSString *)title withMessage:(NSString *)message firstButtonText:(NSString *)fbText secondButtonText:(NSString *)sbText firstButtonCallback:(NSString *)fbCallback secondButtonCallback:(NSString *)sbCallBack isVerticalLayout:(BOOL)isVertical isSecondButtonBold:(BOOL)isBoldButton
{
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:title message:message preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction *firstButtonAction    = [UIAlertAction actionWithTitle:fbText style:UIAlertActionStyleDefault handler:^(UIAlertAction *action)
                                           {
                                               [self performSelector:@selector(invokeCallback:) withObject:action];
                                           }];
    
    UIAlertAction *secondButtonAction   = [UIAlertAction actionWithTitle:sbText style:UIAlertActionStyleDefault handler:^(UIAlertAction *action)
                                           {
                                               [self performSelector:@selector(invokeCallback:) withObject:action];
                                           }];

    [alertController addAction:firstButtonAction];
    [alertController addAction:secondButtonAction];

    if (isBoldButton)
    {
        alertController.preferredAction = secondButtonAction;
    }

    UIPopoverPresentationController *popover = alertController.popoverPresentationController;

    if (popover)
    {
        popover.sourceView = [UIApplication sharedApplication].keyWindow.rootViewController.view;
        popover.permittedArrowDirections = isVertical ? UIPopoverArrowDirectionUp : UIPopoverArrowDirectionLeft;
    }
    
    [UnityGetGLViewController() presentViewController:alertController
                                             animated:YES
                                           completion:nil];
    
    if (fbCallback != nil)
    {
        [_sendersPopUp setObject:firstButtonAction forKey:fbCallback];
    }
    
    if (sbCallBack != nil)
    {
        [_sendersPopUp setObject:secondButtonAction forKey:sbCallBack];
    }
}


- (void)invokeCallback:(UIAlertAction *)action
{
    __block NSString *keyInSet = nil;
    
    [_sendersPopUp enumerateKeysAndObjectsUsingBlock:^(NSString* key, UIAlertAction* obj, BOOL* stop)
     {
         if (obj == action)
         {
             keyInSet = key;
         }
     }];
    
    if (keyInSet != nil)
    {
        if (_systemPopUpDelegate != nil)
        {
            _systemPopUpDelegate(LLUnityStringFromNSString(keyInSet));
        }
        [_sendersPopUp removeObjectForKey:keyInSet];
    }
}

@end

#pragma mark - C interface



void LLSystemPopUpWithoutButtonsShow(LLUnityString title, LLUnityString message)
{
    [[LLSystemPopUp sharedInstance] showAlertControllerWithoutButtonsWithTitle:LLNSStringFromUnityString(title)
                                                                   withMessage:LLNSStringFromUnityString(message)];
}


void LLSystemPopUpWithoutButtonsHide()
{
    [[LLSystemPopUp sharedInstance] hideAlertControllerWithoutButtons];
}


void LLSystemPopUpShow(LLUnityString title, LLUnityString message, LLUnityString button, LLUnityString callbackName)
{
    [[LLSystemPopUp sharedInstance] showAlertControllerWithTitle:LLNSStringFromUnityString(title)
                                                     withMessage:LLNSStringFromUnityString(message)
                                                      withButton:LLNSStringFromUnityString(button)
                                                     andCallback:LLNSStringFromUnityString(callbackName)];
}


void LLSystemPopUpWithTwoButtonsShow(LLUnityString title, LLUnityString message, LLUnityString firstButtonText, LLUnityString secondButtonText, LLUnityString firstButtonCallbackName, LLUnityString secondButtonCallbackName, bool isVerticalLayout)
{
    [[LLSystemPopUp sharedInstance] showAlertControllerWithTitle:LLNSStringFromUnityString(title)
                                                     withMessage:LLNSStringFromUnityString(message)
                                                 firstButtonText:LLNSStringFromUnityString(firstButtonText)
                                                secondButtonText:LLNSStringFromUnityString(secondButtonText)
                                             firstButtonCallback:LLNSStringFromUnityString(firstButtonCallbackName)
                                            secondButtonCallback:LLNSStringFromUnityString(secondButtonCallbackName)
                                                isVerticalLayout:isVerticalLayout
                                              isSecondButtonBold:true];
}


void LLSystemPopUpWithTwoButtons(LLUnityString title, LLUnityString message, LLUnityString firstButtonText, LLUnityString secondButtonText, LLUnityString firstButtonCallbackName, LLUnityString secondButtonCallbackName, int isVerticalLayout, int isSecondButtonBold)
{
    [[LLSystemPopUp sharedInstance] showAlertControllerWithTitle:LLNSStringFromUnityString(title)
                                                     withMessage:LLNSStringFromUnityString(message)
                                                 firstButtonText:LLNSStringFromUnityString(firstButtonText)
                                                secondButtonText:LLNSStringFromUnityString(secondButtonText)
                                             firstButtonCallback:LLNSStringFromUnityString(firstButtonCallbackName)
                                            secondButtonCallback:LLNSStringFromUnityString(secondButtonCallbackName)
                                                isVerticalLayout:(bool)isVerticalLayout
                                              isSecondButtonBold:(bool)isSecondButtonBold];
}


void LLSystemPopUpRegisterCallback(LLLibSetCallbackString unityCallback)
{
    _systemPopUpDelegate = unityCallback;
}

