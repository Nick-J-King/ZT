# Zero Triangles
Show the configuration of "zero width" triangles.

The configuration cube, conceptually, goes from 0 to 1 in the X, Y & Z directions.
The points within this configuration cube represent the edge lengths of putative triangles.
If a triangle IS indeed possible, then that point is "inside" the figure.
If a triangle is NOT possible, then that point lies "outside" the figure.
Thus, the triangles that are of zero area lie on the surface of the figure.

Additional edges (4th edge / 5th edge) are selectable.
These edges are used in conjunction with the three indicated in the configuration cube
to determine whether a triangle is possible, is not possible, or lies on the surface
of "Zero Triangles".

The volume of the figure is also calculated. So if the volume is 0.5, then there
is a 0.5 probability that a triangle can be formed from the edges indicated by
the configuration cube (and possibly allowing selection from any additional edges).

To make calculations exact, the number of divisions is multiplied by 12.
This way, any edges or corners of the "zero surface" are guaranteed to be on integer coordinates.
In this scenario, the surface is formed only by "flats" (an outer face of the cube) x 6, "diagonals" (a facet
cutting across the diagonal of a face) x 6, or "corners" (a triangle through 3 points of the cube) x 8.
The volume a calculated by looking at sub-pieces of the cubic cells.
