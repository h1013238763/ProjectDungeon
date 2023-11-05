using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// The maze generator and maze event controller
/// </summary>
public class MazeController : BaseController<MazeController>
{
    public Room[,] maze = new Room[5,5];                        // the maze
    private int[] room_minimum = { 1, 1, 1, 1, 4, 1, 1, 0};    // minimum room number require
    private int[] room_maximum = { 1, 2, 3, 5, 25, 5, 1, 0};   // maximum room number require
    private int[] room_count;                     // current room number
    private List<int> available_type = new List<int>();         // the current available room type for generate
    private List<int> fullfill_type = new List<int>();          // the room type which not meet the minimum require

    private List<Room> create_rooms = new List<Room>();         // created rooms
    private List<Room> wait_rooms = new List<Room>();           // empty rooms next to the setted room
    public Room start_room;                                     // the start point

    public Vector2Int start_pos;
    public int maze_hope;
    public int maze_alert;
    public int maze_level;                                      // target maze to generate

    // Player Attributs
    public BattleUnit player;
    public Vector2Int player_pos;
    public Vector2Int prev_pos;

    public List<Item> maze_reward;

    public int reward_money;

    public Dictionary<string, GameObject> room_objects = new Dictionary<string, GameObject>();

    public void MazeGenerator()
    {
        StartRouteGenerate();
        
        maze_hope = 3;
        maze_alert = 0;
        player_pos = start_pos;        
    }

    /// <summary>
    /// Generate the maze route
    /// </summary>
    private void StartRouteGenerate()
    {
        // reset all room settings
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
            start_pos = new Vector2Int(pos.x, pos.y);
            AddWaitRooms(pos);
        }
        // create the start route
        else
        {
            Vector2Int pos;
            Vector2Int direct_pos;

            // random get a position next to from room
            pos = from.room_pos;
            direct_pos = AvailableDirection(pos, RoomType.Empty);
                // if find a empty room
            if(maze[direct_pos.x, direct_pos.y].room_type == RoomType.Empty)
            {
                // set room
                maze[direct_pos.x, direct_pos.y].room_type = type;
                create_rooms.Add(maze[direct_pos.x, direct_pos.y]);
                if(type != RoomType.Boss)
                {
                    AddWaitRooms(direct_pos);
                }
                RemoveWaitRoom(direct_pos);
                // set connection
                BuildUpConnection(maze[direct_pos.x, direct_pos.y], from);
                return;
            }
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
        Vector2Int from = AvailableDirection(room.room_pos, RoomType.Empty, false, true);        

        // build up the room and the connection
        room.room_type = type;
        BuildUpConnection(room, maze[from.x, from.y]);

        // regist near room to wait_rooms
        RoomTypeCount(type);
        AddWaitRooms(room.room_pos);
        wait_rooms.Remove(room);
    }

