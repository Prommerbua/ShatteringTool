## Table of Contents

- [Object Destruction / Voronoi Shattering](#object-destruction--voronoi-shattering)
  - [Swapping with Prefractured Object](#swapping-with-prefractured-object)
  - [Voronoi Diagram-Based Destruction](#voronoi-diagram-based-destruction)
  - [Screenshots](#screenshots)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installing](#installing)
- [Built With](#built-with)
- [Authors](#authors)
- [License](#license)




# Object destruction / Voronoi Shattering

This repository contains the project for my master thesis, in which I implemented two methods for destroying objects in video games using Unity. The goal of my thesis was to provide an overview of the current techniques used for object destruction in video games and to give insight into their implementation. In addition, I compared the performance, quality, and complexity of these algorithms.

The following methods were implemented:

## Swapping with prefractured object
The prefractured method is a simple but very effective method for most cases. Basically, you create a fractured counterpart of your asset and replace it during runtime when fracturing occurs.
## Voronoi diagram-based destruction
This is a more complex method to implement but has the possibility for a higher quality if implemented correctly. This is because the fragments are generated during runtime, so you can create the fragments based on an impact point from a projectile and create more realistic results.

## Screenshots

![Screenshot of the game](/path/to/screenshot.png)
![Screenshot of the game](/path/to/screenshot.png)
![Screenshot of the game](/path/to/screenshot.png)

# Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

## Prerequisites

To run this project, you will need to have Unity installed on your machine. You can download the latest version of Unity from the [Unity website](https://unity.com/).

## Installing

1. Clone this repository to your local machine using `git clone https://github.com/YOUR_USERNAME/destroy-object-in-video-games.git`
2. Open the project in Unity by going to `File` > `Open Project` and selecting the project folder.
3. You should now be able to run the project by clicking the play button in the Unity editor.


# Built With

- Unity

# Authors

- [Prommerbua](https://github.com/Prommerbua)

# License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
