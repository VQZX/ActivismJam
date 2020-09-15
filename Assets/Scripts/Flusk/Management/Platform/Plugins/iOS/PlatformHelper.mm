
#import "PlatformHelper.h"
#import <UnityAppController.h>
#import <AudioToolbox/AudioToolbox.h>
#import <StoreKit/StoreKit.h>
#import <WebKit/WebKit.h>
#import <Photos/Photos.h>
#import <AssetsLibrary/AssetsLibrary.h>


@interface NativeWebViewController : UINavigationController
{
	WKWebView	*webView;
}
- (void)openView:(NSString *)urlString;
- (void)closeView;
@end

@implementation NativeWebViewController
{
}

- (id)init
{
	WKWebViewConfiguration *configuration = [[WKWebViewConfiguration alloc] init];
	webView = [[WKWebView alloc] initWithFrame:CGRectZero configuration:configuration];

	UIViewController *webViewController = [[UIViewController alloc] init];
	webViewController.view = webView;

	self = [super initWithRootViewController:webViewController];
	self.navigationBar.barStyle = UIBarStyleDefault;
	UIBarButtonItem *cancelButton = [[UIBarButtonItem alloc] initWithTitle:@"Cancel" style:UIBarButtonItemStylePlain target:self action:@selector(closeView)];
	[self.navigationBar.topItem setLeftBarButtonItem: cancelButton];

	return self;
}

- (void)openView:(NSString *)urlString
{
	NSURL *url = [[NSURL alloc] initWithString:urlString];
	NSURLRequest *request = [[NSURLRequest alloc] initWithURL:url];

    [webView loadRequest:request];
	[GetAppController().rootViewController presentViewController:self animated:YES completion:NULL];
}

- (void)closeView
{
	[GetAppController().rootViewController dismissViewControllerAnimated:YES completion:NULL];
}

- (UIInterfaceOrientation)preferredInterfaceOrientationForPresentation
{
	return [GetAppController().rootViewController preferredInterfaceOrientationForPresentation];
}

- (NSUInteger)supportedInterfaceOrientations
{
	return [GetAppController().rootViewController supportedInterfaceOrientations];
}

- (BOOL)shouldAutorotate
{
	return [GetAppController().rootViewController shouldAutorotate];
}
@end




//custom native store view controller that properly handles orientations
@interface NativeStoreViewController : SKStoreProductViewController<SKStoreProductViewControllerDelegate>
{
}
@end

@implementation NativeStoreViewController
{
}

- (void)openView:(NSString *)productID
{
	[self setDelegate:self];
	[self loadProductWithParameters:@{SKStoreProductParameterITunesItemIdentifier:productID} completionBlock:^(BOOL result, NSError *error)
	{
		if(error)
		{
			NSLog(@"Error attempting to load Store Product View for Product %@", productID);
		}
	}];

	[GetAppController().rootViewController presentViewController:self animated:YES completion:nil];
}

- (void)productViewControllerDidFinish:(SKStoreProductViewController *)viewController
{
	[viewController dismissViewControllerAnimated:YES completion:NULL];
}

- (UIInterfaceOrientation)preferredInterfaceOrientationForPresentation
{
	return [GetAppController().rootViewController preferredInterfaceOrientationForPresentation];
}

- (NSUInteger)supportedInterfaceOrientations
{
	return [GetAppController().rootViewController supportedInterfaceOrientations];
}

- (BOOL)shouldAutorotate
{
	return [GetAppController().rootViewController shouldAutorotate];
}
@end




@interface PlatformHelper()

@property (nonatomic,assign) SystemSoundID soundId;
@property (nonatomic, assign) NSString* targetURL;
@property (nonatomic, assign) int openMethod;

@end


//NOTE: These sync up 1:1 with the enumerations inside of PlatformHelper.cs, so make sure they are up to date if anything changes!
typedef NS_ENUM(NSUInteger, PermissionType)
{
	PermissionTypeReadExternalData,
	PermissionTypeWriteExternalData,
	PermissionTypeRecordAudio,
	PermissionTypeAccessCameraRoll,
	PermissionTypeNumPermissions
};


@implementation PlatformHelper

UIViewController *UnityGetGLViewController();

static PlatformHelper* sharedSingleton;



