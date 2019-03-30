using NekoClient;
using NekoClient.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using OvrInputOrig = FAIDDAKKIGP;
using OvrInputControls = FAIDDAKKIGP.JPKLNHMFHOC;
using OvrInputControllers = FAIDDAKKIGP.FDKADCLJFKK;

namespace PlayspaceMover.Oculus
{
    public class OculusPlayspaceMover : PluginBase
    {
        public OculusPlayspaceMover()
        {
            Tick += OculusPlayspaceMover_Tick;
        }

#if TRACKED_OFFSET
        private Vector3 m_lastTrackerPosition = new Vector3(-1337.0f, 0, 0);

        private Vector3 m_positionOffset = new Vector3(0, 0, 0);
        private Vector3 m_lastPositionOffset = new Vector3(0, 0, 0);
#endif

        // to find OVRInput class: find `OVRGamepad` string
        private static class OVRInput
        {
            // OVRInput.Get() -- find by searching `.None,` (returns: bool)
            public static bool GetTriggerButton(bool right = true) => OvrInputOrig.FOBLBPHKFHA(right ? OvrInputControls.One : OvrInputControls.Three); // if right get A otherwise get X

            // OVRInput.GetLocalControllerPosition -- find `().position` (returns: Vector3)
            public static Vector3 GetLocalControllerPosition(bool right = true) => OvrInputOrig.GPOGMGNBLEG(right ? OvrInputControllers.RTouch : OvrInputControllers.LTouch);
        }

        private void OculusPlayspaceMover_Tick()
        {
            bool leftTrigger = OVRInput.GetTriggerButton(false);
            bool rightTrigger = OVRInput.GetTriggerButton();

            if (leftTrigger || rightTrigger)
            {
                List<VRCVrCameraOculus> ctrls = ((VRCVrCameraOculus[])UnityEngine.Object.FindObjectsOfType(typeof(VRCVrCameraOculus))).ToList();

#if TRACKED_OFFSET

                // TODO: track for both left and right
                Vector3 currentTrackerPosition = OVRInput.GetLocalControllerPosition();

                // set up initial position I guess?
                if (m_lastTrackerPosition.x == -1337.0f)
                {
                    m_lastTrackerPosition = currentTrackerPosition;
                }

                if (currentTrackerPosition != m_lastTrackerPosition)
                {
                    Vector3 currentPositionOffset = currentTrackerPosition - m_lastTrackerPosition;

                    m_positionOffset += currentPositionOffset;

                    m_lastTrackerPosition = currentTrackerPosition;
                }
#endif

                ctrls.ForEach(ctrl =>
                {
#if TRACKED_OFFSET
                    if (m_positionOffset != m_lastPositionOffset)
                    {
                        ctrl.cameraLiftTransform.localPosition += m_positionOffset;

                        m_lastPositionOffset = m_positionOffset;
                    }
#else
                    if (leftTrigger)
                    {
                        ctrl.cameraLiftTransform.localPosition -= new Vector3(0, Time.deltaTime * 0.2f, 0);
                    }

                    if (rightTrigger)
                    {
                        ctrl.cameraLiftTransform.localPosition += new Vector3(0, Time.deltaTime * 0.2f, 0);
                    }
#endif
                });
            }
        }
    }
}
