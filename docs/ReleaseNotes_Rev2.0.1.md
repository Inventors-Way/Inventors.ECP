# Release Notes, Rev. 2.0.1

## Content

This is a release that is backwards compatible with Rev. 2.0.0, and is a release that
is focused on the usability of the ECP Tester.

### Major changes

The release contains the following major changes:

* Update to the handling of opening device and opening and closing connections

### Minor changes

The release contains the following minor changes:

* DebugSignals will now show their signal also as decimal and hex.
* Property grids will refresh for functions

## Changes

### Update to the handling of opening device and opening and closing connections

It has been observed that the naming and shortcuts for the opening of device, and opening and closing of connections were unintuitive for users.

As a consequence this has been refectored to:

| Action                 | Old menu item          | New menu item          |
|:-----------------------|------------------------|------------------------|
| Initialize the program | Open Device (Ctrl + O) | Load Device (Ctrl + L) |
| Open serial port       | Open (Ctrl + C)        | Open (Ctrl + O)        |
| Close serial port      | Close (Ctrl + D)       | Close (Ctrl + X)       |

The rationale for Ctrl + X is that this is usually used for terminating an action, and the shortcut Ctrl + C has been discontinued because it prevented the use of the shortcut to be used for copying text from the ECP Tester. For example, to copy snippets of the log out into emails.