+ (PlatformHelper*)sharedHelper
{
	if(sharedSingleton == nil)
	{
		sharedSingleton = [[PlatformHelper alloc] init];
	}
	return sharedSingleton;
}

- (void)initialize
{
	NSLog(@"DON'T BE NOT INITIALIZED");

	//start off by forcing our external speaker as the output destination (wont happen if headphones are plugged in, so don't worry!)
	[self forceExternalSpeaker];

	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(_routeChangeHandler:) name:AVAudioSessionRouteChangeNotification object:[AVAudioSession sharedInstance]];
}

- (void)_routeChangeHandler:(NSNotification*)notification
{
	int reasonValue = [[notification.userInfo valueForKey:AVAudioSessionRouteChangeReasonKey] intValue];

	if (reasonValue == AVAudioSessionRouteChangeReasonNewDeviceAvailable ||
		reasonValue == AVAudioSessionRouteChangeReasonOldDeviceUnavailable)
	{
		NSString* resultStr = @"false";
		if([self getAreHeadphonesConnected] == YES)
		{
			resultStr = @"true";
		}
		else
		{
			//if headphones are not connected, make sure we have forced the external speaker as our audio output
			[self forceExternalSpeaker];
		}

		UnitySendMessage("PlatformHelper", "OnHeadphonesConnected", [resultStr UTF8String]);
	}
}

- (BOOL)getIsMusicPlaying
{
	BOOL returnValue = NO;
	MPMusicPlayerController* musicPlayer = [MPMusicPlayerController systemMusicPlayer];
	if(musicPlayer.playbackState == MPMusicPlaybackStatePlaying)
	{
		returnValue = YES;
	}
	return returnValue;
}


- (float)getDeviceVolume
{
	return [[AVAudioSession sharedInstance] outputVolume];
}

- (BOOL)getAreHeadphonesConnected
{
	BOOL headphonesLocated = NO;
	AVAudioSessionRouteDescription* route = [[AVAudioSession sharedInstance] currentRoute];
	for(AVAudioSessionPortDescription* portDescription in route.outputs)
	{
		headphonesLocated |= ( [portDescription.portType isEqualToString:AVAudioSessionPortHeadphones] );
	}
	return headphonesLocated;
}

- (void)forceExternalSpeaker
{
	if(![self getAreHeadphonesConnected])
	{
		NSError *error;
		if(![[AVAudioSession sharedInstance] overrideOutputAudioPort:AVAudioSessionPortOverrideSpeaker error:&error])
		{
			NSLog(@"----Error: AudioSession cannot use speakers: %@", [error localizedDescription]);
		}
	}
}


- (BOOL)checkPermission:(int)permission
{
	switch(permission)
	{
		case PermissionTypeReadExternalData:
		case PermissionTypeWriteExternalData:
			return YES;
		case PermissionTypeRecordAudio:
			if([[[UIDevice currentDevice] systemVersion] floatValue] >= 8.0f)
			{
				//if iOS8+, we can check easily!
				return ([[AVAudioSession sharedInstance] recordPermission] == AVAudioSessionRecordPermissionGranted) ? YES : NO;
			}
			else
			{
				//in < iOS8 we need to assume that no permission is granted, so that we can then ask for permission.  If permission HAS been granted, then the call to request should not actually prompt
				return NO;
			}

		case PermissionTypeAccessCameraRoll:
			if([[[UIDevice currentDevice] systemVersion] floatValue] >= 8.0f)
			{
				return ([PHPhotoLibrary authorizationStatus] == PHAuthorizationStatusAuthorized) ? YES : NO;
			}
			else
			{
				return ([ALAssetsLibrary authorizationStatus] == ALAuthorizationStatusAuthorized) ? YES : NO;
			}
	}

	//assume no permission if not checking against one of our supported ones
	return NO;
}


