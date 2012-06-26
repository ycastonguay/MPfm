//
//  Stuff.m
//  TagLib
//
//  Created by Yanick Castonguay on 12-06-25.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "Stuff.h"
//#import "TagLib.h"
#import "tbytevector.h"
#import "mpegfile.h"
#import "id3v2tag.h"
#import "id3v2frame.h"
#import "attachedpictureframe.h"

//#import <tbytevector.h>
//#import <mpegfile.h>
//#import <id3v2tag.h>
//#import <id3v2frame.h>
//#import <attachedPictureFrame.h>

@implementation Stuff

- (id)init
{        
    // Initialize super class
    self = [super init];
    
    // Check for null
    if(self) {
        
    }
    return self;
}

- (void)initializeStuff:(NSString*)filePath 
{
    NSLog(@"init test");
    const char *newFilePath = [filePath UTF8String];
    TagLib::MPEG::File file(newFilePath);
    TagLib::ID3v2::Tag *id3v2tag = file.ID3v2Tag();
    TagLib::ID3v2::FrameList Frame;
    TagLib::ID3v2::AttachedPictureFrame *PicFrame;
    void *RetImage = NULL, *SrcImage;
    unsigned long Size;
    NSLog(@"filez");
}

@end
