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

@synthesize sampleRate;

- (id)init {
    
    // Initialize super class
    self = [super init];
    
    // Check for null
    if(self) {
        
        // Set default values
        sampleRate = 44100;
        
        // Initialize player
        [self initializePlayer];

    }
    return self;
}

- (id)initWithOptions:(int)playerSampleRate {
    
    // Initialize super class
    self = [super init];
    
    // Check for null
    if(self) {
        
        // Set default values
        sampleRate = playerSampleRate;
        
        // Initialize player
        [self initializePlayer];
        
    }
    return self;
}

- (void)initializePlayer {
    
    // Check BASS version
    if (HIWORD(BASS_GetVersion()) != BASSVERSION)
    {
        NSException* e = [NSException exceptionWithName:@"InvalidBASSVersionException"
                                                 reason:@"Invalid BASS version!" userInfo:nil];
        @throw e;
    }
    
    if(!BASS_Init(-1, sampleRate, 0, NULL, NULL))
    {
        NSException* e = [NSException exceptionWithName:@"BASSInitFailedException"
                                                 reason:@"Failed to initialize the BASS library!" userInfo:nil];
        @throw e;        
    }
}

- (void)dealloc {
    // Release other objects
    //[super dealloc]; DONE BY ARC
}

- (void)play {
    
  
}

@end
