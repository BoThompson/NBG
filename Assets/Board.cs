using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Wall : IEquatable<Wall>
{
    GameObject wallObj;
    Vector2Int from;
    Vector2Int to;
    public Wall(Vector2Int a, Vector2Int b, Vector3 offset, GameObject prefab)
    {
        wallObj = GameObject.Instantiate(prefab);
        if(b.x < a.x)
        {
            from = b;
            to = a;
        }else if(b.y < a.y)
        {
            from = b;
            to = a;
        }
        else
        {
            from = a;
            to = b;
        }
        wallObj.transform.position = new Vector3((from.x + to.x) / 2.0f + 0.5f, -(from.y + to.y) / 2.0f - 0.5f, 0) + offset;
        if(a.y != b.y)
        {
            wallObj.transform.Rotate(0, 0, 90);
        }
        wallObj.name = a + " to " + b;
    }
    public static bool operator !=(Wall wall1, Wall wall2)
    {
        return !(wall1 == wall2);
    }
    public static bool operator ==(Wall wall1, Wall wall2)
    {
        return (wall1.to == wall2.to && wall1.from == wall2.from);
    }

    public override bool Equals(object obj)
    {
        return obj is Wall && Equals(this == (Wall)obj);
    }

    public bool Equals(Wall other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        var hashCode = 1888580950;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(from);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(to);
        hashCode = hashCode * -1521134295 + type.GetHashCode();
        return hashCode;
    }

    public Vector2Int From()
    {
        return from;
    }

    public Vector2Int To()
    {
        return to;
    }
    
    public void Destroy()
    {
        GameObject.Destroy(wallObj);
    }
    ~Wall()
    {
        GameObject.Destroy(wallObj);
    }
    public int type;
}
public class Room : IEquatable<Room>
{
    public Vector2Int coord;
    public Room(Vector2Int c)
    {
        coord = c;
    }
    public override bool Equals(object obj)
    {
        return obj is Room && Equals(this == (Room)obj);
    }

    public bool Equals(Room other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return -1469483106 + EqualityComparer<Vector2Int>.Default.GetHashCode(coord);
    }

    public static bool operator ==(Room room1, Room room2)
    {
        return room1.coord == room2.coord;
    }

    public static bool operator !=(Room room1, Room room2)
    {
        return !(room1 == room2);
    }
}
public class Board : MonoBehaviour
{
    public GameObject wallPrefab;
    private List<Wall> wallList;
    public Vector2Int boardSize;
    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }
    void Generate()
    {
        List<Wall> wallCandidates = new List<Wall>();
        wallList = new List<Wall>();
        List<HashSet<Room>> roomSets = new List<HashSet<Room>>();
        Wall wall;
        transform.localScale = new Vector3(boardSize.x, boardSize.y, 1);
        Vector3 offset = new Vector3(-boardSize.x / 2.0f, boardSize.y / 2.0f, 0);
        for(int x = 0;x < boardSize.x; x++)
        {
            for(int y = 0;y < boardSize.y;y++)
            {
                if (x != boardSize.x - 1)
                {
                    //Generate right wall
                    wall = new Wall(new Vector2Int(x, y), new Vector2Int(x + 1, y), offset, wallPrefab);
                    wallCandidates.Add(wall);
                }
                if (y != boardSize.y - 1)
                {
                    //Generate bottom wall
                    wall = new Wall(new Vector2Int(x, y), new Vector2Int(x, y + 1), offset, wallPrefab);
                    wallCandidates.Add(wall);
                }
                HashSet<Room> roomList = new HashSet<Room>();
                roomList.Add(new Room(new Vector2Int(x, y)));
                roomSets.Add(roomList);
            }
        }
        //Randomly select edges
        while (roomSets.Count > 1)
        {
            int wallIndex = UnityEngine.Random.Range(0, wallCandidates.Count);
            Debug.Log("RoomSets is " + roomSets.Count + " and Count is " + wallCandidates.Count + " and index is " + wallIndex);
            wall = wallCandidates[wallIndex];
            wallCandidates.RemoveAt(wallIndex);
            int fromIndex = -1;
            int toIndex = -1;
            //Loop to find from-room and to-room indices
            for (int i = 0; i < roomSets.Count; i++)
            {
                if (roomSets[i].Contains(new Room(wall.From())))
                {
                    fromIndex = i;
                }
                if (roomSets[i].Contains(new Room(wall.To())))
                {
                    toIndex = i;
                }
                
            }
            //If from-room is not in same set as to-room, combine sets, remove old set
            if (fromIndex != toIndex)
            {
                roomSets[fromIndex].UnionWith(roomSets[toIndex]);
                roomSets.RemoveAt(toIndex);
                Debug.Log("Removed " + toIndex + " and now left with " + roomSets.Count);
                wall.Destroy();
            }
            else
            {
                wallList.Add(wall);
            }
        }
    }

    public Vector2Int WorldToBoardSpace(Vector3 p)
    {
        return new Vector2Int(Mathf.RoundToInt(p.x - .5f + boardSize.x / 2.0f), Mathf.RoundToInt(-p.y - .5f + boardSize.y / 2.0f));
    }

    public Vector3 BoardToWorldSpace(Vector2Int p)
    {
        return new Vector3(transform.position.x + p.x, transform.position.y - p.y, transform.position.z);
    }

    public void Place(Piece piece, Vector2Int p)
    {
        piece.transform.position = BoardToWorldSpace(p);
        piece.boardPosition = p;

    }
    public bool IsInBounds(Vector2Int p)
    {
        return p.x >= 0 && p.x < boardSize.x && p.y >= 0 && p.y < boardSize.y;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
