using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MazePanel : PanelBase
{
    public Room[,] maze;
    public Vector2Int start_pos;
    public Vector2Int player_pos = new Vector2Int();
    public List<GameObject> free_tunnels = new List<GameObject>();
    private Dictionary<string, GameObject> tunnels = new Dictionary<string, GameObject>();

    // start stage
    private bool start = true;

    private void RoomCoverAnime(int x, int y)
    {
        Transform room_cover = FindComponent<Image>("PanelBackground").transform;
        float anime_time = 0.6f;

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

    public override void ShowSelf()
    {
        ResetRoom();
        PlayerMove(start_pos.x, start_pos.y);
        FindComponent<Image>("MazeGrid").transform.localPosition = 
            FindComponent<Button>("RoomBtn ("+start_pos.x.ToString()+") ("+start_pos.y.ToString()+")").transform.localPosition;
        start = false;
        FindComponent<Text>("AlertText").text = "1";
    }

    protected override void OnButtonClick(string button_name)
    {
        if(button_name.Contains("RoomBtn"))
        {
            BeforeMove();
            int x = Int32.Parse(button_name.Substring(9,1));
            int y = Int32.Parse(button_name.Substring(13,1));
            PlayerMove(x, y);
        }
        if(button_name == "SettingBtn")
        {
            GUIController.Controller().ShowPanel<SettingPanel>("SettingPanel", 2);
        }
    }

    private void ResetRoom()
    {
        // Sign Tunnels
        for(int i = 0; i < 24; i ++)
        {
            free_tunnels.Add(FindComponent<Image>("Tunnel ("+i+")").gameObject);
        }

        // Set Rooms
        for(int x = 0; x < 5; x ++)
        {
            for(int y = 0; y < 5; y ++)
            {
                Button room = FindComponent<Button>("RoomBtn ("+x+") ("+y+")");
                // reset room icon
                room.transform.GetChild(0).GetComponent<Image>().sprite = ResourceController.Controller().Load<Sprite>("Image/Rooms/"+maze[x,y].room_type.ToString());
                // set visual active
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
    }

    private void SetConnection(Room room_1, Room room_2)
    {
        string part_1 = room_1.room_pos.x.ToString() + room_1.room_pos.y.ToString();
        string part_2 = room_2.room_pos.x.ToString() + room_2.room_pos.y.ToString();
        tunnels["Tunnel ("+part_1+") ("+part_2+")"].SetActive(true);
    }

    private void BeforeMove()
    {
        int x = player_pos.x;
        int y = player_pos.y;

        FindComponent<Button>("RoomBtn ("+x+") ("+y+")").interactable = true;

        if(maze[x,y].north_room != null)
            FindComponent<Button>("RoomBtn ("+x+") ("+(y+1)+")").interactable = false;
        if(maze[x,y].south_room != null)
            FindComponent<Button>("RoomBtn ("+x+") ("+(y-1)+")").interactable = false;
        if(maze[x,y].east_room != null)
            FindComponent<Button>("RoomBtn ("+(x+1)+") ("+y+")").interactable = false;
        if(maze[x,y].west_room != null)
            FindComponent<Button>("RoomBtn ("+(x-1)+") ("+y+")").interactable = false;
    }

    public void PlayerMove(int x, int y)
    {
        Button curr_room = FindComponent<Button>("RoomBtn ("+x+") ("+y+")");
        Button near_room;

        // room switch cover move
        if(!start)
            RoomCoverAnime(x, y);

        // set room mist
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

        
        player_pos.x = x;
        player_pos.y = y;

        if(!start)
            TweenController.Controller().MoveToPosition(FindComponent<Image>("MazeGrid").transform, -curr_room.transform.localPosition, 0.3f, true);
        else
        {
            TweenController.Controller().MoveToPosition(FindComponent<Image>("MazeGrid").transform, -curr_room.transform.localPosition, 0.1f, true);
        }

        MazeController.Controller().EnterRoom(x, y);
    }

    public void SetMaze(Room[,] target_maze)
    {
        maze = target_maze;
    }

    public void CompleteRoom(int x, int y, int alert)
    {
        FindComponent<Button>("RoomBtn ("+x+") ("+y+")").transform.GetChild(0).GetComponent<Image>().sprite = 
            ResourceController.Controller().Load<Sprite>("Image/Rooms/"+maze[x,y].room_type.ToString());
        FindComponent<Text>("AlertText").text = "1";
    }
}
