A line pattern is a collection of lines that all spawn at the same time.
For example, a pattern for two diagonal red lines that originate from the
top-left and top-right of the screen would be:

---
name: red diagonal x top
lineTemplates:
  - lineType: Diagonal
    origin: top-left
    color: red

  - lineType: diagonal
    origin: top-right
    color: red

Each line pattern has a name and at least one line template. A line template
describes a single line. The name of the line pattern is not used in code
(yet?), but can help identify the pattern more easily than reading all the
templates.

Each line pattern must also START (not end) with three - symbols on their
own line.

Values for line templates:
* lineType: either "orthagonal", "diagonal", or "rotate"
* color: either "red" or "cyan"
* origin: where the line starts at. Values are specifc to lineTypes
* isClockwise: "true" or "false". Cannot be "0" or "1". Used only for
	rotate lineTypes

Othagonal Lines
---------------
These lines are either vertical or horizontal depending on their origin.

Valid origin values: "top", "left", "bottom", "right"


Diagonal Lines
--------------
These lines are rotated 45 degrees compared to orthagonal lines and start
in the corner defined by their origin, then move diagonally across the screen
to the opposite corner.

Valid origin values: "top-left", "bottom-left", "bottom-right", "top-right"

Note that the vertical component must come first, so "left-top" is invalid.


Rotate Lines
------------
Lines that have a center of rotation (origin) based in a corner of the screen
and rotate either clockwise or counter-clockwise. These are the only lines
that the "isClockwise" property of a lineTemplate applies to.

Valid origin values: "top-left", "bottom-left", "bottom-right", "top-right"

Note that the vertical component must come first, so "left-top" is invalid.
