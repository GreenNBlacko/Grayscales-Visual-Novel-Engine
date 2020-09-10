# Grayscale's Visual Novel Engine
Welcome to the world of visual novel development!

My visual novel engine aims for simplicity for the user while having complete control on how it works.

# Features
- Absolutely *0* amount of coding knowledge required to create a fully functional visual novel!
- Easy to use dynamic systems that adjust to the user to improve their experience using the engine.
- Simple to edit code for adding in plugins into the engine for people who like to push their experience to the limits!
- Active support for fixing issues that you might encounter in your visual novel creation journey

# Installation
- Download and install your preferred unity version(suggested version: 2020.1.2f1).
- Download the latest release from the releases page.
- Extract the .zip file in an empty folder, preferably named after your project(e.g: Falling leaves of October).

# Usage
- Add the project in unity using the **'Add'** button and navigating to your project files.
- Launch the project and start by creating a new scene.
- Create a new empty game object and name it **'ScriptManager'**.
- Put these scripts in it: **DialogueManager, CGManager, CharacterInfo, CharacterManager, ChapterManager, SaveLoadSystem, CharacterManager, JSONSentenceImporter**(only add this one if you plan to import sentences using a JSON file).
- Create a new empty game object and name it **'SentenceManager'**.
- In it, put the script **'SentenceManager'** in it.
- Now, create a simple UI using the prefab given in the prefabs folder.
- Assign all the variables for the **'DialogueManager'** script.
- Edit the choice and backlog prefabs to match your preference.
- Create a Character prefab by making an empty object in the **Characters** object in the inspector, 
naming it with the character's name, 
setting the object to be centered on the bottom left corner, 
setting the size of the object to the right proportions for the character sprites you'll be using and adding in images state images(base and expression for multi-layer characters), 
then dragging the object into Prefabs/characters and assigning it to **Character Prefabs** in the **CharacterManager** script and removing the object from the inspector. 

That's it! You're ready to create your own visual novel! Good luck in your journey and if you have any problems figuring out the engine--

Head to the docs page for an explanation on how to use one or another feature
