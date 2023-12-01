using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MazePanel : PanelBase
{
    // panel variables
    public Room[,] maze;
    public Vector2Int player_pos = new Vector2Int();
    private Dictionary<string, GameObject> tunnels = new Dictionary<string, GameObject>();
    public MazeController maze_control;

    // maze map initial
    private bool start = true;
    public Vector2Int start_pos;
    public List<GameObject> free_tunnels = new List<GameObject>();      

    // room move animation
    Transform room_cover;
    float anime_time = 0.7f;


    public override void ShowSelf()
    {
        // variable initial
        maze_control = MazeController.Controller();
        maze = maze_control.maze;
        start_pos = maze_control.start_pos;

        // assign self to controller
        maze_control.panel = this;

        // assign free tunnels
        for(int i = 0; i < 24; i ++)
            free_tunnels.Add(FindComponent<Image>("Tunnel ("+i+")").gameObject);

        // set maze map
        for(int x = 0; x < 5; x ++)
        {
            for(int y = 0; y < 5; y ++)
            {
                Button room = FindComponent<Button>("RoomBtn ("+x+") ("+y+")");
                
                if(x >= maze.GetLength(0) || y >= maze.GetLength(1))
                {
                    room.gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    room.gameObject.SetActive(true);
                }

                ResetTargetRoomIcon(x, y);

                 // set tunnels
                if(maze[x,y].north_room != null)
                {
                    tunnels.Add( "Tunnel (" + x.ToString()+y.ToString() + ") (" + x.ToString()+(y+1).ToString() + ")", free_tunnels[free_tunnels.Count-1]);
                    free_tunnels[free_tunnels.Count-1].transform.position= new Vector3(room.transform.position.x, room.transform.position.y+47.5f, 0);
                    free_tunnels[free_tunnels.Count-1].transform.eulerAngles = new Vector3(0, 0, 90);
                    free_tunnels[free_tunnels.Count-1].SetActive(false);
                    free_tunnels.RemoveAt(free_tunnels.Count-1);
                }
                if(maze[x,y].east_room != null)
                {
                    tunnels.Add( "Tunnel (" + x.ToString()+y.ToString() + ") (" + (x+1).ToString()+y.ToString() + ")", free_tunnels[free_tunnels.Count-1]);
                    free_tunnels[free_tunnels.Count-1].transform.position= new Vector3(room.transform.position.x+47.5f, room.transform.position.y, 0);
                    free_tunnels[free_tunnels.Count-1].transform.eulerAngles = new Vector3(0, 0, 0);
                    free_tunnels[free_tunnels.Count-1].SetActive(false);
                    free_tunnels.RemoveAt(free_tunnels.Count-1);
                }
                room.gameObject.SetActive(false);
            }
        }
        foreach( GameObject tunnel in free_tunnels)
            tunnel.SetActive(false);

        // set start position
        room_cover = FindComponent<Image>("MoveCover").transform;
        PlayerMove(start_pos.x, start_pos.y);
        FindComponent<Image>("MazeGrid").transform.localPosition = 
            FindComponent<Button>("RoomBtn ("+start_pos.x.ToString()+") ("+start_pos.y.ToString()+")").transform.localPosition;
        FindComponent<Text>("AlertText").text = "1";

        // finish initial
        start = false;
    }

    protected override void OnButtonClick(string button_name)
    {
        // move to a room
        if(button_name.Contains("RoomBtn"))
        {
            int x = player_pos.x;
            int y = player_pos.y;

            // set map buttons interactable
            FindComponent<Button>("RoomBtn ("+x+") ("+y+")").interactable = true;
            if(maze[x,y].north_room != null)
                FindComponent<Button>("RoomBtn ("+x+") ("+(y+1)+")").interactable = false;
            if(maze[x,y].south_room != null)
                FindComponent<Button>("RoomBtn ("+x+") ("+(y-1)+")").interactable = false;
            if(maze[x,y].east_room != null)
                FindComponent<Button>("RoomBtn ("+(x+1)+") ("+y+")").interactable = false;
            if(maze[x,y].west_room != null)
                FindComponent<Button>("RoomBtn ("+(x-1)+") ("+y+")").interactable = false;

            // move to new room
            x = Int32.Parse(button_name.Substring(9,1));
            y = Int32.Parse(button_name.Substring(13,1));

            if(x == player_pos.x && y == player_pos.y)
                return;

            PlayerMove(x, y);
        }
        // complete current room
        else if(button_name == "CompleteBtn")
        {
            RoomType type = maze[player_pos.x, player_pos.y].room_type;
            // rest room
            if(type == RoomType.Rest)
                AudioController.Controller().StartSound("GetRest");
            // treasure room
            else if(type == RoomType.Treasure)
                AudioController.Controller().StartSound("GetTreasure");
            // event room
            else if(type == RoomType.Event)
                AudioController.Controller().StartSound("GetEvent");

            if(maze_control.CompleteRoom())
                ResetTargetRoomIcon(player_pos.x, player_pos.y);
        }
    }

    // reset the icon of complete room
    public void ResetTargetRoomIcon(int x, int y, int alert = 1)
    {
        Button room = FindComponent<Button>("RoomBtn ("+x+") ("+y+")");

        // set outside room inactive
        if(x > maze.GetLength(0)-1 || y > maze.GetLength(1)-1)
        {
            room.gameObject.SetActive(false);
            return;
        }
        else
            room.gameObject.SetActive(true);

        // reset room icon
        room.transform.GetChild(0).gameObject.SetActive((int)maze[x,y].room_type != 0);
        room.transform.GetChild(0).GetComponent<Image>().sprite = maze_control.GetImage(maze[x,y].room_type+"Room");
        
        if(alert != 1)
            FindComponent<Text>("AlertText").text = alert.ToString();
    }

    // show tunnels when move into room
    private void SetConnection(Room room_1, Room room_2)
    {
        string part_1 = room_1.room_pos.x.ToString() + room_1.room_pos.y.ToString();
        string part_2 = room_2.room_pos.x.ToString() + room_2.room_pos.y.ToString();
        tunnels["Tunnel ("+part_1+") ("+part_2+")"].SetActive(true);
    }

    // player move to another room
    public void PlayerMove(int x, int y)
    {
        Button curr_room = FindComponent<Button>("RoomBtn ("+x+") ("+y+")");

        // room switch animation
        if(!start)
        {
            AudioController.Controller().StartSound("MoveRoom");

            // set animation direction
            if(x < player_pos.x)
            {
                room_cover.localPosition = new Vector3(-1950, 0, 1);
                TweenController.Controller().MoveToPosition(room_cover, new Vector3(1950, 0, 0), anime_time, true);
            }
            if(x > player_pos.x)
            {
                room_cover.localPosition = new Vector3(1950, 0, 1);
                TweenController.Controller().MoveToPosition(room_cover, new Vector3(-1950, 0, 0), anime_time, true);
            }
            if(y < player_pos.y)
            {
                room_cover.localPosition = new Vector3(0, -1120, 1);
                TweenController.Controller().MoveToPosition(room_cover, new Vector3(0, 1120, 0), anime_time, true);
            }
            if(y > player_pos.y)
            {
                room_cover.localPosition = new Vector3(0, 1120, 1);
                TweenController.Controller().MoveToPosition(room_cover, new Vector3(0, -1120, 0), anime_time, true);
            }
        }

        // refresh player position
        player_pos.x = x;
        player_pos.y = y;
        if(!start)
            TweenController.Controller().MoveToPosition(FindComponent<Image>("MazeGrid").transform, -curr_room.transform.localPosition, 0.3f, true);
        else
        {
            FindComponent<Button>("CompleteBtn").gameObject.SetActive(false);
            FindComponent<Image>("RoomObj").gameObject.SetActive(false);
            TweenController.Controller().MoveToPosition(FindComponent<Image>("MazeGrid").transform, -curr_room.transform.localPosition, 0.05f, true);
            FinishMove();
        }

        // trigger controller enter room event
        MazeController.Controller().EnterRoom(x, y);
    }

    public void SetRoomObj()
    {
        int x = player_pos.x;
        int y = player_pos.y;
        // set non-battle room button text and obj
            // show button if it's a non-battle room and not complete
        FindComponent<Button>("CompleteBtn").gameObject.SetActive((int)maze[x, y].room_type < 4 && (int)maze[x, y].room_type > 0);
        FindComponent<Image>("RoomObj").gameObject.SetActive((int)maze[x, y].room_type < 4 && (int)maze[x, y].room_type > 0);

        if(maze[x, y].room_type == RoomType.Rest)
        {
            FindComponent<Text>("RoomText").text = "heal";
            FindComponent<Image>("RoomObj").sprite = maze_control.GetImage("RestObj");
        }
            
        else if(maze[x, y].room_type == RoomType.Treasure)
        {
            FindComponent<Text>("RoomText").text = "loot";
            FindComponent<Image>("RoomObj").sprite = maze_control.GetImage("TreasureObj");
        }
            
        else if(maze[x, y].room_type == RoomType.Event)
        {
            FindComponent<Text>("RoomText").text = "Pray";
            FindComponent<Image>("RoomObj").sprite = maze_control.GetImage("EventObj");
        }
    }

    public void FinishMove()
    {
        int x = player_pos.x;
        int y = player_pos.y;

        Button curr_room = FindComponent<Button>("RoomBtn ("+x+") ("+y+")");
        Button near_room;
        // set room mist ( show all neighbor rooms)
        curr_room.gameObject.SetActive(true);
        if(maze[x,y].north_room != null)
        {
            near_room = FindComponent<Button>("RoomBtn ("+x+") ("+(y+1)+")");
            near_room.gameObject.SetActive(true);
            near_room.interactable = true;
            SetConnection(maze[x,y], maze[x,y+1]);
        }
        if(maze[x,y].south_room != null)
        {
            near_room = FindComponent<Button>("RoomBtn ("+x+") ("+(y-1)+")");
            near_room.gameObject.SetActive(true);
            near_room.interactable = true;
            SetConnection(maze[x,y-1], maze[x,y]);
        }
        if(maze[x,y].east_room != null)
        {
            near_room = FindComponent<Button>("RoomBtn ("+(x+1)+") ("+y+")");
            near_room.gameObject.SetActive(true);
            near_room.interactable = true;
            SetConnection(maze[x,y], maze[x+1,y]);
        }
        if(maze[x,y].west_room != null)
        {
            near_room = FindComponent<Button>("RoomBtn ("+(x-1)+") ("+y+")");
            near_room.gameObject.SetActive(true);
            near_room.interactable = true;
            SetConnection( maze[x-1,y], maze[x,y]);
        }
    }
}
