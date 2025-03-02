//
//  LLApplicationWebViewController.h
//  LLLibSet
//
//  Created by Valera Shostak on 12/19/17.
//  Copyright © 2017 My Company. All rights reserved.
//

#import "LLApplicationWebViewController.h"
#import <WebKit/WebKit.h>


@interface LLApplicationWebViewController() <WKUIDelegate, WKNavigationDelegate>
{
    WKWebView *webView;
    UIViewController *webController;
    UINavigationController *navController;
}

- (void)displayInWebView:(NSString *)url withTitle:(NSString *)title;

@end


@implementation LLApplicationWebViewController

- (instancetype)init
{
    self = [super init];
    if (self != nil)
    {
        webView = [[WKWebView alloc] initWithFrame:CGRectZero];
        webView.backgroundColor = UIColor.whiteColor;
        webView.UIDelegate = self;
        webView.navigationDelegate = self;
        webController = [[UIViewController alloc] init];
        [webController navigationItem].leftBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemStop
                                                                                                         target:self
                                                                                                         action:@selector(dismissWebView)];
        [webController setView:webView];
        
        navController = [[UINavigationController alloc] initWithRootViewController:webController];
        [[navController navigationBar] setBarStyle:UIBarStyleBlack];
        [[navController navigationBar] setTintColor:UIColor.blackColor];
        [[navController navigationBar] setBackgroundImage:[self imageWithColor:UIColor.whiteColor]
                                            forBarMetrics:UIBarMetricsDefault];
        NSDictionary *navbarTitleTextAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                                   [UIFont systemFontOfSize:20],NSFontAttributeName,
                                                   [UIColor blackColor],NSForegroundColorAttributeName, nil];
        [[navController navigationBar] setTitleTextAttributes:navbarTitleTextAttributes];
    }
    return self;
}


+ (instancetype)sharedInstance
{
    static LLApplicationWebViewController *sharedInstance = nil;
    
    if (sharedInstance == nil)
    {
        sharedInstance = [[LLApplicationWebViewController alloc] init];
    }
    
    return sharedInstance;
}

/* ----- METHOD WITH CLEARING REQUEST CACHE ----- */
//NSURLRequest *lastRequest = nil;
//
//- (void)displayInWebView:(NSString *)url withTitle:(NSString *)title
//{
//    if (lastRequest != nil)
//    {
//        [[NSURLCache sharedURLCache] removeCachedResponseForRequest:lastRequest];
//        [[NSURLCache sharedURLCache] removeAllCachedResponses];
//    }
//
//    [webController setTitle:title];
//    NSURL *urlURL = [NSURL URLWithString:url];
//    lastRequest = [NSURLRequest requestWithURL:urlURL
//                                   cachePolicy:NSURLRequestReloadIgnoringCacheData
//                               timeoutInterval:10000];
//
//    [webView loadRequest:lastRequest];
//    UIViewController *viewController = [UIApplication sharedApplication].keyWindow.rootViewController;
//    [viewController presentViewController:navController animated:YES completion:nil];
//}

- (void)displayInWebView:(NSString *)url withTitle:(NSString *)title
{
    [webController setTitle:title];
    NSURL *urlURL = [NSURL URLWithString:url];
    NSURLRequest *requestObj = [NSURLRequest requestWithURL:urlURL];
    [webView loadRequest:requestObj];
    UIViewController *viewController = [UIApplication sharedApplication].keyWindow.rootViewController;
    [viewController presentViewController:navController animated:YES completion:nil];
}


- (void)dismissWebView
{
    [navController dismissViewControllerAnimated:YES completion:nil];
}


- (UIImage *)imageWithColor:(UIColor *)color
{
    CGRect rect = CGRectMake(0.0f, 0.0f, 1.0f, 1.0f);
    UIGraphicsBeginImageContext(rect.size);
    CGContextRef context = UIGraphicsGetCurrentContext();
    
    CGContextSetFillColorWithColor(context, [color CGColor]);
    CGContextFillRect(context, rect);
    
    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    return image;
}


#pragma mark - Web Kit Navigation Delegate

- (void)webView:(WKWebView *)webView
        decidePolicyForNavigationAction:(WKNavigationAction *)navigationAction
        decisionHandler:(void (^)(WKNavigationActionPolicy))decisionHandler
{
    if (navigationAction.navigationType == WKNavigationTypeLinkActivated)
    {
        decisionHandler(WKNavigationActionPolicyCancel);
        
        [[UIApplication sharedApplication] openURL:[navigationAction.request URL]];
    }
    else
    {
        decisionHandler(WKNavigationActionPolicyAllow);
    }
}


#pragma mark - Unity Extern

void LLApplicationWebViewControllerShow(LLUnityString urlString, LLUnityString titleString)
{
    [[LLApplicationWebViewController sharedInstance] displayInWebView:LLNSStringFromUnityString(urlString)
                                                            withTitle:LLNSStringFromUnityString(titleString)];
}

@end


