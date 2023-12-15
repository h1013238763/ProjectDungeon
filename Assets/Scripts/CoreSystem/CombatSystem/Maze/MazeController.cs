using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// The maze generator and maze event controller
/// </summary>
public class MazeController : BaseController<MazeController>
{
    // database
    private Dictionary<string, Maze> dict_maze = new Dictionary<string, Maze>();
    private Dictionary<string, Sprite> image_maze = new Dictionary<string, Sprite>();

    // maze generating varible
    private int[] room_minimum = { 1, 1, 1, 1, 4, 1, 1, 0};    // minimum room number require
    private int[] room_maximum = { 1, 2, 3, 5, 25, 5, 1, 0};   // maximum room number require
    private int[] room_count;                     // current room number
    private List<int> available_type = new List<int>();         // the current available room type for generate
    private List<int> fullfill_type = new List<int>();          // the room type which not meet the minimum require
    private List<Room> create_rooms = new List<Room>();         // created rooms
    private List<Room> wait_rooms = new List<Room>();           // empty rooms next to the setted room
    public Room start_room;                                     // the start point
    public Vector2Int start_pos;
    
    // maze behavior varible
    public BattleController battle_control;
    public Maze maze_base;                      // curr maze infos
    public Room[,] maze;                        // the maze
    public MazePanel panel;                     // the gui
    public int maze_hope;
    public int maze_alert;

    // Player Attributs
    public BattleUnit player_unit;
    public Vector2Int player_pos;
    public Vector2Int prev_pos;

    // maze reward
    public List<Item> reward_item = new List<Item>();
    public int reward_money = 0;
    public int reward_exp = 0;
    
    public void SetMaze(string id)
    {
        if(!dict_maze.ContainsKey(id))
            return;
        maze_base = dict_maze[id];
    }

    public Sprite GetImage(string id)
    {
        if(!image_maze.ContainsKey(id))
            return null;
        return image_maze[id];
    }

    // generate the tutorial maze
    public void TutorialMaze()
    {
        maze_hope = 3;
        maze_alert = 1;
        
        // generate empty rooms
        maze = new Room[1, 5];

        // create room
        maze[0, 0] = new Room(RoomType.Complete, 0, 0);
        maze[0, 1] = new Room(RoomType.Enemy, 0, 1);
        maze[0, 2] = new Room(RoomType.Rest, 0, 2);
        maze[0, 3] = new Room(RoomType.Treasure, 0, 3);
        maze[0, 4] = new Room(RoomType.Boss, 0, 4);

        // create route
        BuildUpConnection(maze[0, 0], maze[0, 1]);
        BuildUpConnection(maze[0, 1], maze[0, 2]);
        BuildUpConnection(maze[0, 2], maze[0, 3]);
        BuildUpConnection(maze[0, 3], maze[0, 4]);

        // ready to start
        player_pos = new Vector2Int(0, 0);
    }

    // a maze for player to test their build
    public void TestMaze()
    {
        maze_hope = 3;
        maze_alert = 0;

        maze = new Room[1, 2];

        // create room
        maze[0, 0] = new Room(RoomType.Complete, 0, 0);
        maze[0, 1] = new Room(RoomType.Enemy, 0, 1);

        // create route
        BuildUpConnection(maze[0, 0], maze[0, 1]);

        // ready to start
        player_pos = new Vector2Int(0, 0);
    }

