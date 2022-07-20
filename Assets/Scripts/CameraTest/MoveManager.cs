using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    protected List<Move> _moves = new List<Move>();

    public MoveManager()
    {
        testMove();
    }
    
    public void Update()
    {
        List<Move> toRemove = new List<Move>();
        
        // Process all active moves
        foreach (Move move in _moves)
        {
            if (move.isFinished()) toRemove.Add(move);
            else move.Step();
        }
        
        toRemove.ForEach(move => _moves.Remove(move));
    }

    public void attemptMove(Move move)
    {
        _moves.Add(move);
    }

    public void testMove()
    {
        Move jab = new Jab();
        attemptMove(jab);
    }
}

abstract public class FrameEvent
{
    abstract public void process();
}

abstract public class Move
{
    // The current frame (realtive to the move starting)
    protected int _currentFrame = -1;
    
    // The frame data object describing how the move works
    protected FrameData _frameData = new FrameData();

    public void Step()
    {
        if (isFinished()) return;
        _currentFrame++;
        _frameData.getFrameEvents(_currentFrame).ForEach(fameEvent => fameEvent.process());
    }

    public bool isFinished()
    {
        return _frameData.isFinished();
    }
}

public class Jab : Move
{
    public Jab()
    {
        // CREATE HITBOX CLASS
        object[] createHitbox1Params = {"hitbox1", new List<(int, int)> {(1,2), (-1, -1), (1, -1), (1, 1)}};
        
        _frameData.addEvent("CreateHitbox", 5, createHitbox1Params);
        _frameData.addEvent("EndMove", 20);
    }
}

public class FrameData
{
    protected List<(int, FrameEvent)> _frameEvents = new List<(int, FrameEvent)>();

    public void addEvent(params object[] frameEventParams)
    {
        string className = (string) frameEventParams[0];
        int onFrame = (int) frameEventParams[1];
        object[] classParams = frameEventParams.Length > 2 ? (object[]) frameEventParams[2] : null;
        
        System.Type frameEventClass = System.Type.GetType(className);
        FrameEvent frameEvent = (FrameEvent) System.Activator.CreateInstance(frameEventClass, classParams);

        _frameEvents.Add((onFrame, frameEvent));
    }
    
    public bool isFinished()
    {
        return _frameEvents.Count == 0;
    }

    public List<FrameEvent> getFrameEvents(int frame)
    {
        List<FrameEvent> frameEvents = new List<FrameEvent>();
        List<(int, FrameEvent)> toRemove = new List<(int, FrameEvent)>(); 
        
        foreach ((int onFrame, FrameEvent frameEvent) in _frameEvents)
        {
            if (onFrame <= frame)
            {
                frameEvents.Add(frameEvent);
                toRemove.Add((onFrame, frameEvent));
            }
        }
        
        // You cannot remove while the enumerator is running so we will remove after
        toRemove.ForEach(frameEvent => _frameEvents.Remove(frameEvent));

        return frameEvents;
    }
}

public class CreateHitbox : FrameEvent
{
    protected List<(int, int)> _hitbox;

    protected string _name;

    public CreateHitbox(string name, List<(int, int)> hitboxCoords)
    {
        _name = name;
        _hitbox = hitboxCoords;
    }
    
    public override void process()
    {
        Debug.Log("CreateHitbox");
    }
}

public class EndMove : FrameEvent
{
    public override void process()
    {
        Debug.Log("EndMove");
    }
}