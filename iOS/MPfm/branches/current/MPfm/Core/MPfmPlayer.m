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
        NSException* e = [NSException exceptionWithName:@"InvalidBASSVersionException"
                                      reason:@"Invalid BASS version!" userInfo:nil];
        @throw e;
    }
    
    // Initialize BASS
    if(!BASS_Init(-1, 44100, 0, NULL, NULL))
    {
        NSException* e = [NSException exceptionWithName:@"BASSInitFailedException"
                                                 reason:@"Failed to initialize BASS!" userInfo:nil];
        @throw e;
    }
    
    // Free BASS
    BASS_Free();
}

- (void)play
{
    
    
}

@end
