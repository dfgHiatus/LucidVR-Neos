using BaseX;
using FrooxEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeosEyeFaceAPI
{
    public class HandDevice : IInputDriver
    {
        private Hand _LeftHand;
        private Hand _RightHand;
        public int UpdateOrder => 100;

        public void CollectDeviceInfos(DataTreeList list)
        {
            var leftHandDataTreeDictionary = new DataTreeDictionary();
            leftHandDataTreeDictionary.Add("Name", "LucidVR Hand Tracking");
            leftHandDataTreeDictionary.Add("Type", "Hand Tracking");
            leftHandDataTreeDictionary.Add("Model", "LucidVR");
            list.Add(leftHandDataTreeDictionary);

            var rightHandDataTreeDictionary = new DataTreeDictionary();
            rightHandDataTreeDictionary.Add("Name", "LucidVR Hand Tracking");
            rightHandDataTreeDictionary.Add("Type", "Hand Tracking");
            rightHandDataTreeDictionary.Add("Model", "LucidVR");
            list.Add(rightHandDataTreeDictionary);
        }

        public void RegisterInputs(InputInterface inputInterface)
        {
            _LeftHand = new Hand(inputInterface, Chirality.Left, 0);
            _RightHand = new Hand(inputInterface, Chirality.Right, 0);
        }

        public void UpdateInputs(float deltaTime)
        {
            _LeftHand.Wrist.
        }
    }
}
