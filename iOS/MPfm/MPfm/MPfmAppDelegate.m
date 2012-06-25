//
//  MPfmAppDelegate.m
//  MPfm
//
//  Created by Yanick Castonguay on 12-06-04.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "MPfmAppDelegate.h"
#import "MPfmPlayerViewController.h"
#import "MPfmPreferencesViewController.h"
#import "MPfmLibraryBrowserViewController.h"

@implementation MPfmAppDelegate

@synthesize window = _window;
@synthesize tabBarController = _tabBarController;

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    // Declare variables
    UIViewController *playerViewController, *libraryBrowserViewController, *preferencesViewController;
    
    // Initialize window
    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];

    // Initialize view controllers
    if([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone)
    {
        // iPhone
        playerViewController = [[MPfmPlayerViewController alloc] initWithNibName:@"MPfmPlayerViewController_iPhone" bundle:nil];
        libraryBrowserViewController = [[MPfmLibraryBrowserViewController alloc] initWithNibName:@"MPfmLibraryBrowserViewController_iPhone" bundle:nil];
        preferencesViewController = [[MPfmPreferencesViewController alloc] initWithNibName:@"MPfmPreferencesViewController_iPhone" bundle:nil];
    }
    else 
    {
        // iPad (nothing for now)
    }
    
    // Initialize tab controller
    self.tabBarController = [[UITabBarController alloc] init];
    self.tabBarController.viewControllers = [NSArray arrayWithObjects:playerViewController, libraryBrowserViewController, preferencesViewController, nil];
    self.window.rootViewController = self.tabBarController;
    [self.window makeKeyAndVisible];
    
    // Override point for customization after application launch.
    return YES;
}
							
- (void)applicationWillResignActive:(UIApplication *)application
{
    // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
    // Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
    // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later. 
    // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
    // Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
}

- (void)applicationDidBecomeActive:(UIApplication *)application
{
    // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
}

- (void)applicationWillTerminate:(UIApplication *)application
{
    // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
}

@end
