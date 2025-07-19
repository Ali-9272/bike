# Wheelie Master: Moto Ride 3D Clone

A Unity 3D motorcycle wheelie game inspired by the popular mobile game "Wheelie Master: Moto Ride 3D". Master the art of motorcycle wheelies in this thrilling endless runner game!

## Game Features

- **Realistic Wheelie Physics**: Authentic motorcycle wheelie mechanics with balance and momentum
- **Endless Gameplay**: Procedurally generated road segments and obstacles
- **Mobile-Friendly Controls**: Touch controls optimized for mobile devices
- **Score System**: Compete for high scores and perfect your wheelie skills
- **App Store Style UI**: Clean, modern interface similar to mobile game listings

## Game Overview

### Wheelie Master Studio
*Contains ads • In-app purchases*

Master the art of motorcycle wheelies in this thrilling 3D racing game! Balance your bike, avoid obstacles, and maintain your wheelie for as long as possible to achieve the highest score.

## Controls

### Desktop
- **W Key / Left Mouse**: Accelerate
- **S Key**: Brake/Slow down
- **Space / Right Mouse**: Perform wheelie
- **Escape**: Pause game or return to main menu

### Mobile
- **Left Side of Screen**: Touch to accelerate
- **Right Side of Screen**: Touch to perform wheelie

## Game Mechanics

1. **Wheelie System**: Hold the wheelie button to lift the front wheel and perform wheelies
2. **Balance**: Maintain balance to avoid crashing backwards or forwards
3. **Speed Control**: Manage your speed to maintain control
4. **Obstacles**: Avoid obstacles on the road to keep your run going
5. **Scoring**: Earn points based on distance traveled and wheelie duration

## Scripts Overview

### Core Scripts
- `GameManager.cs`: Main game controller handling game state, scoring, and UI management
- `MotorcycleController.cs`: Motorcycle physics, input handling, and wheelie mechanics
- `UIManager.cs`: UI system management and app store style interface
- `CameraController.cs`: Third-person camera following the motorcycle with smooth movement
- `EnvironmentController.cs`: Endless road generation and obstacle spawning

### Key Features
- **Physics-based wheelie system** with realistic balance mechanics
- **Endless road generation** for unlimited gameplay
- **Mobile touch controls** with intuitive left/right screen areas
- **High score system** with persistent data storage
- **Visual effects** including particle systems and screen shake

## Setup Instructions

1. Open the project in Unity 2022.3 or later
2. Open the main scene: `Assets/Scenes/MainGame.unity`
3. Create or assign the following prefabs:
   - Motorcycle prefab with Rigidbody and MotorcycleController script
   - Road segment prefab for procedural generation
   - Obstacle prefabs for gameplay challenge
   - Environment objects (trees, buildings, etc.)

## Customization

### Motorcycle Physics
Adjust these parameters in `MotorcycleController.cs`:
- `acceleration`: How fast the motorcycle accelerates
- `maxSpeed`: Maximum speed limit
- `wheelieForce`: Force applied during wheelies
- `balanceForce`: Force that helps return to normal position
- `maxWheelieAngle`: Maximum wheelie angle before crash

### Environment Generation
Modify these settings in `EnvironmentController.cs`:
- `roadSegmentLength`: Length of each road segment
- `obstacleSpawnChance`: Probability of obstacle spawning
- `environmentSpawnChance`: Probability of scenery object spawning

## Mobile Build Settings

For mobile deployment:
1. Switch platform to Android or iOS
2. Set appropriate resolution and orientation (Portrait recommended)
3. Configure touch input in Player Settings
4. Optimize graphics settings for mobile performance

## Future Enhancements

- Multiple motorcycle models and customization options
- Power-ups and special abilities
- Multiplayer leaderboards
- Achievement system
- Additional game modes (time trials, trick challenges)
- In-app purchases for cosmetic items

## Technical Requirements

- Unity 2022.3 LTS or later
- Supports PC, Mac, Android, and iOS platforms
- Minimum RAM: 4GB (8GB recommended)
- Graphics: DirectX 11 or Metal support

## License

This project is created for educational purposes as a game development example. Commercial use requires proper licensing and attribution.

---

*Master your wheelie skills and become the ultimate Moto Ride champion!*