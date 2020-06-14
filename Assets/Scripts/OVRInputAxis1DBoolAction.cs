using UnityEngine;
using Zinnia.Action;

namespace MyScripts
{
    public class OVRInputAxis1DBoolAction : BooleanAction
    {
        public OVRInput.Controller controller = OVRInput.Controller.Active;
        public OVRInput.Axis1D axis1D;
        [Range(0f,1f)]
        public float threshold = 0.1f;
        void Update()
        {
            if (OVRInput.Get(axis1D, controller) > 0.1f)
            {
                Receive(true);
            }
            else
            {
                Receive(false);
            }
        }
    }

}
