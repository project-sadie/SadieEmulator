using Sadie.Game.Rooms.PathFinding.Collections.MultiDimensional;

namespace Sadie.Game.Rooms.PathFinding
{
    /// <summary>
    /// A world grid consisting of integers where a closed cell is represented by 0
    /// </summary>
    public class WorldGrid : Grid<short>
    {
        /// <summary>
        /// Creates a new world with values set from the provided 2d array.
        /// Height will be first dimension, and Width will be the second,
        /// e.g [4,2] will have a height of 4 and a width of 2.
        /// </summary>
        /// <param name="worldArray">A 2 dimensional array of short where 0 indicates a closed node</param>
        public WorldGrid(short[,] worldArray) : base(worldArray.GetLength(0), worldArray.GetLength(1))
        {
            for (var row = 0; row < worldArray.GetLength(0); row++)
            {
                for (var column = 0; column < worldArray.GetLength(1); column++)
                {
                    this[row, column] = worldArray[row, column];
                }
            }
        }
    }
}