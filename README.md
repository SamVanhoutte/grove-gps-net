# grove-gps-net
Repository with sample to use the GrovePI GPS sensor in .NET on Raspberry PI.

This sample scans for the Serial ports and takes the first available port. (this means you'll need to adapt the code in case you have multiple serial ports connected). 

It then starts listening on the port and leverages the [NMEAPaser NuGet package](https://github.com/dotMorten/NmeaParser "NMEAParser NuGet package") to parse the NMEA specific messages.

The GPS module has to be connected to the RPISER port on the GrovePi board, as can be seen in the following image:
![connected module](/doc/pics/gps-rpiser.jpg "Connected module")

__Details__ 
* RPISER port (right green box)
* GPS module (middle green box)
* GPS antenna (left green box)