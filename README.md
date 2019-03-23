# CommandLineFPS
A Dot Net Core adaption of the Command Line FPS tutorial by OneLoneCoder (https://github.com/OneLoneCoder/CommandLineFPS)
https://github.com/OneLoneCoder/videos/blob/master/LICENSE

It started off as a curious experiment to see what it would be like to get it to work on .NET Core.

# 2019/03/23 - Part Two
I have finally gotten to the point where I can upgrade the resolution. I took the logic from the second version of the CommandLineFPS
sample by OneLoneCoder to assist me with the Win32API Console interface.
Not sure why I can up the resolution to 320x240 but I can get it to 240x120.
Looks a bit more crisp and still runs well enough. I am quite impressed with how far I managed to get with .NET Core on this.
I have disabled other OS support at this point as most of the enhancements are starting to rely now on these new features and
I have not been in a position (or will power) to look at something like Linux, etc. Though, I'm sure it is entirely possible.
I was dissapointed that adding a `Parrallel.For` on the `MapRender` didn't make any difference to the render speed since that
is supposed to be an ideal scenario for multithreading performance gains!

![Screenshot](Screenshots\Screenshot5.png)

# 2019/03/23 - Part One
Fixed a couple of keyboard issues. Now it runs as smoothly as the C++ counterpart. I ran the C++ version on my machine and noticed that even
on a release build my version runs just as fast in terms of FPS. Also realized that my maze rendering was wrong and fixed it to match up with
the map view. Seems like the map and rendering coordinates need to be flipped for some reason.

![Screenshot](Screenshots\Screenshot4.png)

# 2019/03/22 - Entry Three
Now it is using OS detection to determine to use the native Windows Console and keyboard code to have a better experience.
Any other OS will have the default implementations that may not be so satisfactory (since I haven't targeted anything else at the moment).

# 2019/03/22 - Entry Two
Whoah! I reverted my workaround for the "proper" way of doing things. It turns out that `WriteConsole` was the culprint all along.
It renders very slowly in .NET and even when I tried it myself. `WriteConsoleOutputCharacter` renders much faster and I was able
to leverage `Encoding.GetBytes()` as well. I even got as much as 500fps at some point!

![Screenshot](Screenshots\Screenshot3.png)

# 2019/03/22 - Entry One
I think I have finally beat the system 😁. So instead of doing the normal `Encoding.GetBytes()` call, I've changed my Native Windows
Console to use a byte array under the hood and where I've picked up that a character that is unicode gets written (that looks like my wall)
I've secretly been changing it to the correct byte code. Also, I reverted to using the original Win32API call `WriteConsoleOutputCharacter`
which is writing much faster to the console than the previous `WriteConsole` one (plus the flickering stopped). Now I can achieve over 300 fps
at certain times.

![Screenshot](Screenshots\Screenshot2.png)

# 2019/03/21
I was very happy when I got to the point where I saw my maze being rendered, but the framerate was quite low
in comparison to the C++ version. I thought that maybe it is because I'm not working directly with the
underlying Console handle to write to the Console buffer (ignorance is bliss 😉).
After some time I managed to figure out how to use the underlying Console Handle from .NET's Console so that
I could write to the Console directly. This gave a significant improvement to my rendering however, I noticed
that my wall characters were messed up. After many hours of googling the Win32API functions for interacting
with the Console and inspecting the .NET Core source code I almost gave up trying to figure out what I was doing wrong
since the .NET Core Console was using the Win32API calls underneath and was working.
Then it finally dawned upon me that perhaps there is a marshalling concern that is happening between .NET Core and
the Win32API. I used the current Encoder on the Console to get the byte version of my character array and BOOM it worked!
Except that I got the almost exact frame rate than the original effort I put in using the normal .NET Core Console 😔.
Also, there is a bit of a flicker happening as well. I've reverted to using my original renderer that won't flicker if you want to try it out but it is "configurable" when you look through the code and un/comment some lines.

This was an interesting experiment and something I quite enjoyed persuing, especially to take my mind off my normal programming
routine. I felt almost like I was a university or high school student again. So big thanks to OneLoneCoder for the idea!
Maybe I might tinker on it some more but I can't promise anything.

![Screenshot](Screenshots\Screenshot.png)









