# Bobs Graph
[Hearthstone Decktracker][1] plugin to visualize the damage distribution of the simulation of Bobs Helper.

## Install
- Install [Hearthstone Decktracker][1]
- Download the plugin dll: [Release page][2]
- Follow the [plugin installation instructions][3]

## Features / Explanation
![Bobs Graph in action](/readmeMedia/graph.png)
- The graph uses only data which is given by Bobs Helper simulation.
- Bottom numbers showing min and max possible damage.
- Each bar stands for the damage between min and max.
- The height of the bars show the probability of the damage.
- The height is scaled by the highest probability.
- The white bar marks the tie probability (zero damage).
- If there are lethal chances, red and green areas will mark the damages for it.

## Known issues
- Access to Bobs Helper simulation data is kinda hacky.
- No hide / hide while combat / etc. is implemented.
- Sometimes, the graph will not refresh while combat.
- Dragging the graph is implemented, but mouse events are ignored.


[1]: https://github.com/HearthSim/Hearthstone-Deck-Tracker
[2]: https://github.com/Wasserwecken/BobsGraph/releases/
[3]: https://github.com/HearthSim/Hearthstone-Deck-Tracker/wiki/Available-Plugins