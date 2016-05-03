using UnityEngine;
using System.Collections;
using Spine.Unity;

public class AnimationController : MonoBehaviour {
    #region variables
    //references
    PlayerController playerController;
    GirlAudioScript girlAudio;
    GameObject girl;
    Rigidbody2D playerRb;
    [HideInInspector]
    public SkeletonAnimation skeletonAnimation;
    [HideInInspector]
    public Spine.SkeletonData skeletonData;
    Spine.Skeleton skeleton;
    Spine.AnimationStateData state;
    //animations
    public Spine.Animation idle;
    public Spine.Animation jump;
    public Spine.Animation run;
    public Spine.Animation fall;
    public Spine.Animation ledgeGrab;

    [Header("Foot tranforms for footsteps")]
    public Transform frontFootTransform;
    public Transform rearFootTransform;


    private float stupidTimer = .5f;
    #endregion

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerRb = GetComponent<Rigidbody2D>();
        girlAudio = GetComponent<GirlAudioScript>();
        girl = GameObject.Find("Girl");
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        skeletonData = skeletonAnimation.Skeleton.Data;
        skeleton = new Spine.Skeleton(skeletonData);
        skeletonAnimation.AnimationName = "get up";
        skeletonAnimation.timeScale = 100;
    }


    void Update()
    {
        //turning the character around
        if (playerController.moveVector.x != 0)
        {
            girl.transform.localScale = new Vector3(Mathf.Sign(playerController.moveVector.x), 1, 1);
        }

        AnimationState();
        //update state data
        state = skeletonAnimation.state.Data;
    }

    void SoundFX()
    {
        //find distance from foot to ground
        //front foot
        Spine.Bone frontFoot = skeleton.FindBone("foot_front");
        Vector3 frontFootPos = frontFoot.GetWorldPosition(frontFootTransform);
        float frontDist = Vector3.Distance(frontFootPos, playerController.groundSurface.point);
        //rear foot
        Spine.Bone rearFoot = skeleton.FindBone("foot_real");
        Vector3 rearFootPos = rearFoot.GetWorldPosition(rearFootTransform);
        float backDist = Vector3.Distance(rearFootPos, playerController.groundSurface.point);
        //play footsteps when foot is near ground
        if (frontDist <= 1 || backDist <= 1)
        {
            girlAudio.FootStepAudio();
        }
    }

    void AnimationState()
    {
        switch (playerController.moveState)
        {
            case PlayerController.MoveState.getup:
                skeletonAnimation.loop = false;
                skeletonAnimation.AnimationName = "get up";

                if (Input.GetAxis("Vertical") > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || stupidTimer >= 0)
                {
                    skeletonAnimation.timeScale = 1;
                    stupidTimer -= Time.deltaTime;
                }
                else
                {
                    skeletonAnimation.timeScale = Mathf.Lerp(skeletonAnimation.timeScale, 0, Time.deltaTime * 10);
                }
                break;
            case PlayerController.MoveState.standing:
                skeletonAnimation.loop = true;
                skeletonAnimation.timeScale = 1;
                skeletonAnimation.AnimationName = "idle";
                break;
            case PlayerController.MoveState.walking:
                skeletonAnimation.loop = true;
                skeletonAnimation.timeScale = playerRb.velocity.magnitude * .07f;
                skeletonAnimation.AnimationName = "run";
                SoundFX();
                break;
            case PlayerController.MoveState.jumping:
                skeletonAnimation.loop = false;
                skeletonAnimation.timeScale = .5f;
                skeletonAnimation.AnimationName = "jump";
                break;
            case PlayerController.MoveState.ledgegrab:
                skeletonAnimation.loop = false;
                if ((!playerController.yClimbed && Input.GetAxis("Vertical") > 0)
                    || !playerController.xClimbed && Input.GetAxis("Horizontal") > 0)
                {
                    skeletonAnimation.timeScale = 3;
                }
                else
                {
                    skeletonAnimation.timeScale = 0;
                }
                skeletonAnimation.AnimationName = "grab";
                break;
            case PlayerController.MoveState.falling:
                skeletonAnimation.loop = false;
                skeletonAnimation.timeScale = -.5f;
                skeletonAnimation.AnimationName = "fall";
                break;
            default:
                skeletonAnimation.loop = true;
                skeletonAnimation.timeScale = 1;
                skeletonAnimation.AnimationName = "get up";
                break;
        }
    }
}
