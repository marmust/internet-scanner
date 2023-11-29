using UnityEngine;
using System.Collections.Generic;

public class TutorialProgress : MonoBehaviour
{
    public ParticleSystem particles;

    public List<GameObject> ToDisableOnContact;
    public List<GameObject> ToEnableOnContact;

    // when something comes  into contact
    public void OnTriggerEnter(Collider other)
    {
        // check if the camera is the foreighn object
        if (other.CompareTag("MainCamera"))
        {
            // do particles
            particles.Play();

            // first enable then disable
            // (because might disable self and stop running idk)
            foreach (GameObject TargetObject in ToEnableOnContact)
            {
                TargetObject.SetActive(true);
            }

            foreach (GameObject TargetObject in ToDisableOnContact)
            {
                TargetObject.SetActive(false);
            }
        }
    }
}