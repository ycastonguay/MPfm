#import <Foundation/Foundation.h>

@interface TagLib : NSObject {
  NSString *title, *artist, *album;
}

@property (nonatomic, copy) NSString *title;
@property (nonatomic, copy) NSString *artist;
@property (nonatomic, copy) NSString *album;

@end
