Advanced Polygon Collider for Unity
(c) 2015 Digital Ruby, LLC

Version 1.0.6

Change Log:
1.0.6	-	Update to Unity 5.6.5
1.0.5	-	Added half pixel of padding to polygon to better match the edges
1.0.4	-	Fix offset other than 'center' to work for sprites
1.0.3	-	Add scale parameter and added RecalculatePolygon method to force a refresh if all that has changed is the sprites underlying data
1.0.2	-	More bug fixes, and remove islands and holes parameters. These are always true now.
1.0.1	-	Small tweaks, bug fix to try and prevent crash / freeze
1.0		-	Initial release

Advanced Polygon Collider is a fast and easy way to manage the shapes of your 2d physics objects and is a significant upgrade from the standard Unity PolygonCollider2d component.

Requirements:
- You must be using Unity sprites. 2d toolkit sprites, etc. are not yet supported.
- Your textures must be set to read/write and in sprite mode, with "Full Rect" mesh type option and no mip-maps.

Instructions:
- Setup your sprite texture first. Single or multiple sprite are supported. Use full rect for multiple sprites.
- Set the compression type to automatic true color for best results
- Create a sprite game object and add your sprite texture
- Drag the AdvancedPolygonCollider script on to your game object
- Tweak the script parameters until the points are to your liking
- Most often you only need to tweak the "Distance Threshold" parameter
- Scale your polygon depending on your needs
- See the Demo scene on how to do animated sprites
- Please watch the tutorial video (https://www.youtube.com/watch?v=Z_4jb6WUxgo) for a complete overview

Please send me feature requests, feedback or bug reports to jeff@digitalruby.com

Thank you!

- Jeff Johnson

