using System.Collections;
using System.Collections.Generic;
using PinataMasters;
using UnityEngine;

public class ArenaResource : MonoBehaviour
{
    #region Fields
    
    public static readonly ResourceGameObject<ArenaResource> Prefab =
        new ResourceGameObject<ArenaResource>("Game/Game/Arena");

    [SerializeField] SceneArena sceneArena;
    [SerializeField] ScenePlayer scenePlayer;

    #endregion



    #region Properties

    public SceneArena SceneArena => sceneArena;
    
    
    public ScenePlayer ScenePlayer => scenePlayer;

    #endregion
}
