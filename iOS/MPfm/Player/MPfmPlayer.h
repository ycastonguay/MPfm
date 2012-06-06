//
//  MPfmPlayer.h
//  MPfm
//
//  Created by Yanick Castonguay on 12-06-05.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "bass.h"

@interface MPfmPlayer : NSObject {
    int sampleRate;
}

@property (nonatomic, readonly) int sampleRate;

-(id) init;
-(id) initWithOptions:(int)playerSampleRate;
-(void) play;

@end
