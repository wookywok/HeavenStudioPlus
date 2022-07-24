using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.InputSystem;

using static JSL;

namespace HeavenStudio
{
    public class PlayerInput
    {
        //Clockwise
        public const int UP = 0;
        public const int RIGHT = 1;
        public const int DOWN = 2;
        public const int LEFT = 3;

        ///////////////////////////////
        ////TEMPORARY JSL FUNCTIONS////
        ///////////////////////////////

        static int jslDevicesFound = 0;
        static int jslDevicesConnected = 0;
        static int[] jslDeviceHandles;

        static List<InputController> inputDevices;

        public static int InitInputControllers()
        {
            inputDevices = new List<InputController>();
            //Keyboard setup
            InputKeyboard keyboard = new InputKeyboard();
            keyboard.SetPlayer(1);
            keyboard.InitializeController();
            inputDevices.Add(keyboard);
            //end Keyboard setup

            //JoyShock setup
            Debug.Log("Flushing possible JoyShocks...");
            DisconnectJoyshocks();

            jslDevicesFound = JslConnectDevices();
            if (jslDevicesFound > 0)
            {
                jslDeviceHandles = new int[jslDevicesFound];
                jslDevicesConnected = JslGetConnectedDeviceHandles(jslDeviceHandles, jslDevicesFound);
                if (jslDevicesConnected < jslDevicesFound)
                {
                    Debug.Log("Found " + jslDevicesFound + " JoyShocks, but only " + jslDevicesConnected + " are connected.");
                }
                else
                {
                    Debug.Log("Found " + jslDevicesFound + " JoyShocks.");
                    Debug.Log("Connected " + jslDevicesConnected + " JoyShocks.");
                }

                foreach (int i in jslDeviceHandles)
                {
                    Debug.Log("Setting up JoyShock: ( Handle " + i + ", type " + JslGetControllerType(i) + " )");
                    InputJoyshock joyshock = new InputJoyshock(i);
                    joyshock.InitializeController();
                    joyshock.SetPlayer(inputDevices.Count + 1);
                }
            }
            else
            {
                Debug.Log("No JoyShocks found.");
            }
            //end JoyShock setup

            //TODO: XInput setup (boo)
            //end XInput setup

            return inputDevices.Count;
        }

        public static int GetNumControllersConnected()
        {
            return inputDevices.Count;
        }

        public static List<InputController> GetInputControllers()
        {
            return inputDevices;
        }

        public static InputController GetInputController(int player)
        {
            //select input controller that has player field set to player
            //this will return the first controller that has that player number in the case of controller pairs (eg. Joy-Cons)
            //so such controllers should have a reference to the other controller in the pair
            foreach (InputController i in inputDevices)
            {
                if (i.GetPlayer() == player)
                {
                    return i;
                }
            }
            return null;
        }

        public static void UpdateInputControllers()
        {
            foreach (InputController i in inputDevices)
            {
                i.UpdateState();
            }
        }

        public static void DisconnectJoyshocks()
        {
            if (jslDeviceHandles != null && jslDevicesConnected > 0 && jslDeviceHandles.Length > 0)
            {
                foreach (InputController i in inputDevices)
                {
                    if (typeof(InputJoyshock) == i.GetType())
                    {
                        InputJoyshock joy = (InputJoyshock)i;
                        joy.DisconnectJoyshock();
                    }
                }
            }
            JslDisconnectAndDisposeAll();
            jslDevicesFound = 0;
            jslDevicesConnected = 0;
        }

        // The autoplay isn't activated AND
        // The song is actually playing AND
        // The GameManager allows you to Input
        public static bool playerHasControl()
        {
            return !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }

        /*--------------------*/
        /* MAIN INPUT METHODS */
        /*--------------------*/

        // BUTTONS
        //TODO: refactor for controller and custom binds, currently uses temporary button checks

        public static bool Pressed(bool includeDPad = false)
        {
            bool keyDown = GetInputController(1).GetButtonDown((int) InputController.ButtonsPad.PadE) || (includeDPad && GetAnyDirectionDown());
            return keyDown && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput ;
        }

        public static bool PressedUp(bool includeDPad = false)
        {
            bool keyUp = GetInputController(1).GetButtonUp((int) InputController.ButtonsPad.PadE) || (includeDPad && GetAnyDirectionUp());
            return keyUp && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }

        public static bool Pressing(bool includeDPad = false)
        {
            bool pressing = GetInputController(1).GetButton((int) InputController.ButtonsPad.PadE) || (includeDPad && GetAnyDirection());
            return pressing && !GameManager.instance.autoplay && Conductor.instance.isPlaying && GameManager.instance.canInput;
        }


        public static bool AltPressed()
        {
            bool down = GetInputController(1).GetButtonDown((int) InputController.ButtonsPad.PadS);
            return down && playerHasControl();
        }

        public static bool AltPressedUp()
        {
            bool up = GetInputController(1).GetButtonUp((int) InputController.ButtonsPad.PadS);
            return up && playerHasControl();
        }

        public static bool AltPressing()
        {
            bool pressing = GetInputController(1).GetButton((int) InputController.ButtonsPad.PadS);
            return pressing && playerHasControl();
        }

        //Directions

        public static bool GetAnyDirectionDown()
        {
            return (GetInputController(1).GetButtonDown((int) InputController.ButtonsPad.PadUp)
                    || GetInputController(1).GetButtonDown((int) InputController.ButtonsPad.PadDown)
                    || GetInputController(1).GetButtonDown((int) InputController.ButtonsPad.PadLeft)
                    || GetInputController(1).GetButtonDown((int) InputController.ButtonsPad.PadRight)
                    ) && playerHasControl();

        }

        public static bool GetAnyDirectionUp()
        {
            return (GetInputController(1).GetButtonUp((int) InputController.ButtonsPad.PadUp)
                    || GetInputController(1).GetButtonUp((int) InputController.ButtonsPad.PadDown)
                    || GetInputController(1).GetButtonUp((int) InputController.ButtonsPad.PadLeft)
                    || GetInputController(1).GetButtonUp((int) InputController.ButtonsPad.PadRight)
                    ) && playerHasControl();

        }

        public static bool GetAnyDirection()
        {
            return (GetInputController(1).GetButton((int) InputController.ButtonsPad.PadUp)
                    || GetInputController(1).GetButton((int) InputController.ButtonsPad.PadDown)
                    || GetInputController(1).GetButton((int) InputController.ButtonsPad.PadLeft)
                    || GetInputController(1).GetButton((int) InputController.ButtonsPad.PadRight)
                    ) && playerHasControl();

        }

        public static bool GetSpecificDirection(int direction)
        {
            return GetInputController(1).GetHatDirection((InputController.InputDirection) direction) && playerHasControl();
        }

        public static bool GetSpecificDirectionDown(int direction)
        {
            return GetInputController(1).GetHatDirectionDown((InputController.InputDirection) direction) && playerHasControl();
        }

        public static bool GetSpecificDirectionUp(int direction)
        {
            return GetInputController(1).GetHatDirectionUp((InputController.InputDirection) direction) && playerHasControl();
        }
    }
}