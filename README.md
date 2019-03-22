# CommandLineFPS
A Dot Net Core adaption of the Command Line FPS tutorial by OneLineCoder (https://github.com/OneLoneCoder/CommandLineFPS)

#2019/03/21
It started off as a curious experiment to see what it would be like to get it to work on .NET Core.
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

#2019/03/22
I think I have finally beat the system 😁. So instead of doing the normal `Encoding.GetBytes()` call, I've changed my Native Windows
Console to use a byte array under the hood and where I've picked up that a character that is unicode gets written (that looks like my wall)
I've secretly been changing it to the correct byte code. Also, I reverted to using the original Win32API call `WriteConsoleOutputCharacter`
which is writing much faster to the console than the previous `WriteConsole` one (plus the flickering stopped). Now I can achieve over 300 fps
at certain times.

![Screenshot](Screenshot.png)

https://github.com/OneLoneCoder/videos/blob/master/LICENSE
