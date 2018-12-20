using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using System.Collections;

public class TwoBoneIK : MonoBehaviour
{
    public Transform endJoint;
    public Transform bullet;
    public Transform rightHand;

    Transform m_TopJoint;
    Transform m_MidJoint;
    GameObject m_Effector;

    PlayableGraph m_Graph;
    AnimationScriptPlayable m_IKPlayable;

    Transform bul;

    float time;

    void Start() {
        m_Effector = SampleUtility.CreateEffector("Effector_" + endJoint.name, new Vector3(2, 2, -.5f), Quaternion.identity);

        m_MidJoint = endJoint.parent;
        m_TopJoint = m_MidJoint.parent;

        m_Graph = PlayableGraph.Create("TwoBoneIK");
        var output = AnimationPlayableOutput.Create(m_Graph, "ouput", GetComponent<Animator>());

        var twoBoneIKJob = new TwoBoneIKJob();
        twoBoneIKJob.Setup(GetComponent<Animator>(), m_TopJoint, m_MidJoint, endJoint, m_Effector.transform);

        m_IKPlayable = AnimationScriptPlayable.Create(m_Graph, twoBoneIKJob);

        output.SetSourcePlayable(m_IKPlayable);
        m_Graph.Play();
    }

    void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        m_Effector.transform.position = new Vector3(mousePos.x, mousePos.y, -.5f);
        rightHand.LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        if (Input.GetMouseButton(0) && time <= 0) {
            bul = Instantiate(bullet, rightHand.transform.position, Quaternion.identity);
            time = 2;   
        } 

        if (bul != null)  bul.transform.Translate((new Vector3(mousePos.x, mousePos.y, -.5f) - rightHand.transform.position).normalized * .1f, Space.World);

        time -= Time.deltaTime;

    }
    
    private void OnBecameInvisible() {
        Destroy(bul);    
    }

    void OnDisable()
    {
        m_Graph.Destroy();
        Object.Destroy(m_Effector);
    }
}