- (void)requestPermission:(int)permission :(NSString *)caller :(NSString*)method
{
	switch(permission)
	{
		case PermissionTypeReadExternalData:
		case PermissionTypeWriteExternalData:
			//do nothing, just immediately send the message as Granted as these permissions are implied on iOS always
			UnitySendMessage([caller cStringUsingEncoding:NSUTF8StringEncoding], [method cStringUsingEncoding:NSUTF8StringEncoding], "Granted");
			break;
		case PermissionTypeRecordAudio:
			[[AVAudioSession sharedInstance] requestRecordPermission:^(BOOL granted)
			{
				//on iOS, you should never ask again
				UnitySendMessage([caller cStringUsingEncoding:NSUTF8StringEncoding],
									[method cStringUsingEncoding:NSUTF8StringEncoding],
									(granted == YES) ? "Granted" : "DeniedNeverAsk");
			}];
			break;
		case PermissionTypeAccessCameraRoll:
			if([[[UIDevice currentDevice] systemVersion] floatValue] >= 8.0f)
			{
				[PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status)
				{
					//on iOS, you should never ask again
					UnitySendMessage([caller cStringUsingEncoding:NSUTF8StringEncoding],
										[method cStringUsingEncoding:NSUTF8StringEncoding],
										(status == PHAuthorizationStatusAuthorized) ? "Granted" : "DeniedNeverAsk");
				}];
			}
			else
			{
				//pre-iOS8, we cannot explicitly ask for photo library permission, so we assume it has been granted and then on 1st photo library access via code, the permission will then appear
				UnitySendMessage([caller cStringUsingEncoding:NSUTF8StringEncoding], [method cStringUsingEncoding:NSUTF8StringEncoding], "Granted");
			}
			break;
	}
}


//handles opening of the url via the specified custom method
- (void)openURLWithMethod:(NSString *)url openMethod:(int)method
{
	switch(method)
	{
		//standard openURL
		case 0:
		{
			[[UIApplication sharedApplication] openURL:[NSURL URLWithString:url]];
			break;
		}

		//open native web view
		case 1:
		{
			[[[[NativeWebViewController alloc] init] autorelease] openView:url];
			break;
		}

		//open native store view
		case 2:
		{
			[[[[NativeStoreViewController alloc] init] autorelease] openView:url];
			break;
		}
	}
}


//called if you wish to have a pre-url view dialog that confirms proceeding out of the current app
- (void)openURLWithMethodConfirm:(NSString *)titleText message:(NSString *)messageText cancel:(NSString *)cancelText confirm:(NSString *)confirmText url:(NSString *)urlText openMethod:(int)method
{
	//UIAlertView was deprecated from iOS 8+...
	if([[[UIDevice currentDevice] systemVersion] floatValue] >= 8.0f)
	{
		UIAlertController *alertCtrl = [UIAlertController alertControllerWithTitle:titleText
															message:messageText
															preferredStyle:UIAlertControllerStyleAlert];

		UIAlertAction* cancelAction = [UIAlertAction actionWithTitle:cancelText
										style:UIAlertActionStyleDefault
										handler:^(UIAlertAction * action) {}];
		[alertCtrl addAction:cancelAction];
		UIAlertAction* confirmAction = [UIAlertAction actionWithTitle:confirmText
										style:UIAlertActionStyleDefault
										handler:^(UIAlertAction * action)
										{
											[self openURLWithMethod:urlText openMethod:method];
										}];
		[alertCtrl addAction:confirmAction];

		[GetAppController().rootViewController presentViewController:alertCtrl animated:YES completion:nil];
	}
	else
	{
		_targetURL = [[NSString alloc] initWithString:urlText];
		_openMethod = method;

		// show the alert dialog (titles purposefully swapped so we can have the CONFIRM text as bold... sneaky!)
		UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:titleText
													message:messageText
													delegate:self
													cancelButtonTitle:confirmText
													otherButtonTitles:cancelText,
													nil];
		[alertView show];
		[alertView release];
	}
}

//Alert View delegate to handle the YES/NO option of the pre-OpenURL
- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
	//button 0 is the 'cancel" button, but we've set it to have the 'confirm' text in it
	if(buttonIndex == 0)
	{
		[self openURLWithMethod:_targetURL openMethod:_openMethod];
	}
	_targetURL = nil;
}

- (void)dealloc
{
	if (self.soundId != -1)
	{
		AudioServicesRemoveSystemSoundCompletion(self.soundId);
		AudioServicesDisposeSystemSoundID(self.soundId);
	}
	[[NSNotificationCenter defaultCenter] removeObserver:self name:AVAudioSessionRouteChangeNotification object:[AVAudioSession sharedInstance]];

	[super dealloc];
}

- (BOOL)isInGuidedAccessMode
{
    return UIAccessibilityIsGuidedAccessEnabled();
}
@end
