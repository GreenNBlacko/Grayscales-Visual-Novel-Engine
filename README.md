# Grayscale's Visual Novel Engine
Welcome to the world of visual novel development!

This engine is based on unity and is made for ease-of-use and is highly focused on localisation.

## DISCLAIMER
This project is still a Work-In-Progress(WIP), some features may not be implemented or function as intended

## Features
- Low to no code development cycle
- A graphical script editor
- Easily extendable libraries for advanced users
- A drag-and-drop setup process with modules that inform you of missing dependencies

## Installation
- Download and install your preferred unity version(suggested version: 2022.3.43 for best support).
- Download the latest release from the releases page.
- Extract the .zip file in an empty folder, preferably named after your project(e.g: Falling leaves of October).

## Usage
- Add the project in unity using the **'Add'** button and navigating to your project files.

![image](https://user-images.githubusercontent.com/45263750/132753210-2fcea2a1-433f-4dc8-9882-e6b74ed23fd2.png)

- Launch the project and start by creating a new scene.
- Create a new empty game object and name it `ScriptManager`.
- Add a component named `MainManager` onto the project.

![image](https://github.com/user-attachments/assets/0d3d4349-f44b-41f9-8817-9ce0fd85f47b)

- Right click in the `project` window.
- Then, go to `Create>Sentence Manager` like shown in the image.

![image](https://user-images.githubusercontent.com/45263750/132752576-bbe18e99-7a1c-4d61-b578-c1214cb63af8.png)

- Name the file to your preference and don't forget to assign it to the `MainManager` script

![image](https://github.com/user-attachments/assets/3779e2d8-2501-4661-a5c7-34f29b44fdf9)

- Now, create a canvas and name it `UI`, attach the `UIManager` component to it

![image](https://github.com/user-attachments/assets/41cfbcb1-3e9f-456f-8358-c1991d8ecd56)

- Then, create your textbox using UI elements, don't forget to add a `TextBox` component to the textbox and assign the `name`(optional) and sentence `text` objects

![image](https://github.com/user-attachments/assets/d5b776d5-57c4-4846-b34f-1a614e29ac6d)

![image](https://github.com/user-attachments/assets/4918ce1b-7381-4ed8-a8cf-9c8cc531df6d)

- Open the custom editor util panel if it is not already open, this will allow you to modify the script

![image](https://github.com/user-attachments/assets/67ec825c-5867-48c7-ae45-122b7a421a51)

![image](https://github.com/user-attachments/assets/465bcfa0-6393-4693-a669-c8846d8841ed)

This will give you the utmost basic setup to be able to develop your story, if you wish to add character sprites, add the `CharacterManager` script to the `ScriptManager`, `AudioManager` for audio and `CGManager` for backgrounds
