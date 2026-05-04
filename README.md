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
1. Open the baking Window

![image](./RepoAssets/Images/open_bake.png)

2. Drag the `.anim` file into the Animation slot
3. Fill out your BPM andStart Beat Offset
4. Filling out the output properties:
   1. Use the "Get Properties" button to find the object paths (this will be logged to the console) and the name of the properties. 
      - For Vector-like properties (such as Color and Vector), do not animate the `.x`, `.y` or `.r`, `.g`, etc properties individually - just set the property name as a whole.
   2. "Material Name" will be the output of the Vivify Event material name.
      - It does not technically need to match the actual material name.
   3. "Object Name" will be the object path logged to the console. Fill it out exactly for each layer.
   4. "Property Names" will be the name of each property as they appear in your shader file / in the console.
5. Hit "Bake" and choose where you want to save the resulting bake.
