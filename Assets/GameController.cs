using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject birdPrefab;
    public GameObject columnPrefab;
    public int GenerationNumber = 0;
    public float BestGenerationNumber = 0;
  
    private float silingHeight = 5.0f;
    private float floorHeight = -5.0f;

    private float leftEdgeX = -8.88f;
    private float rightEdgeX = 8.88f;

    private float colunmMinHeinght = -2.0f;
    private float colunmMaxHeinght = 2.0f;

    private float moveSpeed = 8.0f;
    private float columnCreationTime = 0.8f;
    private float columnCreationTimer = 0.8f;

    private float birdStartLineX = 0.0f;

    private List<GameObject> Columns;
    private List<GameObject> Birds;

    private int numberOfBirds = 500;
    private int liveBirdCount;

    private Vector2 currentColumnPosition;


    public float[] bestGenInput1Weights;
    public float[] bestGenInput2Weights;
    public float[] bestGenInput3Weights;
    public float[] bestGenInput4Weights;
    public float[] bestGenHiddenLayerWeights;

    public float GenerationDistance;
    public float BestDistance;
    public float TotalDistance;
    public float AverageDistance;
    

    private Label textLabel;
    private string outputText;

    void Start()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        textLabel = rootVisualElement.Q<Label>("TextLabel");
        textLabel.text = "EDITED TEXT";

        Columns = new List<GameObject>();
        Birds = new List<GameObject>();

        bestGenInput1Weights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        bestGenInput2Weights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        bestGenInput3Weights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        bestGenInput4Weights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        bestGenHiddenLayerWeights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

        currentColumnPosition = new Vector2(0.0f,0.0f);
        GenerationDistance = 0.0f;
        BestDistance = 0.0f;
        TotalDistance = 0.0f;
        AverageDistance = 0.0f;
        

        CreateBirds();
    }

    void OutputDataText()
    {
        outputText = "";
        outputText += "Поколение: " + GenerationNumber.ToString() + "\n\n";
        outputText += "Осталось птиц: " + liveBirdCount.ToString() + "\n";
        outputText += "Дистанция поколения: " + GenerationDistance.ToString("0.00") + "\n\n";
        outputText += "Лучшая дистанция: " + BestDistance.ToString("0.00") + "\n";
        outputText += "Рекорд поставлен в: " + BestGenerationNumber.ToString() + " поколени\n\n";
        outputText += "Общая дистанция: " + TotalDistance.ToString("0.00") + "\n";
        outputText += "Средняя дистанция: " + AverageDistance.ToString("0.00") + " \n\n";

        textLabel.text = outputText;
    }
    void Update()
    {
        GenerationDistance += Time.deltaTime * moveSpeed;
        TotalDistance+= Time.deltaTime * moveSpeed;
        AverageDistance = TotalDistance / (GenerationNumber + 1);

        columnCreationTimer += Time.deltaTime;
        if (columnCreationTimer >= columnCreationTime)
        {
            columnCreationTimer = 0.0f;
            CreateColumn();
        }

        MoveColumns();
        CheckBirdsConditions();
        CheckGenerationConditions();
        FindCurrentTargetColumnPosition();
        SetBirdsInputData();
        SetBirdsDistance();
        OutputDataText();
        
    }

    private void CreateBirds()
    {
        for (int i = 0; i < numberOfBirds; i++)
        {
            GameObject bird;
            bird = Instantiate(birdPrefab, new Vector2(birdStartLineX, 0.0f), Quaternion.identity);
            Birds.Add(bird);
        }
    }

    private void CheckBirdsConditions()
    {
        foreach (GameObject bir in Birds)
        {
            if (bir.transform.position.y < floorHeight || bir.transform.position.y > silingHeight)
            {
                bir.GetComponent<BirdController>().SetDead();
            }
        }
    }

    private void CheckGenerationConditions()
    {
        int birdDeadCount = 0;
        liveBirdCount = numberOfBirds;
        foreach (GameObject bir in Birds)
        {
            if (bir.GetComponent<BirdController>().GetIsDead() == true)
            {
                birdDeadCount++;
                liveBirdCount--;
            }
        }
        if (birdDeadCount == Birds.Count)
        {
            RestartGeneration();
        }
    }

    private void CreateColumn()
    {
        GameObject column;
        column = Instantiate(columnPrefab, new Vector2(rightEdgeX,Random.Range(colunmMinHeinght,colunmMaxHeinght)), Quaternion.identity);
        Columns.Add(column);
    }

    private void MoveColumns()
    {
        for (int i = Columns.Count - 1; i >= 0; i--)
        {
             Columns[i].transform.position = new Vector2(Columns[i].transform.position.x - moveSpeed * Time.deltaTime, Columns[i].transform.position.y);
            if (Columns[i].transform.position.x < leftEdgeX)
            {
                    GameObject Clone = Columns[i];
                    Columns.RemoveAt(i);
                    Destroy(Clone);
            }
        }
    }
    private void DestroyAllColumns()
    {
        for (int i = Columns.Count - 1; i >= 0; i--)
        {
            GameObject Clone = Columns[i];
            Columns.RemoveAt(i);
            Destroy(Clone);
        }

        columnCreationTime = 0.8f;
        columnCreationTimer = 0.8f;
    }
    private void FindCurrentTargetColumnPosition()
    {

        float minDist = float.PositiveInfinity;
        for (int i = Columns.Count - 1; i >= 0; i--)
        {
            if (Columns[i].transform.position.x > birdStartLineX)
            {
                if (Columns[i].transform.position.x - birdStartLineX < minDist)
                {
                    currentColumnPosition = Columns[i].GetComponent<ColumnController>().GetCentralPointPosition();
                    minDist = Columns[i].transform.position.x - birdStartLineX;
                }
            }
        }

        //Debug.DrawLine(new Vector3(birdStartLineX, 0.0f, 0.0f), new Vector3(currentColumnPosition.x, currentColumnPosition.y, 0.0f));

    }

    private float CalculateNormalizedTargetDistance()
    {
        float result = 0.0f;
        
        float dist = currentColumnPosition.x - birdStartLineX;
        result = dist / (rightEdgeX - birdStartLineX);
        
        return result;
    }

    private float CalculateNormalizedRelativeAttitude(float birdY)
    {
        float result = 0.0f;

        if (birdY == currentColumnPosition.y)
        {
            result = 0.0f;
        }

        if (birdY > currentColumnPosition.y)
        {
            float dist = birdY - currentColumnPosition.y;
            result = dist / (silingHeight - currentColumnPosition.y);
        }

        if (birdY < currentColumnPosition.y)
        {
            float dist = currentColumnPosition.y - birdY;
            result = - dist / (currentColumnPosition.y - floorHeight);
        }
        return result;
    }

    private void SetBirdsInputData()
    {
        foreach (GameObject bir in Birds)
        {
            bir.GetComponent<BirdController>().input1 = CalculateNormalizedTargetDistance();
            bir.GetComponent<BirdController>().input2 = CalculateNormalizedRelativeAttitude(bir.transform.position.y);
            bir.GetComponent<BirdController>().input3 = bir.GetComponent<BirdController>().GetVelocityX() / 10.0f;
            bir.GetComponent<BirdController>().input4 = bir.GetComponent<BirdController>().GetVelocityY() / 10.0f;
        }

    }
    private void SetBirdsDistance()
    {
        foreach (GameObject bir in Birds)
        {
            if (bir.GetComponent<BirdController>().GetIsDead() == false)
            {
                bir.GetComponent<BirdController>().birdDistance += Time.deltaTime * moveSpeed;
            }
        }
    }
    private void FindBestBirdInGeneration()
    {
        float bestDist = float.NegativeInfinity;
        int bestIndex = 0;

        for (int i = Birds.Count - 1; i >= 0; i--)
        {
            if (Birds[i].GetComponent<BirdController>().birdDistance > bestDist)
            {
                bestDist = Birds[i].GetComponent<BirdController>().birdDistance;
                bestIndex = i;
            }
        }

        for (int i = 0; i < bestGenInput1Weights.Length; i++)
        {
            bestGenInput1Weights[i] = Birds[bestIndex].GetComponent<BirdController>().GetInput1Weights()[i];
        }
        for (int i = 0; i < bestGenInput2Weights.Length; i++)
        {
            bestGenInput2Weights[i] = Birds[bestIndex].GetComponent<BirdController>().GetInput2Weights()[i];
        }
        for (int i = 0; i < bestGenInput3Weights.Length; i++)
        {
            bestGenInput3Weights[i] = Birds[bestIndex].GetComponent<BirdController>().GetInput1Weights()[i];
        }
        for (int i = 0; i < bestGenInput4Weights.Length; i++)
        {
            bestGenInput4Weights[i] = Birds[bestIndex].GetComponent<BirdController>().GetInput2Weights()[i];
        }
        for (int i = 0; i < bestGenHiddenLayerWeights.Length; i++)
        {
            bestGenHiddenLayerWeights[i] = Birds[bestIndex].GetComponent<BirdController>().GetHiddenLayerWeinghts()[i];
        }

    }
    private void CreateNextGeneration()
    {
        if (GenerationDistance > BestDistance)
        {
            for (int i = Birds.Count - 1; i >= 0; i--)
            {
                Birds[i].GetComponent<BirdController>().SetInpit1Weights(bestGenInput1Weights);
                Birds[i].GetComponent<BirdController>().SetInpit2Weights(bestGenInput2Weights);
                Birds[i].GetComponent<BirdController>().SetInpit3Weights(bestGenInput1Weights);
                Birds[i].GetComponent<BirdController>().SetInpit4Weights(bestGenInput2Weights);
                Birds[i].GetComponent<BirdController>().SetHiddenLayerWeights(bestGenHiddenLayerWeights);
            }
            BestDistance = GenerationDistance;
            BestGenerationNumber = GenerationNumber;
        }

    }
    private void RestartGeneration()
    {
        FindBestBirdInGeneration();
        CreateNextGeneration();

        GenerationNumber++;
        GenerationDistance = 0.0f;
        

        for (int i = Birds.Count - 1; i >= 0; i--)
        {
            Birds[i].GetComponent<BirdController>().SetAlive();
            Birds[i].transform.position = new Vector2(birdStartLineX, 0.0f);
            if (i % 2 == 0)
            {
                Birds[i].GetComponent<BirdController>().NeyroNetMutate(Random.Range(Random.Range(-5.0f,-0.1f),Random.Range(0.1f,5.0f)));
            }
        }
        DestroyAllColumns();
    }
}
