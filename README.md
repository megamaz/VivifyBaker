# Vivify Baker
This is a simple tool that allows you to bake Unity `.anim` file data to Vivify Events.

Roadmap might invole baking more event types in the future.

## Usage
First you wanna install the package before you can start using it.

Download the `.unitypackage` from the releases tab, and install it. A `VivifyBaker` option should appear at the top of your window.

As of right now, VivifyBaker is mainly built for animating PostProcess animations. This is because it is not possible to directly animate PostProcess data inside of a Unity `.anim` file. Using this project, you can animate PostProcess directly inside of Unity, and bake it to one (or more) Vivify `SetMaterialProperty` events.
### 1. Creating your animation
This assumes that you already have an object with an Animator on it, but haven't animated any PostProcess yet.
1. Right click the animated object and navigate to "VivifyBaker > Create Post Process Controller"

![image](./RepoAssets/Images/create_controller.png)

2. Set the Camera, then click "Add Layer"

![image](./RepoAssets/Images/set_camera.png)

3. On the newly created layer, set your material in the AnimatablePostProcessLayer

![image](./RepoAssets/Images/set_mat.png)

**You do NOT need to touch the `SkinnedMeshRenderer`.**

You are now set to begin animating! Unity will have created a material dropdown at the bottom of the inspector, within which you can directly animate your material. VivifyBaker will apply that material as post-process.

### Baking your animation
1. Select your post process layer object
2. Add a "Material Baker Component" Component to it.
3. Fill out your BPM and start offset.
4. Select the properties you wish to bake in the properties dropdown.
5. Hit "Bake", and select where you wish to save the bake.

## Known Issues
- WIP: Conflict with [VivifyTemplate](https://github.com/Swifter1243/VivifyTemplate) `Newtonsoft.Json.dll`. I am working with Swifter to resolve this. For now, VivifyBaker relies on [VivifyTemplate](https://github.com/Swifter1243/VivifyTemplate) being included in your project.