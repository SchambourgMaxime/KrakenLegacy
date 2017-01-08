Monsters of the Deep
Created for the Unity 3D game engine, by Trideka (Jess Triska)

Support : jtriska@gmail.com


Thanks for purchasing my asset pack! I hope it serves you well. As follows are notes on usage and modifying it for your own purposes.

All assets include their own animations, and are all configured to work with Mecanim. 

Included are the following assets:
	
	Angler Fish
		A feisty little chomper of the deep. I call her Gilda, but you can call her whatever you like. 
		She has the following animations:
			Idle
			Swim
			Bite
			
	Hatchet Fish
		A common creature of the deep. Highly reflective, with razor sharp teeth.
		Included are the following animations:
			Idle
			Swim
			Bite
			
	Vampire Squid
		Feared by people over centuries, in reality this monster is generally harmless. But they look cool!
		Due to the more complicated nature of this creature, there are more animations to give better control within Mecanim and your game.
		Included are the following animations:
			Idle
			Swim Left, Swim Right, Swim forward, Swim back, Swim up
			Drift Down
			Squirt
			

Possibly more to come later. Suggestions? Email me! (jtriska@gmail.com)	

Original model sources to all the assets can be found in Sources.zip 

Each is fully rigged. Documentation on how to animate and export for use in Unity from Blender is beyond the scope of this documentation. 
Feel free to email me (jtriska@gmail.com) should you need some help. 

Also included are a few shaders, crafted with the excellent Shader Forge tool by Joachim Holmer (http://acegikmo.com/shaderforge/)
Should you want to modify the shaders, I HIGHLY recommend purchasing Shader Forge. 
Please note that these shaders are not mobile friendly and require a graphics card capable of Shader Model 3.0 instructions.

The following shaders are included:
	MoTD_Body
		A shader with simple SSS that also provides a nice emissive glow on select regions based on vertex color
		Used on all assets included
	MoTD_Eyes
		A shader that gives a soft translucent and reflective look to the eyes of the Vampire Squid and Hatchet Fish assets
		
Each asset uses a vertex color to control the photofore (glowing) elements of the creature. This vertex color is RED. The shader parses the mesh for 
vertex colors, and assigns an emissive glow texture, masked by the red channel. Keep this in mind should you want to alter the models and their 
photofore regions for yourself.

You will also find 2 examples in the /Examples folder.
	Slideshow
		Just a simple showcase with a few blends between animations for each model
	Squid Control
		An example of using Mecanim and a simple script to control the vampire squid in an interactive manner.
		
	Both examples use their own AnimatorControllers that will likely not be best suited for you own purposes. Use them as a guideline, as it is
	suggested that you author your own. See the Unity documentation and Unity3d.com/Learn for more information.
		
Problems? Suggestions? Again, please email me! jtriska@gmail.com

				