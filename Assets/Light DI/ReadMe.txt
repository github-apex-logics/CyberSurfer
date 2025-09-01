Light DI is copyright (c) 2022-2025 Mark "Heroicsolo" Tsemma

// WHAT IS THIS? ////////////////////////////////////////////
Light DI – light-weighted, simple and easy-to-integrate Dependency Injection system for small Unity projects. It can be used in Hypercasual/Hybridcasual/Midcore projects and provides enough functionality for such projects, making them more flexible and enhancing their architecture.

// GET STARTED //////////////////////////////////////////////
You need only 4 steps to use Light DI in your project:
1. Drag InjectionManager prefab onto your scene.
2. Inherit your systems (managers) from SystemBase class (you can find an example in IInputManager and CameraController scripts).
3. Inject these systems in your classes via [Inject] attribute (you can find an example in PlayerController script).
4. Call InjectionManager.InjectTo(this) inside the Start or Awake method in these classes (you can find an example in PlayerController script).

// INJECTING NON-SYSTEM CLASSES /////////////////////////////
You can also use it for injecting MonoBehaviours or any class instances. Here is an example:

//Player.cs
public class Player : MonoBehaviour, IInjectable
{
    [Inject] private IEventsManager eventsManager;

    public void PostInject()
    {
        eventsManager.TriggerEvent("Player initialized");
    }
}

//Enemy.cs
public class Enemy : MonoBehaviour, IInjectable
{
    [Inject] private IEventsManager eventsManager;
    [Inject] private Player player;

    public void PostInject()
    {
        eventsManager.TriggerEvent("Enemy initialized");
        transform.LookAt(player.transform);
    }
}

//GameManager.cs
public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform enemySpawnPoint;

    private void Start()
    {
        Player player = InjectionManager.CreateGameObject<Player>();
        player.transform.position = spawnPoint.position;
        Enemy enemy = InjectionManager.CreateGameObject<Enemy>();
        enemy.transform.position = enemySpawnPoint.position;
    }
}

// FAQ /////////////////////////////////////////////////////
1. Does it support interfaces?
	Yes, it supports interfaces and you can inject them too.

2. How it works with several scenes?
	It works fine, you just need to add InjectionManager prefab and your global systems (which may be needed in all other scenes) onto your first scene (Loading scene, Main menu scene or whatever else). In this case, all registered systems will be moved into DontDestroyOnLoad scene and will be alive even after switching between scenes.

This DI system is a part of Heroic Engine package, so you can find more detailed description here in Heroic Engine docs:
https://heroicsolo.gitbook.io/heroic-engine/basics/injection-manager

You can also ask questions via my email: mark.tsemma@gmail.com
Or join Heroicsolo Assets Discord server: https://discord.gg/gTbzY4vhvD