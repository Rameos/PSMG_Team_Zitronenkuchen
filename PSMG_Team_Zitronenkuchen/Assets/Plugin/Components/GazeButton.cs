using UnityEngine;
using System.Collections;
using iViewX;


namespace iViewX
{
    public delegate void buttonCallbackListener();

    public class GazeButton : GUIElement{

        public GUIStyle myStyle;
        public bool isVisible = true;

        private string content = "";
        private Texture2D contentImage; 
        private Rect position;
        private Rect colliderPosition; 
        private GUIStyle actualStyleOfTheElement = new GUIStyle();

        private buttonCallbackListener actionToDo;
    

	    public GazeButton(Rect position,string content,GUIStyle myStyle,buttonCallbackListener callback)
        {
            this.position = position;
            this.content = content;

            colliderPosition = new Rect(position.x, Camera.main.pixelHeight-position.y-position.height,position.width,position.height);

            if(myStyle!= null)
            {
                initStyle(myStyle);

            }
            actionToDo = callback;
        }

        public GazeButton(Rect position, string content)
        {
            this.position = position;
            this.content = content;
        }

        public GazeButton(Rect position, Texture2D content, GUIStyle myStyle, buttonCallbackListener callback)
        {
            this.position = position;
            this.contentImage = content;

            colliderPosition = new Rect(position.x, Camera.main.pixelHeight - position.y - position.height, position.width, position.height);

            if (myStyle != null)
            {
                initStyle(myStyle);

            }
            actionToDo = callback;
        }

        public void OnGUI()
        {
            if(isVisible)
            {
                if(content != "")
                    GUI.Label(position, content,actualStyleOfTheElement);
                else
                {
                    GUI.Box(position, contentImage, actualStyleOfTheElement);
                }
            }

        }


        public bool Update()
        {
            //Use mouseInput, if no EyeTracker runs
            Vector2 positionGaze = Input.mousePosition;

            //Load the GazeData From the gazemodel
            if(gazeModel.isEyeTrackerRunning)
            {
                positionGaze = (gazeModel.posGazeLeft + gazeModel.posGazeRight) * 0.5f;
                positionGaze.y = Screen.height - positionGaze.y;

            }

            //Check Colision
            if (colliderPosition.Contains(positionGaze))
            {
                setFocused();

                if (Input.GetButtonUp("SelectGUI"))
                    actionToDo();
            }

            else
            {
                setActive();
            }
            return false; 
        }

        private void setActive()
        {
            actualStyleOfTheElement.normal = myStyle.active;
            //actualStyleOfTheElement.normal.background= myStyle.active.background;
        }

        private void setFocused()
        {


            actualStyleOfTheElement.normal = myStyle.focused;
            //actualStyleOfTheElement.normal.background= myStyle.focused.background;

       } 

        private void initStyle(GUIStyle myStyle)
        {
            actualStyleOfTheElement.normal = myStyle.normal;
            actualStyleOfTheElement.fontSize = myStyle.fontSize;
            actualStyleOfTheElement.font = myStyle.font;
            actualStyleOfTheElement.imagePosition = myStyle.imagePosition;
            actualStyleOfTheElement.alignment = myStyle.alignment;
            actualStyleOfTheElement.richText = myStyle.richText;
            actualStyleOfTheElement.fontStyle = myStyle.fontStyle;
            actualStyleOfTheElement.overflow = myStyle.overflow;
            actualStyleOfTheElement.richText = myStyle.richText;
            actualStyleOfTheElement.clipping = myStyle.clipping;
            actualStyleOfTheElement.contentOffset = myStyle.contentOffset;
            actualStyleOfTheElement.fixedWidth = myStyle.fixedWidth;
            actualStyleOfTheElement.fixedHeight = myStyle.fixedHeight;
            actualStyleOfTheElement.stretchHeight = myStyle.stretchHeight;
            actualStyleOfTheElement.stretchWidth = myStyle.stretchWidth;
            this.myStyle = myStyle;
        }
    }
}