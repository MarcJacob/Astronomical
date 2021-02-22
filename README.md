# Astronomical
An attempt at building a simple Universe simulator, where the player can travel around a Universe with realistic astronomical scales

## Objectives

Main :

- Create a universal positioning system that allows things to be placed meters away from one another just as well as light years away.
- Being able to seamlessly travel at any point in the universe, to the millimeter.
- Being able to see far away things in a realistic way.
- Generate a "complex" universe with galaxies, star systems, planets.

Secondary :

- Use gradual procedural generation instead of generating everything at once.
- Support multiple entities moving around and interacting with one another.
- Being able to move large celestial bodies to simulate orbits
- Rendering large celestial bodies properly when approaching them (planet & star surfaces, asteroids, moons...)
- Make it all look good

## Current code

Currently a galaxy gets generated with a central black hole and star systems (composed of only a single star) located from a handful to 52000 light years away (parameterizable, following the Milky Way's figures). It is done through a *Zone Generator* scriptable objects system, where the *Universe Generation Manager* makes use of a single object to generate the *root zone / anchor*. That object in turn can make use of other objects to generate sub-zones like star systems. This means universe generation is extremely extendable and parameterizable : the idea is to be able to generate varied universes of all sizes (a single planet, star system, galaxy, group of galaxies...)

The stars and black hole are rendered from very far away using a billboard system of sorts that creates draw calls according to the current structure of the universe and where the point of observation is. The player ship (IE the point of view) can be located in any direction, at any amount of light years away from the center of the universe.

All objects (stars and blackholes) are currently simple, very small spheres. If the ship goes *exactly* on top of one of these objects, they can fly around at sublight speed and see them moving accordingly.

### Anchors

The main principle I'm working with is the idea that the position of objects should always be defined in relation to some nearby, arbitrarily chosen point (usually corresponding to the next major object of a higher "scale"). These points are themselves defined in relation to another, higher-scale point in a tree fashion until reaching the *root* of the universe which, as of now, is on the scale of Light Years.

These points, named "*Anchors*", use their scale (currently either Light Years, AUs, or Meters), a distance (in said scale unit) and a direction (as a normalized 3D vector) to define where things "anchored" to them directly are. For example, star system *anchors* (which work in AUs) are anchored to the galaxy / black hole *anchor*, and their position relative to it is defined in light years and a direction.

This implies that *moving* these star systems can only be done in units of light years which, even though distances are defined as decimal numbers, might be a little imprecise. This general issue of imprecision (which is what makes having very large distances in a 3D world a technical challenge in the first place) is what I aim to solve with this "multi scale" system. 

In order to place the *Player Ship* accurately (*to the meter*), a system of *Dynamic anchors* is being put in place : at any moment, the simulation tries to find the best "natural anchor" (pre-generated anchor) to anchor the ship to. Now, if that natural anchor is one defined to the meter, then problem solved. However, that is usually not the case (never currently, as the lowest scale anchor that naturally exists are star systems which are defined in AUs) and thus we need to dynamically generate a chain of anchors for every unit of scale until we reach Meters.

The idea is to have these dynamic anchors allow things to move / happen / exist at a meter precision even in empty interstellar space where there wasn't anything at all at universe generation. That includes the player's ship. It also helps the rendering system which relies on an "observing anchor" as opposed to the actual player ship position.

### Next moves ?

Probably refactor a lot of the code. Yes, already. Initially I was unaware of the Decimal type, and after reading about it it seems like a very powerful type I could make use of. Though the fact it is 128 bits means that it's probably not the most clever solution to throw it at the problem until we can lazily have everything exist at the meter scale (which is something I considered doing - defining everything from the center of the universe or any other Light Year scale object as a 3D vector of Decimal values).

Also, the whole "anchor" structure needs cleaning up and more strict rules about how to navigate the tree and how parenting works. Currently there is no encapsulation and that can only lead to worsening code quality with continued development.
