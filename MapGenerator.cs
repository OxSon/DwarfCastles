using System;
using System.Drawing;

namespace DwarfCastles
{
    public class MapGenerator
    {
        public static Map GenerateMap(Point size)
        {
            var m = new Map(size);
            var r = new Random();

            for (var i = r.Next(20) + 5; i >= 0; i--)
            {
                var pos  = new Point(r.Next(size.X), r.Next(size.Y));
                var treeClone = ResourceMasterList.GetDefaultClone("tree");
                treeClone.Pos = pos;
                m.AddEntity(treeClone);
            }
            for (var i = r.Next(20) + 5; i >= 0; i--)
            {
                var pos  = new Point(r.Next(size.X), r.Next(size.Y));
                var ironVeinClone = ResourceMasterList.GetDefaultClone("ironvein");
                ironVeinClone.Pos = pos;
                m.AddEntity(ironVeinClone);
            }
            for (var i = r.Next(20) + 5; i >= 0; i--)
            {
                var pos  = new Point(r.Next(size.X), r.Next(size.Y));
                var bigstoneClone = ResourceMasterList.GetDefaultClone("bigstone");
                bigstoneClone.Pos = pos;
                m.AddEntity(bigstoneClone);
            }
            for (var i = r.Next(20) + 5; i >= 0; i--)
            {
                var pos  = new Point(r.Next(size.X), r.Next(size.Y));
                var coalVeinClone = ResourceMasterList.GetDefaultClone("coalvein");
                coalVeinClone.Pos = pos;
                m.AddEntity(coalVeinClone);
            }

            var remaining = 6;
            var attempts = 0;
            var mapCenter = new Point(m.Size.X / 2, m.Size.Y / 2);
            while (remaining > 0)
            {
                for (var i = -attempts; i < attempts + 1; i++)
                {
                    for (var j = -attempts; j < attempts + 1 && remaining != 0; j++)
                    {
                        if (m.Impassables[mapCenter.X + i, mapCenter.Y + j]) continue;
                        var pos = new Point(mapCenter.X + i, mapCenter.Y + j);
                        var dwarfClone = ResourceMasterList.GetDefaultClone("dwarf");
                        if (dwarfClone is Actor)
                        {
                            ((Actor) dwarfClone).Map = m;
                        }
                        dwarfClone.Pos = pos;
                            
                        m.AddEntity(dwarfClone);
                        remaining--;
                        if (remaining == 0)
                        {
                            break;
                        }
                    }
                }

                attempts++;
            }
            
            return m;
        }
    }
}