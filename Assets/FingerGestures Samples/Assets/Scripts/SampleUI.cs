using UnityEngine;
using System.Collections;

// iphone3 480 x 320
// iphone4 960 x 640
public class SampleUI : MonoBehaviour
{
    public GUISkin skin;
    
    public Color titleColor = Color.white;
    GUIStyle statusStyle;
    
    Rect titleRect = new Rect( 100, 2, 400, 46 );
    Rect statusTextRect = new Rect( 30, 336, 540, 60 );

    string statusText = "";//"status text goes here";
    public string StatusText
    {
        get { return statusText; }
        set { statusText = value; }
    }

    public bool showStatusText = true;

    public string helpText = "";
    public bool showHelpButton = true;
    public bool showHelp = false;


    void Awake()
    {

        statusStyle = new GUIStyle( skin.label );
        statusStyle.alignment = TextAnchor.LowerCenter;
       
    }

    #region Virtual Screen for automatic UI resolution scaling
    
    public static readonly float VirtualScreenWidth = 100;
    public static readonly float VirtualScreenHeight = 50;

    public static void ApplyVirtualScreen()
    {
        // resolution scaling matrix
        GUI.matrix = Matrix4x4.Scale( new Vector3( Screen.width / VirtualScreenWidth, Screen.height / VirtualScreenHeight, 1 ) );
    }

    #endregion

    protected virtual void OnGUI()
    {
        if( skin != null )
            GUI.skin = skin;

        ApplyVirtualScreen();
        

        if( showStatusText )
            GUI.Label( statusTextRect, statusText, statusStyle );

        if( showHelp )
        {
            
            GUILayout.BeginVertical();
            {
                GUILayout.Space( 25 );

                GUILayout.FlexibleSpace();

                if( GUILayout.Button( "Close", GUILayout.Height( 40 ) ) )
                    showHelp = false;
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

    }
}
