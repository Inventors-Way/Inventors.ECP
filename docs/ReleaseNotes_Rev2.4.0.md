# Release Notes, Rev. 2.4.0

## Content

This is a major revision that backward incompatible with revision 2.3.1

### Major changes

The release contains the following major changes:

* Devices no longer implements INotifyPropertyChanged interface
* ECP Tester will now refresh the device state if it is selected.

### Minor changes

The release contains the following minor changes:

* None

## Changes

### Devices no longer implements INotifyPropertyChanged interface

This functionality has been removed from the library as it was found not to be usefull.

### ECP Tester will now refresh the device state if it is selected.

When the device is selected the property grid will now be refreshed.