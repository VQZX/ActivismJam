

#import <Foundation/Foundation.h>
#import <MediaPlayer/MediaPlayer.h>
#import <AVFoundation/AVAudioSession.h>


@interface PlatformHelper : NSObject<UIAlertViewDelegate>
{
}


+ (PlatformHelper*)sharedHelper;
- (void)initialize;
- (BOOL)getIsMusicPlaying;
- (BOOL)getAreHeadphonesConnected;
- (void)forceExternalSpeaker;
- (float)getDeviceVolume;
- (void)openURLWithMethod:(NSString *)url :(int)method;
- (void)openURLWithMethodConfirm:(NSString *)titleText :(NSString *)messageText :(NSString *)cancelText :(NSString *)confirmText :(NSString *)urlText :(int)method;
- (BOOL)checkPermission:(int)permission;
- (void)requestPermission:(int)permission :(NSString*)caller :(NSString*)method;
- (BOOL)_isInGuidedAccessMode;

@end
