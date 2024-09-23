# crosspipe

This software was written to enable a sort of 'null modem' connection between 86Box emulator instances.

It does this shuffling bytes between two named pipes to which the emulated serial ports are redirected.

Unfortunately, named pipe client connections aren't yet implemented in 86Box, but servers are. The main advantage of this approach over com0com or other virtual serial port/null modem drivers is in avoiding issues with driver signing or any need to configure external software.

For simplicity's (laziness') sake, I've hardcoded the two pipe names.

In your emulator configurations, redirect a serial port to one each of:

\\\\.\\pipe\\86Box1

\\\\.\\pipe\\86Box2

Run the emulators.

Run the program.

The pipes should then be cross-connected.

Press 'h' for help. Press 'd' to spew hex/ascii on your screen and slow things to a crawl. Press 'q' to quit.

Tested mainly with 115200 8N1 and 9600 8N1 in LapLink 3 and QModem xmodem and zmodem file transfers.

It did not seem like 86Box can handle reconnections, so you will probably need to restart your emulators before restarting this program if something goes wrong.

This code should also compile in .Net Framework 4.x. (currently in .Net 8.0 because I was too lazy to change it)

The license is MIT; please don't come crying to me.