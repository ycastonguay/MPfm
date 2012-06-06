//
//  MPfmFirstViewController.m
//  MPfm
//
//  Created by Yanick Castonguay on 12-06-04.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "MPfmPlayerViewController.h"

@interface MPfmPlayerViewController ()

@end

@implementation MPfmPlayerViewController

@synthesize player, timer;

- (void)viewDidLoad
{
    [super viewDidLoad];
	
    // Do any additional setup after loading the view, typically from a nib.
    
    // Create player
    player = [[MPfmPlayer alloc] initWithOptions:44100];
    int sampleRate = player.sampleRate;
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone) {
        return (interfaceOrientation != UIInterfaceOrientationPortraitUpsideDown);
    } else {
        return YES;
    }
}

@end
