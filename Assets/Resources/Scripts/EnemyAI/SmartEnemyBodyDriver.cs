using System;
using System.Collections;
using System.Collections.Generic;
using Resources.Scripts;
using RootMotion.Demos;
using UnityEngine;

public class SmartEnemyBodyDriver : MonoBehaviour
{
    public Transform Body;
    [HideInInspector]
    public SmartEnemyDriver SmartEnemyDriver;
    
    // private PriorityQueue<MovementTask> MovementTasks;
    protected MovementTask currentMovementTask;
    public bool hasMovementTask { get; private set; }

    public enum BotMovementType
    {
        Regular, Strafe, Rush
    }

    public class MovementTask
    {
        public MovementTask(MoveTarget target, float speed = 0.5f, float stopDistance = 0f,
            float brakeDistance = 1f, BotMovementType botMovementType = BotMovementType.Regular)
        {
            this.target = target;
            this.speed = speed;
            this.botMovementType = botMovementType;
            this.stopDistance = stopDistance;
            this.brakeDistance = brakeDistance;
        }

        public MoveTarget target;
        public float speed;
        public BotMovementType botMovementType;
        public float stopDistance;
        public float brakeDistance;
    }
    
    // public class PriorityQueue<T>
    // {
    //     internal class QueueElement<TJ>
    //     {
    //         public TJ item;
    //         public int priority;
    //
    //         public QueueElement(TJ item, int priority)
    //         {
    //             this.item = item;
    //             this.priority = priority;
    //         }
    //     }
    //     
    //     private LinkedList<QueueElement<T>> queue = new LinkedList<QueueElement<T>>();
    //     
    //     public void Add(T item, int priority)
    //     {
    //         var newElem = new QueueElement<T>(item, priority);
    //
    //         var currentNode = queue.First;
    //         while (currentNode != null)
    //         {
    //             if (currentNode.Value.priority > priority)
    //             {
    //                 queue.AddBefore(currentNode, newElem);
    //                 return;
    //             }
    //             currentNode = currentNode.Next;
    //         }
    //         queue.AddLast(newElem);
    //     }
    //
    //     public T Peek()
    //     {
    //         return queue.Last.Value.item;
    //     }
    //
    //     public T Pop()
    //     {
    //         var ret = queue.Last.Value.item;
    //         queue.RemoveLast();
    //         return ret;
    //     }
    //
    //     public void Clear()
    //     {
    //         queue.Clear();
    //     }
    //
    //     public bool isEmpty => queue.Count == 0;
    // }

    public void AddTask(MovementTask task)//, int priority=0)
    {
        // MovementTasks.Add(task, priority);
        print($"Add task {task.target.position}");
        currentMovementTask = task;
        hasMovementTask = true;
    }
    public void CompleteTask()
    {
        character.moveMode = CharacterThirdPerson.MoveMode.Directional;
        navController.state.move = Vector3.zero;
        currentMovementTask = null;
        hasMovementTask = false;
        print("Task completed");
    }

    // private void Start()
    // {
    //     // MovementTasks = new PriorityQueue<MovementTask>();
    // }

    protected MoveTarget rotationTarget;
    protected bool rotationLocked;
    
    public void LockRotationToTarget(MoveTarget target)
    {
        rotationLocked = true;
        rotationTarget = target;
    }

    public void ReleaseRotation()
    {
        rotationLocked = false;
        rotationTarget = null;
    }

    protected void UpdateRotation()
    {
        navController.state.lookPos = rotationTarget.position;
    }

    public CharacterPuppet character;
    public CustomControllerLol navController;

    void FixedUpdate()
    {
        if (!hasMovementTask)
            return;

        if (currentMovementTask.target.isEmpty)
        {
            CompleteTask();
            return;
        }
            

        navController.navigator.Update(currentMovementTask.target.position);

        if (currentMovementTask.botMovementType == BotMovementType.Strafe)
            character.moveMode = CharacterThirdPerson.MoveMode.Strafe;
        else
            character.moveMode = CharacterThirdPerson.MoveMode.Directional;
        
        if (rotationLocked)
            UpdateRotation();

        var speed = currentMovementTask.speed;

        if (navController.navigator.lastCorner)
        {
            if (navController.navigator.Distance < currentMovementTask.stopDistance + 0.2f)
            {
                CompleteTask();
                return;
            }

            var taskBrakingDistance = currentMovementTask.brakeDistance + currentMovementTask.stopDistance;
            var deltaDist = navController.navigator.Distance - taskBrakingDistance;
            // Торможение
            if (deltaDist <= 0)
                speed = Mathf.Lerp(speed, 0f, -deltaDist / taskBrakingDistance);
            if (speed < 0.001f)
                CompleteTask();
        }
        
        
        navController.state.move = navController.navigator.normalizedDeltaPosition * speed;
    }

}
