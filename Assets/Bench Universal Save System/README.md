# Introduction

The Bench Universal Save System is a Unity Asset that was originally developed for the [**Creating a Metroidvania (like Hollow Knight)** tutorial series](https://blog.terresquall.com/series/creating-a-metroidvania-like-hollow-knight/) by Terresquall. Because the Save System got very complicated and robust, we thought that it would be a good idea to turn it into a Unity Asset instead.

Because of the complexity of setting up a saving system in a video game, we wanted to create an asset that is very easy to set up, so that you can get the system up and running as quickly as possible.

## How the Bench Save System works

Components which have data to be saved must inherit from the `PersistentObject` class. This will introduce functions that allow data to be initialised, set and loaded.

During saving, the system automatically searches the current scene for all components inheriting from the `PersistentObject` class and calls its `Save()` method. This will initialise data set in the local `SaveData` class, compiling the data into a single save file along with information such as the scene name and timestamp. This save file would be written into the disk through the save slot that was selected, making every save slot have its own unique save file.

When loading, the process is reversed. The system will match each data entry's `SaveID` to the `SaveID` of existing `PersistentObject` instances found in the current scene, passing data back through the `Load()` method. 

As long as the component has a `SaveID` generated, the data found in the component can be saved.

Component with a successfully generated `SaveID` attribute:

![Save ID example](/Promo/README_Images/saveID.png)

The `SaveID` attribute allows each `PersistentObject` instance to have its own unique identification which allows the save system to differentiate components from each other, omitting the risk of data being mixed up. The next section will show you how to properly extend your components from `PersistentObject`.

## Implementating PersistentObject

To make use of the Bench Universal Save System, you will need to extend your scripts for any component you would like to save using the `PersistentObject` class, instead of the default Unity `MonoBehaviour` class as such:

![Class extension example](/Promo/README_Images/class-extension.png)

Then, you will need to implement 3 things in your class:

1. The `Save()` method to define what variables get saved.
2. The `Load()` method to restore variables when loading.
3. A `SaveData` class that inherits from `PersistentObject.SaveData`.

For example:

```
public class Player : PersistentObject {

	public int health;
	public int maxHealth;
	public int level;

	//Other code...
	
	// Defines what are the variables in the save object of this class.
	[System.Serializable]
	public new class SaveData : PersistentObject.SaveData {
		public int maxHealth;
		public int level;
		public float positionX, positionY, positionZ;
	}
	
	// Defines how the variables will be saved into SaveData.
	public override PersistentObject.SaveData Save() {
		
		if (CanSave()) {
			// We want to save the level, maxHealth and position, not the current health.
            return new SaveData
            {
                saveID = saveID, // You always need to save the save ID.
				
                positionX = transform.position.x,
                positionY = transform.position.y,
                positionZ = transform.position.z,
				maxHealth = maxHealth
            };
        }
        
        return null;
	}
	
	// Defines how the variables will be loaded into our object.
	public override bool Load(PersistentObject.SaveData data) {
		if (data == null) return false;
		SaveData gameData = data as SaveData;
		if (gameData == null) return false;

		maxHealth = data.maxHealth;
		transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);

		return true;
	}
}
```

All the components that inherit from `PersistentObject` in any of your Scenes can be saved, **as long as they have a unique Save ID generated**. 

## Saving and Loading data

Bench Save System defines a few different functions for your use depending on how you want data to be saved and  loaded.

| Function  | Description |
| ------------- | ------------- |
| `SaveGame()`  | Saves all active `PersistentObject` instances in the current scene into the active save slot. |
| `SaveGameAsync()`  | Asynchronously saves all active `PersistentObject` instances into the active save slot without interfering with the main code. |
| `LoadGame()` (*Does not exist yet) | Loads the specified save slot into memory and restores all `PersistentObject` states in the current scene. |
| `LoadGameAsync()`  | Asynchronously loads a save file from disk and restores scene data without interfering with the main code. |
| `QuickLoad()`  | Either creates or updates a cached save file in memory using the current state of all `PersistentObject` instances in the current scene. |
| `QuickSave()`  | Applies the cached save file data to all matching `PersistentObject` instances in the scene. |

Add the namespace `Terresquall` in the script you would like to control saving and loading of data (eg. Game Manager).

To save your game at any point, you will need to call `Bench.SaveGame(Bench.currentSlot, true);`. To load an existing save, you will need to call `Bench.LoadScene()`, although you are recommended to use one of the Save Slot UI Prefabs in the Prefabs for this, as `Bench.LoadGame()` only loads the data into memory. It does not load and restore object data.

To load the game from the UI Save Slot, call the function `Bench.LoadGame(Bench.currentSlot)` in the `Awake()` method of GameManager in your main game scene. This would allow the game to assign the most recent saved data to the variables that had been previously saved.

For example: 

```
using Terresquall

public class GameManager : MonoBehaviour {
	// Call loading in Awake() if you want to load data immediately.
	void Awake()
	{
		Bench.LoadGame(Bench.currentSlot);
		// Or 	
		LoadGame();
	}

	// Making the functions public allows them to be called anywhere, allowing you to save or load your game at any point.
	// Bench.SaveGame() and Bench.LoadGame() do not need to be in functions.
	public void SaveGame()
	{
		Bench.SaveGame(Bench.currentSlot, true);
		Debug.Log("Game saved");
	}
	public void LoadGame()
	{
		Bench.LoadGame(Bench.currentSlot);
		Debug.Log("Game loaded");
	}
}
```

## Setting Up the UI

Import the asset into your project. The asset should be unpacked into a folder called `Bench Save System` in your `Assets` folder.

Firstly, in a different scene (eg. main menu scene) as your main gameplay scene, drag the asset `Native Save UI Canvas` into its Hierachy. This UI prefab contains clickable save slots, where each slots has a unique save file linked to it.

This is how the prefab of the asset looks like:
![Canvas Asset Preview](/Promo/README_Images/canvas-preview.png)

In the `UI Save Slot` component attached to `Save Slot Template`, change the value of `Default Scene` to the name of the scene of your main game, in this case our main gameplay scene is called `Action`.

![Where Save Slot Template can be found in the scene](/Promo/README_Images/scene-hierarchy.png)

![Showing where to change scene name](/Promo/README_Images/scene-name.png)

Remember to add your main game scene into the Build Profile Scene List.

![Scene List in build profiles](/Promo/README_Images/scene-list.png)

One feature of the Bench Save System is the ability to set the number of save slots in the Save UI Canvas. To do so, look for 'Settings' and change the value of 'Max Save Slots' to however many save slots you need. 

![Maximum Slots](/Promo/README_Images/slot-size.png)

Next, in the 'UI Save Slot Manager' component attached to the 'UI Save Slot Manager', click on the button 'Generate Save Slot Elements'. This would create the specified number of 'Save Slot Template' instances which duplicate all the settings from the original template.

![Slots generated](/Promo/README_Images/generate-save-slots.png)

With that, you have set up the UI and environment for saving to take place. To start a game and generate a new save, simply click on any of the UI save slots. This will call `Bench.LoadGame()` and load the last saved scene and `PersistentObject` data for that slot. Each `PersistentObject` is restored by matching its `SaveID` with the corresponding `SaveData` and applying it via the `Load()` method.

Do take note that you will have to call `Bench.QuickLoad()` after the data has been loaded into their respective `PersistentObject` instances. For optimisation, its best to call the quick load function from `async void Start()` with a 100ms delay to ensure that all data is initialised.

## Setting up the Save Indicator

The Bench Save System includes a save indicator UI element which provides visual feedback for the player whenever a save is in progress. This is useful for both developers and players to know that a game is actively saving data, its especially useful for async saves or longer saves.

Firstly, in your main gameplay scene, drag the `Default Indicator` prefab into your canvas if you already have one. If you do not have a canvas yet simply create a new one by right-clicking your `Hierarchy`, hovering over `UI` and then clicking on `Canvas`.

![Creating a Canvas](/Promo/README_Images/create-canvas.png)

The prefab contains a UI icon that will appear whenever a save occurs. This is how it looks like by default:

![Save Indicator icon](/Promo/README_Images/save-indicator.png)

Next, locate the `SaveIndicator` script and attach it to `Default Indicator` prefab if its not already attached. The script references the indicator to toggle on and off and animates the icon when a save is in progress.

The save system automatically calls `SaveIndicator.Toggle(true)` when a save begins and `SaveIndicator.Toggle(false)` when it ends. No additional scripting is required as long as the prefab is in your scene and the `Target` is assigned correctly. 

![Save Indicator component](/Promo/README_Images/indicator-script.png)

The `SaveIndicator` component allows for optional customisation where you can change the icon, animation settings and sprites to fit your game's aesthetics.

Whenever a save occurs, whether its called from `QuickSave()`, `SaveGame()` or `SaveGameAsync()`, the indicator will appear, providing developers and players with immediate feedback that game data is currently being saved.

## Configuring Settings and Debugging

There are some universal settings for the asset that you can tweak by going to Window > Terresquall > Bench Universal Save System. The asset will create a ScriptableObject in your project that contains the save settings.

![Accessing the Bench Save window](/Promo/README_Images/accessing-bench-window.png)

To help in viewing your saved data, you can access the `Settings` or the Bench Save System window and use the UI there to browse the data present in your save slots by clicking on `View Save Data`. This allows you to view all the variables and its values that you have saved which are categorised by their `SaveID`.

Here is an example of what you would be able to see when you choose to view a specified save slot's data:

![Accessing the Bench Save window](/Promo/README_Images/saved-data.png)

To check the variables and values saved during gameplay testing, look out for a debug message `Quick save on slot [n]. Added data for the following:`, this should show the most recent saved data and its variable names grouped by the script it was saved from.

To check if data was successfully loaded, look out for the debug message `Quick load on slot [n]. The following data was loaded:` with the same data shown in the save data debug message.