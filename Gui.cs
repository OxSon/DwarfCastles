using System;

namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// handles drawing in console
    /// </summary>
    public class Gui
    {
        public int CameraOffset { get; }

        public Gui(int cameraOffset)
        {
            CameraOffset = cameraOffset;
        }

        public void Draw(Map map)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}