using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The maze generator and maze event controller
/// </summary>
public class MazeController : BaseController<MazeController>
{
    public Room[,] maze = new Room[5,5];                        // the maze
    private int[] room_minimum = { 1, 1, 1, 1, 4, 1, 1 };    // minimum room number require
    private int[] room_maximum = { 1, 3, 3, 5, 25, 5, 1 };   // maximum room number require
    private int[] room_count;                                   // current room number
    private List<int> available_type;                           // the current available room type for generate
    private List<int> fullfill_type;                            // the room type which not meet the minimum require

    private List<Room> create_rooms;                            // created rooms
    private List<Room> wait_rooms;                              // empty rooms next to the setted room
    public Room start_room;                                     // the start point

    public int maze_level;
    public int maze_hope;
    public int maze_alert;

    // Player Attributs
    public Vector2Int player_pos;

    public void MazeGenerator()
    {
        StartRouteGenerate();
        
        maze_hope = 3;
        maze_alert = 0;
        player_pos = start_pos;
    }

    private void StartRouteGenerate()
    {
        create_rooms.Clear();
        ResetRoomTypeCount();

        // Create empty maze and walls
        // horizon
        for(int i = 0; i < 5; i ++)
        {
            // vertical
            for(int j = 0; j < 5; j++)
            {
                maze[i,j] = new Room();
                maze[i,j].room_pos = new Vector2Int(i,j);
            }
        }

        // generate main route and place route into maze
            // complete - normal - normal - elite - normal - normal - normal - boss
        PlaceStartRoute(RoomType.Complete);
        PlaceStartRoute(RoomType.Enemy, create_rooms[0]);
        PlaceStartRoute(RoomType.Enemy, create_rooms[1]);
        PlaceStartRoute(RoomType.Elite, create_rooms[2]);
        PlaceStartRoute(RoomType.Enemy, create_rooms[3]);
        PlaceStartRoute(RoomType.Enemy, create_rooms[4]);
        PlaceStartRoute(RoomType.Enemy, create_rooms[5]);
        PlaceStartRoute(RoomType.Boss,  create_rooms[6]);
        
        // generate new room at empty place and create connection
        for(int i = 0; i < 17; i ++)
        {
            PlaceRoute();
        }
    }

    /// <summary>
    /// place room and build connection for main route
    /// </summary>
    /// <param name="type">the type of new room</param>
    /// <param name="from">the room to connect with</param> 
    private void PlaceStartRoute(RoomType type, Room from = null)
    {   
        RoomTypeCount(type);
        // create start room
        if(from == null)
        {
            Vector2Int pos = new Vector2Int(Random.Range(1, 3), Random.Range(1, 3));
            maze[pos.x, pos.y].room_type = RoomType.Complete;
            start_room = maze[pos.x, pos.y];
            create_rooms.Add(maze[pos.x, pos.y]);
            // add near rooms into wait_rooms
            AddWaitRooms(pos);
        }
        // create the start route
        else
        {
            Vector2Int pos;
            Vector2Int direct_pos;

            do{
                // random get a position next to from room
                pos = from.room_pos;
                direct_pos = RandomDirection(pos);

                // if find a empty room
                if(maze[direct_pos.x, direct_pos.y].room_type == RoomType.Empty)
                {
                    // set room
                    maze[direct_pos.x, direct_pos.y].room_type = type;
                    if(type != RoomType.Boss)
                    {
                        AddWaitRooms(direct_pos);
                    }
                    RemoveWaitRoom(direct_pos);
                    // set connection
                    BuildUpConnection(maze[direct_pos.x, direct_pos.y], from);
                    return;
                }
            }while(true);
        } 
    }
    private void PlaceRoute()
    {
        // Random a room type
        RoomType type;
            // fullfill minimum require
        if(fullfill_type.Count > 0)
            type = (RoomType)fullfill_type[Random.Range(0, fullfill_type.Count-1)];
            // select from maximum available
        else
            type = (RoomType)available_type[Random.Range(0, available_type.Count-1)];
        // Random a room
        Room room = wait_rooms[Random.Range(0, wait_rooms.Count-1)];
        // Random a direction
        Vector2Int from;
        do{
            from = RandomDirection(room.room_pos);
        // find the exist room next to it
        }while(maze[from.x, from.y].room_type != RoomType.Empty &&
               maze[from.x, from.y].room_type != RoomType.Boss);
        
        // build up the room and the connection
        room.room_type = type;
        BuildUpConnection(room, maze[from.x, from.y]);
    }


