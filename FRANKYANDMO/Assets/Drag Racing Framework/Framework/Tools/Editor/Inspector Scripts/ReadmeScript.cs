using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GercStudio.DragRacingFramework
{
    public class ReadmeScript : ScriptableObject
    {
        public Texture2D icon;
        public string title;
        public List<Section> sections = new List<Section>();
        public bool loadedLayout;
        public bool loadOnStart;

        [Serializable]
        public class Section
        {
            public string heading, text, linkText, url;
        }
    }
}
