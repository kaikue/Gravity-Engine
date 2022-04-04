# Gravity-Engine

## TODO

### Mechanics

- Custom gravity for crates etc.? or use builtin?
- Fix sprite layer ordering
- More crate friction, better physics when jumping under crate, make crate image smaller, make crates stick to head or other moving objects below
- Use gamepad right stick to change gravity
- Make jump faster (more powerful + increase gravity- keep 2 tiles)
- Animate light on backpack
- Background effects- particles fall in gravity direction (with trail)
- Spikes? make you restart
- Conveyor belts? move things on top of them
- slippery surfaces? that you can't flip off of, only walk off
- sticky surfaces? that you can't walk on, only flip off of
- sticky crates- not affected by gravity when attached to wall, can be pushed
- creatures that push crates (different types)
	- cling to floor and climb up/down walls- not affected by gravity
	- turn around when at edge- fall with gravity
		can get squished by crates? respawn? or carry crates on head
		kill you on contact?
- antigravity fields- gravity goes in opposite direction (or fixed direction?)
- magnets???
- hub overworld leads to single-screen puzzles- exploration and movement
	- collect machine part? in each level to fix your spaceship, unlock doors in overworld (multiple level options)
	- portals suck you into levels automatically at first, then you can manually trigger to replay? allows blocking sequence

### Puzzles

- Land crate on own head, use to press button on ceiling, unlocks door that crate 2 falls out of (you can't press the button upside down because then crate 2 won't fall)
- move during midair fall
- land crate & self simultaneously
- pass crate over button while passing through door
- stack crates to position before flipping gravity
- flip your own gravity but that will move a crate off a button- stack another crate on top of it so it hits the ceiling
- fall alongside crate to press two buttons at the same time
- get crate stuck in door so it doesn't close

### Art

- Use palette for everything
- Player jump, fall, push sprites
- Color tilemap (GameManager? or will that be persistent)
- graphical bg effects (colored falling particles) show direction of gravity

### Sound

- Button press/unpress
- Lever flip/flip back
- Door open/close
- Player jump, land, walk
- Crate land

# Credits

- Endesga 36 palette by Endesga: https://lospec.com/palette-list/endesga-36
