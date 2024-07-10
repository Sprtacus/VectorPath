<p align="center">
  <img src="https://github.com/Sprtacus/VectorPath/blob/main/VectorPath/Logo/Icon%202.png?raw=true" alt="VectorPath Logo">
</p>

# VectorPath
VectorPath is a high-performance 2D pathfinding algorithm for Unity, designed to handle dozens of actors efficiently. It can be used with a tilemap in Unity, where all actors navigate to a single destination.

## Demonstration Video
Watch the demonstration video to see VectorPath in action:

https://github.com/Sprtacus/VectorPath/assets/61117535/55c1b8d9-9de9-4ab1-b8fd-20101de4b964

## Features
Flowfield Pathfinding: The repository includes the core logic for generating and updating the flowfield.
Example Scene: An example scene is provided to demonstrate how to set up and use VectorPath in your Unity project.
Optimized Performance: The flowfield refresh is optimized to maintain performance even with many actors.
## Usage
**Integration:** Add the VectorPath scripts to your Unity project.  
**Tilemap Setup:** Ensure your scene uses a tilemap for pathfinding. Add the *Navigation Manager* and *Navigation Flow Field* to the *Grid*. Add your Tilemap with the walkable floor in the *Navigation Flow Field* -> *Ground Tilemaps*.  
**Single Destination:** All actors will move towards *Target Position*.  
**Actor Logic:** Implement your own actor movement logic. Use the Navigator class to get the direction each actor should move in.
## Future Features and Known Bugs
**Single Flowfield:** Currently, only one flowfield can be active per level.  
**Collision Issues:** Actors may get stuck in corners if collision is disabled.  
**Out of Flowfield:** Actors may get lost if they move outside the flowfield area.  
## Contributing
Contributions are welcome! Please fork the repository and submit pull requests for any enhancements or bug fixes.

## License
This project is licensed under a Creative Commons Attribution-NonCommercial 4.0 International License. You must give appropriate credit if you use this project, and you may not use it for commercial purposes. For more details, see the LICENSE file.
