screengrab todo
===============

Bugs
----

Deleting the last image leaves the highlights on the screen

Issues
------

Clipboard clears data on exit. Need to keep or at least give an option to clear on exit.
Memory consumption
* Doesn't appear to release resources
* Maybe have a mem check on the screen or a limit on the number of grabs

Enhancements
------------

* JPG/PNG option - quality setting on settings UI
* License Screen - store property when viewed
* About screen
* Registration
* Change log on update

* user properties
* set thumbnail size
* set shadow on preview
* save shadow with image
* save as png / jpg

Highlight 2 images, then subtract one from the other and show the differences - need the replay option to make this any use

Need to remember the last selected / taken so if you delete them all you can still replay the last one...

Toobar button animation - http://msdn.microsoft.com/en-us/library/bb613545.aspx

Add tag/note to thumbnail
some way to constrain the selection ratio....

finish highlighting
* store highlights with image
* annotation on image
* Lines / circles / other shapes
* Grid (Ctrl?) + snapto option

cropping on cropped image
multi crops on on image

Save all - select file to use as prefix (options to determine file pattern) and save all images with ~1, ~2, etc

preview on mouse over.... zoom control on t'internet
Drag drop thumbnails to reorder

Timed grab (5 second self timer)
Grab based on keyboard / mouse action ??

Completed
---------

* v1.0.19
Need to save as JPG by default - png doesn't work in all mail clients when pasting...
Need to make sure the highlights are persistent. They disappear if you do another capture
Move the highlights into the grabbed image class, and render them from there...

* v1.0.18
Key press (escape and enter) don't work

* v1.0.17
Settings screen work. Can now set where the captured screen appears
Highlight colors in combo, and saved with highlight

* v1.0.16

v1.0.15
* replay screen grab (last or selection)
* border moves image off by a pixel
* Make border thicker, surround whole image and float over the image.
* Select screen offset for screen ID not correct - only uses the x position
* Select last grab when done, rather then just the first at the moment
* coords (start, end + dimension) on selection
* click once integration - use API and provide some options.
* Don't update on start up
* kick off a thread to check for updates
* display an animation if a new version is available
* move list box to top and constrain the largest dimension to make the image fit in the thumbnail size
* Redo listbox using graphics code in other project
* Show grab on selected screen rather than screen 1
