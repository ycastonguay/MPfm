Sessions
====

Sessions (formely known as MPfm) is a music player made for musicians. It shares a lot of features with other common music players, but it also adds loops, markers, pitch shifting, time shifting, wave form rendering, and much more. 

It is also available on multiple platforms: __Windows__, __OSX__, __Linux__, __iOS__ and __Android__. 

The source code is released under the GPLv3 license. For more information on the Sessions license, go to http://www.mp4m.org/license.

For more information, go to the MPfm website: http://mp4m.org. A new web site will soon be online for Sessions.

Important notes:
--

I'm currently refactoring the project with version 0.7.0.0. Please consult the [Mantis bug tracker for Sessions](http://www.mp4m.org/mantis/roadmap_page.php).

Sessions uses the __BASS audio library__, which is a commercial library, but available for free for open source projects. The Sessions source code repository contains the BASS static/dynamic libraries needed to build each platform. To download updated versions of the BASS library, go to the [BASS library home page](http://www.un4seen.com). Please note that newer versions might introduce breaking changes.

Sessions also uses the __BASS.NET audio library__, which is a commercial library, but available for free for open source projects. The Sessions source code repository contains the BASS.NET assemblies needed to build each platform. To download updated versions of the BASS.NET library, go to the [BASS.NET library home page](http://bass.radio42.com). Please note that newer versions might introduce breaking changes. 

To build Sessions successfully, you will also need a valid BASS.NET registration key which you can generate for free for open source projects [here](http://bass.radio42.com/bass_register.html). Simply uncomment the [BassNetKey.cs](Sessions.Sound/BassNetKey.cs) file in Sessions.Sound and add your email address and key.

If you want to deploy the iOS and Android versions to hardware devices, you'll need a valid Xamarin license. You can obtain a license [here](http://www.xamarin.com).

The project doesn't contain any makefiles at the moment, they will be added later for the Linux and OSX platforms.

Please use the [VSProjectSync](https://github.com/ycastonguay/VSProjectSync) tool to synchronize project files before submitting any pull requests.

How to build Sessions
--

There are no makefiles at the moment, so you will need [Visual Studio](http://www.microsoft.com/visualstudio/) or [MonoDevelop](http://monodevelop.com/) to build the project depending on the platform.

There are several solution files in the root directory of the project. Here is a list of the solution files per platform:

+  __Windows__: `Sessions_WPF.sln`
+  __OSX__: `Sessions_OSX.sln`
+  __Linux__: `Sessions_Linux.sln`
+  __iOS__: `Sessions_iOS.sln`
+  __Android__: `Sessions_Android.sln`

The project source code already contains a few dependencies (such as the BASS audio library, TinyIoC, etc.), but you will need the following libraries/software installed to build Sessions:

__Windows__:
+ [.NET Framework 4.0+](http://www.microsoft.com/net)

__OSX__:
+ [Mono](http://www.mono-project.com)
+ [MonoMac](https://github.com/mono/monomac)
+ [Xcode](https://developer.apple.com/xcode/)

__Linux__:
+ [Mono](http://www.mono-project.com)
+ [GTK#](http://www.mono-project.com/GtkSharp)

__iOS__:
+ [Xamarin.iOS](http://xamarin.com/monotouch)
+ [Xcode](https://developer.apple.com/xcode/)

__Android__:
+ [Xamarin.Android](http://xamarin.com/monoforandroid)

*'Sessions' and 'MPfm: Music Player for Musicians' are © 2011-2014 Yanick Castonguay and is released under the GPLv3 license.*
*The BASS audio library is © 1999-2014 Un4seen Developments.*
*The BASS.NET audio library is © 2005-2014 Bernd Niedergesäß.*
