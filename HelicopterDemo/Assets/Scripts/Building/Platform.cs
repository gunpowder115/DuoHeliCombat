using UnityEngine;

public class Platform : MonoBehaviour
{
    public CommandCenter CommandCenter { get; private set; }

    public void SetCommandCenter(CommandCenter baseCenter) => CommandCenter = baseCenter;
}
