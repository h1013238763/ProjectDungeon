using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : BaseController<MazeController>
{
    public Room[,] maze = new Room[5,5];                        // the maze
    private int[] room_minimum = { 1, 1, 1, 1, 4, 0, 1, 1 };    // minimum room number require
    private int[] room_maximum = { 1, 3, 3, 5, 25, 2, 5, 1 };   // maximum room number require
    private int[] room_count = new int[8];                      // current room number

    public void MazeGenerator(int maze_level)
    {
        StartRouteGenerate();


    }

    private void StartRouteGenerate()
    {
        // form route from start point to boss room
        Queue<Room> route = new Queue<Room>();
        // 1 start point
        route.Enqueue(new Room(RoomType.Complete));     
        // 2 normal enemies
        route.Enqueue(new Room(RoomType.Enemy));
        route.Enqueue(new Room(RoomType.Enemy));
        // 1 elite enemy
        route.Enqueue(new Room(RoomType.Elite));
        // 2 normal enemies
        route.Enqueue(new Room(RoomType.Enemy));
        route.Enqueue(new Room(RoomType.Enemy));
        // 1 normal enemies or 1 rest room depends on quest
        // if(true)
            // route.Enqueue(new Room(RoomType.Enemy));
        // else
            // route.Enqueue(new Room(RoomType.Rest));
        // 1 boss room
        route.Enqueue(new Room(RoomType.Boss));

        // place route into maze
        
    }

    /// <summary>
    /// Generate a new random vector2int for room position
    /// </summary>
    /// <returns></returns>
    private Vector2Int RandomPosition()
    {
        return new Vector2Int(Random.Range(0, 4), Random.Range(0, 4));
    }

    private void PlaceRoom(Vector2Int pos, RoomType type)
    {
        maze[pos.x, pos.y] = new Room(type);
    }
}

public enum RoomType
{
    Complete,
    Rest,
    Treasure,
    Event,
    Enemy,
    Quest,
    Elite,
    Boss,
}