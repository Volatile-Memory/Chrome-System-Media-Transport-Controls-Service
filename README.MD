# Chrome Compatible Windows SystemMediaTransportControls Wrapper

Displays modern Windows app style playback controls based on player metadata sent over STDIO in JSON form -- e.g. for Google Chrome Native Messaging.

SMTC_API.cs contains a generic class that will generate a JSON schema and example for controlling the Windows System Media Transport Controller (SMTC) with a near identical api.
All events will fire back a message object that closely matches the event API/Arguments in the SystemMediaTransportControls.

Sway.FM Media Controls support is built in and should integrate with the extension after registering the manifest.