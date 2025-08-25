using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{

    private int[] floorPlan;

    private int floorPlanCount;
    private int minRooms;
    private int maxRooms;
    public List<int> endRooms;

    private int bossRoomIndex;
    private int secretRoomIndex;
    private int shopRoomIndex;
    private int itemRoomIndex;

    public Cell cellPrefab;
    private float cellSize;
    public Queue<int> cellQueue;
    public List<Cell> spawnedCells;
    public List<int> bigRoomIndexes;

    [Header("Sprite References")]
    [SerializeField] private Sprite item;
    [SerializeField] private Sprite shop;
    [SerializeField] private Sprite boss;
    [SerializeField] private Sprite secret;

    [Header("Room Variations")]
    [SerializeField] private Sprite largeRoom;
    [SerializeField] private Sprite verticalRoom;
    [SerializeField] private Sprite horizontalRoom;
    [SerializeField] private Sprite lShapeRoom;

     private static readonly List<int[]> roomShapes = new()
    {
        new int[]{-1 },
        new int[]{1 },

        new int[]{10 },
        new int[]{-10 },

        new int[] {1,10 },
        new int[] {1,11 },
        new int[] {10,11 },

        new int[] {9,10 },
        new int[] {-1, 9},
        new int[] {-1,10 },

        new int[] {1, -10 },
        new int[] {1, -9 },
        new int[] {-9, -10 },

        new int[] {-1, -10},
        new int[] {-1, -11 },
        new int[]{-10,-11 },

        new int[] { 1,10,11 },
        new int[] {1,-9,-10 },
        new int[] {-1, 9, 10},
        new int[] {-1, -10, -11}
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minRooms = 7;
        maxRooms = 15;
        cellSize = 0.5f;
        spawnedCells = new();

        setupDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            setupDungeon();
            // Debug.Log("Reloading Dungeon");
        }   
    }

    void setupDungeon() {
        // Debug.Log("Setting up Dungeon...");
        
        for(int i = 0; i < spawnedCells.Count; i++) {
            Destroy(spawnedCells[i].gameObject);
        }



        spawnedCells.Clear();

        floorPlan = new int[100];
        floorPlanCount = default;
        cellQueue = new Queue<int>();
        endRooms = new List<int>();

        bigRoomIndexes = new List<int>();


        //Midpoint
        visitCell(45);

        generateDungeon();


    }

    void generateDungeon() {
        while(cellQueue.Count > 0) {
            // Debug.Log(cellQueue.Count);
            int index = cellQueue.Dequeue();
            int x = index % 10;

            bool created = false;

            //If the created bool is already set to true, ignore the result of the visit cell function
            if(x > 1) created |= visitCell(index - 1);
            if(x < 9) created |= visitCell(index + 1);
            if(x > 20) created |= visitCell(index - 10);
            if(x < 70) created |= visitCell(index + 10);

            if(created == false) {
                endRooms.Add(index);
            }
        }

        if(floorPlanCount < minRooms) {
            setupDungeon();
            return;
        }

        cleanEndRoomsList();

        setupSpecialRooms();
    }



    void cleanEndRoomsList() {
        endRooms.RemoveAll(item => bigRoomIndexes.Contains(item) || getNeighbourCount(item) > 1);
    }



    void setupSpecialRooms() {

        bossRoomIndex = endRooms.Count > 0 ? endRooms[endRooms.Count - 1] : -1;

        if(bossRoomIndex != -1) {
            endRooms.RemoveAt(endRooms.Count - 1);
        }

        itemRoomIndex = randomEndRoom();
        shopRoomIndex = randomEndRoom();
        secretRoomIndex = pickSecretRoom();

        if(itemRoomIndex == -1 || shopRoomIndex == -1 || bossRoomIndex == -1 || secretRoomIndex == -1) {
            setupDungeon();
            return;
        }

        spawnRoom(secretRoomIndex);
        updateSpecialRoomVisuals();


    }

    void updateSpecialRoomVisuals() {
    foreach(var cell in spawnedCells) {
        if(cell.index == itemRoomIndex) {
            cell.setSpecialRoomSprite(item);
        }

        if(cell.index == shopRoomIndex) {
            cell.setSpecialRoomSprite(shop);
        }

        if(cell.index == bossRoomIndex) {
            cell.setSpecialRoomSprite(boss);
        }

        if(cell.index == secretRoomIndex) {
            cell.setSpecialRoomSprite(secret);
        }
    }

    }

    int randomEndRoom() {
        if(endRooms.Count == 0) {
            return -1;
        }

        int randomRoom = Random.Range(0, endRooms.Count);
        int index = endRooms[randomRoom];

        endRooms.RemoveAt(randomRoom);

        return index;
    }

    int pickSecretRoom() {
        for(int attempt = 0; attempt < 900; attempt++) {
            int x = Mathf.FloorToInt(Random.Range(0f, 1f) * 9) + 1;
            int y = Mathf.FloorToInt(Random.Range(0f, 1f) * 8) + 2;

            int index = y * 10 + x;
            Debug.Log(index);

            if(floorPlan[index] != 0) {
                continue;
            }

            if(bossRoomIndex == index - 1 || bossRoomIndex == index + 1 || bossRoomIndex == index + 10 || bossRoomIndex == index - 10) {
                continue;
            }

            if(index - 1 < 0 || index + 1 >= floorPlan.Length || index - 10 < 0 || index + 10 >= floorPlan.Length) {
                continue;
            }

            int neighbours = getNeighbourCount(index);

            if(neighbours >= 3 || (attempt > 300 && neighbours >= 2) || (attempt > 600 && neighbours >= 1)) {
                return index;
            }

        }

        return -1;
    }

    private int getNeighbourCount(int index) {
        // Debug.Log("this is the index:");
        // Debug.Log(index);
        // Debug.Log("this is what is returned:");
        // Debug.Log(floorPlan[index - 10] + floorPlan[index -1] + floorPlan[index + 1] + floorPlan[index + 10]);
        // Debug.Log("this is array bounds:");
        // Debug.Log(floorPlan.Count);

        //Wasn't working, needed new boundary checks
        // return floorPlan[index - 10] + floorPlan[index -1] + floorPlan[index + 1] + floorPlan[index + 10];

        int count = 0;

        if (index - 10 >= 0) {
            count += floorPlan[index - 10];
        }
        if (index - 1 >= 0) {
            count += floorPlan[index - 1];
        }
        if (index + 1 < floorPlan.Length){
            count += floorPlan[index + 1];
        } 
        if (index + 10 < floorPlan.Length) {
            count += floorPlan[index + 10];
        }

        return count;
    }

    
    //Checks cell to see if parameters are good
    private bool visitCell(int index) {
        

        if(index < 0 || index >= floorPlan.Length) {
            return false;
        }


        //If floor is occupied, there are multiple neighbours, the floorplan is greater than the max rooms, or randomly 50% chance, return false
        if(floorPlan[index] != 0 || getNeighbourCount(index) > 1 || floorPlanCount > maxRooms || Random.value < 0.5f) {
            
            // Debug.Log("Visit Cell returning false");
            return false;
        }

        if(Random.value < 0.3f && index != 45) {
            foreach(var shape in roomShapes.OrderBy(_ => Random.value))
            {
                if(tryPlaceRoom(index, shape)) {
                    return true;
                }
            }
        }

        //Add cell to queue, increase floor plan, set floorplan to occupied (1)
        cellQueue.Enqueue(index);
        // Debug.Log(cellQueue.Count);
        floorPlan[index] = 1;
        floorPlanCount++;

        spawnRoom(index);

        return true;
    }

    private void spawnRoom(int index) {
        int x = index % 10; //1s place
        int y = index / 10; //10s place
        Vector2 position = new Vector2(x * cellSize, -y * cellSize);

        Cell newCell = Instantiate(cellPrefab, position, Quaternion.identity);
        newCell.value = 1;
        newCell.index = index;

        spawnedCells.Add(newCell);

        // Debug.Log(newCell);

    }

    private bool tryPlaceRoom(int origin, int[] offsets)   {
        List<int> currentRoomIndexes = new List<int>() { origin };

        foreach(var offset in offsets)
        {
            int currentIndexChecked = origin + offset;

            if(currentIndexChecked - 10 < 0 || currentIndexChecked + 10 >= floorPlan.Length)
            {
                return false;
            }

            if (floorPlan[currentIndexChecked] != 0)
            {
                return false;
            }

            if (currentIndexChecked == origin) continue;
            if (currentIndexChecked % 10 == 0) continue;

            currentRoomIndexes.Add(currentIndexChecked);
        }

        if (currentRoomIndexes.Count == 1) return false;

        foreach(int index in currentRoomIndexes)
        {
            floorPlan[index] = 1;
            floorPlanCount++;
            cellQueue.Enqueue(index);

            bigRoomIndexes.Add(index);
        }

        spawnLargeRoom(currentRoomIndexes);

        return true;
    }

    private void spawnLargeRoom(List<int> largeRoomIndexes) {
        Cell newCell = null;

        int combinedX = default;
        int combinedY = default;
        float offset = cellSize / 2f;

        for(int i = 0; i < largeRoomIndexes.Count; i++)
        {
            int x = largeRoomIndexes[i] % 10;
            int y = largeRoomIndexes[i] / 10;
            combinedX += x;
            combinedY += y;
        }

        if(largeRoomIndexes.Count == 4)
        {
            Vector2 position = new Vector2(combinedX / 4 * cellSize + offset, -combinedY / 4 * cellSize - offset);

            newCell = Instantiate(cellPrefab, position, Quaternion.identity);
            newCell.setRoomSprite(largeRoom);
        }

        if(largeRoomIndexes.Count == 3)
        {
            Vector2 position = new Vector2(combinedX / 3 * cellSize + offset, -combinedY / 3 * cellSize - offset);
            newCell = Instantiate(cellPrefab, position, Quaternion.identity);
            newCell.setRoomSprite(lShapeRoom);
            newCell.rotateCell(largeRoomIndexes);
        }

        if (largeRoomIndexes.Count == 2)
        {
            if (largeRoomIndexes[0] + 10 == largeRoomIndexes[1] || largeRoomIndexes[0] - 10 == largeRoomIndexes[1])
            {
                Vector2 position = new Vector2(combinedX / 2 * cellSize, -combinedY / 2 * cellSize - offset);
                newCell = Instantiate(cellPrefab, position, Quaternion.identity);
                newCell.setRoomSprite(verticalRoom);
            }
            else if (largeRoomIndexes[0] + 1 == largeRoomIndexes[1] || largeRoomIndexes[0] - 1 == largeRoomIndexes[1])
            {
                Vector2 position = new Vector2(combinedX / 2 * cellSize + offset, -combinedY / 2 * cellSize);
                newCell = Instantiate(cellPrefab, position, Quaternion.identity);
                newCell.setRoomSprite(horizontalRoom);
            }
        }
        spawnedCells.Add(newCell);
    }
}
