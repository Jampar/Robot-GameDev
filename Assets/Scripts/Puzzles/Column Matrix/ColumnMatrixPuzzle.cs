using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FloorPoint
{
    public Vector3 point;
    public bool isGoal;
    public bool hasColumnOn;

    public FloorPoint(Vector3 _point, bool _goal)
    {
        point = _point;
        isGoal = _goal;
        hasColumnOn = false;
    }
}

public class ColumnMatrixPuzzle : Puzzle
{
    [SerializeField]
    public float floorSpacing = 7;
    [SerializeField]
    public int[] floorDimensions = { 3, 3 };

    float columnElevation = 12;
    public Vector3 columnOffset;

    public Vector3 floorGridStart;
    public FloorPoint[] floorPoints;
    public List<FloorPoint> avaliablePoints = new List<FloorPoint>();

    [SerializeField]
    GameObject standardFloor;
    [SerializeField]
    GameObject specialFloor;

    [SerializeField]
    Column[] columns;

    bool playCompleteAnim = true;

    PlayableDirector director;
    public TimelineAsset completionEventTimeline;


    int[,] GenerateFloorArray()
    {
        int[,] floorArray = new int[floorDimensions[0], floorDimensions[1]];

        for (int x = 0; x < floorDimensions[0]; x++)
        {
            int[] floorRow = new int[3];
            int randomIndex = Random.Range(0, floorDimensions[1]);
            floorRow[randomIndex] = 1;

            for (int y = 0; y < floorDimensions[1]; y++)
            {
                floorArray[x, y] = floorRow[y];
            }
        }

        return floorArray;
    }


    FloorPoint[] GenerateFloorPoints()
    {
        int[,] floorArray = GenerateFloorArray();
        List<FloorPoint> floorPoints = new List<FloorPoint>();

        floorGridStart = new Vector3(transform.position.x + -Mathf.FloorToInt(((floorDimensions[0] * floorSpacing) / 2) - 3),
                                    transform.position.y,
                                    transform.position.z + -Mathf.FloorToInt((floorDimensions[1] * floorSpacing) / 2) + 3);

        Debug.Log("Matrix Grid Start " + floorGridStart.ToString());

        for (int x = 0; x < floorDimensions[0]; x++)
        {
            for (int y = 0; y < floorDimensions[1]; y++)
            {
                bool isGoal = false;
                if (floorArray[x, y] == 1)
                {
                    isGoal = true;
                }

                Vector3 floorPoint = new Vector3(floorGridStart.x + (x * floorSpacing),
                                            floorGridStart.y,
                                        floorGridStart.z + (y * floorSpacing));

                FloorPoint point = new FloorPoint(floorPoint, isGoal);
                floorPoints.Add(point);
            }
        }
        return floorPoints.ToArray();
    }


    void CreateFloor()
    {
        floorPoints = GenerateFloorPoints();
        foreach (FloorPoint point in floorPoints)
        {
            GameObject floorType;

            if (point.isGoal) floorType = specialFloor;
            else floorType = standardFloor;

            GameObject floorInstance = Instantiate(floorType);
            floorInstance.transform.position = point.point;
            floorInstance.transform.SetParent(transform.Find("Floors"));
        }
    }

    public FloorPoint GetPoint(Vector3 columnPosition)
    {
        Vector3 floorPos = columnPosition - columnOffset;
        foreach(FloorPoint point in floorPoints)
        {
            if(point.point == floorPos)
            {
                return point;
            }
        }
        return null;
    }

    List<int> numbers = new List<int>();


    public int NewNumber(int r)
    {
        int a = 0;

        while (a == 0)
        {
            a = Random.Range(0, r);
            if (!numbers.Contains(a))
            {
                numbers.Add(a);
            }
            else
            {
                a = 0;
            }
        }
        return a;
    }


    void PlaceColumns()
    {
        foreach (Column column in columns)
        {
            FloorPoint point = floorPoints[NewNumber(floorPoints.Length)];
            column.movementIndex = Random.Range(0, 2);

            column.transform.position = point.point + columnOffset;
            column.puzzle = this;
            column.startPoint = point.point;
        }
    }


    bool CheckColumns()
    {
        foreach (Column column in columns)
        {
            RaycastHit hit;
            Vector3 direction = column.movementVectors[column.movementIndex];

            if (Physics.Raycast(column.transform.position, direction, out hit))
            {
                if (hit.transform.GetComponent<Column>())
                {
                    return false;
                }
            }
        }
        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        director = GameObject.Find("Director").GetComponent<PlayableDirector>();
        columnOffset = new Vector3(0, columnElevation, 0);
        GeneratePuzzle();
    }

    // Update is called once per frame
    void Update()
    {
        if (isComplete() && playCompleteAnim)
        {
            CompletePuzzle();
        }
    }


    void GeneratePuzzle()
    {
        CreateFloor();
        PlaceColumns();
        while (!CheckColumns())
        {
            PlaceColumns();
        }
    }


    public override bool isComplete()
    {
        bool complete = true;
        foreach (Column col in columns)
        {
            if (col.inPlace != true) complete = false;
        }
        return complete;
    }

    FloorPoint[] RemovePointsOnDimension(FloorPoint[] array, Vector3 point, Vector3 direction)
    {
        float ommittingValue = 0.0f;

        List<FloorPoint> remainingPoints = new List<FloorPoint>();

        foreach (FloorPoint element in array)
        {
            if (direction == Vector3.forward)
            {
                ommittingValue = point.x;
                if (element.point.x != ommittingValue)
                {
                    remainingPoints.Add(element);
                }
            }
            if (direction == Vector3.right)
            {
                ommittingValue = point.z;
                if (element.point.z != ommittingValue)
                {
                    remainingPoints.Add(element);
                }
            }
        }

        return remainingPoints.ToArray();
    }


    public bool inBounds(Vector3 test)
    {

        float[] xBounds = { floorGridStart.x, floorGridStart.x + (floorDimensions[0] - 1) * floorSpacing };
        float[] yBounds = { floorGridStart.z, floorGridStart.z + (floorDimensions[1] - 1) * floorSpacing };

        if (test.x <= xBounds[1] && test.x >= xBounds[0])
        {
            if (test.z <= yBounds[1] && test.z >= yBounds[0])
            {
                return true;
            }
        }
        return false;
    }


    public bool IsSharedDestination(Vector3 dest)
    {
        bool result = true;
        foreach (Column col in columns)
        {
            if(col.nextPoint == dest)
            {
                result = false;
            }
        }
        return result;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (floorGridStart != null) Gizmos.DrawCube(floorGridStart, Vector3.one);

        if (floorPoints != null)
        {
            foreach (FloorPoint point in floorPoints)
            {
                if (point.isGoal) Gizmos.color = Color.green;
                else Gizmos.color = Color.red;

                Gizmos.DrawCube(point.point + new Vector3(0, 0.5f, 0), Vector3.one);
            }
        }
    }


    public override void CompletePuzzle()
    {
        director.playableAsset = completionEventTimeline;
        director.Play();
        GetComponent<Animation>().Play();
        playCompleteAnim = false;
    }
}