    /// <summary>
    /// Reset the count of each room type
    /// </summary>
    private void ResetRoomTypeCount()
    {
        room_count = new int[8];
        for(int i = 0; i < 7; i ++)
        {
            fullfill_type.Add(i);
            available_type.Add(i);
        }
        if(room_maximum[7] > 0)
            available_type.Add(7);
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


    private Vector2Int AvailableDirection(Vector2Int pos, RoomType type, bool not_except = true, bool normal_route = false)
    {
        Vector2Int new_pos = new Vector2Int(pos.x, pos.y);

        List<int> avail_direct = new List<int>();


        if(pos.y != 0 && (maze[pos.x, pos.y-1].room_type == type) == not_except)  // south available
        {
            if(normal_route)
            {
                if(maze[pos.x, pos.y-1].room_type != RoomType.Boss)
                    avail_direct.Add(0);
            }
            else
                avail_direct.Add(0);
        }
        if(pos.x != 0 && (maze[pos.x-1, pos.y].room_type == type) == not_except)  // west available
        {
            if(normal_route)
            {
                if(maze[pos.x-1, pos.y].room_type != RoomType.Boss)
                    avail_direct.Add(1);
            }
            else
                avail_direct.Add(1);
        }
        if(pos.y != 4 && (maze[pos.x, pos.y+1].room_type == type) == not_except)  // north available
        {
            if(normal_route)
            {
                if(maze[pos.x, pos.y+1].room_type != RoomType.Boss)
                    avail_direct.Add(2);
            }
            else
                avail_direct.Add(2);
        }
        if(pos.x != 4 && (maze[pos.x+1, pos.y].room_type == type) == not_except)  // east available
        {
            if(normal_route)
            {
                if(maze[pos.x+1, pos.y].room_type != RoomType.Boss)
                    avail_direct.Add(3);
            }
            else
                avail_direct.Add(3);
        }
        
        switch(avail_direct[Random.Range(0, avail_direct.Count-1)])
        {
            case 0:
                new_pos.y --;
                break;
            case 1:
                new_pos.x --;
                break;
            case 2:
                new_pos.y ++;
                break;
            case 3:
                new_pos.x ++;
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
            room1.east_room = room2;
            room2.west_room = room1;
        }
        else if(h_direct > 0) // room1 at west
        {
            room1.west_room = room2;
            room2.east_room = room1;
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
    public async void EnterRoom(int x, int y)
    {
        if(room_objects.Count == 0)
            InitialRoomObject();

        // wait for animation
        await Task.Delay(300);

        player_pos.x = x;
        player_pos.y = y;

        switch(maze[player_pos.x, player_pos.y].room_type)
        {
            case RoomType.Complete:
                prev_pos = player_pos;
                break;
            case RoomType.Rest:
                room_objects["Rest"].SetActive(true);
                break;
            // TODO : Give player random equipment or items
            case RoomType.Treasure:
                room_objects["Treasure"].SetActive(true);
                break;
            // TODO : Trigger random events
            case RoomType.Event:
                room_objects["Event"].SetActive(true);
                break;
            // TODO : Trigger normal enemy battle
            case RoomType.Enemy:
                room_objects["Battle"].SetActive(true);
                BattleController.Controller().BattleStart(maze[player_pos.x, player_pos.y], maze_level, maze_alert, 1);
                break;
            // TODO : Trigger elite enemy battle
            case RoomType.Elite:
                room_objects["Battle"].SetActive(true);
                BattleController.Controller().BattleStart(maze[player_pos.x, player_pos.y], maze_level, maze_alert, 2);
                break;
            // TODO : Trigger boss battle
            case RoomType.Boss:
                room_objects["Battle"].SetActive(true);
                BattleController.Controller().BattleStart(maze[player_pos.x, player_pos.y], maze_level, maze_alert, 3);
                break;
            // TODO : trigger quest event
            case RoomType.Quest:
                room_objects["Quest"].SetActive(true);
                break;
            // do nothing
            default:
                break;
        }
        
    }

    // Complete target room
    public void CompleteRoom()
    {
        if( maze[player_pos.x, player_pos.y].room_type == RoomType.Enemy ||
            maze[player_pos.x, player_pos.y].room_type == RoomType.Elite || 
            maze[player_pos.x, player_pos.y].room_type == RoomType.Boss )
        {
            maze_alert ++;
            if(maze_alert > 10)
                maze_alert = 10;
        }
        if( maze[player_pos.x, player_pos.y].room_type == RoomType.Rest )
        {
            maze_hope += (maze_hope < 3) ? 1 : 0;
            player.health_curr = player.health_max;
        }
        if( maze[player_pos.x, player_pos.y].room_type == RoomType.Treasure )
        {
            // TODO : Get random reward
        }
        if( maze[player_pos.x, player_pos.y].room_type == RoomType.Event )
        {
            // TODO : Trigger event effect
        }
        

        maze[player_pos.x, player_pos.y].room_type = RoomType.Complete;
        GUIController.Controller().GetPanel<MazePanel>("MazePanel").CompleteRoom(player_pos.x, player_pos.y, maze_level);
        prev_pos = player_pos;

        foreach(var pair in room_objects)
        {
            pair.Value.SetActive(false);
        }
    }

    // exit battle by setting panel
    public void ExitRoom()
    {
        GUIController.Controller().GetPanel<MazePanel>("MazePanel").PlayerMove(prev_pos.x, prev_pos.y);

        maze_hope -= ((int)maze[player_pos.x, player_pos.y].room_type-3);
        maze_alert -= (maze_alert > 1) ? 1 : 0 ;

        if(maze_hope <= 0)
            ExitMaze();
    }

    public void ExitMaze()
    {
        // get reward
        foreach( var item in maze_reward)
        {
            switch(ItemController.Controller().CheckItemType(item.item_id))
            {
                case "Equip":
                    ItemController.Controller().GetEquip((item as Equip));
                    break;
                case "Potion":
                    ItemController.Controller().GetPotion(item.item_id, item.item_num);
                    break;
                case "Item":
                    ItemController.Controller().GetItem(item.item_id, item.item_num);
                    break;
                default:
                    break;
            }
            
        }
        // get money
        PlayerController.Controller().player_money += reward_money;
        // back to town
        StageController.Controller().SwitchScene("TownScene");
    }

    public void GetRandomReward()
    {

    }

    private void InitialRoomObject()
    {
        MazeController.Controller().room_objects.Add("Battle", GameObject.Find("BattleRoom"));
        MazeController.Controller().room_objects.Add("Quest", GameObject.Find("QuestRoom"));
        MazeController.Controller().room_objects.Add("Event", GameObject.Find("EventRoom"));
        MazeController.Controller().room_objects.Add("Treasure", GameObject.Find("TreasureRoom"));
        MazeController.Controller().room_objects.Add("Rest", GameObject.Find("RestRoom"));

        foreach(var pair in room_objects)
        {
            pair.Value.SetActive(false);
        }
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