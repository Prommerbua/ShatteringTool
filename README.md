## Table of Contents

- [Object Destruction / Voronoi Shattering](#object-destruction--voronoi-shattering)
  - [Swapping with Prefractured Object](#swapping-with-prefractured-object)
  - [Voronoi Diagram-Based Destruction](#voronoi-diagram-based-destruction)
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

# Built With

- Unity

# Authors

- [Prommerbua](https://github.com/Prommerbua)

# License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
