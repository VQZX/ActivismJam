

#import <AdSupport/ASIdentifierManager.h>
#import "PlatformHelper.h"


#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

static NSString *ToNSString(const char *str)
{
    return (str != NULL) ? [NSString stringWithUTF8String:str] : [NSString stringWithUTF8String:""];
}




void _initPlatformHelper()
{
    [[PlatformHelper sharedHelper] initialize];
}


const char* _getLocale()
{
    NSString* localeStr = [[NSLocale currentLocale] objectForKey:NSLocaleIdentifier];
    return MakeStringCopy( localeStr );
}


const char* _getCountryCode()
{
    NSString* countryCodeStr = [[NSLocale currentLocale] objectForKey:NSLocaleCountryCode];
    return MakeStringCopy( countryCodeStr );
}


const char* _getBuildNumber()
{
    NSString* versionCodeStr = [[NSBundle mainBundle] objectForInfoDictionaryKey: @"CFBundleVersion"];
    return MakeStringCopy( versionCodeStr );
}


const char* _getOSVersion()
{
    NSString* osVerStr = [[UIDevice currentDevice] systemVersion];
    return MakeStringCopy( osVerStr );
}


int _getFreeDiskSpace()
{
    int returnValue = 0;

    uint64_t totalSpace = 0;
    uint64_t totalFreeSpace = 0;
    int totalFreeSpaceMegabytes = 0;

    NSError* error = nil;
    NSArray* paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSDictionary* attributes = [[NSFileManager defaultManager] attributesOfFileSystemForPath:[paths lastObject] error:&error];

    if (attributes != nil)
    {
        NSNumber* fileSystemSizeInBytes = [attributes objectForKey:NSFileSystemSize];
        NSNumber* freeFileSystemSizeInBytes = [attributes objectForKey:NSFileSystemFreeSize];
        totalSpace = [fileSystemSizeInBytes unsignedLongLongValue];
        totalFreeSpace = [freeFileSystemSizeInBytes unsignedLongLongValue];
        totalFreeSpaceMegabytes = ( ( totalFreeSpace / 1024LL ) / 1024LL );
        returnValue = totalFreeSpaceMegabytes;
        NSLog(@"[PlatformHelperBridge] _getFreeDiskSpace: %d MB / %llu MB", totalFreeSpaceMegabytes, ((totalSpace / 1024LL) / 1024LL));
    }
    else
    {
        NSLog(@"[PlatformHelperBridge] _getFreeDiskSpace error: %@", [error description]);
    }
    return returnValue;
}


BOOL _getIsMusicPlaying()
{
    return [[PlatformHelper sharedHelper] getIsMusicPlaying];
}


BOOL _getAreHeadphonesConnected()
{
    return [[PlatformHelper sharedHelper] getAreHeadphonesConnected];
}


void _forceExternalSpeaker()
{
    [[PlatformHelper sharedHelper] forceExternalSpeaker];
}


void _showSystemAlert(const char *title, const char *desc, const char *oktext)
{
    UIAlertView *alert = [[UIAlertView alloc] initWithTitle: [NSString stringWithUTF8String:title]
                                                message: [NSString stringWithUTF8String:desc]
                                                delegate: nil
                                                cancelButtonTitle:[NSString stringWithUTF8String:oktext]
                                                otherButtonTitles:nil];

    [alert show];
    [alert release];
}


BOOL _isAppInstalled(const char *appurl)
{
    NSString *urlString = [NSString stringWithUTF8String:appurl];
    NSURL *urlURL = [NSURL URLWithString:urlString];
    BOOL canOpen = [[UIApplication sharedApplication] canOpenURL:urlURL];
    return canOpen;
}


void _openURLWithMethod(const char *urlText, int method)
{
    [[PlatformHelper sharedHelper] openURLWithMethod:ToNSString(urlText) openMethod:method];
}


void _openURLWithMethodConfirm(const char *titleText, const char *messageText, const char *cancelText, const char *confirmText, const char *urlText, int method)
{
    [[PlatformHelper sharedHelper] openURLWithMethodConfirm:ToNSString(titleText)
                                        message:ToNSString(messageText)
                                        cancel:ToNSString(cancelText)
                                        confirm:ToNSString(confirmText)
                                        url:ToNSString(urlText)
                                        openMethod:method];
}


float _getDeviceVolume()
{
    return [[PlatformHelper sharedHelper] getDeviceVolume];
}


void _setClipboardString(const char *textToCopy)
{
    [[UIPasteboard generalPasteboard] setString:[NSString stringWithUTF8String:textToCopy]];
}


const char* _getClipboardString()
{
    return MakeStringCopy([[UIPasteboard generalPasteboard] string]);
}


bool _checkPermission(int permission)
{
    return [[PlatformHelper sharedHelper] checkPermission:permission];
}


bool _isInGuidedAccessMode()
{
    return [[PlatformHelper sharedHelper] isInGuidedAccessMode];
}


void _requestPermission(int permission, const char* objName, const char* funcName)
{
	[[PlatformHelper sharedHelper] requestPermission:permission :ToNSString(objName) :ToNSString(funcName)];
}


void _openAppSettings()
{
    if(&UIApplicationOpenSettingsURLString != NULL)
    {
        NSURL *url = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
        [[UIApplication sharedApplication] openURL:url];
    }
}

