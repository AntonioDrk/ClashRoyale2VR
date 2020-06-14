using Zinnia.Action;

namespace MyScripts
{
    public class OVRInputButtonUpAction : BooleanAction
    {
        public OVRInput.Controller controller = OVRInput.Controller.Active;
        public OVRInput.Button button;
        void Update()
        {
            Receive(OVRInput.GetUp(button, controller));
        }
    }
}

