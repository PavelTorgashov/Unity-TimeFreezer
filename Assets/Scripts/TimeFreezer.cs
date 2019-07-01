using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slows down moving, rotating and interaction inside trigger
/// </summary>
public class TimeFreezer : MonoBehaviour
{
    public float TimeFactor = 0.5f;
    const float EXIT_DUMB = 0.98f;

    Dictionary<Rigidbody, BodyInfo> bodyInfos = new Dictionary<Rigidbody, BodyInfo>();

    void FixedUpdate()
    {
        foreach (var pair in bodyInfos)
        {
            var rb = pair.Key;
            var info = pair.Value;

            if (info.PrevVelocity != null)
            {
                //calc acceleration
                var acc = rb.velocity - info.PrevVelocity.Value;

                //calc angular acceleration
                var angularAcc = rb.angularVelocity - info.PrevAngularVelocity.Value;

                //assign new velocity
                info.PrevVelocity = rb.velocity = info.UnscaledVelocity * TimeFactor;
                info.PrevAngularVelocity = rb.angularVelocity = info.UnscaledAngularVelocity * TimeFactor;

                //assign acceleration
                info.UnscaledVelocity += acc;
                info.UnscaledAngularVelocity += angularAcc;
            }
            else
            {
                //first step
                info.PrevVelocity = rb.velocity = info.UnscaledVelocity * TimeFactor;
                info.PrevAngularVelocity = rb.angularVelocity = info.UnscaledAngularVelocity * TimeFactor;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var info = new BodyInfo();
        info.PrevVelocity = null;
        info.UnscaledVelocity = other.attachedRigidbody.velocity;
        info.UnscaledAngularVelocity = other.attachedRigidbody.angularVelocity;
        bodyInfos.Add(other.attachedRigidbody, info);
    }

    private void OnTriggerExit(Collider other)
    {
        if (bodyInfos.ContainsKey(other.attachedRigidbody))
        {
            var info = bodyInfos[other.attachedRigidbody];
            other.attachedRigidbody.angularVelocity = info.UnscaledAngularVelocity * EXIT_DUMB;
            other.attachedRigidbody.velocity = info.UnscaledVelocity * EXIT_DUMB;
            bodyInfos.Remove(other.attachedRigidbody);
        }
    }
}

class BodyInfo
{
    public Vector3 UnscaledVelocity;
    public Vector3 UnscaledAngularVelocity;
    public Vector3? PrevVelocity;
    public Vector3? PrevAngularVelocity;
}
