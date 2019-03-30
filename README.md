# Autodoka

A simulation with simple custom 2D physics.

## Features
- All initial parameters are set up from JSON config file in the assets folder.
- It's possible to add as many unit colors as needed in SimalationField object in the scene.
- The program contains one main entry point acting as composition root.
- A separate physics class for units. It can be easily replaced with interface to implement different versions of physics.
- Game physics implements simple chunking technique for faster calculations.
- Collisions response is located in the Unit class as a kind of "physics material". It might be moved to physics.
- The physics doesn't contain continuous collision detection feature thus high movement speeds might cause problems.