    private void ResetRoomTypeCount()
    {
        room_count = new int[7];
        for(int i = 0; i < 8; i ++)
        {
            available_type.Add(i);
            fullfill_type.Add(i);
        }
    }
    private void RoomTypeCount(RoomType type)
    {
        room_count[(int)type]++;

        // remove fullfilled room type
        if(room_count[(int)type] >= room_minimum[(int)type])
            fullfill_type.Remove((int)type);
        if(room_count[(int)type] >= room_maximum[(int)type])
            available_type.Remove((int)type);
    }


    private Vector2Int RandomDirection(Vector2Int pos)
    {
        Vector2Int new_pos = new Vector2Int(pos.x, pos.y);

        List<int> avail_direct = new List<int>();

        if(pos.y != 0)  // north available
            avail_direct.Add(0);
        if(pos.x != 0)  // west available
            avail_direct.Add(1);
        if(pos.y != 4)  // south available
            avail_direct.Add(2);
        if(pos.x != 4)  // east available
            avail_direct.Add(4);
        
        switch(avail_direct[Random.Range(0, avail_direct.Count-1)])
        {
            case 0:
                new_pos.y ++;
                break;
            case 1:
                new_pos.x ++;
                break;
            case 2:
                new_pos.y --;
                break;
            case 3:
                new_pos.x --;
                break;
            default:
                break;
        }
        return new_pos;
    }
    private void BuildUpConnection(Room room1, Room room2)
    {
        int h_direct = room1.room_pos.x - room2.room_pos.x;
        int v_direct = room1.room_pos.y - room2.room_pos.y;
        if(h_direct < 0)  // room1 at east
        {
            room1.west_room = room2;
            room2.east_room = room1;
        }
        else if(h_direct > 0) // room1 at west
        {
            room1.east_room = room2;
            room2.west_room = room1;
        }
        else if(v_direct > 0)   // room1 at north
        {
            room1.south_room = room2;
            room2.north_room = room1;
        }
        else    // room1 at south
        {
            room1.north_room = room2;
            room2.south_room = room1;
        }
    }


    /// <summary>
    /// add all rooms next to target position into wait rooms list
    /// </summary>
    /// <param name="pos">target position</param>
    private void AddWaitRooms(Vector2Int pos)
    {
        if(RoomAvailable(pos.x-1, pos.y))
            wait_rooms.Add(maze[pos.x-1, pos.y]);
        if(RoomAvailable(pos.x+1, pos.y))
            wait_rooms.Add(maze[pos.x+1, pos.y]);
        if(RoomAvailable(pos.x, pos.y-1))
            wait_rooms.Add(maze[pos.x, pos.y-1]);
        if(RoomAvailable(pos.x, pos.y+1))
            wait_rooms.Add(maze[pos.x, pos.y+1]);
    }
    private void RemoveWaitRoom(Vector2Int pos)
    {
        if(wait_rooms.Contains(maze[pos.x, pos.y]))
            wait_rooms.Remove(maze[pos.x, pos.y]);
    }
    /// <summary>
    /// Check if this room is a legal empty room
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="y">y position</param>
    /// <returns>true if room in maze range and empty</returns>
    private bool RoomAvailable(int x, int y)
    {
        return (x >= 0 && x <= 4 && y >= 0 && y <= 4 && 
                maze[x, y].room_type == RoomType.Empty && 
                !wait_rooms.Contains(maze[x, y]));
    }

    // Player Actions
    public void EnterRoom()
    {
        EventController.Controller().EventTrigger("BattleStart");
    }

    public void CompleteRoom()
    {

    }

    public void ExitRoom()
    {
        
    }
}

public enum RoomType
{   
    Empty = -1,
    Complete = 0,
    Rest = 1,
    Treasure = 2,
    Event = 3,
    Enemy = 4,
    Elite = 5,
    Boss = 6,
    Quest = 7
}