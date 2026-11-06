# ITU-CS-464-LAB-06-BSSE23040

CS-464 Game Development, Lab 06. I made the arena game from Lecture 07 (enemies, obstacles, power-ups) and then turned it into 5 levels. Each level has its own layout and its own goal, and they get harder one after the other.

Unity 6000.6.2f1 (URP), C#. Roll number: BSSE23040.

## How to play

Open `Assets/Scenes/Level01_WarmUp.unity` and press Play. The five levels are in the build settings in order, so the **Next Level** button on the win panel goes through the whole game. After Level 5 you can play again.

Move the ball with `W A S D` or the arrow keys. You have 3 lives with 3 hearts each. If you run out of lives or time, the lose panel lets you restart the level.

## The 5 levels

| | Level | Objective | What is in it |
|---|---|---|---|
| 1 | Warm-up | Collect all 8 coins | An open room and 3 spike patches. No enemies, no timer. |
| 2 | First Guard | Reach the green goal | Two rooms with a door between them, spikes in the doorway and a guard that patrols the second room. |
| 3 | Against the Clock | Collect 10 coins in 45 seconds | A crusher with a coin on it, a guard that walks back and forth, spikes and a speed power-up. |
| 4 | Survive | Stay alive for 45 seconds | Two sweepers, two crushers, a shield and a speed power-up, and waves of chasers that get bigger. |
| 5 | The Gauntlet | Collect 3 coins, then reach the exit in 90 seconds | Lava pools, a sweeper, a crusher on a door, guards that patrol, chase and go back, spikes and both power-ups. |

It gets harder because each level has more kinds of danger, the enemies are faster, the timers are shorter, and the goal changes: collect, reach, beat the clock, survive, then everything together.

![Level 1](Docs/screenshots/level1.png)
![Level 2](Docs/screenshots/level2.png)
![Level 3](Docs/screenshots/level3.png)
![Level 4](Docs/screenshots/level4.png)
![Level 5](Docs/screenshots/level5.png)

## The scripts

These are the scripts from the lecture, plus a few more that the levels needed.

| Script | What it does |
|---|---|
| `PlayerController` | Reads the keys in `Update`, pushes the ball with `AddForce` in `FixedUpdate`. `BoostSpeed` runs the speed power-up as a coroutine on the player. |
| `Health` | The hearts. It gives 1 second of i-frames after a hit, so standing on spikes costs one heart a second and not fifty. It dies at `<= 0` because the crusher hits for 2. |
| `GameManager` | One per level. Keeps the score, lives, timer and objective, respawns the player, shows the win and lose panels, and loads the next level. |
| `Hazard` | A trigger that hurts and knocks the ball back with one impulse. With a rest time it becomes a crusher: rest, warn for 0.7 s, then strike. |
| `EnemyPatrol` | Walks a list of waypoints. Loop, PingPong or Random. |
| `EnemyGuard` | A small state machine on top of the patrol: Patrol, Chase when it sees the ball, Return when it loses it. Seeing uses range, field of view and a raycast, so walls block it. |
| `MovingHazard` | A sweeper that slides back and forth on a steady rhythm. |
| `KillZone` | Lava. It skips the hearts and takes a life at once. |
| `PowerUp` | A speed boost for 5 seconds, or a shield that absorbs one hit. |
| `Coin`, `Goal` | A coin reports to the GameManager. The goal stays grey until the objective lets it open. |
| `Chaser`, `WaveSpawner` | Small enemies that walk at the ball and give up after 12 seconds, sent in waves of 2, 3, 4 and so on. |

## Choices I made

- **Each level sets its own GameManager values.** The objective, coin target and time limit are set in the Inspector, so one script works for all 5 levels.
- **Running out of time is a loss in every level except Level 4.** There the timer is the goal, so reaching zero wins.
- **Guards are slower than the ball**, so you can always run away from them. Chasers are slower than guards.
- **A respawn waits 0.6 seconds** and then gives 1.5 seconds of protection, so the knockback from the hit that killed you does not push the new ball away.
- The ball cannot jump, so there are no gaps to fall into. Lava pools are the floor danger instead.

## Folders

```
Assets/Scripts     all the scripts above
Assets/Prefabs     Player, Coin, Guard, ChaseGuard, Crusher, Sweeper, Chaser, SpeedPowerUp, ShieldPowerUp, Goal
Assets/Materials   one material per colour
Assets/Scenes      Level01_WarmUp ... Level05_TheGauntlet
Docs/screenshots   pictures used in this README
```

## Git

I worked on the `development` branch with small commits: the project first, then each script, then one commit per level. Then I merged it into `main` with a pull request.
