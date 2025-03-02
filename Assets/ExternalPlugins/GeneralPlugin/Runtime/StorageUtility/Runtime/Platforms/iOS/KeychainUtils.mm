//
//  KeychainUtils.m
//  Unity-iPhone
//
//  Created by valery.p on 02.09.2020.
//

#import "KeychainUtils.h"

static KeychainUtils *_sharedInstance = [KeychainUtils sharedInstance];


@interface KeychainUtils ()
{
    NSString* _accessGroup;
}

@end

@implementation KeychainUtils


+ (instancetype)sharedInstance
{
    if (_sharedInstance == nil)
    {
        _sharedInstance = [[KeychainUtils alloc] init];
    }
    
    return _sharedInstance;
}


- (id)init
{
    return self;
}


- (NSString *) accessGroup
{
    return _accessGroup;
}


- (void)setAccessGroup:(NSString *)accessGroup
{
    _accessGroup = accessGroup;
}


- (NSString *)bundleSeedID
{
    NSDictionary *query = [NSDictionary dictionaryWithObjectsAndKeys:
                           (__bridge NSString *)kSecClassGenericPassword, (__bridge NSString *)kSecClass,
                           @"bundleSeedID", kSecAttrAccount,
                           @"", kSecAttrService,
                           (id)kCFBooleanTrue, kSecReturnAttributes,
                           nil];
    
    CFDictionaryRef result = nil;
    OSStatus status = SecItemCopyMatching((__bridge CFDictionaryRef)query, (CFTypeRef *)&result);
    if (status == errSecItemNotFound)
    {
        status = SecItemAdd((__bridge CFDictionaryRef)query, (CFTypeRef *)&result);
    }
    
    if (status != errSecSuccess)
    {
        return nil;
    }
    
    NSString *accessGroup = [(__bridge NSDictionary *)result objectForKey:(__bridge NSString *)kSecAttrAccessGroup];
    NSArray *components = [accessGroup componentsSeparatedByString:@"."];
    NSString *bundleSeedID = [[components objectEnumerator] nextObject];
    CFRelease(result);
    
    return bundleSeedID;
}


- (NSMutableDictionary *)getKeychainQuery:(NSString *)service
{
    NSMutableDictionary* result = [NSMutableDictionary dictionaryWithObjectsAndKeys:
    (__bridge id)kSecClassGenericPassword, (__bridge id)kSecClass,
    service, (__bridge id)kSecAttrService,
    service, (__bridge id)kSecAttrAccount,
    service, (__bridge id)kSecAttrSynchronizable,
    (__bridge id)kSecAttrAccessibleAfterFirstUnlock, (__bridge id)kSecAttrAccessible,
    nil];
    
    if ([self accessGroup] && ![[self accessGroup] isEqual:@""])
    {
        NSString* keychainAccessGroup = [NSString stringWithFormat: @"%@.%@", [self bundleSeedID], [self accessGroup]];
        [result setValue:keychainAccessGroup forKey:(__bridge id)kSecAttrAccessGroup];
    }
    
    return result;
}


- (void)save:(NSString *)service data:(id)data
{
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    SecItemDelete((__bridge CFDictionaryRef)keychainQuery);
    [keychainQuery setObject:[NSKeyedArchiver archivedDataWithRootObject:data] forKey:(__bridge id)kSecValueData];
    SecItemAdd((__bridge CFDictionaryRef)keychainQuery, NULL);
}


- (id)load:(NSString *)service
{
    id ret = nil;
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    [keychainQuery setObject:(id)kCFBooleanTrue forKey:(__bridge id)kSecReturnData];
    [keychainQuery setObject:(__bridge id)kSecMatchLimitOne forKey:(__bridge id)kSecMatchLimit];
    
    CFDataRef keyData = NULL;
    if (SecItemCopyMatching((__bridge CFDictionaryRef)keychainQuery, (CFTypeRef *)&keyData) == noErr)
    {
        if (@available(iOS 12, *))
        {
            NSError *err;
            NSString *stored = [NSKeyedUnarchiver unarchivedObjectOfClass:[NSString class] fromData:(__bridge NSData *)keyData error:&err];
            if (err)
            {
                NSLog(@"%@",err);
            }
            else
            {
                ret = stored;
            }
        }
        else
        {
            ret = [NSKeyedUnarchiver unarchiveObjectWithData:(__bridge NSData *)keyData];
        }
    }
    
    if (keyData)
    {
        CFRelease(keyData);
    }
    
    return ret;
}


- (void)remove:(NSString *)service
{
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    SecItemDelete((__bridge CFDictionaryRef)keychainQuery);
}

@end



#pragma mark - Unity Extern

void KeychainUtilsInitialize(LLUnityString accessGroup)
{
    [[KeychainUtils sharedInstance] setAccessGroup:LLNSStringFromUnityString(accessGroup)];
}


LLUnityString KeychainUtilsLoadByKey(LLUnityString key)
{
    NSString* keyString = LLNSStringFromUnityString(key);
    NSString* loadedString = [[KeychainUtils sharedInstance] load:keyString];
    return LLUnityStringFromNSString(loadedString);
}


void KeychainUtilsSaveForKey(LLUnityString key, LLUnityString value)
{
    NSString* keyString = LLNSStringFromUnityString(key);
    NSString* valueString = LLNSStringFromUnityString(value);
    [[KeychainUtils sharedInstance] save:keyString data:valueString];
}


void KeychainUtilsRemoveForKey(LLUnityString key)
{
    NSString* keyString = LLNSStringFromUnityString(key);
    [[KeychainUtils sharedInstance] remove:keyString];
}
