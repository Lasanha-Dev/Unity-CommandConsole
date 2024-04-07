# Unity-CommandConsole
I've recently worked on a game that required a lot of tests and gameplay adjustments. So I came up with the idea of creating this custom Command Console that can be very helpful for testing your game. It executes commands on instances and static objects/methods, so you can add any command to any class.

This system uses Reflection to execute the commands, so it costs some performance during runtime.
More about this: https://docs.unity3d.com/6000.0/Documentation/Manual/dotnetReflectionOverhead.html

I tried to reduce as much as possible the reflection overhead by caching the types and assemblies names in a ScriptableObject and just getting what is necessary from the reflection system.

If you want to use this in your project, you need to set up the package before starting to use it. I demonstrate how to do that in the video.

You can check the .Net Trie implementation here: https://github.com/gmamaladze/trienet

You can check the console usage and implementation here: https://youtu.be/gf2NaL04vUg

![commandconsoleunityc#cheatsdebug](https://github.com/Lasanha-Dev/Unity-CommandConsole/assets/138812250/b5059ee5-b9f4-4b1b-a6a9-e39de2b42085)