    // normal mazes
    public void NormalMaze()
    { 
        maze_hope = 3;
        maze_alert = 0;

        // generate empty rooms
        maze = new Room[5,5];
        // reset all room settings
        create_rooms.Clear();
        // reset room type count
        room_count = new int[8];
        for(int i = 0; i < 7; i ++)
        {
            fullfill_type.Add(i);
            available_type.Add(i);
        }
        if(room_maximum[7] > 0)
            available_type.Add(7);

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
        PlaceStartRoute(RoomType.Rest,  create_rooms[5]);
        PlaceStartRoute(RoomType.Boss,  create_rooms[6]);

        // generate new room at empty place and create connection
        for(int i = 0; i < 17; i ++)
        {
            // Random a room type
            RoomType type;
            
            if(fullfill_type.Count > 0) // fullfill minimum require
                type = (RoomType)fullfill_type[Random.Range(0, fullfill_type.Count-1)];
            else                        // select from maximum available
                type = (RoomType)available_type[Random.Range(0, available_type.Count-1)];

            // Random a room next to exist route
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

        // ready to start
        player_pos = start_pos;
    }  
    
    // Player Actions
    public async void EnterRoom(int x, int y)
    {
        // enter maze
        

        // wait for animation
        await Task.Delay(300);
        panel.SetRoomObj();

        player_pos.x = x;
        player_pos.y = y;

        int room_type = (int)maze[player_pos.x, player_pos.y].room_type;

        string event_name = "EnterRoom:{0}, {1}";
        EventController.Controller().EventTrigger(string.Format(event_name, maze_base.maze_id, maze[player_pos.x, player_pos.y].room_type));

        // wall
        if(room_type == -1)
            return;
        // non-battle room
        else if(room_type < 4)
        {
            prev_pos = player_pos;
            if(room_type != 2)
                panel.FinishMove();
        }
        // battle room
        else if(room_type < 7)
        {
            // room_objects[3].SetActive(true);
            // enemy level = (maze level - 1 * 10) + maze alert
            Debug.Log((maze_base.maze_level-1)*10 + maze_alert);
            BattleController.Controller().BattleStart( maze[player_pos.x, player_pos.y], maze_base, (maze_base.maze_level-1)*10 + maze_alert );
        }
        // quest room
        else
        {
            
        }
    }

    // Complete target room
    public bool CompleteRoom()
    {
        if( (int)maze[player_pos.x, player_pos.y].room_type < 7 && (int)maze[player_pos.x, player_pos.y].room_type >= 4)
        {
            maze_alert ++;
            if(maze_alert > 10)
                maze_alert = 10;
        }

        // rest room, recover health and maze hope
        if( maze[player_pos.x, player_pos.y].room_type == RoomType.Rest )
        {
            maze_hope += (maze_hope < 3) ? 1 : 0;
            player_unit.OnHeal(player_unit.health_max);
        }
        // Treasure room, get loot from elite enemy
        else if( maze[player_pos.x, player_pos.y].room_type == RoomType.Treasure )
        {
            for(int i = 0; i < 3; i ++)
            {
                reward_item.Add(maze_base.GetRandomDrop(2, maze_alert+(maze_base.maze_level-1)*10));
            }                
        }
        else if( (int)maze[player_pos.x, player_pos.y].room_type > 3 && (int)maze[player_pos.x, player_pos.y].room_type < 6 )
        {
            maze_alert += ( maze_alert > 10 ) ? 0 : 1 ;
        }
        else if( (int)maze[player_pos.x, player_pos.y].room_type == 7 )
        {
            ExitMaze("Victory");
        }
        // event room, maze alert -2
        else if( maze[player_pos.x, player_pos.y].room_type == RoomType.Event )
        {
            maze_alert -= ( maze_alert > 2) ? 2 : maze_alert-1;
        }
        
        string event_name = "CompleteRoom:{0}, {1}";
        EventController.Controller().EventTrigger(string.Format(event_name, maze_base.maze_id, maze[player_pos.x, player_pos.y].room_type));

        maze[player_pos.x, player_pos.y].room_type = RoomType.Complete;
        panel.ResetTargetRoomIcon(player_pos.x, player_pos.y, maze_alert);
        panel.SetRoomObj();
        prev_pos = player_pos;

        panel.FinishMove();

        return true;
    }

    // exit battle by setting panel
    public void ExitRoom()
    {
        // move to prev room
        panel.PlayerMove(prev_pos.x, prev_pos.y);
        // loss hope and decrease difficulty
        maze_hope -= ((int)maze[player_pos.x, player_pos.y].room_type-3);
        maze_alert -= (maze_alert > 1) ? 1 : 0 ;

        if(maze_hope <= 0)
            ExitMaze("Fail");
    }

    public void ExitMaze(string reason)
    {
        if(reason == "Victory")
        {
            int progress = PlayerController.Controller().data.maze_progress;
            if(maze_base.maze_level > progress)
                PlayerController.Controller().data.maze_progress = maze_base.maze_level;
        }
        // get reward
        for( int i = 0; i < reward_item.Count; i ++)
        {
            if( reward_item[i] == null )
                continue;

            string type = ItemController.Controller().CheckItemType(reward_item[i].item_id);

            if(type == "Equip")
                ItemController.Controller().GetEquip((reward_item[i] as Equip));
            else if(type == "Potion")
                ItemController.Controller().GetPotion(reward_item[i].item_id, reward_item[i].item_num);
            else if(type == "Item")
                ItemController.Controller().GetItem(reward_item[i].item_id, reward_item[i].item_num);            
        }
        // get money
        PlayerController.Controller().data.player_money += reward_money;
        PlayerController.Controller().GetExp(reward_exp);
        // back to town
        StageController.Controller().SwitchScene("TownScene");

        // Clear battle data
        player_unit = null;
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
                // remove wait room
                if(wait_rooms.Contains(maze[direct_pos.x, direct_pos.y]))
                    wait_rooms.Remove(maze[direct_pos.x, direct_pos.y]);
                // set connection
                BuildUpConnection(maze[direct_pos.x, direct_pos.y], from);
                return;
            }
        }
    }
    
    // count number of each room type
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

    public void InitialData()
    {
        // load dialogue
        Maze[] mazes = Resources.LoadAll<Maze>("Object/Maze/");
        if(mazes != null)
        {
            foreach(Maze maze in mazes)
                dict_maze.Add(maze.maze_id, maze);
        }

        Sprite[] images = Resources.LoadAll<Sprite>("Image/Maze/");
        if(images != null)
        {
            foreach(Sprite image in images)
                image_maze.Add(image.name, image);
        }
    }
}