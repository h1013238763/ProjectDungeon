using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public RoomType room_type;      // the type of room
    
    // channels between rooms
    public Room north_room;
    public Room south_room;
    public Room east_room;
    public Room west_room;

    // default constructor
    public Room()
    {
        room_type = RoomType.Complete;
    }

    public Room(RoomType type)
    {
        room_type = type;
    }
}
