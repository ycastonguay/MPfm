//
//  MPfmFirstViewController.h
//  MPfm
//
//  Created by Yanick Castonguay on 12-06-04.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MPfmPlayer.h"

@interface MPfmPlayerViewController : UIViewController {
    NSTimer *timer;
}

@property (retain) MPfmPlayer* player;
@property (retain) NSTimer *timer;

@end
