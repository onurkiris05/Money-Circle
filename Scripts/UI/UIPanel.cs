using UnityEngine;
using UnityEngine.UI;
using VP.Nest.UI;

public abstract class UIPanel : MonoBehaviour
{
    private Canvas canvas;
    protected UIManager UIManager;

    #region EncapsulationMethods

    public bool IsCanvasEnable => canvas.enabled;

    #endregion

    #region InitializeMethods

    internal virtual void Initialize()
    {
        UIManager = UIManager.Instance;
        canvas = GetComponent<Canvas>();
    }

    #endregion

    #region UIPanelMethods

    public void SetCanvasEnable(bool value)
    {
        canvas.enabled = value;
    }

    #endregion
}