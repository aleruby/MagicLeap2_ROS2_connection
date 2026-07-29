There are two options for configuring the application for Magic Leap 2:

1. Use the pre-built application
    The APK available in the `Application` folder can be installed using the 'Magic Leap Hub 3' application.
    Once launched, the application will attempt to connect to IP address 192.168.0.203 on port 10000.
    Note: If the computer running ROS2 has a different IP address, you must either assign it the corresponding static IP or modify and recompile the application (see Option 2).
    Once the application is installed and you have verified that the computer is reachable at the correct address, you can proceed to use it.
    After completing the startup procedure on the ROS2 side (described in the README within the `ROS2_development` folder), you can launch the app on the device: streaming will start automatically.

2. Create a Unity project that allows you to modify the application and rebuild it.
    Follow the official Magic Leap 2 guide at this link: https://developer-docs.magicleap.cloud/docs/guides/unity-openxr/getting-started/openxr-unity-getting-started/
    Once the project is ready, create an empty GameObject in the main scene.
    Add all the scripts from the 'Scripts' folder to the object; ensure they are all enabled.
    Now, you need to add the Android plugin that handles certain sensors.
    In Unity, go to Edit > Project Settings > Player.
    Select the Android platform tab (the little robot icon).
    Expand the 'Publishing Settings' section at the bottom.
    Scroll down to the 'Build' section and check the 'Custom Main Gradle Template' box.
    This will generate a file named 'mainTemplate.gradle' in your project folder: Assets/Plugins/Android/mainTemplate.gradle.
    Open the newly generated file with a text editor (or directly within Unity/VS Code).
    Look for the 'dependencies' section (usually found near the end of the file) and add the line for the Kotlin stdlib. The file should look something like this:

    dependencies {
        // ... other existing dependencies added by Unity ...

        // ADD THIS LINE:
        implementation "org.jetbrains.kotlin:kotlin-stdlib:1.9.20" 
    }

    (Note: version 1.9.20 is a good standard, but if you used a specific version like 1.8.x or 2.x in Android Studio, set that one instead).
    Finally, download the .aar file available in the 'Android_Plugin' folder of this repository.
    Place this file in your Unity project at the path `Assets/Plugins/Android`.
    Next, you need to install the ROS TCP Connector library.
    Click "Window" in the top menu bar.
    Click "Package Manager" from the menu that appears.
    Click the "+" icon in the top-left corner.
    Click "Add package from git URL".
    Paste the following URL and click "Add":
    https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.ros-tcp-connector
    If everything was done correctly, a 'Robotics' entry should appear in the top menu.
    Clicking on it allows you to modify the IP address of the computer you intend to connect to.
    In this window, you also need to select 'ROS2', since we are working with ROS2.
    You can now build the project or proceed with further modifications.

    If you wish to add options to the 'config.yaml' file, you must modify the 'Configurator.cs' script.
    Specifically, the structure of the AppConfig class needs to be updated to accommodate the additional options.
    Upon application startup, the 'Configurator.cs' script loads the options specified in 'config.yaml' into the AppConfig structure.
    Subsequently, 'Configurator.cs' simply updates variables in other scripts based on the loaded configuration.
    Therefore, anyone wishing to add functionality is advised to follow this same approach.
