# SeeingVR: A Set of Tools to Make Virtual Reality More Accessible to People with Low Vision

Authors: Yuhang Zhao, Ed Cutrell, Christian Holz, Meredith Ringel Morris, Eyal Ofek and Andy Wilson.

Current virtual reality applications do not support people who have low vision, i.e., vision loss that falls short of complete blindness but is not correctable by glasses. We present SeeingVR, a set of 14 tools that enhance a VR application for people with low vision by providing visual and audio augmentations. A user can select, adjust, and combine different tools based on their preferences. Nine of our tools modify an existing VR application post hoc via a plugin without developer effort. The rest require simple inputs from developers using a Unity toolkit we created that allows integrating all 14 of our low vision support tools during development. Our evaluation with 11 participants with low vision showed that SeeingVR enabled users to better enjoy VR and complete tasks more quickly and accurately. Developers also found our Unity toolkit easy and convenient to use.

See more info and download the CHI 2019	paper at the [project's website](https://www.microsoft.com/en-us/research/publication/seeingvr-a-set-of-tools-to-make-virtual-reality-more-accessible-to-people-with-low-vision-2/).

# Get Started
The code is a [Unity](https://unity.com/) project that contains a set of plugins and a demo scene.
Open the project in Unity and the scene `SeeingVR demo` for usage examples. 

The demo scene contains a GameObject `AccessibilityManager` that was created for easy showcase of SeeingVR
tools.  Play the scene and use the parameters in the inspector to enable/disable and configure tools.

![Accessibility Manager in the Unity Inspector window](/images/demo-accessibility-manager.png)

Refer to the [manual in the SeeingVR folder](Assets/SeeingVR/SeeingVR%20user%20manual.pdf) for using the tools in your own applications. 

# Current Status
The project was last tested on Unity version 2019.1.3f1, SteamVR 2.2.0.  We tested on Windows Mixed Reality
headset and controllers, and configured them using the new [SteamVR Input system](https://steamcommunity.com/games/250820/announcements/detail/3809361199426010680).  If you want to use different headset and controllers,
please refer to the [documentation](https://valvesoftware.github.io/steamvr_unity_plugin/tutorials/SteamVR-Input.html). 

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
