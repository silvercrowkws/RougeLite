using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// DFS(깊이 우선 탐색)
// 그래프나 트리 구조를 탐색할 때 사용하는데
// 미로처럼 방과 방이 연결된 구조또한 사용할 수 있다.
// DFS는 시작 노드(방)에서 한 방향으로 쭉 깊게 들어가다가,
// 더 이상 갈 곳이 없으면 뒤로 돌아와서 다른 경로를 탐색하는 방식

// 1. 현재 방을 방문 표시하고
// 2. 갈 수 있는 이웃 방 중 방문하지 않은 방을 하나 골라 이동
// 3. 이동한 방에서도 1,2 번 방식으로 계속 탐색
// 4. 더 이상 갈 수 있는 방이 없으면 한 단계 이전 방으로 되돌아가서 다른 이웃 방을 찾고
// 5. 모든 방을 방문할 때까지 반복

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;     // 벽으로 둘러싸인 방 프리팹
    [SerializeField] private Transform roomParent;      // Grid나 다른 부모 오브젝트

    [Tooltip("마름모 반지름 (반지름은 홀수여야 함 ex : 반지름이 3이면 9개의 방 생성)")]
    [SerializeField] private int radius = 3;

    /*/// <summary>
    /// 미로를 구성하는 방들의 고정된 위치 배열
    /// </summary>
    private Vector2Int[] mazePositions = new Vector2Int[]
    {
        new Vector2Int(0, 0),
        new Vector2Int(0, 2),
        new Vector2Int(2, 1),
        new Vector2Int(4, 0),
        new Vector2Int(2, -1),
        new Vector2Int(0, -2),
        new Vector2Int(-2, -1),
        new Vector2Int(-4, 0),
        new Vector2Int(-2, 1),
        new Vector2Int(0, 4),
        new Vector2Int(2, 3),
        new Vector2Int(4, 2),
        new Vector2Int(6, 1),
        new Vector2Int(8, 0),
        new Vector2Int(6, -1),
        new Vector2Int(4, -2),
        new Vector2Int(2, -3),
        new Vector2Int(0, -4),
        new Vector2Int(-2, -3),
        new Vector2Int(-4, -2),
        new Vector2Int(-6, -1),
        new Vector2Int(-8, 0),
        new Vector2Int(-6, 1),
        new Vector2Int(-4, 2),
        new Vector2Int(-2, 3)
    };*/

    /// <summary>
    /// 자동 생성된 방 위치 리스트
    /// </summary>
    private List<Vector2Int> mazePositions;

    /// <summary>
    /// 시드 값(-1 이면 완전 랜덤)
    /// </summary>
    public int seed = -1;

    /// <summary>
    /// 완전 랜덤 여부를 나타내는 상수
    /// </summary>
    private const int AllRandom = -1;

    /// <summary>
    /// 좌표에 대응하는 Room 객체 저장 딕셔너리(키 : 위치, 값 : 룸)
    /// </summary>
    private Dictionary<Vector2Int, Room> roomDictionary = new();

    /// <summary>
    /// 미로 생성 시 방문한 방 기록
    /// </summary>
    private HashSet<Vector2Int> visited = new();

    /// <summary>
    /// 마름모 형태의 방 연결 방향 정의 (위, 오른쪽, 아래, 왼쪽)
    /// </summary>
    private static readonly List<(Vector2Int offset, Direction dir)> DirectionOffsets = new()
    {
        (new Vector2Int(2, 1), Direction.Top),
        (new Vector2Int(2, -1), Direction.Right),
        (new Vector2Int(-2, -1), Direction.Bottom),
        (new Vector2Int(-2, 1), Direction.Left)
    };

    void Awake()
    {
        // 시드가 랜덤이 아니면 그 값을 기반으로 결정
        if (seed != AllRandom)
        {
            UnityEngine.Random.InitState(seed);
            Debug.Log($"[Seed 설정됨] : {seed}");
        }
        else
        {
            Debug.Log("[랜덤 시드] 완전 랜덤으로 실행");
        }

        // 자동으로 좌표 생성
        mazePositions = GenerateOrderedIsometricPositions(radius);
    }

    void Start()
    {
        GenerateRooms();                    // 방을 위치에 맞게 생성
        GenerateMazeWithInitialOpening();   // (0,0) 에서 시작해 한 벽을 열고 DFS(깊이 우선 탐색)로 미로 생성
        //GenerateMaze(mazePositions[0]);
    }

    /// <summary>
    /// 방을 생성하는 함수
    /// </summary>
    private void GenerateRooms()
    {
        for (int i = 0; i < mazePositions.Count; i++)
        {
            Vector2Int pos = mazePositions[i];                  // 생성할 위치
            Vector3 worldPos = new Vector3(pos.x, pos.y, 0);    // 월드 좌표로 변환
            GameObject roomObj = Instantiate(roomPrefab, worldPos, Quaternion.identity, roomParent);    // 프리팹 생성
            roomObj.name = $"Room_({pos.x},{pos.y})";           // 이름 변경

            Room room = roomObj.GetComponent<Room>();           // Room 스크립트 가져옴
            if (room == null)
            {
                Debug.LogError($"Room 컴포넌트가 Room Prefab에 없습니다: {roomObj.name}");
                continue;
            }

            roomDictionary[pos] = room;                         // 딕셔너리에 저장
        }

        // 방 생성이 모두 완료되었으면 미로로 변경하는 부분이 필요함
    }

    /// <summary>
    /// 시작 방 (0,0)의 벽 하나를 무작위로 열고,
    /// 해당 방향으로부터 DFS로 미로 생성하는 함수
    /// </summary>
    private void GenerateMazeWithInitialOpening()
    {
        Vector2Int startPos = new Vector2Int(0, 0);     // 시작 방의 좌표
        Room startRoom = roomDictionary[startPos];      // 시작 방

        // 무작위로 방향 섞기
        var directions = DirectionOffsets.OrderBy(_ => UnityEngine.Random.value).ToList();

        foreach (var (offset, dir) in directions)
        {
            Vector2Int neighborPos = startPos + offset;                 // 이웃 방의 좌표 계산

            if (!roomDictionary.ContainsKey(neighborPos)) continue;     // 이웃 방이 없으면 스킵

            // 시작 방과 이웃 방의 벽 제거
            startRoom.RemoveWall(dir);
            roomDictionary[neighborPos].RemoveWall(GetOppositeDirection(dir));

            // DFS 시작
            visited.Clear();                // 방문 기록 초기화
            visited.Add(startPos);          // 시작 방 방문 체크
            GenerateMaze(neighborPos);      // DFS로 미로 생성 시작
            break;
        }
    }

    /// <summary>
    /// DFS(깊이 우선 탐색)로 미로 생성하는 함수
    /// </summary>
    /// <param name="currentPos"></param>
    private void GenerateMaze(Vector2Int currentPos)
    {
        visited.Add(currentPos);        // 들어온 방 방문 체크

        /*List<(Vector2Int offset, Direction dir)> directions = new()
        {
            (new Vector2Int(2, 1), Direction.Top),
            (new Vector2Int(2, -1), Direction.Right),
            (new Vector2Int(-2, -1), Direction.Bottom),
            (new Vector2Int(-2, 1), Direction.Left)
        };

        directions = directions.OrderBy(x => UnityEngine.Random.value).ToList();*/

        // 무작위로 방향 섞기
        var directions = DirectionOffsets.OrderBy(_ => UnityEngine.Random.value).ToList();

        foreach (var (offset, dir) in directions)
        {
            Vector2Int neighborPos = currentPos + offset;       // 이웃 방 좌표 계산

            // 이웃 방이 없거나 이미 방문했으면 스킵
            if (!roomDictionary.ContainsKey(neighborPos) || visited.Contains(neighborPos)) continue;

            Room currentRoom = roomDictionary[currentPos];          // 현재 방
            Room neighborRoom = roomDictionary[neighborPos];        // 이웃 방

            currentRoom.RemoveWall(dir);                            // 현재 방의 벽 제거
            neighborRoom.RemoveWall(GetOppositeDirection(dir));     // 이웃 방의 벽 제거

            GenerateMaze(neighborPos);                              // 재귀 호출로 DFS 계속 진행
        }
    }

    /// <summary>
    /// 마름모 구조에 따라 생성 순서 보장된 좌표 생성
    /// </summary>
    /// <param name="radius"> 몇 겹의 껍질(링)을 만들 것인지 지정하는 값 (3, 5, 7, 9, 11)
    /// 3: 25개, 5: 25개, 7 : 49개, 9 : 81개, 11 : 121개... </param>
    /// <returns></returns>
    private List<Vector2Int> GenerateOrderedIsometricPositions(int radius)
    {
        // 결과를 순서대로 담을 리스트
        List<Vector2Int> positions = new();

        // BFS 탐색용 큐
        Queue<Vector2Int> frontier = new();

        // 중복 방문 방지
        HashSet<Vector2Int> visited = new();

        Vector2Int start = Vector2Int.zero;     // 시작 지점
        positions.Add(start);                   // 시작 지점 위치 리스트에 추가
        visited.Add(start);                     // 시작 지점 방문했다고 표시
        frontier.Enqueue(start);                // 탐색 시작점으로 큐에 추가

        // BFS 시작
        while (frontier.Count > 0)
        {
            // 큐에서 현재 좌표 꺼냄
            var current = frontier.Dequeue();

            // 네 방향(↗, ↘, ↙, ↖) 순회
            foreach (var (offset, _) in DirectionOffsets)
            {
                Vector2Int neighbor = current + offset;     // 인접 좌표 계산

                // Isometric Tilemap 기준 마름모 거리 계산
                // x는 2씩 이동하기 때문에 x/2로 보정
                int distance = Mathf.Abs(neighbor.x / 2) + Mathf.Abs(neighbor.y);

                // 이미 방문했거나 반지름 초과 시 무시
                if (visited.Contains(neighbor) || distance > radius) continue;

                // 새 좌표 등록
                visited.Add(neighbor);
                frontier.Enqueue(neighbor);
                positions.Add(neighbor);
            }
        }

        return positions;
    }

    /// <summary>
    /// 허물어진 벽의 반대 방향을 구하기 위한 함수
    /// (벽을 없앨때 2개씩 없애야 하기 때문)
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Direction GetOppositeDirection(Direction dir)
    {
        return dir switch
        {
            Direction.Top => Direction.Bottom,
            Direction.Bottom => Direction.Top,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => dir        // 여기 들어오면 버그
        };
    }
}
