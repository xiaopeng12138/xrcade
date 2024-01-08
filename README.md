
# Xrcade

**An open-source arcade simulation system for XR devices based on Unity (Will be soon changed to Godot).**

## What is Xrcade?

Xrcade is a project that simulates arcade cabinets on XR-supported devices. It is similar to VRChat but specifically designed for arcade games. Xrcade provides the fundamental VR interaction environment, and support for different cabinets can be added through "Steam Workshop" mods or an official Git list.

## The plan includes the following features

- Custom cabinet support
- Custom environment support
- Custom avatar support
- Multiplayer support (via Steam)
- Lobby system (via Steam)
- Cross-cabinet functionality (different games in the same room)

## To-do list

- [x] Switch from Unity to Godot
- [x] Basic VR interaction
- [ ] Start menu
- [ ] Cabinet selection implementation
  - [ ] Cabinet browser
  - [ ] Runtime cabinet loader
  - [ ] Runtime script loader
- [ ] World selection implementation
  - [ ] World browser
  - [ ] World cabinet loader
- [ ] Avatar implementation
  - [ ] Avatar browser
  - [ ] Runtime VRM loader
  - [ ] VRM inverse kinematics (IK)
  - [ ] Full-body capture support
- [ ] Universal settings panel
- [ ] Hooks
  - [ ] DX11 hook for game capture
  - [ ] DX11 hook DLL
  - [ ] DX9 hook for game capture
  - [ ] DX9 hook DLL
  - [ ] Genetic serial hook for I/O
- [ ] Multiplayer implementation
  - [ ] Steamworks network implementation
  - [ ] Lobby browser
  - [ ] XR player spawn system
  - [ ] Cabinet synchronization over the network
  - [ ] World synchronization over the network
  - [ ] Avatar transmission over the network
  - [ ] Avatar synchronization over the network
  - [ ] Screen encoding
  - [ ] Screen synchronization over the network
