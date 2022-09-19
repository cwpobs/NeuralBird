using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer birdVisualSpriterenderer;

    private Rigidbody2D birdRigidBody;
    private CircleCollider2D birdCollider;
    private SpriteRenderer birdSpriteRenderer;
    
    private Vector2 birdFlapForceVec = new Vector2(0.0f, 5.0f);

    private bool isDead = false;

    public float input1 = 0.0f;
    public float input2 = 0.0f;
    public float input3 = 0.0f;
    public float input4 = 0.0f;

    public float[] input1Weights;
    public float[] input2Weights;
    public float[] input3Weights;
    public float[] input4Weights;

    public float[] hiddenLayer;
    public float[] hiddenLayerWeights;

    public float neyroResult = 0.0f;

    public float birdDistance = 0.0f;

    private float flapTimer = 0.0f;
    private float flapTime = 0.01f;

    void Start()
    {
        birdRigidBody = GetComponent<Rigidbody2D>();
        birdCollider = GetComponent<CircleCollider2D>();
        birdSpriteRenderer = GetComponent<SpriteRenderer>();

        input1Weights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        input2Weights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        input3Weights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        input4Weights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

        hiddenLayer = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        hiddenLayerWeights = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

        RandomizeNeyroNetwork(1.0f);
    }

    public float[] GetInput1Weights()
    {
        return input1Weights;
    }
    public float[] GetInput2Weights()
    {
        return input2Weights;
    }
    public float[] GetInput3Weights()
    {
        return input2Weights;
    }
    public float[] GetInput4Weights()
    {
        return input2Weights;
    }
    public float[] GetHiddenLayerWeinghts()
    {
        return hiddenLayerWeights;
    }

    public void SetInpit1Weights(float[] weinghts)
    {
        for (int i = 0; i < input1Weights.Length; i++)
        {
            input1Weights[i] = weinghts[i];
        }
    }
    public void SetInpit2Weights(float[] weinghts)
    {
        for (int i = 0; i < input2Weights.Length; i++)
        {
            input2Weights[i] = weinghts[i];
        }
    }
    public void SetInpit3Weights(float[] weinghts)
    {
        for (int i = 0; i < input3Weights.Length; i++)
        {
            input1Weights[i] = weinghts[i];
        }
    }
    public void SetInpit4Weights(float[] weinghts)
    {
        for (int i = 0; i < input4Weights.Length; i++)
        {
            input1Weights[i] = weinghts[i];
        }
    }
    public void SetHiddenLayerWeights(float[] weinghts)
    {
        for (int i = 0; i < hiddenLayerWeights.Length; i++)
        {
            hiddenLayerWeights[i] = weinghts[i];
        }
    }
    public void NeyroNetMutate(float randomSeed)
    {
        for (int i = 0; i < input1Weights.Length; i++)
        {
            input1Weights[i] += Random.Range(-randomSeed, randomSeed);
        }

        for (int i = 0; i < input2Weights.Length; i++)
        {
            input2Weights[i] += Random.Range(-randomSeed, randomSeed);
        }

        for (int i = 0; i < input3Weights.Length; i++)
        {
            input3Weights[i] += Random.Range(-randomSeed, randomSeed);
        }

        for (int i = 0; i < input4Weights.Length; i++)
        {
            input4Weights[i] += Random.Range(-randomSeed, randomSeed);
        }

        for (int i = 0; i < hiddenLayerWeights.Length; i++)
        {
            hiddenLayerWeights[i] += Random.Range(-randomSeed, randomSeed);
        }


    }
    void Update()
    {
        SetBirdRotation();
        flapTimer += Time.deltaTime;
        if (flapTimer >= flapTime)
        {
            flapTimer = 0.0f;
            if (isDead == false)
            {
                CalculateNeuroNet();
                if (neyroResult > 0.5f)
                {
                    Flap();
                }
            }
        }
    }

    private void SetBirdRotation()
    {
        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), 30 * input4);
       
    }
    private float SigmoidActivationFunction(float input)
    {
        return (1.0f / (1 + Mathf.Exp(-input)));
    }

    private void CalculateNeuroNet()
    {
        neyroResult = 0.0f;


        for (int i = 0; i < hiddenLayer.Length; i++)
        {
            hiddenLayer[i] = 0.0f;
        }

        for (int i = 0; i < hiddenLayer.Length; i++)
        {
            hiddenLayer[i] += input1 * input1Weights[i] + input2 * input2Weights[i]+ input3 * input3Weights[i] + input4 * input4Weights[i];
            hiddenLayer[i] = SigmoidActivationFunction(hiddenLayer[i]);
        }

        for (int i = 0; i < hiddenLayer.Length; i++)
        {
            neyroResult += hiddenLayer[i] * hiddenLayerWeights[i];
        }
       
    }

    public void RandomizeNeyroNetwork(float randomSeed)
    {
        for (int i = 0; i < input1Weights.Length; i++)
        {
            input1Weights[i] = Random.Range(-randomSeed, randomSeed);
            input2Weights[i] = Random.Range(-randomSeed, randomSeed);
            input3Weights[i] = Random.Range(-randomSeed, randomSeed);
            input4Weights[i] = Random.Range(-randomSeed, randomSeed);
            hiddenLayerWeights[i] = Random.Range(-randomSeed, randomSeed);
        }
    }

    public void Flap()
    {
        birdRigidBody.AddForce(birdFlapForceVec,ForceMode2D.Impulse);
    }

    public void SetDead()
    {
        if (isDead == false)
        {
            isDead = true;
            birdCollider.enabled = false;
            birdVisualSpriterenderer.enabled = false;
            //birdSpriteRenderer.enabled = false;
            //Debug.Log("bird is dead");
        }
    }

    public void SetAlive()
    {
        isDead = false;
        birdCollider.enabled = true;
        birdRigidBody.velocity = Vector2.zero;
        birdRigidBody.angularVelocity = 0.0f;
        transform.rotation = Quaternion.identity;
        birdRigidBody.angularDrag = 0.0f;
        birdDistance = 0.0f;
        //birdSpriteRenderer.enabled = true;
        birdVisualSpriterenderer.enabled = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        SetDead();
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public float GetVelocityX()
    {
        return birdRigidBody.velocity.x;
    }
    public float GetVelocityY()
    {
        return birdRigidBody.velocity.y;
    }

}
