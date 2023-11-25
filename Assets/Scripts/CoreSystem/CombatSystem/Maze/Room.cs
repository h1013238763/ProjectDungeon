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

    public EnemyGroup room_enemy;

    // Regist Enemy

    // default constructor
    public Room(bool is_wall = false)
    {
        room_type = RoomType.Empty;
    }

    public Room(RoomType type, int x = -1, int y = -1)
    {
        room_type = type;

        if(x != -1 && y != -1)
            room_pos = new Vector2Int(x, y);
    }

    public override string ToString()
    {
        return room_type.ToString();
    }
}
