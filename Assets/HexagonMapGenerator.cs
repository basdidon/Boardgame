using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[RequireComponent(typeof(Grid))] 
public class HexagonMapGenerator : MonoBehaviour
{
    public Grid Grid;
    public GameObject hexTile;
    public int circumRadius;
    public float inRadius;
    public Vector2Int mapSize;

    public void Setup()
    {
        inRadius = circumRadius * Mathf.Cos(Mathf.PI / 6);

        Grid.cellSwizzle = GridLayout.CellSwizzle.XZY;
        Grid.cellSize = new(2 * inRadius, 2 * circumRadius);
    }

    public void Create()
    {

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.y; z++)
            {
                var clone = Instantiate(hexTile, transform);
                clone.transform.position = Grid.GetCellCenterWorld(new Vector3Int(x, z, 0));// new Vector3(posX, 0, posZ);
            }
        }
    }

    public void ClearMap()
    {
        var loopBlocker = 0;
        while(loopBlocker < 10000 && transform.childCount > 0)
        {
            loopBlocker++;
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}

[CustomEditor(typeof(HexagonMapGenerator))]
public class HexagonMapGeneratorEditor: Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new();
        InspectorElement.FillDefaultInspector(container, serializedObject, this);

        Button createMapButton = new() { text = "GenerateMap"};
        createMapButton.clicked += (target as HexagonMapGenerator).Create;

        Button clearButton = new() { text = "ClearMap"};
        clearButton.clicked += (target as HexagonMapGenerator).ClearMap;

        Button setupButton = new() { text = "Setup" };
        setupButton.clicked += (target as HexagonMapGenerator).Setup;

        container.Add(createMapButton);
        container.Add(clearButton);
        container.Add(setupButton);
        
        return container;

    }
}