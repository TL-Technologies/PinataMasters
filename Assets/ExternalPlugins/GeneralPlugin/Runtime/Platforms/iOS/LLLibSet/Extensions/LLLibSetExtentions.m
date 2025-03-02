//
//  LLLibSetExtentions.m
//  LLLibSet
//

#import "LLLibSetExtentions.h"

LLUnityString BundleVersionForType(NSString *versionType)
{
    NSString *bundleVersionStrng = [[[NSBundle mainBundle] infoDictionary] objectForKey:versionType];
    
    return LLUnityStringFromNSString(bundleVersionStrng);
}


LLUnityString LLBundleName()
{
    return BundleVersionForType(@"CFBundleName");
}


LLUnityString LLBundleDisplayName()
{
    return BundleVersionForType(@"CFBundleDisplayName");
}


LLUnityString LLBundleVersion()
{
    return BundleVersionForType(@"CFBundleVersion");
}


LLUnityString LLBundleShortVersionString()
{
    return BundleVersionForType(@"CFBundleShortVersionString");
}
