using UnityEngine;
using PathCreation;


public class PathFollower : MonoBehaviour
{
    // fields
    private float _speed;
    private float _distanceTravelled;

    // cache
    public PathCreator PathCreator;
    public EndOfPathInstruction EndOfPathInstruction;

    public float DistanceTravelled => _distanceTravelled;

    void Start()
    {
        PathCreator = GameObject.FindObjectOfType<PathCreator>();
        SetPathCreator();
    }

    void Update()
    {
        FollowPath();
    }

    private void SetPathCreator()
    {


        if (PathCreator != null)
        {
            PathCreator.pathUpdated += OnPathChanged;
        }

    }

    private void FollowPath()
    {
        if (PathCreator != null)
        {
            _speed = GetComponent<Enemy>().Speed;
            _distanceTravelled += _speed * Time.deltaTime;
            transform.position = PathCreator.path.GetPointAtDistance(_distanceTravelled, EndOfPathInstruction);
        }
    }

    private void OnPathChanged()
    {
        _distanceTravelled = PathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

}// class

