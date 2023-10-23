using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public RoomType room_type;      // the type of room
    public Vector2Int room_pos;     // the position of room in maze

    // channels between rooms
    public Room north_room;
    public Room south_room;
    public Room east_room;
    public Room west_room;

    // Regist Enemy

    // default constructor
    public Room(bool is_wall = false)
    {
        room_type = RoomType.Empty;
    }

    public override string ToString()
    {
        return room_type.ToString();
    }
}
