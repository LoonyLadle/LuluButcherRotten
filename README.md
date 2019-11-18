# Lulu's Butcher Corpses
 
Meaningful butchery of rotten and dessicated corpses.

## Gameplay Notes

Corpses which are not fresh do not yield their full butchery product. Items which deteriorate at a speed of more than 0.5/day will be destroyed. Items that rot will spawn rotted if possible or else be destroyed. This usually means that meat and leather will be destroyed, but other butcher products such as thrumbo horns or elephant tusks will be preserved. Lastly, if the corpse is rotting but not dessicated the butcher will get a strong negative thought.

For animals that don't have special butcher products, you probably want mods that add additional products (like [LuluBones](https://github.com/LoonyLadle/LuluBones)!) for this to be useful.

## Technical Details

This mod changes the worker class of the special filter AllowRotten to one which only allows rotten, not dessicated, items. A new special filter AllowDessicated was added to fill the created gap. This allows more fine grained control over what non-fresh corpses to butcher. This should not cause any unexpected problems, but other mods may expect the modified filter's previous behavior.
