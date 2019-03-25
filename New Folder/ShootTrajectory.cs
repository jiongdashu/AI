using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public struct RegisteredArrows
{
    public Arrow real;
    public Arrow hidden;

}
public class ShootTrajectory : MonoBehaviour
{

    public static bool charging;

    public GameObject arrow;
    public Transform referenceArrow;
    public GameObject marker;
    public GameObject objectsToSpawn;

    private Scene mainScene;
    private Scene physicsScene;

    private List<GameObject> markers = new List<GameObject>();
    private Dictionary<string, RegisteredArrows> allArrows = new Dictionary<string, RegisteredArrows>();

    private void Start()
    {
        Physics2D.autoSimulation = false;

        mainScene = SceneManager.GetActiveScene();
        physicsScene = SceneManager.CreateScene("physice-scene", new CreateSceneParameters(LocalPhysicsMode.Physics2D));

        PreparePhysicsScene();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShowTrajectory();
        }

        mainScene.GetPhysicsScene2D().Simulate(Time.fixedDeltaTime);

    }

    public void RegisterArrow(Arrow arrow)
    {
       
        if (!allArrows.ContainsKey(arrow.gameObject.name))
            allArrows[arrow.gameObject.name] = new RegisteredArrows();

        var arrows = allArrows[arrow.gameObject.name];
        if (string.Compare(arrow.gameObject.scene.name, physicsScene.name) == 0)
            arrows.hidden = arrow;
        else
            arrows.real = arrow;

        
        allArrows[arrow.gameObject.name] = arrows;

    }

    public void PreparePhysicsScene()
    {
        SceneManager.SetActiveScene(physicsScene);

        GameObject g = Instantiate(objectsToSpawn);
        g.transform.name = "ReferenceArrow";
        g.GetComponent<Arrow>().isReference = true;
        Destroy(g.GetComponent<SpriteRenderer>());

        SceneManager.SetActiveScene(mainScene);
         
    }

    public void CreateMovementMarkers()
    {
        foreach(var arrowType in allArrows)
        {
            var arrows = arrowType.Value;
            Arrow hidden = arrows.hidden;

            GameObject g = Instantiate(marker, hidden.transform.position, Quaternion.identity);
            g.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            markers.Add(g);


        }
    }

    public void ShowTrajectory()
    {
        SyncArrows();
        allArrows["ReferenceArrow"].hidden.transform.position = referenceArrow.transform.position;
        allArrows["ReferenceArrow"].hidden.GetComponent<Rigidbody2D>().velocity = referenceArrow.transform.TransformDirection(Vector3.up * 15f);
        allArrows["ReferenceArrow"].hidden.GetComponent<Rigidbody2D>().gravityScale = 1.0f;

        int steps =(int)(2f / Time.fixedDeltaTime);
        for(int i = 0; i < steps; i++)
        {
            physicsScene.GetPhysicsScene().Simulate(Time.fixedDeltaTime);
            CreateMovementMarkers();
        }


    }

    public void SyncArrows()
    {

        foreach (var arrowType in allArrows)
        {
            var arrows = arrowType.Value;

            Arrow visual = arrows.real;
            Arrow hidden = arrows.hidden;
            //var rb = hidden.GetComponent<Rigidbody2D>();
            print(hidden);
            print(visual);
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = 0;

            hidden.transform.position = visual.transform.position;
            hidden.transform.rotation = visual.transform.rotation;


        }

    }


}