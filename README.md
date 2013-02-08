MPfm: Music Player for Musicians
====

MPfm is a music player made for musicians. It shares a lot of features with other common music players, but it also adds loops, markers, pitch shifting, time shifting, wave form rendering, and much more. 

It is also available on multiple platforms: __Windows__, __Mac__, __Linux__, __iOS__ and __Android__. 

The source code is released under the GPLv3 license. For more information on the MPfm license, go to http://www.mp4m.org/license.

For more information, go to the MPfm website: http://mp4m.org.

Important notes:
--

Please note that I'm currently refactoring the application to support multiple platforms using a Model-View-Presenter pattern. Only the Windows version is stable at this moment. The other platforms should be considered as alpha, so use them at your own risk.

The project doesn't contain any makefiles at the moment, they will be added later.

If you want to deploy the iOS and Android versions to hardware devices, you'll need a valid Xamarin license. You can obtain a license here: http://www.xamarin.com

Please use the [ProjectSync](https://github.com/ycastonguay/MPfm/tree/master/ProjectSync) tool (included on this repository) to synchronize desktop/iOS/Android project files before commiting any changes.

How to build MPfm
--

There are no makefiles at the moment, so you will need Visual Studio and/or MonoDevelop to build the project depending on the platform.

There are several solution files in the root directory of the project. Here is a list of the solution files per platform:

+  __Windows__: `MPfm_Windows.sln`
+  __Mac__: `MPfm_Mac.sln`
+  __Linux__: `MPfm_Linux.sln`
+  __iOS__: `MPfm_iOS.sln`
+  __Android__: `MPfm_Android.sln`

The project source code already contains a few dependencies (such as the BASS audio library, TinyIoC, etc.), but you will need the following libraries/software installed to build MPfm:

__Windows__:
+ .NET Framework 4.0+
+ WinForms

__Mac__:
+ [Mono](http://www.mono-project.com)
+ [MonoMac](https://github.com/mono/monomac)
+ [Xcode](https://developer.apple.com/xcode/)

__Linux__:
+ [Mono](http://www.mono-project.com)
+ [GTK#](http://www.mono-project.com/GtkSharp)

__iOS__:
+ [MonoTouch](http://xamarin.com/monotouch)
+ [Xcode](https://developer.apple.com/xcode/)

__Android__:
+ [Mono for Android](http://xamarin.com/monoforandroid)
