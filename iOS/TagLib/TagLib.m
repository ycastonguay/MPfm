#import "TagLib.h"
#include "tag_c.h"

void Init_TagLibBundle(void) { }

@implementation TagLib

@synthesize title, artist, album;

- (id)initWithFileAtPath:(NSString *)filePath {
	if (self = [super init]) {

        //TagLib_File_FLAC *filzzz;
        
        
		// Initialisation as per the TagLib example C code
		TagLib_File *file;
		TagLib_Tag *tag;

		// We want UTF8 strings out of TagLib
		taglib_set_strings_unicode(TRUE);

		// Convert the NSString filePath into a native c string
		file = taglib_file_new([filePath cStringUsingEncoding:NSUTF8StringEncoding]);

		if (file != NULL) {
			tag = taglib_file_tag(file);

			if (tag != NULL) {
				// If we have a valid 'title' tag, assign it to our 'title' ivar
				if (taglib_tag_title(tag) != NULL &&
					strlen(taglib_tag_title(tag)) > 0) {
					self.title = [NSString stringWithCString:taglib_tag_title(tag)
                                                               encoding:NSUTF8StringEncoding];
				}

				if (taglib_tag_artist(tag) != NULL &&
					strlen(taglib_tag_artist(tag)) > 0) {
					self.artist = [NSString stringWithCString:taglib_tag_artist(tag)
                                                                encoding:NSUTF8StringEncoding];
				}

				if (taglib_tag_album(tag) != NULL &&
					strlen(taglib_tag_album(tag)) > 0) {
					self.album = [NSString stringWithCString:taglib_tag_album(tag)
                                                               encoding:NSUTF8StringEncoding];
				}
			}

			// Free up the allocated memory from TagLib
			taglib_tag_free_strings();
			taglib_file_free(file);
		}
	}

	return self;
}

@end
