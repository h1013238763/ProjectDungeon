using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public RoomType room_type;      // the type of room
    public Vector2Int room_pos;     // the position of room in maze
    
    public bool in_mist;

    // channels between rooms
    public Room north_room;
    public Room south_room;
    public Room east_room;
    public Room west_room;

    // default constructor
    public Room(bool is_wall = false)
    {
        in_mist = true;

        room_type = RoomType.Empty;
    }

    public override string ToString()
    {
        return room_type.ToString();
    }
}
