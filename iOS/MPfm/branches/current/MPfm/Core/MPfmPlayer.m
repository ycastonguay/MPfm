//
//  MPfmPlayer.m
//  MPfm
//
//  Created by Yanick Castonguay on 12-06-02.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "MPfmPlayer.h"
#import "bass.h"

@implementation MPfmPlayer

- (void)initWithFrame
{
    // Check BASS version
    if (HIWORD(BASS_GetVersion()) != BASSVERSION)
    {
        //[NSException raise:@"Invalid BASS version!"];
        NSException* e = [NSException exceptionWithName:@"InvalidBASSVersionException"
                                      reason:@"Invalid BASS version!" userInfo:nil];
        @throw e;
    }
}

- (void)play
{
    
    
}

@end
