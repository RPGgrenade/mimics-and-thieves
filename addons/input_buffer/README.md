# Input Action Buffer

Input Action Buffer plugin
by Energyxxer

# Description

This plugin allows for input actions to have a "buffer window" of time during which the player is able to press a button to perform the action, shortly before the action becomes available.
This is particularly useful for character controllers to feel more responsive, as even if the player presses a button a couple frames early, the game can still respond to that input as expected.

# Installation
First, create an addons folder at the root of your project (if one doesn't exist) and drop in the `input_buffer` folder. Once this is done click on Project Settings at the top of the screen, then go to the Plugins tab, you should now be able to see the plugin as toggleable.

![Screenshot](https://cdn.discordapp.com/attachments/1132083965700882563/1192958709819789442/image.png?ex=65aaf860&is=65988360&hm=c3a5070299ef76d45c3c7cebf1a0bf60b4667ab6d5c0afc0f0da805171307f0b&)

# Usage
You can use the input buffer in any piece of code that would otherwise use the standard `Input.IsActionJustPressed()` to detect a button press.
To make such code use the input buffer, replace it with a call to `InputActionBuffer.Instance.ConsumeAction()` with the same action ID parameter.
Example:
```CS
// Old:
bool shouldJump = Input.IsActionJustPressed("jump");
// New:
bool shouldJump = InputActionBuffer.Instance.ConsumeAction("jump");
```

`ConsumeAction` does two things:
1. It checks if the action button has been pressed recently during the buffer window. This is the returned value.
2. If the action _was_ pressed, it is marked as "consumed", so subsequent calls to the input buffer with the same action will return false until the button is released and pressed again by the user.

If you instead don't want to consume the input, and only want to check if the button was pressed recently, use the `IsActionBuffered` method instead with the same parameter.

## Pre-registering actions
The input buffer only tracks actions that have been previously sent as parameters to InputBufferAction methods. This is an optimization to avoid tracking actions that do not use the input buffer.
However, as a consequence, this means that if the player presses a button before the first call to the input buffer, that button press will be missed.
For example, if your player character controller calls `ConsumeAction("jump")` every frame when they have control, and the player presses the jump button a couple frames before they gain control for the first time, the input buffer will miss that press.
To fix this, call `InputActionBuffer.Instance.RegisterAction("jump")` earlier; when the game starts or the scene loads.

# Adjusting buffer window
