#!/bin/bash
gcc -Wall -fPIC -c wayland-wrapper.c -o libwayland-wrapper.o -lwayland-client
gcc -shared -o libwayland-wrapper.so libwayland-wrapper.o -lwayland-client

mv libwayland-wrapper.so ../Demos/SampleApp/bin/Debug/netcoreapp1.1/wayland-wrapper.so
rm *.o
