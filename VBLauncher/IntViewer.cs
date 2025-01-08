using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AltUI.Forms;

namespace VBLauncher;

public class IntViewer : DarkForm
{
// Loads and draws image fragments from files based on object data. It iterates through objects, filters fragments by dimensions, and renders textures using GDI+.
    public void LoadData(INT intFile)
    {
        var g = CreateGraphics();
        foreach (var obj in intFile.objects)
        {
            var fragsToDraw = obj.fragments.Where(f => f.width != 16 && f.height != 16);
            foreach (var frag in fragsToDraw)
            //var frag = obj.fragments[4];
            {
                var tex = frag.texture;
                if (string.IsNullOrEmpty(tex)) continue;
                // find texture file in root directory
                // TODO: create a resource subsystem for various previews (interface, editor previews). should support reading from Overrides directory.
                var texFile = Directory.GetFiles("./", tex, SearchOption.AllDirectories).FirstOrDefault() ?? Directory.GetFiles("./", tex.Replace(".tga", ".bmp"), SearchOption.AllDirectories).FirstOrDefault();
                if (texFile is null) continue;
                var image = texFile.EndsWith(".tga") ? GrpBrowser.TargaToBitmap(File.ReadAllBytes(texFile)) : new Bitmap(texFile);
                g.DrawImage(image, frag.rect.x1, frag.rect.y1, frag.rect.x2 - frag.rect.x1, frag.rect.y2 - frag.rect.y1);
            }
        }
    }
}