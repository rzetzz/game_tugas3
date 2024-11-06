using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LSystem : MonoBehaviour
{
    public int iterations = 5;
    public float angle = 25f;
    public float angleZ = 25f;
    public float length = 1f;
    private string axiom = "F";
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private string currentString;
    public string theRules = "FF+[z+F-F-FD]-[-ZF+F+FD]z[z--FZFZFD]";
    public GameObject cylinderPrefab;
    public GameObject leafPrefab;
    private List<GameObject> cylinders = new List<GameObject>();

    private int currentIteration = 0;
    private bool iterationInProgress = false;

    [SerializeField] TextMeshProUGUI textIteration;

    private void Start()
    {
        rules.Add('F', theRules);
        currentString = axiom;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentIteration < iterations && !iterationInProgress)
        {
            iterationInProgress = true;
            currentString = GenerateNextString(currentString);
            ClearCylinders();
            DrawLSystem(currentString);
            currentIteration++;
            iterationInProgress = false;

            
        }

        textIteration.text = "Current Iteration = " + currentIteration;
    }

    private string GenerateNextString(string input)
    {
        string result = "";

        foreach (char c in input)
        {
            result += rules.ContainsKey(c) ? rules[c] : c.ToString();
        }
        return result;
    }

    public void ClearCylinders()
    {
        foreach (GameObject cylinder in cylinders)
        {
            Destroy(cylinder);
        }
        cylinders.Clear();
    }

    public void ResetTree()
    {
        ClearCylinders();
        currentIteration = 0;
        currentString = axiom;
    }

    private void DrawLSystem(string input)
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Vector3 position = Vector3.zero;
        Vector3 direction = Vector3.up;

        foreach (char c in input)
        {
            if (c == 'F')
            {
                Vector3 newPosition = position + direction * length;
                CreateCylinder(position, newPosition);
                position = newPosition;
            }
            else if (c == '+')
            {
                direction = Quaternion.Euler(angle, 0, 0) * direction;
            }
            else if (c == 'D')
            {
                CreateLeaf(position, direction);
            }
            else if (c == '-')
            {
                direction = Quaternion.Euler(-angle, 0, 0) * direction;
            }
            else if (c == 'Z')
            {
                direction = Quaternion.Euler(0, angleZ, 0) * direction;
            }
            else if (c == 'z')
            {
                direction = Quaternion.Euler(0, -angleZ, 0) * direction;
            }
            else if (c == '[')
            {
                transformStack.Push(new TransformInfo
                {
                    position = position,
                    direction = direction
                });
            }
            else if (c == ']')
            {
                TransformInfo ti = transformStack.Pop();
                position = ti.position;
                direction = ti.direction;
            }
        }
    }

    private void CreateLeaf(Vector3 position, Vector3 direction)
    {
        GameObject leaf = Instantiate(leafPrefab, position, Quaternion.identity);
        leaf.transform.up = direction;
        cylinders.Add(leaf);
    }

    private void CreateCylinder(Vector3 start, Vector3 end)
    {
        GameObject cylinder = Instantiate(cylinderPrefab);
        float distance = Vector3.Distance(start, end);
        Vector3 midPoint = (start + end) / 2;
        cylinder.transform.position = midPoint;
        cylinder.transform.up = end - start;
        cylinder.transform.localScale = new Vector3(1f, distance / 2, 1f);
        cylinders.Add(cylinder);
    }

    private struct TransformInfo
    {
        public Vector3 position;
        public Vector3 direction;
    }
}
