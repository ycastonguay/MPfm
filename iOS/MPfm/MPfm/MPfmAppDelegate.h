//
//  MPfmAppDelegate.h
//  MPfm
//
//  Created by Yanick Castonguay on 12-06-04.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "TagLib.h"
#import "Stuff.h"

@interface MPfmAppDelegate : UIResponder <UIApplicationDelegate, UITabBarControllerDelegate>

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) UITabBarController *tabBarController;

@end
