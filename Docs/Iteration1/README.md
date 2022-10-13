# Tanks

## [Iteration 1](https://github.com/users/cseitz/projects/11/views/2?filterQuery=iteration%3A%22Iteration+1%22)

### [Project Board](https://github.com/users/cseitz/projects/11)
- [Iteration 1](https://github.com/users/cseitz/projects/11/views/2?filterQuery=iteration%3A%22Iteration+1%22) progress
- Lists assigned tickets for each iteration
- Completed and imcomplete tickets

## Comments

Iteration 1 primarily consists of me learning how to use Unity while performing initial tasks for the project.

#### Important Note
In the following sections, I will mention "side projects" that include code not present in this submission.
Given that I do not want to mess with what I have working, it feels safer for me to use separate Unity projects to do testing
and prototyping. Even though I have git set up, it is less concerning if I use another project.

I am working on preventing this in the future, since I know you want everything in one submission.

### Multiplayer Server

Before the project began, I found some tank assets and built a websocket server prototype.
I will be reimplementing the websocket server in iteration 2.
Because of the prototype, I know how to properly implement the socket connection in Unity; so even though said work is not
present in this project, I will be able to make it work.

### Camera

A simple camera system that moves a Cinemamachine camera is in use.
The camera orbits the tank based on mouse movement.

### Tank Aiming

Tank aiming, as it is in the current project, is quite mediocre.
I am using a combination of weird math to make it work, and it doesn't work very well and has many transition issues.

Although this is the case, I have properly rigged major portions of the tank and combined it with the camera system
for buttery smooth movement.

Here is a video of my progress.

<video src="./initial_aiming.mp4"></video>

#### Rewritten

Thanks to the `Roll A Ball` Assignment, I have learned much more about how Unity works and I have begun re-rigging the tank using **Hinges** and other colliders.

The new tank controller, which is only present in a separate project and not yet merged into this one, does a much better job at rotating the tank turret and barrel.

<video src="./improved_aiming.mp4"></video>

The source code for this is attached [here](./improved_aiming.cs).

This will be merged into the main project once I finish tank movement.

## Tank Movement

Currently, tanks do listen to user input for their movement; but its really janky and I am working on improvements in the `Rewritten` side-project mentioned above.
Reimplementing the tank rig has been very useful so far, and overall the final implementation of the tank is shaping up quite nicely.

## Concerns

I am concernced about some of the physics in unity. The physics of things seem to be the main block for most of my features.
For example, resolving friction and applied forces only working in specific situations, stuff like that.

I am working on figuring these out little by little, but ultimately that is the only issue I forsee.

Ultimately, I think I am making good progress given that I am almost at the point of tanks being able to move and shoot.
Once they can move, all that is left is projectiles, syncing data to the multiplayer server, and then minor features to make the game feel better.



