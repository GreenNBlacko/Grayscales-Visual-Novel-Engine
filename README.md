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
![image](https://user-images.githubusercontent.com/45263750/132753210-2fcea2a1-433f-4dc8-9882-e6b74ed23fd2.png)
- Launch the project and start by creating a new scene.
- Create a new empty game object and name it **'ScriptManager'**.
- Put these scripts in it: **DialogueManager, CharacterManager, ChapterManager, SaveLoadSystem, CharacterManager, JSONSentenceImporter**(only add this one if you plan to import sentences using a JSON file).
- Right click in the 'project' window.
- then, go to Create>Sentence Manager like shown in the image.
![image](https://user-images.githubusercontent.com/45263750/132752576-bbe18e99-7a1c-4d61-b578-c1214cb63af8.png)
- Name the file to your preference and don't forget to assign it to the **'DialogueManager'** script
- Now, create a simple UI using the prefab given in the prefabs folder.
- Assign all the variables you intend to use with your UI in the **'DialogueManager'** script.
![image](https://user-images.githubusercontent.com/45263750/132753604-e4f976b2-580d-40b7-809e-ba1b92fa1a71.png)
- Edit the choice and backlog prefabs to match your preference.

That's it! You're ready to create your own visual novel! Good luck in your journey and if you have any problems figuring out the engine--

Head to the docs page for an explanation on how to use one or another feature
