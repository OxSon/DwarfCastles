using System;
using System.Drawing;

namespace DwarfCastles
{
    public class MapGenerator
    {
        public static Map GenerateMap(Point size)
        {
            Map m = new Map(size);
            Random r = new Random();

            for (int i = r.Next(20) + 5; i >= 0; i--)
            {
                var pos  = new Point(r.Next(size.X), r.Next(size.Y));
                var treeClone = ResourceMasterList.GetDefaultClone("tree");
                treeClone.Pos = pos;
                m.AddEntity(treeClone);
            }
            for (int i = r.Next(20) + 5; i >= 0; i--)
            {
                var pos  = new Point(r.Next(size.X), r.Next(size.Y));
                var ironVeinClone = ResourceMasterList.GetDefaultClone("ironvein");
                ironVeinClone.Pos = pos;
                m.AddEntity(ironVeinClone);
            }
            for (int i = r.Next(20) + 5; i >= 0; i--)
            {
                var pos  = new Point(r.Next(size.X), r.Next(size.Y));
                var bigstoneClone = ResourceMasterList.GetDefaultClone("bigstone");
                bigstoneClone.Pos = pos;
                m.AddEntity(bigstoneClone);
            }
            for (int i = r.Next(20) + 5; i >= 0; i--)
            {
                var pos  = new Point(r.Next(size.X), r.Next(size.Y));
                var coalVeinClone = ResourceMasterList.GetDefaultClone("coalvein");
                coalVeinClone.Pos = pos;
                m.AddEntity(coalVeinClone);
            }

            int remaining = 6;
            int attempts = 0;
            Point mapCenter = new Point(m.Size.X / 2, m.Size.Y / 2);
            while (remaining > 0)
            {
                for (int i = -attempts; i < attempts + 1; i++)
                {
                    for (int j = -attempts; j < attempts + 1 && remaining != 0; j++)
                    {
                        if (!m.Impassables[mapCenter.X + i, mapCenter.Y + j])
                        {
                            var pos = new Point(mapCenter.X + i, mapCenter.Y + j);
                            Entity dwarfClone = ResourceMasterList.GetDefaultClone("dwarf");
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
                }

                attempts++;
            }
            
            return m;
        }
    }
}